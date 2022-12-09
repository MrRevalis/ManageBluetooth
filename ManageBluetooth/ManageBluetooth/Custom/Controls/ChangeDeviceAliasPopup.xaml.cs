using System;

using ManageBluetooth.Helpers;
using ManageBluetooth.Resources;

using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ManageBluetooth.Custom.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeDeviceAliasPopup : Popup
    {
        private const string LightCyan = "LightCyan";
        private const string DarkCyan = "DarkCyan";

        private double verticalPosition;
        private string originalDeviceName;

        public string DeviceName { get; set; }

        public LocalizedString ChangeName { get; set; }
        public LocalizedString Cancel { get; set; }

        public ChangeDeviceAliasPopup(string deviceName)
        {
            this.DeviceName = deviceName;

            this.ChangeName = new LocalizedString(() => AppResources.ChangeName);
            this.Cancel = new LocalizedString(() => AppResources.Cancel);

            this.verticalPosition = 0;
            this.originalDeviceName = deviceName;

            InitializeComponent();
            this.BindingContext = this;

            this.Size = new Size(Application.Current.MainPage.Width, 300);
        }

        private void EnumFrameTappedEvent(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    if (e.TotalY > 0)
                    {
                        RoundedFrame.TranslateTo(0, e.TotalY);
                        verticalPosition = e.TotalY;
                    }
                    break;
                case GestureStatus.Completed:
                    if (verticalPosition > 150)
                    {
                        RoundedFrame.TranslateTo(0, 200, 100);
                        Dismiss(null);
                    }
                    else
                    {
                        RoundedFrame.TranslateTo(0, e.TotalY);
                    }
                    break;
            }
        }

        public void ClosePopupEvent(object sender, EventArgs e)
        {
            if (Boolean.TryParse((e as TappedEventArgs).Parameter?.ToString(), out var shouldSave))
            {
                if (shouldSave)
                {
                    Dismiss(this.DeviceName);
                    return;
                }
            }

            Dismiss(null);
        }

        private void EntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue)
                || string.Equals(this.originalDeviceName, e.NewTextValue))
            {
                this.ChangeNameLabel.IsEnabled = false;
                this.ChangeNameLabel.TextColor = Helper.Get<Color>(LightCyan);
            }
            else
            {
                this.ChangeNameLabel.IsEnabled = true;
                this.ChangeNameLabel.TextColor = Helper.Get<Color>(DarkCyan);
            }
        }
    }
}