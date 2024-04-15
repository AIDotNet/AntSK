using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AntSK.LLM.StableDiffusion
{
    using static AntSK.LLM.StableDiffusion.Structs;
    using SdContext = IntPtr;
    using SDImagePtr = IntPtr;
    using UpscalerContext = IntPtr;

    public static class SDHelper
    {
        public static bool IsInitialized => SdContext.Zero != sd_ctx;
        public static bool IsUpscalerInitialized => UpscalerContext.Zero != upscaler_ctx;

        private static SdContext sd_ctx = new SdContext();
        private static UpscalerContext upscaler_ctx = new UpscalerContext();

        public static event EventHandler<StableDiffusionEventArgs.StableDiffusionLogEventArgs> Log;
        public static event EventHandler<StableDiffusionEventArgs.StableDiffusionProgressEventArgs> Progress;
        static readonly Native.SdLogCallback sd_Log_Cb;
        static readonly Native.SdProgressCallback sd_Progress_Cb;

        //Hide the code below so that the process can be seen in console.
        //static SDHelper()
        //{
        //    sd_Log_Cb = new Native.SdLogCallback(OnNativeLog);
        //    Native.sd_set_log_callback(sd_Log_Cb, IntPtr.Zero);

        //    sd_Progress_Cb = new Native.SdProgressCallback(OnProgressRunning);
        //    Native.sd_set_progress_callback(sd_Progress_Cb, IntPtr.Zero);

        //}

        public static bool Initialize(ModelParams modelParams)
        {
            sd_ctx = Native.new_sd_ctx(modelParams.ModelPath,
                                        modelParams.VaePath,
                                        modelParams.TaesdPath,
                                        modelParams.ControlnetPath,
                                        modelParams.LoraModelDir,
                                        modelParams.EmbeddingsPath,
                                        modelParams.StackedIdEmbeddingsPath,
                                        modelParams.VaeDecodeOnly,
                                        modelParams.VaeTiling,
                                        modelParams.FreeParamsImmediately,
                                        modelParams.Threads,
                                        modelParams.SdType,
                                        modelParams.RngType,
                                        modelParams.Schedule,
                                        modelParams.KeepClipOnCpu,
                                        modelParams.KeepControlNetOnCpu,
                                        modelParams.KeepVaeOnCpu);
            return SdContext.Zero != sd_ctx;
        }

        public static bool InitializeUpscaler(UpscalerParams @params)
        {
            upscaler_ctx = Native.new_upscaler_ctx(@params.ESRGANPath, @params.Threads, @params.SdType);
            return UpscalerContext.Zero != upscaler_ctx;
        }

        public static void FreeSD()
        {
            if (SdContext.Zero != sd_ctx)
            {
                Native.free_sd_ctx(sd_ctx);
                sd_ctx = SdContext.Zero;
            }
        }

        public static void FreeUpscaler()
        {
            if (UpscalerContext.Zero != upscaler_ctx)
            {
                Native.free_upscaler_ctx(upscaler_ctx);
                upscaler_ctx = UpscalerContext.Zero;
            }
        }

        public static Bitmap[] TextToImage(TextToImageParams textToImageParams)
        {
            if (!IsInitialized) throw new ArgumentNullException("Model not loaded!");

            IntPtr cnPtr = IntPtr.Zero;
            if (textToImageParams.ControlCond != null)
            {
                if (textToImageParams.ControlCond.Width > 1)
                {
                    SDImage cnImg = GetSDImageFromBitmap(textToImageParams.ControlCond);
                    cnPtr = GetPtrFromImage(cnImg);
                }
            }

            SDImagePtr sd_Image_ptr = Native.txt2img(sd_ctx,
                          textToImageParams.Prompt,
                          textToImageParams.NegativePrompt,
                          textToImageParams.ClipSkip,
                          textToImageParams.CfgScale,
                          textToImageParams.Width,
                          textToImageParams.Height,
                          textToImageParams.SampleMethod,
                          textToImageParams.SampleSteps,
                          textToImageParams.Seed,
                          textToImageParams.BatchCount,
                          cnPtr,
                          textToImageParams.ControlStrength,
                          textToImageParams.StyleStrength,
                          textToImageParams.NormalizeInput,
                          textToImageParams.InputIdImagesPath);

            Bitmap[] images = new Bitmap[textToImageParams.BatchCount];
            for (int i = 0; i < textToImageParams.BatchCount; i++)
            {
                SDImage sd_image = Marshal.PtrToStructure<SDImage>(sd_Image_ptr + i * Marshal.SizeOf<SDImage>());
                images[i] = GetBitmapFromSdImage(sd_image);
            }
            return images;
        }


        public static Bitmap ImageToImage(ImageToImageParams imageToImageParams)
        {
            if (!IsInitialized) throw new ArgumentNullException("Model not loaded!");
            SDImage input_sd_image = GetSDImageFromBitmap(imageToImageParams.InputImage);

            SDImagePtr sdImgPtr = Native.img2img(sd_ctx,
                  input_sd_image,
                  imageToImageParams.Prompt,
                  imageToImageParams.NegativePrompt,
                  imageToImageParams.ClipSkip,
                  imageToImageParams.CfgScale,
                  imageToImageParams.Width,
                  imageToImageParams.Height,
                  imageToImageParams.SampleMethod,
                  imageToImageParams.SampleSteps,
                  imageToImageParams.Strength,
                  imageToImageParams.Seed,
                  imageToImageParams.BatchCount);
            SDImage sdImg = Marshal.PtrToStructure<SDImage>(sdImgPtr);

            return GetBitmapFromSdImage(sdImg);
        }

        public static Bitmap UpscaleImage(Bitmap image, int upscaleFactor)
        {
            if (!IsUpscalerInitialized) throw new ArgumentNullException("Upscaler not loaded!");
            SDImage inputSDImg = GetSDImageFromBitmap(image);
            SDImage result = Native.upscale(upscaler_ctx, inputSDImg, upscaleFactor);
            return GetBitmapFromSdImage(result);
        }

        private static Bitmap GetBitmapFromSdImage(SDImage sd_Image)
        {
            int width = (int)sd_Image.Width;
            int height = (int)sd_Image.Height;
            int channel = (int)sd_Image.Channel;
            byte[] bytes = new byte[width * height * channel];
            Marshal.Copy(sd_Image.Data, bytes, 0, bytes.Length);
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            int stride = bmp.Width * channel;
            byte[] des = new byte[bytes.Length];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    des[stride * i + channel * j + 0] = bytes[stride * i + channel * j + 2];
                    des[stride * i + channel * j + 1] = bytes[stride * i + channel * j + 1];
                    des[stride * i + channel * j + 2] = bytes[stride * i + channel * j + 0];
                }
            }
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(des, 0, bitmapData.Scan0, bytes.Length);
            bmp.UnlockBits(bitmapData);

            return bmp;
        }

        private static SDImage GetSDImageFromBitmap(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int channel = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
            int stride = width * channel;
            byte[] bytes = new byte[width * height * channel];
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Marshal.Copy(bitmapData.Scan0, bytes, 0, bytes.Length);
            bmp.UnlockBits(bitmapData);

            byte[] sdImageBytes = new byte[bytes.Length];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sdImageBytes[stride * i + j * 3 + 0] = bytes[stride * i + j * 3 + 2];
                    sdImageBytes[stride * i + j * 3 + 1] = bytes[stride * i + j * 3 + 1];
                    sdImageBytes[stride * i + j * 3 + 2] = bytes[stride * i + j * 3 + 0];
                }
            }

            SDImage sd_Image = new SDImage
            {
                Width = (uint)width,
                Height = (uint)height,
                Channel = 3,
                Data = Marshal.UnsafeAddrOfPinnedArrayElement(sdImageBytes, 0),
            };

            return sd_Image;
        }

        private static IntPtr GetPtrFromImage(SDImage sdImg)
        {
            IntPtr imgPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SDImage)));
            Marshal.StructureToPtr(sdImg, imgPtr, false);
            return imgPtr;
        }

        private static void OnNativeLog(SdLogLevel level, string text, IntPtr data)
        {
            Log?.Invoke(null, new StableDiffusionEventArgs.StableDiffusionLogEventArgs { Level = level, Text = text });
        }

        private static void OnProgressRunning(int step, int steps, float time, IntPtr data)
        {
            Progress?.Invoke(null, new StableDiffusionEventArgs.StableDiffusionProgressEventArgs { Step = step, Steps = steps, Time = time });
        }


    }
}


