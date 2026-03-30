using CommunityToolkit.Mvvm.Messaging;

namespace CVJoyMAUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageGaugesBmwM8_2020 : ContentPage
    {
        Gauge rpmGauge;
        Gauge speedGauge;
        Pedals pedals;

        public PageGaugesBmwM8_2020()
        {
            InitializeComponent();
            this.Loaded += Page_Loaded;

            speedGauge = new Gauge(speedAbsolute);
            rpmGauge = new Gauge(rpmAbsolute);
            pedals = new Pedals(gridPedals,false);

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
                lbTime.Text = DateTime.Now.ToString("H:mm");

                //slipFL.Color = udpReceiver.Info.slipFL;
                //slipFR.Color = udpReceiver.Info.slipFR;
                //slipRL.Color = udpReceiver.Info.slipRL;
                //slipRR.Color = udpReceiver.Info.slipRR;
                //speedText.Text = udpReceiver.Info.speed.ToString();
                speedGauge.needleValue(udpReceiver.Info.speed);
                gear.Text = udpReceiver.Info.gear;
                //rpm.WidthRequest = udpReceiver.RpmPercent() * horizLine1.Width;
                //rpm.Color = udpReceiver.RpmColor();
                rpmGauge.needleValue(udpReceiver.Info.rpm);
                //rpmText.Text = udpReceiver.Info.rpm.ToString();
                lbGearAuto.Text = udpReceiver.Info.gearAuto ? "Auto" : "Manual";
                pedals.SetValues(udpReceiver);
                //double turboWidth = lineTurbo.Width;
                //turbo.WidthRequest = udpReceiver.TurboPercent() * turboWidth;

                if (extra)
                {
                    //turboMax.Text = ((Single)udpReceiver.InfoExtra.turboMax).ToString("0.0");
                    lbDistance.Text = ((Single)udpReceiver.InfoExtra.DistanceTraveled).ToString("0.0 KMs");
                    //Lap.Text = (udpReceiver.InfoExtra.CompletedLaps + 1).ToString() + " / " + udpReceiver.InfoExtra.NumberOfLaps.ToString();
                    if (udpReceiver.InfoExtra.FuelAvg == 0)
                    {
                        lbFuelKMs.Text = "- KMs";
                        //lbFuelAvg.Text = "-";
                    }
                    else
                    {
                        lbFuelKMs.Text = ((Single)udpReceiver.InfoExtra.Fuel / udpReceiver.InfoExtra.FuelAvg * 100).ToString("0") +" KMs";
                        //lbFuelAvg.Text = (udpReceiver.InfoExtra.FuelAvg).ToString(udpReceiver.InfoExtra.FuelAvg < 10 ? "0.0" : "0");
                    }
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
            speedGauge.Init(0, 9999, 300,
                Colors.Transparent,
                Colors.Transparent,
                Colors.Transparent,
                Colors.White,
                7,
                -143, 0, Gauge.enumGaugeRadiusSize.Fit,
                Colors.White);
        }

        private void rpmAbsolute_SizeChanged(object sender, EventArgs e)
        {
            rpmGauge.Init(0, 9999, 8000,
                Colors.Transparent,
                Colors.Transparent,
                Colors.Transparent,
                Colors.White,
                7,
                135, 0, Gauge.enumGaugeRadiusSize.Fit,
                Colors.White);
        }

    }
}