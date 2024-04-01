using System;
using System.Drawing;
using System.Runtime.InteropServices;


namespace AntSK.LLM.StableDiffusion
{
    using int64_t = Int64;
    using uint32_t = UInt32;

    public class Structs
    {
        public class ModelParams
        {
            public string ModelPath = string.Empty;
            public string VaePath = string.Empty;
            public string TaesdPath = string.Empty;
            public string ControlnetPath = string.Empty;
            public string LoraModelDir = string.Empty;
            public string EmbeddingsPath = string.Empty;
            public string StackedIdEmbeddingsPath = string.Empty;
            public bool VaeDecodeOnly = false;
            public bool VaeTiling = true;
            public bool FreeParamsImmediately = false;
            public int Threads = Native.get_num_physical_cores();
            public WeightType SdType = WeightType.SD_TYPE_COUNT;
            public RngType RngType = RngType.CUDA_RNG;
            public ScheduleType Schedule = ScheduleType.DEFAULT;
            public bool KeepClipOnCpu = false;
            public bool KeepControlNetOnCpu = false;
            public bool KeepVaeOnCpu = false;
        }

        public class TextToImageParams
        {
            public string Prompt = string.Empty;
            public string NegativePrompt = string.Empty;
            public int ClipSkip = 0;
            public float CfgScale = 7;
            public int Width = 512;
            public int Height = 512;
            public SampleMethod SampleMethod = SampleMethod.EULER_A;
            public int SampleSteps = 20;
            public int64_t Seed = -1;
            public int BatchCount = 1;
            public Bitmap ControlCond = new Bitmap(1, 1);
            public float ControlStrength = 0.9f;
            public float StyleStrength = 0.75f;
            public bool NormalizeInput = false;
            public string InputIdImagesPath = string.Empty;
        }

        public class ImageToImageParams
        {
            public Bitmap InputImage;
            public string Prompt = string.Empty;
            public string NegativePrompt  = string.Empty;
            public int ClipSkip = -1;
            public float CfgScale  = 7.0f;
            public int Width  = 512;
            public int Height = 512;
            public SampleMethod SampleMethod = SampleMethod.EULER_A;
            public int SampleSteps  = 20;
            public float Strength = 0.75f;
            public int64_t Seed = 42;
            public int BatchCount  = 1;
        }

        public class UpscalerParams
        {
            public string ESRGANPath = string.Empty;
            public int Threads = Native.get_num_physical_cores();
            public WeightType SdType = WeightType.SD_TYPE_COUNT;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SDImage
        {
            public uint32_t Width;
            public uint32_t Height;
            public uint32_t Channel;
            public IntPtr Data;
        }

        public enum WeightType
        {
            SD_TYPE_F32 = 0,
            SD_TYPE_F16 = 1,
            SD_TYPE_Q4_0 = 2,
            SD_TYPE_Q4_1 = 3,
            // SD_TYPE_Q4_2 = 4, support has been removed
            // SD_TYPE_Q4_3 (5) support has been removed
            SD_TYPE_Q5_0 = 6,
            SD_TYPE_Q5_1 = 7,
            SD_TYPE_Q8_0 = 8,
            SD_TYPE_Q8_1 = 9,
            // k-quantizations
            SD_TYPE_Q2_K = 10,
            SD_TYPE_Q3_K = 11,
            SD_TYPE_Q4_K = 12,
            SD_TYPE_Q5_K = 13,
            SD_TYPE_Q6_K = 14,
            SD_TYPE_Q8_K = 15,
            SD_TYPE_IQ2_XXS = 16,
            SD_TYPE_IQ2_XS = 17,
            SD_TYPE_IQ3_XXS = 18,
            SD_TYPE_IQ1_S = 19,
            SD_TYPE_IQ4_NL = 20,
            SD_TYPE_IQ3_S = 21,
            SD_TYPE_IQ2_S = 22,
            SD_TYPE_IQ4_XS = 23,
            SD_TYPE_I8,
            SD_TYPE_I16,
            SD_TYPE_I32,
            SD_TYPE_COUNT,
        };

        public enum RngType
        {
            STD_DEFAULT_RNG,
            CUDA_RNG
        };

        public enum ScheduleType
        {
            DEFAULT,
            DISCRETE,
            KARRAS,
            N_SCHEDULES
        };

        public enum SampleMethod
        {
            EULER_A,
            EULER,
            HEUN,
            DPM2,
            DPMPP2S_A,
            DPMPP2M,
            DPMPP2Mv2,
            LCM,
            N_SAMPLE_METHODS
        };

        public enum SdLogLevel
        {
            Debug,
            Info,
            Warn,
            Error
        }

    }
}
