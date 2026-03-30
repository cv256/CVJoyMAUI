namespace CVJoyMAUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class PageAskForPage : ContentPage
    {

        public PageAskForPage()
        {
            InitializeComponent();

            txtWidth.Text = (Application.Current as CVJoyMAUI.App).WidthPercentage.ToString();
            txtHeight.Text = (Application.Current as CVJoyMAUI.App).HeightPercentage.ToString();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (!int.TryParse(txtWidth.Text, out int tmpW) || !int.TryParse(txtHeight.Text, out int tmpH))
            {
                lbError.Text = "Wrong Width x Height percentages";
                return;
            }
             (Application.Current as CVJoyMAUI.App).WidthPercentage = tmpW;
             (Application.Current as CVJoyMAUI.App).HeightPercentage = tmpH;

            switch (((Button)sender).Text)
            {
                case "Back": // TODO:  simply returning does not work , because the previous page has already executed Application.Current.udpReceiver.Updated -= UdpReceiver_Updated; we would need to re-subscribe to the event, which is not ideal. We should probably use a different approach to navigate back to the previous page without losing the event subscription.
                    return;
                case "Button Box":
                    Application.Current.MainPage = new PageButtonBox();
                    return;
                case "Generic Digital":
                    Application.Current.MainPage = new PageDigital();
                    return;
                case "Generic Diesel":
                    Application.Current.MainPage = new PageGaugesDiesel();
                    return;
                case "Generic Fast":
                    Application.Current.MainPage = new PageGaugesFast();
                    return;
                case "Ford Focus 2015 ( 220 / 6500 )":
                    Application.Current.MainPage = new PageGaugesFordFocus_2015();
                    return;
                case "Ford Focus 2015 diesel ( 240 / 5000 )":
                    Application.Current.MainPage = new PageGaugesFordFocus_2015d();
                    return;
                case "VW Polo  ( 240 / 5500 )":
                    //Application.Current.MainPage = new PageGaugesVW_Polo();
                    break;
                case "BMW M2  ( 300 / 8000 )":
                    Application.Current.MainPage = new PageGaugesBmwM2();
                    return;
                case "BMW M8 2020  ( 330 / 8000 )":
                    Application.Current.MainPage = new PageGaugesBmwM8_2020();
                    return;
                case "Mercedes W124 1990  ( 260 / 7000 )":
                    Application.Current.MainPage = new PageGaugesW124();
                    return;
                case "Mercedes S 2008  ( 260 / 7000 )":
                    //Application.Current.MainPage = new PageGaugesMercedes_S_2008();
                    break;
                case "Mercedes S 2015  ( 260 / 8000 )":
                    //Application.Current.MainPage = new PageGaugesMercedes_S_2015();
                    break;
                case "Mercedes AMG 2021  ( 360 / 8000 )":
                    //Application.Current.MainPage = new PageGaugesMercedes_AMG_2021();
                    break;
                case "Debug":
                    Application.Current.MainPage = new PageDebug();
                    return;
            }

            lbError.Text = "Not yet implemented...";
        }

    }
}