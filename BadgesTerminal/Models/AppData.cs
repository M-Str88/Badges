using Windows.Storage;

namespace BadgesTerminal.Class
{
    static class AppData
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static string TypePrint
        {
            get => localSettings.Values["TypePrint"] == null ? "": localSettings.Values["TypePrint"].ToString();
            set => localSettings.Values["TypePrint"] = value;
        }

        public static string strIP
        {
            get => localSettings.Values["strIP"] == null ? "" : localSettings.Values["strIP"].ToString();
            set => localSettings.Values["strIP"] = value;
        }

        public static int Port
        {
            get => (int)(localSettings.Values["Port"] == null ? 0 : localSettings.Values["Port"]);
            set => localSettings.Values["Port"] = value;
        }
    }
}
