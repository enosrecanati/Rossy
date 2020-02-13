using Plugin.Media;
using Plugin.Media.Abstractions;
using Rossy.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rossy.App
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Configuration AppConfiguration { get; set; }

        public MainPage()
        {
            InitializeComponent();
            AppConfiguration = new Configuration
            {
                GeordiConfig = new Geordi.Configuration
                {
                    Endpoint = "",
                    SubscriptionKey = ""
                },
                ModemConfig = new Modem.Configuration
                {
                    Endpoint = "",
                    Key = "",
                    Region = ""
                },
                RosettaConfig = new Rosetta.Configuration
                {
                    AppIdEN = "",
                    AppIdIT = "",
                    Endpoint = "",
                    PredictionKey = "",
                    TextAnalysisEndpoint = "",
                    TextAnalysisSubscriptionKey = ""
                },
                StorageConfig = new Storage.Configuration
                {
                    ConnectionString = "",
                    ContainerName = ""
                }
            };

            TxtUtterance.Text = "what's up?";
            TxtFilePath.Text = "";
        }

        private async void BtnAnalyze_Clicked(object sender, EventArgs e)
        {
            string blobUrl = string.Empty;
            if (!string.IsNullOrWhiteSpace(TxtFilePath.Text))
            {
                blobUrl = TxtFilePath.Text;
            }
            else
            {
                await TakePicture();
            }

            string utterance;
            if (!string.IsNullOrWhiteSpace(TxtUtterance.Text))
                utterance = TxtUtterance.Text;
            else
            {
                utterance = "what's up?";
            }

            var analyzer = new Geordi(AppConfiguration);
            Geordi.AnalysisResult response = analyzer.Analyze(blobUrl, utterance);

            TxtAnalysisResult.Text = response.Log;
            DeletePicture(blobUrl);
        }

        private async void BtnPickFile_Clicked(object sender, EventArgs e)
        {
            await PickFile();
        }

        private async void BtnTakePicture_Clicked(object sender, EventArgs e)
        {
            await TakePicture();
        }

        private void DeletePicture(string blobUrl)
        {
            var storageManager = new Storage(AppConfiguration.StorageConfig);
            var blobUri = new Uri(blobUrl);
            storageManager.DeleteFile(blobUri);
        }

        private async Task PickFile()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium
            });


            if (file == null)
                return;

            var blobUrl = UploadPicture(file);

            CurrentImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

        private string UploadPicture(MediaFile file)
        {
            var extension = file.Path.Substring(file.Path.LastIndexOf('.'));

            using (Stream stream = file.GetStream())
            {
                var storageManager = new Storage(AppConfiguration.StorageConfig);
                var blobUrl = storageManager.UploadFile(stream, extension);
                TxtFilePath.Text = blobUrl;
                return blobUrl;
            }
        }

        private async Task TakePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Rossy",
                SaveToAlbum = true,
                CompressionQuality = 75,
                CustomPhotoSize = 50,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                DefaultCamera = CameraDevice.Front
            });

            if (file == null)
                return;

            await DisplayAlert("File Location", file.Path, "OK");

            var blobUrl = UploadPicture(file);

            CurrentImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }
    }
}
