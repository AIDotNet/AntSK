using System;
using System.Runtime.InteropServices;

namespace AntSK.LLM.StableDiffusion
{
    using static AntSK.LLM.StableDiffusion.Structs;
    using int32_t = Int32;
    using int64_t = Int64;
    using SdContext = IntPtr;
    using SDImagePtr = IntPtr;
    using UpscalerContext = IntPtr;

    internal class Native
    {
        const string DllName = "stable-diffusion";

        internal delegate void SdLogCallback(SdLogLevel level, [MarshalAs(UnmanagedType.LPStr)] string text, IntPtr data);
        internal delegate void SdProgressCallback(int step, int steps, float time, IntPtr data);

        [DllImport(DllName, EntryPoint = "new_sd_ctx", CallingConvention = CallingConvention.Cdecl)]
        internal extern static SdContext new_sd_ctx(string model_path,
                                                     string vae_path,
                                                     string taesd_path,
                                                     string control_net_path_c_str,
                                                     string lora_model_dir,
                                                     string embed_dir_c_str,
                                                     string stacked_id_embed_dir_c_str,
                                                     bool vae_decode_only,
                                                     bool vae_tiling,
                                                     bool free_params_immediately,
                                                     int n_threads,
                                                     WeightType weightType,
                                                     RngType rng_type,
                                                     ScheduleType s,
                                                     bool keep_clip_on_cpu,
                                                     bool keep_control_net_cpu,
                                                     bool keep_vae_on_cpu);


        [DllImport(DllName, EntryPoint = "txt2img", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SDImagePtr txt2img(SdContext sd_ctx,
                           string prompt,
                           string negative_prompt,
                           int clip_skip,
                           float cfg_scale,
                           int width,
                           int height,
                           SampleMethod sample_method,
                           int sample_steps,
                           int64_t seed,
                           int batch_count,
                           SDImagePtr control_cond,
                           float control_strength,
                           float style_strength,
                           bool normalize_input,
                           string input_id_images_path);

        [DllImport(DllName, EntryPoint = "img2img", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SDImagePtr img2img(SdContext sd_ctx,
                    SDImage init_image,
                    string prompt_c_str,
                    string negative_prompt_c_str,
                    int clip_skip,
                    float cfg_scale,
                    int width,
                    int height,
                    SampleMethod sample_method,
                    int sample_steps,
                    float strength,
                    int64_t seed,
                    int batch_count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr preprocess_canny(IntPtr imgData,
                                 int width,
                                 int height,
                                 float high_threshold,
                                 float low_threshold,
                                 float weak,
                                 float strong,
                                 bool inverse);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UpscalerContext new_upscaler_ctx(string esrgan_path,
                                        int n_threads,
                                        WeightType wtype);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int32_t get_num_physical_cores();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void free_sd_ctx(SdContext sd_ctx);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void free_upscaler_ctx(UpscalerContext upscaler_ctx);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SDImage upscale(UpscalerContext upscaler_ctx, SDImage input_image, int upscale_factor);

        [DllImport(DllName, EntryPoint = "sd_set_log_callback", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void sd_set_log_callback(SdLogCallback cb, IntPtr data);

        [DllImport(DllName, EntryPoint = "sd_set_progress_callback", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void sd_set_progress_callback(SdProgressCallback cb, IntPtr data);

    }
}

