using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Windows.Threading;
using HeatonResearchNeural.Feedforward;
using HeatonResearchNeural.Feedforward.Train;
using Microsoft.Win32;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;

namespace NeuralNetworkEdgeDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#if TRAIN_NETWORK
        private BackgroundWorker annTrainingBackgroundWorker;
#endif
        private BackgroundWorker edgesDrawingBackgroundWorker;

        private static uint counter = 0;
  
        public MainWindow()
        {
            InitializeComponent();
#if TRAIN_NETWORK
            annTrainingBackgroundWorker = FindResource("AnnTrainingBackgroundWorker") as BackgroundWorker;
            annTrainingBackgroundWorker.DoWork += AnnTrainingBackgroundWorker_OnDoWork;
            annTrainingBackgroundWorker.ProgressChanged += AnnTrainingBackgroundWorker_ProgressChanged;
            annTrainingBackgroundWorker.RunWorkerCompleted += AnnTrainingBackgroundWorker_RunWorkerCompleted;
            annTrainingBackgroundWorker.WorkerReportsProgress = true;
#endif
            edgesDrawingBackgroundWorker = FindResource("EdgesDrawingBackgroundWorker") as BackgroundWorker;
            edgesDrawingBackgroundWorker.DoWork += edgesDrawingBackgroundWorker_OnDoWork;
            edgesDrawingBackgroundWorker.ProgressChanged += edgesDrawingBackgroundWorker_ProgressChanged;
            edgesDrawingBackgroundWorker.RunWorkerCompleted += edgesDrawingBackgroundWorker_RunWorkerCompleted;
            edgesDrawingBackgroundWorker.WorkerReportsProgress = true;
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();
            if (result == false)
            {
                return;
            }

            BitmapImage sourceBitmapImage = new BitmapImage(new Uri(dialog.FileName));
            SourceImage.Source = sourceBitmapImage;

            Bitmap grayScaleBitmap = null;
            BitmapImage grayBitmapImage = DrawingUtils.GetGrayscaleImage(sourceBitmapImage, out grayScaleBitmap);
            GrayScaleImage.Source = grayBitmapImage;

            Dictionary<int, int> histogram = Algorithms.GetHistogram(grayScaleBitmap);
            int treshold = Algorithms.GetOtsuTreshold(histogram, grayScaleBitmap.Width * grayScaleBitmap.Height);
            Bitmap binaryBitmap = null;
            BitmapImage binaryBitmapImage = DrawingUtils.GetBinaryImage(grayScaleBitmap, out binaryBitmap, treshold);
            BinaryImage.Source = binaryBitmapImage;

            UploadImageButton.IsEnabled = false;
            
#if TRAIN_NETWORK
            annTrainingBackgroundWorker.RunWorkerAsync();            
#endif
            edgesDrawingBackgroundWorker.RunWorkerAsync(binaryBitmapImage);
        }

        private void edgesDrawingBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            /*counter++;
            //MessageLabel.Content = e.UserState;
            double[][] inputOutputValues = e.UserState as double[][];
            double[] input = inputOutputValues[0];
            double[] output = inputOutputValues[1]; 

            Bitmap inputBitmap = new Bitmap(2, 2);

            for (int i = 0; i < input.Length; i++)
            {
                double value = input[i];
                int intValue = (int)value;
                if (value - intValue == 0.5)
                {
                    value += 0.01;
                }

                input[i] = (int)(Math.Round(value));
            }

            for (int i = 0; i < output.Length; i++)
            {
                double value = output[i];
                int intValue = (int)value;
                if (value - intValue == 0.5)
                {
                    value += 0.01;
                }

                output[i] = (int)(Math.Round(value));
            }

            inputBitmap.SetPixel(0, 0, input[0] == 0 ? Color.Black : Color.White);
            inputBitmap.SetPixel(1, 0, input[1] == 0 ? Color.Black : Color.White);
            inputBitmap.SetPixel(0, 1, input[2] == 0 ? Color.Black : Color.White);
            inputBitmap.SetPixel(1, 1, input[3] == 0 ? Color.Black : Color.White);
            //inputBitmap.Save("image" + counter + ".bmp");

            SourcePartImage.Source = DrawingUtils.GetBitmapImage(inputBitmap);

            Bitmap outputBitmap = new Bitmap(2, 2);
            outputBitmap.SetPixel(0, 0, output[0] == 0 ? Color.Black : Color.White);
            outputBitmap.SetPixel(1, 0, output[1] == 0 ? Color.Black : Color.White);
            outputBitmap.SetPixel(0, 1, output[2] == 0 ? Color.Black : Color.White);
            outputBitmap.SetPixel(1, 1, output[3] == 0 ? Color.Black : Color.White);

            DestinationPartImage.Source = DrawingUtils.GetBitmapImage(outputBitmap);*/
        }

        private void edgesDrawingBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BitmapImage edgesBitmapImage = e.Result as BitmapImage;  
            EdgeImage.Source = edgesBitmapImage;
            UploadImageButton.IsEnabled = true;
        }

        private void edgesDrawingBackgroundWorker_OnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            FeedforwardNetwork network = Algorithms.GetANN();

            e.Result = DrawingUtils.GetEdges(e.Argument as BitmapImage, network, worker);          
        }

#if TRAIN_NETWORK
        private void AnnTrainingBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MessageLabel.Content = e.UserState;
        }

        private void AnnTrainingBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FeedforwardNetwork network = e.Result as FeedforwardNetwork;
            EdgeImage.Source = DrawingUtils.GetEdges(BinaryImage.Source as BitmapImage, network);
            UploadImageButton.IsEnabled = true;
        }

        private void AnnTrainingBackgroundWorker_OnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = Algorithms.TrainANN(worker);
        }
#endif
    }
}
