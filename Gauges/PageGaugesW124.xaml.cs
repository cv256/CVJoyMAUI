using CommunityToolkit.Mvvm.Messaging;

namespace CVJoyMAUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageGaugesW124 : ContentPage
    {
        Gauge rpmGauge;
        Gauge speedGauge;

        public PageGaugesW124()
        {
            InitializeComponent();
            this.Loaded += Page_Loaded;

            speedGauge = new Gauge(speedAbsolute);
            rpmGauge = new Gauge(rpmAbsolute);

            (Application.Current as CVJoyMAUI.App).udpReceiver.Updated += UdpReceiver_Updated;
        }
        private void Page_Loaded(object? sender, EventArgs e)
        {
            Grid1.WidthRequest = Window.Width * (Application.Current as CVJoyMAUI.App).WidthPercentage / 100; // DeviceDisplay.MainDisplayInfo.Width / Height 
            Grid1.HeightRequest = Window.Height * (Application.Current as CVJoyMAUI.App).HeightPercentage / 100;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            WeakReferenceMessenger.Default.Send(new FullScreenMessage("HideOsNavigationBar"));
        }

        private void UdpReceiver_Updated(BaseUdpReceiver udpReceiver, Boolean extra)
        {
            (Application.Current as CVJoyMAUI.App).Dispatcher.Dispatch(() =>
            {
                this.BatchBegin();
                //slipFL.Color = udpReceiver.Info.slipFL;
                //slipFR.Color = udpReceiver.Info.slipFR;
                //slipRL.Color = udpReceiver.Info.slipRL;
                //slipRR.Color = udpReceiver.Info.slipRR;
                //speedText.Text = udpReceiver.Info.speed.ToString();
                speedGauge.needleValue(udpReceiver.Info.speed);
                //gear.Text = udpReceiver.Info.gear;
                //rpm.WidthRequest = udpReceiver.RpmPercent() * horizLine1.Width;
                //rpm.Color = udpReceiver.RpmColor();
                rpmGauge.needleValue(udpReceiver.Info.rpm);
                //rpmText.Text = udpReceiver.Info.rpm.ToString();
                //gearAuto.Text = udpReceiver.Info.gearAuto ? "Auto" : "Manual";
                //double pedalsHeight = linePedals.Height;
                //clutch.HeightRequest = udpReceiver.Info.clutch * pedalsHeight;
                //brake.HeightRequest = udpReceiver.Info.brake * pedalsHeight;
                //accel.HeightRequest = udpReceiver.Info.accel * pedalsHeight;
                //double turboWidth = lineTurbo.Width;
                //turbo.WidthRequest = udpReceiver.TurboPercent() * turboWidth;

                if (extra)
                {
                    //turboMax.Text = ((Single)udpReceiver.InfoExtra.turboMax).ToString("0.0");
                    lbDistance.Text = ((Single)udpReceiver.InfoExtra.DistanceTraveled).ToString("0 0 0 0 0.0");
                    //Lap.Text = (udpReceiver.InfoExtra.CompletedLaps + 1).ToString() + " / " + udpReceiver.InfoExtra.NumberOfLaps.ToString();
                    //if (udpReceiver.InfoExtra.FuelAvg == 0)
                    //{
                    //    FuelKMs.Text = "-";
                    //    FuelAvg.Text = "-";
                    //}
                    //else
                    //{
                    //    FuelKMs.Text = ((Single)udpReceiver.InfoExtra.Fuel / udpReceiver.InfoExtra.FuelAvg * 100).ToString("0");
                    //    FuelAvg.Text = (udpReceiver.InfoExtra.FuelAvg).ToString(udpReceiver.InfoExtra.FuelAvg < 10 ? "0.0" : "0");
                    //}
                }

                this.BatchCommit();
            });
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            (Application.Current as CVJoyMAUI.App).udpReceiver.Updated -= UdpReceiver_Updated;
            (Application.Current as CVJoyMAUI.App).AskForPage();
        }

        private void speedAbsolute_SizeChanged(object sender, EventArgs e)
        {
            speedGauge.Init(20, 9999, 260,
                Colors.Transparent,
                Colors.Transparent,
                Colors.Transparent,
                Colors.Orange,
                10,
                -133, 135, Gauge.enumGaugeRadiusSize.ExpandStart,
                Colors.Black);
        }

        private void rpmAbsolute_SizeChanged(object sender, EventArgs e)
        {
            rpmGauge.Init(0, 9999, 7000,
                Colors.Transparent,
                Colors.Transparent,
                Colors.Transparent,
                Colors.Orange,
                10,
                -125, 125, Gauge.enumGaugeRadiusSize.Fit,
                Colors.Black);
        }

    }
}