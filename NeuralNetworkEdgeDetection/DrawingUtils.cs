using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using HeatonResearchNeural.Feedforward;
using HeatonResearchNeural.Feedforward.Train;
using HeatonResearchNeural.Feedforward.Train.Backpropagation;
using HeatonResearchNeural.Matrix;

namespace NeuralNetworkEdgeDetection
{
    class DrawingUtils
    {
        public static BitmapImage GetBinaryImage(Bitmap sourceBitmap, out Bitmap outBitmap, int treshold)
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

        public static BitmapImage GetGrayscaleImage(BitmapImage source, out Bitmap grayScaleBitmap)
        {
            Bitmap sourceBitmap = GetBitmap(source);

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

        public static BitmapImage GetEdges(BitmapImage binaryBitmapImage, FeedforwardNetwork network, 
            BackgroundWorker backgroundWorker)
        {
            Bitmap sourceBitmap = GetBitmap(binaryBitmapImage);

            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;

            if (width < 2 || height < 2)
            {
                return null;
            }

            //todo:переделать
            if (width % 2 != 0 || height % 2 != 0)
            {
                return null;
            }

            Bitmap edgeBitmap = new Bitmap(width, height);
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    edgeBitmap.SetPixel(x, y, Color.White);
                }
            }

            /*using (StreamWriter file = new StreamWriter("debug.txt"))
            {*/
                for (int y = 0; y < height - 1; y += 1)
                {
                    for (int x = 0; x < width - 1; x += 1)
                    {
                        double[] values = 
                        { 
                            (sourceBitmap.GetPixel(x, y).R) / 255, 
                            (sourceBitmap.GetPixel(x + 1, y).R) / 255, 
                            (sourceBitmap.GetPixel(x, y + 1).R) / 255, 
                            (sourceBitmap.GetPixel(x + 1, y + 1).R) / 255
                        };

                        double[] output = network.ComputeOutputs(values);

                        backgroundWorker.ReportProgress(0, new[] {values, output});
                        //Thread.Sleep(300);

                        double[,] outputValues =
                        {
                            {
                                output[0], output[1]
                            },
                            {
                                output[2], output[3]
                            }
                        };

                        /*file.Write(x + "," + y + " " + (x + 1) + "," + y + " " + x + "," + (y + 1) + " " + (x + 1) + "," + (y + 1) + ": "
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
                        }*/

                        for (int y1 = 0; y1 < 2; y1++)
                        {
                            for (int x1 = 0; x1 < 2; x1++)
                            {
                                double value = outputValues[y1, x1];
                                int intValue = (int)value;
                                if (value - intValue == 0.5)
                                {
                                    value += 0.01;
                                }

                                int pixelValue = (int)(Math.Round(value) * 255);
                                Color color = Color.FromArgb(pixelValue, pixelValue, pixelValue);

                                if (edgeBitmap.GetPixel(x + x1, y + y1).R == 255)
                                {
                                    edgeBitmap.SetPixel(x + x1, y + y1, color);
                                }
                                
                               /* file.Write(color.R);
                                if (x1 == 1 && y1 == 1)
                                {
                                    file.Write("\r\n");
                                }
                                else
                                {
                                    file.Write(" ");
                                }*/
                            }
                        }

                    }
                }
           // }

            edgeBitmap.Save("edges.jpg");
            
            BitmapImage resultBitmapImage = GetBitmapImage(edgeBitmap);
            resultBitmapImage.Freeze();
            return resultBitmapImage;
        }

        private static Bitmap GetBitmap(BitmapImage bitmapImage)
        {
            Bitmap bitmap = null;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }

            return bitmap;
        }

        public static BitmapImage GetBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
