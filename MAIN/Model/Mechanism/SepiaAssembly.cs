using System.Runtime.InteropServices;

namespace MAIN.Model.Mechanism
{
    internal class SepiaAssembly : SepiaInterface
    {
        public SepiaAssembly(float[] sepiaRates, int bytesPerPixel,
            int startIndex, int endIndex)
            : base(sepiaRates, bytesPerPixel, startIndex, endIndex)
        { }

        [DllImport("DLL_ASM.dll", EntryPoint = "sepia")]
        private static extern void SepiaAsmAlgorithm(
            float[] pixels, float[] sepiaRates, 
            float[] rgbRates, float[] array255, int startIndex, int endIndex);

        public override void ApplyEffect(float[] allPixels)
        {
            float[] array255 = { 255.0f, 255.0f, 255.0f, 255.0f };
            SepiaAsmAlgorithm(allPixels, _sepiaRates, 
                _rgbRates, array255, _startIndex, _endIndex);
        }
    }
}
