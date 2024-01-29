using MAIN.Model;
using MAIN.ViewModel.Helper;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using OxyPlot.Axes;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows;
using MAIN.View;

namespace MAIN.ViewModel
{
    class ApplicationViewModel : BindableClass
    {
        private SepiaManager _sepiaManager;
        private ICommand _loadImage;
        private ICommand _executeEffectCommand;
        private ICommand _saveImage;
        private BitmapSource _beforeBitmapImage;
        private BitmapSource _afterBitmapImage;
        private int _elapsedTime;
        private bool[] _sepiaType;
        private int _sepiaRate;
        private int _threadsNumber;
        private ObservableCollection<int> _allElapsedTimes;

        HistogramWindow histogramWindow = new HistogramWindow();

        public ApplicationViewModel()
        {
            SepiaType = new bool[] { false, false };
            ThreadsNumber = System.Environment.ProcessorCount;
            SepiaRate = 20;
            AllElapsedTimes = new ObservableCollection<int>();
        }

        public BitmapSource BeforeBitmapImage
        {
            get { return _beforeBitmapImage; }
            private set
            {
                _beforeBitmapImage = value;
                RaisePropertyChanged(nameof(BeforeBitmapImage));
            }
        }

        public BitmapSource AfterBitmapImage
        {
            get { return _afterBitmapImage; }
            private set
            {
                _afterBitmapImage = value;
                RaisePropertyChanged(nameof(AfterBitmapImage));
            }
        }

        public bool[] SepiaType
        {
            get { return _sepiaType; }
            set
            {
                _sepiaType = value;
                RaisePropertyChanged(nameof(SepiaType));
            }
        }

        public int ThreadsNumber
        {
            get { return _threadsNumber; }
            set
            {
                _threadsNumber = value;
                RaisePropertyChanged(nameof(ThreadsNumber));
            }
        }

        public int SepiaRate
        {
            get { return _sepiaRate; }
            set
            {
                _sepiaRate = value;
                RaisePropertyChanged(nameof(SepiaRate));
            }
        }

