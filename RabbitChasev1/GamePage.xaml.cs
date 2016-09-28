using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MonoGame.Framework;
using Windows.Phone.Media;
using Windows.Media.Capture;      //For MediaCapture  
using Windows.Media.MediaProperties;  //For Encoding Image in JPEG format  
using Windows.Storage;         //For storing Capture Image in App storage or in Picture Library  
using Windows.UI.Xaml.Media.Imaging;  //For BitmapImage. for showing image on screen we need BitmapImage format.
using Windows.Devices.Enumeration;
using System.Threading.Tasks;
// For the media library
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RabbitChasev1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly Game1 _game;
        MediaCapture captureManager;
        bool isPreviewing = false;

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();
            _game = XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, this);

        }
        private static async Task<DeviceInformation> GetCameraID(Windows.Devices.Enumeration.Panel camera)
        {
            DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == camera);

            return deviceID;
        }

        private async void InitializePreview()
        {
            captureManager = new MediaCapture();

            var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Back);

            await captureManager.InitializeAsync(new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.Photo,
                AudioDeviceId = string.Empty,
                VideoDeviceId = cameraID.Id,
            });

            StartPreview();
        }
        private async void StartPreview()
        {
            previewElement.Source = captureManager;
            await captureManager.StartPreviewAsync();
            isPreviewing = true;
        }
        private async void CleanCapture()
        {
            if (captureManager != null)
            {
                if (isPreviewing == true)
                {
                    await captureManager.StopPreviewAsync();
                    isPreviewing = false;
                }
                previewElement.Source = null;
                captureButton.Content = "capture";
                captureManager.Dispose();
            }
        }
        private void captureButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPreviewing == false)
            {
                InitializePreview();
                captureButton.Content = "cancel";
            }
            else if (isPreviewing == true)
            {
                CleanCapture();
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
        }

    }
}
