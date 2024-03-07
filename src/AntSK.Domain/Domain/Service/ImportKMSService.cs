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

        public async Task ImportKMSTask(ImportKMSTaskReq req)
        {
            await Task.Delay(20000);
            //try
            //{
            //    var km = await _kmss_Repositories.GetFirstAsync(p => p.Id == req.KmsId);
            //    var _memory = _kMService.GetMemory(textPartitioningOptions: new TextPartitioningOptions()
            //    {
            //        MaxTokensPerLine = km.MaxTokensPerLine,
            //        MaxTokensPerParagraph = km.MaxTokensPerParagraph,
            //        OverlappingTokens = km.OverlappingTokens
            //    });
            //    string fileid = Guid.NewGuid().ToString();
            //    switch (req.ImportType)
            //    {
            //        case ImportType.File:
            //            //导入文件
            //            {
            //                await _memory.ImportDocumentAsync(new Document(fileid)
            //             .AddFile(req.FilePath)
            //             .AddTag("kmsid", req.KmsId)
            //             , index: "kms");
            //                //查询文档数量
            //                var docTextList = await _kMService.GetDocumentByFileID(fileid);
            //                string fileGuidName = Path.GetFileName(req.FilePath);
            //                req.KmsDetail.FileName = req.FileName;
            //                req.KmsDetail.FileGuidName = fileGuidName;
            //                req.KmsDetail.DataCount = docTextList.Count;
               
            //            }
            //            break;
            //        case ImportType.Url:
            //            {
            //                //导入url                  
            //                await _memory.ImportWebPageAsync(req.Url, fileid, new TagCollection() { { "kmsid", req.KmsId } }
            //                     , index: "kms");
            //                //查询文档数量
            //                var docTextList = await _kMService.GetDocumentByFileID(fileid);
            //                req.KmsDetail.Url = req.Url;
            //                req.KmsDetail.DataCount = docTextList.Count;
            //            }
            //            break;
            //        case ImportType.Text:
            //            //导入文本
            //            {
            //                await _memory.ImportTextAsync(req.Text, fileid, new TagCollection() { { "kmsid", req.KmsId } }
            //           , index: "kms");
            //                //查询文档数量
            //                var docTextList = await _kMService.GetDocumentByFileID(fileid);
            //                req.KmsDetail.Url = req.Url;
            //                req.KmsDetail.DataCount = docTextList.Count;

            //            }
            //            break;
            //    }
            //    req.KmsDetail.Status = Model.Enum.ImportKmsStatus.Success;
            //    await _kmsDetails_Repositories.UpdateAsync(req.KmsDetail);
            //    await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == req.KmsId);
            //    Console.WriteLine("后台导入任务成功:" + req.KmsDetail.DataCount);
            //}
            //catch (Exception ex)
            //{
            //    req.KmsDetail.Status = Model.Enum.ImportKmsStatus.Fail;
            //    await _kmsDetails_Repositories.UpdateAsync(req.KmsDetail);
            //    Console.WriteLine("后台导入任务异常:" + ex.Message);
            //}
        }
    }
}
