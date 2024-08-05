using System;
using static AntSK.LLM.StableDiffusion.Structs;

namespace AntSK.LLM.StableDiffusion
{
    public class StableDiffusionEventArgs
    {
        public class StableDiffusionProgressEventArgs : EventArgs
        {
            #region Properties & Fields

            public int Step { get; set; }
            public int Steps { get; set; }
            public float Time { get; set; }
            public IntPtr Data { get; set; }

            public double Progress => (double)Step / Steps;
            public float IterationsPerSecond => 1.0f / Time;

            #endregion
        }

        public class StableDiffusionLogEventArgs : EventArgs
        {
            #region Properties & Fields

            public SdLogLevel Level { get; set; }
            public string Text { get; set; }

            #endregion
        }
    }
}
