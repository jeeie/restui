using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using System.Windows.Threading;
using ZXing;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Photonware.RestUI.Core;

namespace Photonware.RestUI.SL8.Xamls
{
    public partial class QRCodePage : PhoneApplicationPage
    {

        private string parsedContent = string.Empty;
        private PhotoCamera photoCamera = null;
        private DispatcherTimer timer = null;
        private PhotoCameraLuminanceSource luminance;
        private IBarcodeReader reader;
        private readonly WriteableBitmap dummyBitmap = new WriteableBitmap(1, 1);
        private WebClient webClient = new WebClient();

        public QRCodePage()
        {
            InitializeComponent();
            webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            photoCamera = new PhotoCamera(CameraType.Primary);
            //photoCamera.FlashMode = FlashMode.Off;
            photoCamera.CaptureStarted += photoCamera_CaptureStarted;
            photoCamera.Initialized += OnPhotoCameraInitialized;
            previewVideo.SetSource(photoCamera);

            CameraButtons.ShutterKeyHalfPressed += (o, arg) => photoCamera.Focus();

            if (timer == null)
            {
                timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                if (photoCamera.IsFocusSupported)
                {
                    photoCamera.AutoFocusCompleted += (o, arg) => { if (arg.Succeeded) ScanPreviewBuffer(); };
                    timer.Tick += (o, arg) => { try { photoCamera.Focus(); } catch (Exception) { } };
                }
                else
                {
                    timer.Tick += (o, arg) => ScanPreviewBuffer();
                }
            }

            BarcodeImage.Visibility = System.Windows.Visibility.Collapsed;
            previewRect.Visibility = System.Windows.Visibility.Visible;
            timer.Start();
        }

        void photoCamera_CaptureStarted(object sender, EventArgs e)
        {
            try
            {
                photoCamera.FlashMode = FlashMode.Off;
            }
            catch { }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
            if (photoCamera != null)
            {
                try
                {
                    photoCamera.CaptureStarted -= photoCamera_CaptureStarted;
                    photoCamera.Initialized -= OnPhotoCameraInitialized;
                    photoCamera.Dispose();
                }
                catch { }
                photoCamera = null;
            }

            if (webClient != null)
            {
                try
                {
                    webClient.DownloadStringCompleted -= webClient_DownloadStringCompleted;
                    webClient.CancelAsync();
                    webClient = null;
                }
                catch { }
            }
        }

        private void OnPhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
        {
            try
            {
                photoCamera.FlashMode = FlashMode.Off;
            }
            catch { }

            var width = Convert.ToInt32(photoCamera.PreviewResolution.Width);
            var height = Convert.ToInt32(photoCamera.PreviewResolution.Height);

            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    previewTransform.Rotation = photoCamera.Orientation;
                    // create a luminance source which gets its values directly from the camera
                    // the instance is returned directly to the reader
                    luminance = new PhotoCameraLuminanceSource(width, height);
                    reader = new BarcodeReader(null, bmp => luminance, null);
                }
                catch { }
            });
        }

        private void ScanPreviewBuffer()
        {
            if (luminance == null)
                return;

            try
            {
                photoCamera.GetPreviewBufferY(luminance.PreviewBufferY);
                // use a dummy writeable bitmap because the luminance values are written directly to the luminance buffer
                var result = reader.Decode(dummyBitmap);
                Dispatcher.BeginInvoke(() => DisplayResult(result));
            }
            catch { }
        }

        private void DisplayResult(Result result)
        {
            if (result != null)
            {
                //BarcodeType.SelectedItem = result.BarcodeFormat;
                parsedContent = result.Text;
                BarcodeContent.Text = result.Text;
                addButton.IsEnabled = true;
            }
            else
            {
                //BarcodeType.SelectedItem = null;
                //BarcodeContent.Text = "No barcode found.";
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            object saved = addButton.Content;
            addButton.Content = "Adding...";
            addButton.IsEnabled = false;
            Task.Run(() =>
            {
                webClient.DownloadStringAsync(new Uri(parsedContent));
            });
           
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Result.Trim()))
                {
                    UserCaseManager.Instance.AddUserCase(UserCase.ParseJsonString(e.Result.Trim()));
                    UserCaseManager.Instance.save();
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("Succeed to add usercase", "OK", MessageBoxButton.OK);
                    });
                    

                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("Wrong content downloaded", "Error", MessageBoxButton.OK);
                    });
                }
            }
            catch (Exception ex){
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                });

            }
            finally
            {
                if (addButton != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            addButton.Content = "Get Usercase";
                            addButton.IsEnabled = true;
                        }
                        catch { }
                    });

                }
            }
        }



    }
}