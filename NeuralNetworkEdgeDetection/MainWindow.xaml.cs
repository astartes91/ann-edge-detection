using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HeatonResearchNeural.Feedforward;
using Microsoft.Win32;
using Color = System.Drawing.Color;

namespace NeuralNetworkEdgeDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();

            BitmapImage sourceBitmapImage = new BitmapImage(new Uri(dialog.FileName));
            SourceImage.Source = sourceBitmapImage;

            Bitmap grayScaleBitmap = null;
            BitmapImage grayBitmapImage = GetGrayscaleImage(sourceBitmapImage, out grayScaleBitmap);
            GrayScaleImage.Source = grayBitmapImage;

            Dictionary<int, int> histogram = GetHistogram(grayScaleBitmap);
            int treshold = GetOtsuTreshold(histogram, grayScaleBitmap.Width * grayScaleBitmap.Height);
            Bitmap binaryBitmap = null;
            BitmapImage binaryBitmapImage = GetBinaryImage(grayScaleBitmap, out binaryBitmap, treshold);
            BinaryImage.Source = binaryBitmapImage;

            EdgeImage.Source = GetEdges(binaryBitmap);
        }

        private BitmapImage GetEdges(Bitmap sourceBitmap)
        {
            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;

            if (width < 2 || height < 2)
            {
                return null;
            }

            //todo:переделать
            if (width % 2 != 0 || height %2 != 0)
            {
                return null;
            }

            Bitmap edgeBitmap = new Bitmap(width, height);
            FeedforwardNetwork network = GetANN();

            using (StreamWriter file = new StreamWriter("debug.txt"))
            {
                for (int y = 0; y < height; y+=2)
                {
                    for (int x = 0; x < width; x+=2)
                    {
                        double[] values = 
                        { 
                                              (sourceBitmap.GetPixel(x, y).R) / 255, 
                                              (sourceBitmap.GetPixel(x + 1, y).R) / 255, 
                                              (sourceBitmap.GetPixel(x, y + 1).R) / 255, 
                                              (sourceBitmap.GetPixel(x + 1, y + 1).R) / 255
                        };

                        /*Thread thread = new Thread();
                        Bitmap srcPartBitmap = new Bitmap(2, 2);
                        for (int k = 0; k < 2; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                srcPartBitmap.SetPixel(k, l, sourceBitmap.GetPixel(i + k, j + l));
                            }
                        }
                        SourcePartImage.Source = GetBitmapImage(srcPartBitmap);*/

                         double[] output = network.ComputeOutputs(values);
                         double[,] outputValues =
                         {
                             {
                                 output[0], output[1]
                             },
                             {
                                 output[2], output[3]
                             }
                         };

                         file.Write(x + "," + y + " "+ (x + 1) + "," + y + " " + x + "," + (y + 1) + " " + (x + 1) + "," + (y + 1) + ": " 
                             + (sourceBitmap.GetPixel(x, y).R) + " " + (sourceBitmap.GetPixel(x + 1, y).R)
                             + " " + (sourceBitmap.GetPixel(x, y + 1).R) + " " + (sourceBitmap.GetPixel(x + 1, y + 1).R) + " -> ");

                         for (int y1 = 0; y1 < 2; y1++)
                         {
                             for (int x1 = 0; x1 < 2; x1++)
                             {
                                 file.Write((x + x1) + "," + (y + y1));
                                 if (x1 == 1 && y1 == 1)
                                 {
                                     file.Write(": ");
                                 }
                                 else
                                 {
                                     file.Write(" ");
                                 }
                             }
                         }

                         for (int y1 = 0; y1 < 2; y1++)
                         {
                             for (int x1 = 0; x1 < 2; x1++)
                             {
                                 double value = outputValues[y1, x1];
                                 int intValue = (int)value;
                                 if (value - intValue == 0.5)
                                 {
                                     value += 0.1;
                                 }

                                 int pixelValue = (int)(Math.Round(value) * 255);
                                 Color color = Color.FromArgb(pixelValue, pixelValue, pixelValue);

                                 edgeBitmap.SetPixel(x + x1, y + y1, color);
                                 file.Write(color.R);
                                 if (x1 == 1 && y1 == 1)
                                 {
                                     file.Write("\r\n");
                                 }
                                 else
                                 {
                                     file.Write(" ");
                                 }
                             }
                         }
                                         
                    }
                }
            }            

            edgeBitmap.Save("edges.jpg");
            return GetBitmapImage(edgeBitmap);           
        }

        private BitmapImage GetBinaryImage(Bitmap sourceBitmap, out Bitmap outBitmap, int treshold)
        {
            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;
            outBitmap = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color originalColor = sourceBitmap.GetPixel(i, j);
                    byte b = originalColor.R;

                    Color newColor;
                    if (b < treshold)
                    {
                        newColor = Color.FromArgb(0, 0, 0);
                    }
                    else
                    {
                        newColor = Color.FromArgb(255, 255, 255);
                    }
                    //set the new image's pixel
                    outBitmap.SetPixel(i, j, newColor);
                }
            }
            
            outBitmap.Save("binary.jpg");
            return GetBitmapImage(outBitmap);
        }

        private BitmapImage GetGrayscaleImage(BitmapImage source, out Bitmap grayScaleBitmap)
        {
            Bitmap sourceBitmap = null;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(source));
                enc.Save(outStream);
                sourceBitmap = new Bitmap(outStream);
            }

            grayScaleBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            for (int i = 0; i < sourceBitmap.Width; i++)
            {
                for (int j = 0; j < sourceBitmap.Height; j++)
                {
                    //get the pixel from the original image
                    Color originalColor = sourceBitmap.GetPixel(i, j);

                    //create the grayscale version of the pixel
                    int grayScale = (int)((originalColor.R * 0.299) + (originalColor.G * 0.587)
                        + (originalColor.B * 0.114));

                    //create the color object
                    Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);

                    //set the new image's pixel to the grayscale version
                    grayScaleBitmap.SetPixel(i, j, newColor);
                }
            }
       

            return GetBitmapImage(grayScaleBitmap);
        }

        private BitmapImage GetBitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapImage grayScaleBitmapImage = new BitmapImage();

            try
            {
                BitmapSource grayScaleBitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(grayScaleBitmapSource));
                    encoder.Save(memoryStream);

                    grayScaleBitmapImage.BeginInit();
                    grayScaleBitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
                    grayScaleBitmapImage.EndInit();
                }
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return grayScaleBitmapImage;
        }

        private Dictionary<int, int> GetHistogram(Bitmap bitmap)
        {
            Dictionary<int, int> histogram = new Dictionary<int, int>();

            for (int i = 0; i < 256; i++)
            {
                histogram.Add(i, 0);
            }

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color originalColor = bitmap.GetPixel(i, j);
                    byte b = originalColor.R;
                    histogram[b]++;
                }
            }

            return histogram;
        }

        private int GetOtsuTreshold(Dictionary<int, int> histogram, long total)
        {
            int sum = 0;
            for (int i = 1; i < 256; ++i)
            {
                sum += i * histogram[i];
            }

            int sumB = 0;
            int wB = 0;
            double max = 0.0;
            int threshold1 = 0;
            int threshold2 = 0;
            for (int i = 0; i < 256; ++i)
            {
                wB += histogram[i];
                if (wB == 0)
                {
                    continue;
                }

                long wF = total - wB;
                if (wF == 0)
                {
                    break;
                }

                sumB += i * histogram[i];
                double mB = (double)sumB / wB;
                double mF = (double)(sum - sumB) / wF;
                double between = wB * wF * Math.Pow(mB - mF, 2);
                if (between >= max)
                {
                    threshold1 = i;
                    if (between > max)
                    {
                        threshold2 = i;
                    }
                    max = between;
                }
            }
            return (threshold1 + threshold2) / 2;
            /*var sum = 0;
            for (var i = 1; i < 256; ++i)
                sum += i * histogram[i];
            var sumB = 0;
            var wB = 0;
            long wF = 0;
            double mB;
            double mF;
            double max = 0;
            double between;
            var threshold = 0;
            for (var i = 0; i < 256; ++i)
            {
                wB += histogram[i];
                if (wB == 0)
                    continue;
                wF = total - wB;
                if (wF == 0)
                    break;
                sumB += i * histogram[i];
                mB = sumB / wB;
                mF = (sum - sumB) / wF;
                between = wB * wF * Math.Pow(mB - mF, 2);
                if (between > max)
                {
                    max = between;
                    threshold = i;
                }
            }

            return threshold;*/
        }

        private FeedforwardNetwork GetANN()
        {
            List<String> lines = File.ReadAllLines("weights.txt").ToList();
            double[,] inputLayerWeights = new double[5, 12];
            double[,] hiddenLayerWeights = new double[13, 4];
            bool forInput = true;

            int counter = 0;
            foreach (string line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    forInput = false;
                    counter = 0;
                }
                else
                {
                    if (forInput)
                    {
                        string[] strings = line.Split(' ');
                        for (int i = 0; i < strings.Count(); i++)
                        {
                            inputLayerWeights[counter, i] = double.Parse(strings[i]);
                        }
                    }
                    else
                    {
                        string[] strings = line.Split(' ');
                        for (int i = 0; i < strings.Count(); i++)
                        {
                            hiddenLayerWeights[counter, i] = double.Parse(strings[i]);
                        }
                    }
                    counter++;
                }
            }

            HeatonResearchNeural.Matrix.Matrix inputLayerMatrix = new HeatonResearchNeural.Matrix.Matrix(inputLayerWeights);
            HeatonResearchNeural.Matrix.Matrix hiddenLayermatrix = new HeatonResearchNeural.Matrix.Matrix(hiddenLayerWeights);

            FeedforwardNetwork network = new FeedforwardNetwork();
            network.AddLayer(new FeedforwardLayer(4));
            network.AddLayer(new FeedforwardLayer(12));
            network.AddLayer(new FeedforwardLayer(4));
            network.Reset();

            FeedforwardLayer inputLayer = network.InputLayer;
            inputLayer.LayerMatrix = inputLayerMatrix;

            FeedforwardLayer hiddenLayer = network.HiddenLayers.ToList()[0];
            hiddenLayer.LayerMatrix = hiddenLayermatrix;

            return network;
        } 
    }
}
