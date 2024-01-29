using System.Runtime.InteropServices;

namespace MAIN.Model.Mechanism
{
    internal class SepiaCpp : SepiaInterface
    {
        public SepiaCpp(float[] sepiaRates, int bytesPerPixel, 
            int startIndex, int endIndex)
            : base(sepiaRates, bytesPerPixel, startIndex, endIndex)
        { }

        [DllImport("DLL_C.dll", EntryPoint = "SepiaCppAlgorithm")]
        private static extern void SepiaCppAlgorithm(
            float[] pixels, int size, 
            float[] sepiaRate, float[] rgbRates, 
            int bytesPerPixel, int startIndex, int endIndex);

        public override void ApplyEffect(float[] allPixels)
        {
            SepiaCppAlgorithm(allPixels, allPixels.Length,
                _sepiaRates, _rgbRates, 
                _bytesPerPixel, _startIndex, _endIndex);
        }
    }
}