        public int ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                _elapsedTime = value;
                RaisePropertyChanged(nameof(ElapsedTime));
            }
        }

        public Enum.SepiaMechanismType SepiaMechanismType
        {
            get
            {
                for (int i = 0; i < SepiaType.Length; i++)
                    if (SepiaType[i])
                        return (Enum.SepiaMechanismType)(i + 1);
                return Enum.SepiaMechanismType.Undefined;
            }
        }

        public ObservableCollection<int> AllElapsedTimes
        {
            get { return _allElapsedTimes; }
            set
            {
                _allElapsedTimes = value;
                RaisePropertyChanged(nameof(AllElapsedTimes));
            }
        }

        public ICommand LoadImageCommand
        {
            get
            {
                if (_loadImage == null)
                {
                    _loadImage = new RelayCommand(
                        p => LoadImageFromFile(),
                        p => { return true; });
                }
                return _loadImage;
            }
        }

        public ICommand SaveImageCommand
        {
            get
            {
                if (_saveImage == null)
                {
                    _saveImage = new RelayCommand(
                        p => SaveImageToFile(),
                        p =>
                        {
                            return !(AfterBitmapImage == null);
                        });
                }
                return _saveImage;
            }
        }

        public ICommand ExecuteEffectCommand
        {
            get
            {
                if (_executeEffectCommand == null)
                {
                    _executeEffectCommand = new RelayCommand(
                        p => ExecuteEffect(),
                        p =>
                        {
                            return (_beforeBitmapImage != null &&
                                SepiaMechanismType != Enum.SepiaMechanismType.Undefined);
                        });
                }
                return _executeEffectCommand;
            }
        }

        public Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap(bitmapSource, PixelFormats.Bgra32, null, 0);

            Bitmap bitmap = new Bitmap(
                convertedBitmap.PixelWidth,
                convertedBitmap.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            convertedBitmap.CopyPixels(
                Int32Rect.Empty,
                bitmapData.Scan0,
                bitmapData.Stride * bitmapData.Height,
                bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public PlotModel PlotModelRBefore { get; set; }
        public PlotModel PlotModelRAfter { get; set; }
        public PlotModel PlotModelGBefore { get; set; }
        public PlotModel PlotModelGAfter { get; set; }
        public PlotModel PlotModelBBefore { get; set; }
        public PlotModel PlotModelBAfter { get; set; }

        private void ExecuteEffect()
        {
            long elapsedTicks;

            _sepiaManager = new SepiaManager(
                _beforeBitmapImage, SepiaMechanismType,
                (float)_sepiaRate, _threadsNumber);

            Bitmap beforeBitmap = ConvertBitmapSourceToBitmap(_beforeBitmapImage);

            ObservableCollection<int>[] histogramsBefore = GenerateHistograms(beforeBitmap);

            AfterBitmapImage = _sepiaManager.ExecuteEffect(out elapsedTicks);

            Bitmap afterBitmap = ConvertBitmapSourceToBitmap(AfterBitmapImage);

            ObservableCollection<int>[] histogramsAfter = GenerateHistograms(afterBitmap);

            TimeSpan elapsedTime = TimeSpan.FromTicks(elapsedTicks);
            ElapsedTime = (int)elapsedTime.TotalMilliseconds;

            AllElapsedTimes.Add(ElapsedTime);

            PlotModelRBefore = GenerateHistogramPlotModel(histogramsBefore[0], histogramsBefore[3], histogramsBefore[6], "R Histogram Before");
            PlotModelRAfter = GenerateHistogramPlotModel(histogramsAfter[0], histogramsAfter[3], histogramsAfter[6], "R Histogram After");

            PlotModelGBefore = GenerateHistogramPlotModel(histogramsBefore[1], histogramsBefore[4], histogramsBefore[7], "G Histogram Before");
            PlotModelGAfter = GenerateHistogramPlotModel(histogramsAfter[1], histogramsAfter[4], histogramsAfter[7], "G Histogram After");

            PlotModelBBefore = GenerateHistogramPlotModel(histogramsBefore[2], histogramsBefore[5], histogramsBefore[8], "B Histogram Before");
            PlotModelBAfter = GenerateHistogramPlotModel(histogramsAfter[2], histogramsAfter[5], histogramsAfter[8], "B Histogram After");

            HistogramWindow histogramWindow = new HistogramWindow();
            histogramWindow.DataContext = this;
            histogramWindow.Show();
        }

        private PlotModel GenerateHistogramPlotModel(
    ObservableCollection<int> data1, ObservableCollection<int> data2, ObservableCollection<int> data3, string title)
        {
            var model = new PlotModel();
            model.Title = title;

            var linearAxis1 = new LinearAxis() { Title = "Value" };
            linearAxis1.Position = AxisPosition.Bottom;
            model.Axes.Add(linearAxis1);

            var linearAxis2 = new LinearAxis() { Title = "Frequency" };
            linearAxis2.Position = AxisPosition.Left;
            model.Axes.Add(linearAxis2);

            var series1 = new LineSeries { Title = "Channel 1" };

            for (int i = 0; i < 256; i++)
            {
                series1.Points.Add(new DataPoint(i, data1[i]));
            }

            model.Series.Add(series1);

            return model;
        }

        private ObservableCollection<int>[] GenerateHistograms(Bitmap bitmap)
        {
            ObservableCollection<int>[] histograms = new ObservableCollection<int>[9];

            for (int i = 0; i < 9; i++)
            {
                histograms[i] = new ObservableCollection<int>(Enumerable.Repeat(0, 256));
            }

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);
                    histograms[0][pixel.R]++;   
                    histograms[1][pixel.G]++;   
                    histograms[2][pixel.B]++;   
                }
            }

            return histograms;
        }

        private void LoadImageFromFile()
        {
            Microsoft.Win32.OpenFileDialog dlg =
                new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".bmp";
            dlg.Filter = "BMP Files (*.bmp)|*.bmp";
            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                BeforeBitmapImage = LoadImageToMemory(dlg.FileName);
            }
        }

        private void SaveImageToFile()
        {
            Microsoft.Win32.SaveFileDialog dlg =
                new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "sepia image";
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "BMP File (.bmp)|*.bmp";

            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                SaveImageToDisk(AfterBitmapImage, dlg.FileName);
            }
        }

        private void SaveImageToDisk(BitmapSource image, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private BitmapSource LoadImageToMemory(string path)
        {
            BitmapSource newBitmap;
            try
            {
                newBitmap = new BitmapImage(new System.Uri(path));
                return newBitmap;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
