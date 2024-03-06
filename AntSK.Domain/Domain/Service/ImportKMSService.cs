using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using Microsoft.KernelMemory;
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
        IKmsDetails_Repositories _kmsDetails_Repositories
        ) : IImportKMSService
    {

        public async Task ImportKMSTask(ImportKMSTaskReq req)
        {
            try
            {
                KmsDetails detial = new KmsDetails() ;
                string fileid = Guid.NewGuid().ToString();
                switch (req.ImportType)
                {
                    case ImportType.File:
                        //导入文件
                        {
                            await req.Memory.ImportDocumentAsync(new Document(fileid)
                         .AddFile(req.FilePath)
                         .AddTag("kmsid", req.KmsId)
                         , index: "kms");
                            //查询文档数量
                            var docTextList = await _kMService.GetDocumentByFileID(fileid);
                            string fileGuidName = Path.GetFileName(req.FilePath);
                            detial = new KmsDetails()
                            {
                                Id = fileid,
                                KmsId = req.KmsId,
                                Type = "file",
                                FileName = req.FileName,
                                FileGuidName = fileGuidName,
                                DataCount = docTextList.Count,
                                CreateTime = DateTime.Now
                            };
                        }
                        break;
                    case ImportType.Url:
                        {
                            //导入url                  
                            await req.Memory.ImportWebPageAsync(req.Url, fileid, new TagCollection() { { "kmsid", req.KmsId } }
                                 , index: "kms");
                            //查询文档数量
                            var docTextList = await _kMService.GetDocumentByFileID(fileid);

                            detial = new KmsDetails()
                            {
                                Id = fileid,
                                KmsId = req.KmsId,
                                Type = "url",
                                Url = req.Url,
                                DataCount = docTextList.Count,
                                CreateTime = DateTime.Now
                            };
                        }
                        break;
                    case ImportType.Text:
                        //导入文本
                        {
                            await req.Memory.ImportTextAsync(req.Text, fileid, new TagCollection() { { "kmsid", req.KmsId } }
                       , index: "kms");
                            //查询文档数量
                            var docTextList = await _kMService.GetDocumentByFileID(fileid);

                            detial = new KmsDetails()
                            {
                                Id = fileid,
                                KmsId = req.KmsId,
                                Type = "text",
                                DataCount = docTextList.Count,
                                CreateTime = DateTime.Now
                            };
                        }
                        break;
                }
                await _kmsDetails_Repositories.InsertAsync(detial);
                await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == req.KmsId);
                Console.WriteLine("后台导入任务成功:"+ detial.DataCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("后台导入任务异常:" + ex.Message);
            }
        }
    }
}
