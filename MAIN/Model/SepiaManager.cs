using MAIN.Extension;
using MAIN.Model.Mechanism;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace MAIN.Model
{
    class SepiaManager
    {
        private BitmapSource _oldBitmap;
        private List<SepiaInterface> _sepiaInterfaces = new List<SepiaInterface>();
        private List<Task>_tasks = new List<Task>();
        private int _numberOfThreads;
        private float[] _allPixels;
        private BitmapSource _resultImage;
        const int _bitsInByte = 8;

        public BitmapSource ResultImage
        {
            get { return _resultImage; }
        }
        private SepiaManager() { }
        
        public SepiaManager(BitmapSource bitmapImage, 
            Enum.SepiaMechanismType mechanismType, 
            float sepiaRate, int numberOfThreads)
        {
            _oldBitmap = bitmapImage;
            _allPixels = RetrievePixels(bitmapImage);
            _numberOfThreads = numberOfThreads;
            int pieceLenght = AdjustPieceLenght();
            float[] sepiaRates = { 0, sepiaRate, 2 * sepiaRate, 0 };
            for (int partNumber = 0; partNumber < _numberOfThreads; partNumber++)
            {
                int tempPartNumber = partNumber;
                int pieceEnd;
                if (partNumber + 1 == _numberOfThreads)
                    pieceEnd = _allPixels.Length;                    
                else
                    pieceEnd = pieceLenght * (tempPartNumber + 1) - 1;

                 _sepiaInterfaces.Add(SepiaMechanismFactory.Create(
                         mechanismType, sepiaRates,
                         bitmapImage.Format.BitsPerPixel / _bitsInByte,
                         pieceLenght * tempPartNumber,
                         pieceEnd));
                 _tasks.Add(new Task(() =>
                     _sepiaInterfaces[tempPartNumber].ApplyEffect(_allPixels)));

            }
        }

        private int AdjustPieceLenght()
        {
            int pieceLenght = _allPixels.Length / _numberOfThreads;
            while (pieceLenght % (_oldBitmap.Format.BitsPerPixel / _bitsInByte) != 0)
                pieceLenght++;
            return pieceLenght;
        }

        private float[] RetrievePixels(BitmapSource bitmapImage)
        {
            return bitmapImage.ConvertToBGRArray();
        }

        public BitmapSource ExecuteEffect(out long elapsedTicks)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.ForEach(_tasks, (task) => task.Start());
            Task.WaitAll(_tasks.ToArray());

            stopwatch.Stop();
            elapsedTicks = stopwatch.ElapsedTicks;

            _resultImage = _allPixels.ConvertBGRArrayToImage(_oldBitmap.PixelWidth,
                _oldBitmap.PixelHeight, _oldBitmap.Format);

            return _resultImage;
        }
    }
}