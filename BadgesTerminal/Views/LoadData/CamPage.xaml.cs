using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Media;
using Windows.Graphics.Imaging;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using clEventLoggingUWP;

namespace UwpCamButton
{
    public sealed partial class CamPage : Page
    {
        public MainPage mainPage { get; set; }
        private RecapitulationPage recapitulationPage { get; set; }
        private DispatcherTimer dispatcherTimer { get; set; }
        private VideoFrame videoFrame { get; set; }
        private int _timerTickInt { get; set; } = 0;
        private int timerTickInt
        {   
            set
            { 
                _timerTickInt = value;
                txbTimerCykle.Text = _timerTickInt.ToString();

                if (_timerTickInt==0)
                {
                    txbTimerCykle.Visibility = Visibility.Collapsed;
                    BtnCapture.Visibility = Visibility.Visible;
                }
                else
                {
                    txbTimerCykle.Visibility = Visibility.Visible;
                    BtnCapture.Visibility = Visibility.Collapsed;
                }
            } 
            get
            => _timerTickInt; 
        }

        public CamPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            OpenCamera();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is MainPage)
            {
                mainPage = (MainPage)e.Parameter;
            }
            else if (e.Parameter is RecapitulationPage)
            {
                recapitulationPage = (RecapitulationPage)e.Parameter;
            }
        }
        private void CameraStart_Click(object sender, RoutedEventArgs e)
        {
            OpenCamera();
        }

        private void CameraStop_Click(object sender, RoutedEventArgs e)
        {
            CameraPreview.Stop();
        }

        public async void OpenCamera()
        {
            CameraPreview.PreviewFailed += CPTest_PreviewFailed;
            await CameraPreview.StartAsync();
            CameraPreview.CameraHelper.FrameArrived += CPTest_FrameArrived;

            MainPage.ListActivitiesAdd("Camera", "Start camera");
        }

        private void CPTest_FrameArrived(object sender, FrameEventArgs e)
        {
            videoFrame = e.VideoFrame;
        }
        private void CPTest_PreviewFailed(object sender, PreviewFailedEventArgs e)
        {
            MainPage.ListActivitiesAdd("Camera", "Error:" + e.Error.ToString());
        }
        private void BtnCaptureTimer_Click(object sender, RoutedEventArgs e)
        {
            timerTickInt = 5;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += timerTickCamera;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            MainPage.ListActivitiesAdd("Camera", "Start timer");
        }
        private async void timerTickCamera(object sender, object e)
        {
            timerTickInt -= 1;

            if (timerTickInt == 0)
            {
                dispatcherTimer.Stop();
                await startCaptureAsync();

                mainPage.ChangePage("Recapitulation");

                MainPage.ListActivitiesAdd("Camera", "Create photo");
            }
        }

        private async Task startCaptureAsync()
        {
            if (videoFrame!=null)
            {
                SoftwareBitmap softwareBitmap = videoFrame?.SoftwareBitmap;
                // Převeďte pixelový formát na Rgba16, abychom jej mohli uložit do souboru
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Rgba16);

                if (softwareBitmap != null)
                {
                    try
                    {
                        BitmapImage bmpImage2 = new BitmapImage();
                        //Poskytuje náhodný přístup k datům ve vstupních a výstupních streamech, které jsou uloženy v paměti místo na disku.
                        InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                        {
                            // Vytvořte kodér v požadovaném formátu
                            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                            //encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees;
                            // Nastavte bitmapu softwaru
                            encoder.SetSoftwareBitmap(softwareBitmap);
                            await encoder.FlushAsync();


                            await bmpImage2.SetSourceAsync(stream);
                            mainPage.img= bmpImage2;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainPage.ListActivitiesAdd("Camera", "Error:" + ex.ToString());
                        EventLogging.Error(1, ex.ToString());
                    }
                }

                MainPage.ListActivitiesAdd("Camera", "Save photo:" + ApplicationData.Current.LocalCacheFolder.Path.ToString());
            }
            else
            {
                MainPage.ListActivitiesAdd("Camera", "Pokus o vyfocení.Kamera není připojena.");
            }
        }
    }
}
