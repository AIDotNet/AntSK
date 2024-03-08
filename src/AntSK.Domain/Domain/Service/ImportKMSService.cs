using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.SemanticKernel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IImportKMSService), ServiceLifetime.Scoped)]
    public class ImportKMSService(
        IKMService _kMService,
        IKmsDetails_Repositories _kmsDetails_Repositories,
        IKmss_Repositories _kmss_Repositories
        ) : IImportKMSService
    {

        public void ImportKMSTask(ImportKMSTaskReq req)
        {
            try
            {
                var km = _kmss_Repositories.GetFirst(p => p.Id == req.KmsId);

                var _memory = _kMService.GetMemoryByKMS(km.Id);
                string fileid = req.KmsDetail.Id;
                switch (req.ImportType)
                {
                    case ImportType.File:
                        //导入文件
                        {
                           var importResult=  _memory.ImportDocumentAsync(new Document(fileid)
                         .AddFile(req.FilePath)
                         .AddTag("kmsid", req.KmsId)
                         , index: "kms").Result;
                            //查询文档数量
                            var docTextList =  _kMService.GetDocumentByFileID(km.Id,fileid).Result;
                            string fileGuidName = Path.GetFileName(req.FilePath);
                            req.KmsDetail.FileName = req.FileName;
                            req.KmsDetail.FileGuidName = fileGuidName;
                            req.KmsDetail.DataCount = docTextList.Count;

                        }
                        break;
                    case ImportType.Url:
                        {
                            //导入url                  
                            var importResult = _memory.ImportWebPageAsync(req.Url, fileid, new TagCollection() { { "kmsid", req.KmsId } }
                                 , index: "kms").Result;
                            //查询文档数量
                            var docTextList = _kMService.GetDocumentByFileID(km.Id,fileid).Result;
                            req.KmsDetail.Url = req.Url;
                            req.KmsDetail.DataCount = docTextList.Count;
                        }
                        break;
                    case ImportType.Text:
                        //导入文本
                        {
                            var importResult = _memory.ImportTextAsync(req.Text, fileid, new TagCollection() { { "kmsid", req.KmsId } }
                       , index: "kms").Result;
                            //查询文档数量
                            var docTextList =  _kMService.GetDocumentByFileID(km.Id,fileid).Result;
                            req.KmsDetail.Url = req.Url;
                            req.KmsDetail.DataCount = docTextList.Count;

                        }
                        break;
                }
                req.KmsDetail.Status = Model.Enum.ImportKmsStatus.Success;
                 _kmsDetails_Repositories.Update(req.KmsDetail);
                 //_kmsDetails_Repositories.GetList(p => p.KmsId == req.KmsId);
                Console.WriteLine("后台导入任务成功:" + req.KmsDetail.DataCount);
            }
            catch (Exception ex)
            {
                req.KmsDetail.Status = Model.Enum.ImportKmsStatus.Fail;
               _kmsDetails_Repositories.Update(req.KmsDetail);
                Console.WriteLine("后台导入任务异常:" + ex.Message);
            }
        }
    }
}
