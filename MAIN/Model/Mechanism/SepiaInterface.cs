namespace MAIN.Model.Mechanism
{
    public abstract class SepiaInterface
    {
        protected float[] _sepiaRates;
        protected float[] _rgbRates = { 0.114f, 0.587f, 0.299f, 0.0f };
        protected int _startIndex;
        protected int _endIndex;
        protected int _bytesPerPixel;

        public SepiaInterface(float[] sepiaRates, int bytesPerPixel, int startIndex, int endIndex)
        {
            _sepiaRates = sepiaRates;
            _startIndex = startIndex;
            _endIndex = endIndex;
            _bytesPerPixel = bytesPerPixel;
        }

        public abstract void ApplyEffect(float[] allPixels);
    }
}
