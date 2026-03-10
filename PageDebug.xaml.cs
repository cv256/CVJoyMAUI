using System;




namespace CVJoyMAUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageDebug: ContentPage
    {

        public PageDebug()
        {
            InitializeComponent();
            lbDebugPrints.Text = (Application.Current as CVJoyMAUI.App).DebugPrints;
            (Application.Current as CVJoyMAUI.App).DebugPrintsUpdated += DebugPrintsUpdated;
        }

        private void DebugPrintsUpdated(string debugPrints)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.BatchBegin();
                lbDebugPrints.Text = debugPrints;
                this.BatchCommit();
            });
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            (Application.Current as CVJoyMAUI.App).DebugPrintsUpdated -= DebugPrintsUpdated;
            (Application.Current as CVJoyMAUI.App).AskForPage();
        }

    }
}