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
        //private SynchronizationContext sync = SynchronizationContext.Current;
  
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
            //FeedforwardNetwork network = Algorithms.GetANN();

            /*ThreadStart thread = delegate
            {
                //Load the image in a seperate thread
                /*BitmapImage bmpImage = new BitmapImage();
                MemoryStream ms = new MemoryStream();

                //A custom class that reads the bytes of off the HD and shoves them into the MemoryStream. You could just replace the MemoryStream with something like this: FileStream fs = File.Open(@"C:\ImageFileName.jpg", FileMode.Open);
                //MediaCoder.MediaDecoder.DecodeMediaWithStream(ImageItem, true, ms);

                bmpImage.BeginInit();
                bmpImage.StreamSource = ms;
                bmpImage.EndInit();*/
                /*BitmapImage bmpImage = DrawingUtils.GetEdges(binaryBitmapImage, network);

                //**THIS LINE locks the BitmapImage so that it can be transported across threads!! 
                bmpImage.Freeze();

                //Call the UI thread using the Dispatcher to update the Image control
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    EdgeImage.Source = bmpImage;

                    //grdImageContainer.Children.Add(img);
                }));

            };*/

            //Start previously mentioned thread...
            //new Thread(thread).Start();
        }

        private void edgesDrawingBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MessageLabel.Content = e.UserState;
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
