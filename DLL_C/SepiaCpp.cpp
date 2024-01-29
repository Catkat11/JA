#include <algorithm>

const float pixelMaxValue = 255.0f;

float Min(float left, float right)
{
    return std::min(left, right);
}

void Sepia(float* pixels, int size,
    float* sepiaRates, float* rgbRates,
    int bytesPerPixel, int startIndex, int endIndex)
{
    for (int i = startIndex;
        i < endIndex && i < size; i += bytesPerPixel)
    {
        float whiteBlackPixel =
            rgbRates[2] * pixels[i + 2] +
            rgbRates[1] * pixels[i + 1] +
            rgbRates[0] * pixels[i];
        pixels[i] = whiteBlackPixel;
        pixels[i + 1] = Min(
            whiteBlackPixel + sepiaRates[1], pixelMaxValue);
        pixels[i + 2] = Min(
            whiteBlackPixel + sepiaRates[2], pixelMaxValue);
    }
}

extern "C" __declspec(dllexport) void SepiaCppAlgorithm(
    float* pixels, int size,
    float* sepiaRates, float* rgbRates,
    int bytesPerPixel, int startIndex, int endIndex)
{
    Sepia(pixels, size, sepiaRates, rgbRates,
        bytesPerPixel, startIndex, endIndex);
}
