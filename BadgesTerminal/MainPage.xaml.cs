using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UwpCamButton.Pages.Setting;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using clEventLoggingUWP;
using BadgesTerminal.Dialog;

namespace UwpCamButton
{
    public sealed partial class MainPage : Page
    {
        public BitmapImage img { get; set; } = new BitmapImage();

        public static string LastNamePicture
        {
            get=> _lastNamePicture;
            set
            {
                string fullPath = ApplicationData.Current.LocalCacheFolder.Path.ToString() + "\\" + _lastNamePicture;

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                _lastNamePicture = value;
            }
        }
        public static List<Activity> ListActivities { get; set; } = new List<Activity>();
        public static List<string> listLanguage { get; set; } = new List<string>()
        {
            "Czech","English","Deutch"
        };
        private static string _lastNamePicture { get; set; }
        private string password { get; set; } = "button";
        private EventLogging eventLog { get; set; } = new EventLogging("ButtonTerminal");
        /// <summary>
        /// Přidá jednu novou aktivitu a zároveň,Smaže zaznamy activit, které jsou starší jak jedna hodina.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public static void ListActivitiesAdd(string name,string description)
        {
            try
            {
                ListActivities.RemoveAll(x=> x.DateAction<DateTime.Now.AddHours(-1));
                ListActivities.Add(new Activity(name, description));
            }
            catch (Exception ex)
            {
                EventLogging.Warning(1, ex.ToString());
            }
        }

        public static readonly List<(string Tag, Type Page, string Name, int Id)> ListPages = new List<(string Tag, Type Page, string Name, int Id)>
        {
            ("SelectionImport", typeof(CamPage), "SelectionImport",2),
            ("Recapitulation", typeof(RecapitulationPage), "Recapitulation",4),
            ("Info", typeof(InfoPage), "Info",5),
            ("Tutorial", typeof(InfoPage), "Tutorial",6),
            ("SettingInfo", typeof(SettingInfoPage), "SettingInfo",7),
            ("Setting", typeof(Pages.SettingPage), "Setting",8) 
        };

        public MainPage()
        {
            InitializeComponent();
            EventLogging.Info(1,"Start aplication");
          
            //při startu nastaví předdefinovanou stránku
            ChangePage("SelectionImport");
        }

        //funkce pro přepínání stránek
        private void btnSelectView_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ChangePage(btn.Tag.ToString());
        }
        /// <summary>
        /// funkce pro přepínání stránek
        /// stačí zadat název stránky a procedura najde potřebnou stránku v obj. this.ListPages 
        /// </summary>
        /// <param name="NextPage"></param>
        public async void ChangePage(string NextPage)
        {
            if (NextPage== "Setting")
            {
                string psw = await InputTextDialogAsync();
                //*nastavení hesla z důvodu, aby se na danou stranku nedostal kde kdo
                if (psw == password)
                {
                    Type page = ListPages.Find(x => x.Tag.ToString() == NextPage).Page;
                    frame1.Navigate(page, this);
                }
                //!nastavení hesla z důvodu, aby se na danou stranku nedostal kde kdo
            }
            else
            {
                Type page = ListPages.Find(x => x.Tag.ToString() == NextPage).Page;
                frame1.Navigate(page, this);
            }
        }
        /// <summary>
        /// dialog okno pro vkládání hesla
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<string> InputTextDialogAsync()
        {
            Password_Dialog dialog = new Password_Dialog();
            await dialog.ShowAsync() ;
            return dialog.StrPassword;
        }
    }
}
