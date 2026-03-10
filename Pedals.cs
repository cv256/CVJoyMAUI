
using static BaseUdpReceiver;

namespace CVJoyMAUI
{
    public class Pedals
    {
        BoxView boxClutch;
        BoxView boxBreak;
        BoxView boxAccel;
        BoxView boxTurbo;
        BoxView boxSep;

        public Pedals(Grid absoluteLayout, bool showTurbo)
        {
            absoluteLayout.Children.Clear();

            boxSep = new BoxView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Color = Colors.White
            };
            absoluteLayout.Add(boxSep, 1, 0);

            boxSep = new BoxView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Color = Colors.White
            };
            absoluteLayout.Add(boxSep, 3, 0);

            if (showTurbo)
            {
                boxSep = new BoxView
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Color = Colors.White
                };
                absoluteLayout.Add(boxSep, 5, 0);
            }

            boxClutch = new BoxView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                Color = Colors.Blue,
                HeightRequest = 30
            };
            absoluteLayout.Add(boxClutch, 0, 0);

            boxBreak = new BoxView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                Color = Colors.Red,
                HeightRequest = 30
            };
            absoluteLayout.Add(boxBreak, 2, 0);

            boxAccel = new BoxView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                Color = Colors.Green,
                HeightRequest = 30
            };
            absoluteLayout.Add(boxAccel, 4, 0);

            if (showTurbo)
            {
                boxTurbo= new BoxView
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.End,
                    Color = Colors.Yellow,
                    HeightRequest = 30
                };
                absoluteLayout.Add(boxTurbo, 6, 0);
            }
        }

            public void SetValues(BaseUdpReceiver udpReceiver)
        {
            double pedalsHeight = boxSep.Height;

            if (udpReceiver.Info.clutch == 0)
            {
                boxClutch.HeightRequest = 3;
                boxClutch.Color = Colors.White;
            }
            else 
            {
                boxClutch.HeightRequest = udpReceiver.Info.clutch * pedalsHeight;
                boxClutch.Color = udpReceiver.Info.clutch == 1 ? Colors.AliceBlue : Colors.Blue;
            }

            if (udpReceiver.Info.brake == 0)
            {
                boxBreak.HeightRequest = 3;
                boxBreak.Color = Colors.White;
            }
            else
            {
                boxBreak.HeightRequest = udpReceiver.Info.brake * pedalsHeight;
                boxBreak.Color = udpReceiver.Info.brake == 1 ? Colors.OrangeRed : Colors.Red;
            }

            if (udpReceiver.Info.accel == 0)
            {
                boxAccel.HeightRequest = 3;
                boxAccel.Color = Colors.White;
            }
            else
            {
                boxAccel.HeightRequest = udpReceiver.Info.accel * pedalsHeight;
                boxAccel.Color = udpReceiver.Info.accel == 1 ? Colors.LightGreen : Colors.Green;
            }

            if(boxTurbo !=null)
            {
                if (udpReceiver.Info.turbo == 0)
                {
                    boxTurbo.HeightRequest = 3;
                    boxTurbo.Color = Colors.White;
                }
                else
                {
                    boxTurbo.HeightRequest = udpReceiver.TurboPercent() * pedalsHeight;
                }
            }
        }

    }
}