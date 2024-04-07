﻿using Microsoft.KernelMemory.DataFormats;
using Sdcb.OpenVINO.PaddleOCR;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Sdcb.OpenVINO.PaddleOCR.Models;
using OpenCvSharp;

namespace AntSK.OCR
{
    /// <summary>
    /// OCR
    /// </summary>
    public class AntSKOcrEngine : IOcrEngine
    {
        FullOcrModel model;
        public async Task<string> ExtractTextFromImageAsync(Stream imageContent, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                model = await OnlineFullModels.ChineseV4.DownloadAsync();
            }
            using (PaddleOcrAll all = new(model)
            {
                AllowRotateDetection = true,
                Enable180Classification = true,
            })
            {
                Mat src = Cv2.ImDecode(StreamToByte(imageContent), ImreadModes.Color);
                PaddleOcrResult result = all.Run(src);
                return await Task.FromResult(result.Text);
            }
        }

        private byte[] StreamToByte(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[1024]; //自定义大小，例如 1024
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                byte[] bytes = memoryStream.ToArray();
                return bytes;
            }
        }
    }
}