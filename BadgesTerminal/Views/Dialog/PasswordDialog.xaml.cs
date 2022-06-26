using Windows.UI.Xaml.Controls;

namespace UwpCamButton
{
    public sealed partial class PasswordDialog : ContentDialog
    {
        public PasswordDialog()
        {
            //this.InitializeComponent();
        }

        public string strPassword;
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            strPassword = psb.ToString();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
