using MAIN.Model.Mechanism;

namespace MAIN.Model
{
    static class SepiaMechanismFactory
    {
        public static SepiaInterface Create(Enum.SepiaMechanismType mechanismType, float[] sepiaRates, int bytesPerPixel,  int startIndex, int endIndex)
        {
            switch (mechanismType)
            {
                case Enum.SepiaMechanismType.Assembly:
                    return new SepiaAssembly(sepiaRates, bytesPerPixel, startIndex, endIndex);
                case Enum.SepiaMechanismType.Cpp:
                    return new SepiaCpp(sepiaRates, bytesPerPixel, startIndex, endIndex);
                default:
                    return null;
            }
        }
    }
}
