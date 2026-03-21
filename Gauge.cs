using Color = Microsoft.Maui.Graphics.Color;

namespace CVJoyMAUI
{
    public class Gauge
    {
        public Microsoft.Maui.Controls.AbsoluteLayout absoluteLayout;
        Label needle;
        public int valueMin;
        public int valueMax;
        public int angleMin;
        public int angleMax;
        public int valueRedLine;
        public Double needleWidth;
        public enumGaugeRadiusSize radiusSize;
        public int valueMode2;

        public Gauge(Microsoft.Maui.Controls.AbsoluteLayout pAbsoluteLayout, int pvalueMode2 = 0)
        {
            absoluteLayout = pAbsoluteLayout;
            valueMode2 = pvalueMode2;
        }

        public enum enumGaugeRadiusSize
        {
            Fit,
            ExpandStart,
            ExpandCenter,
            ExpandEnd
        }

        public enum enumSpecialModes
        {
            Normal,
            BmwM2Speed
        }


        public void Init(int pValueMin, int pValueRedLine, int pValueMax, Color ticks1Color, Color ticks2Color, Color ticks3Color, Color needleColor, Double pNeedleWidth, int pAngleMin, int pAngleMax, enumGaugeRadiusSize pRadiusSize, Color needleCenterColor)
        {
            valueMin = pValueMin;
            valueRedLine = pValueRedLine;
            valueMax = pValueMax;
            angleMin = pAngleMin;
            angleMax = pAngleMax;
            needleWidth = pNeedleWidth;
            radiusSize = pRadiusSize;

            // tive de usar o Dispatcher porque no Android 6 nao mostrava os Gauges. E o IA diz: para garantir que o código que acede ao absoluteLayout seja executado na thread correta, porque esta função Init pode ser chamada a partir de outra thread (ex: a thread do timer que actualiza os valores do gauge):
            (Application.Current as CVJoyMAUI.App).Dispatcher.Dispatch(() =>
            {

                absoluteLayout.BatchBegin();

                absoluteLayout.Children.Clear();

                // Get the center and radius of the AbsoluteLayout
                Point center = new Point(absoluteLayout.Width / 2, absoluteLayout.Height / 2);
                Double radius;
                if (radiusSize != enumGaugeRadiusSize.Fit)
                {
                    if (absoluteLayout.Width > absoluteLayout.Height)
                    {
                        radius = 0.5 * absoluteLayout.Width;
                        if (radiusSize == enumGaugeRadiusSize.ExpandStart)
                        {
                            center.Y = radius;
                        }
                        else if (radiusSize == enumGaugeRadiusSize.ExpandEnd)
                        {
                            center.Y = absoluteLayout.Height - radius;
                        }
                    }
                    else
                    {
                        radius = 0.5 * absoluteLayout.Height;
                        if (radiusSize == enumGaugeRadiusSize.ExpandStart)
                        {
                            center.X = radius;
                        }
                        else if (radiusSize == enumGaugeRadiusSize.ExpandEnd)
                        {
                            center.X = absoluteLayout.Width - radius;
                        }
                    }
                }
                else
                {
                    radius = 0.5 * Math.Min(absoluteLayout.Width, absoluteLayout.Height);
                }

                if (valueMode2 == 0)
                {
                    drawCircle(center, radius, pAngleMin, pAngleMax, pValueMin, pValueMax, ticks1Color, ticks2Color, ticks3Color);
                }
                else
                {
                    int angleMid = (pAngleMax - pAngleMin) / 2;
                    drawCircle(center, radius, pAngleMin, pAngleMin + angleMid, pValueMin, valueMode2, ticks1Color, ticks2Color, ticks3Color);
                    drawCircle(center, radius, pAngleMin + angleMid, pAngleMax, valueMode2, pValueMax, ticks1Color, ticks2Color, ticks3Color);
                }

                // red line:
                if (pValueRedLine < valueMax)
                {
                    double redLineAngle = GetAngle(pValueRedLine);
                    Double redLineX = center.X + radius * Math.Sin(redLineAngle / 180 * Math.PI);
                    Double redLineY = center.Y - radius * Math.Cos(redLineAngle / 180 * Math.PI);
                    absoluteLayout.Children.Add(new BoxView
                    {
                        AnchorY = 0,
                        TranslationX = redLineX,
                        TranslationY = redLineY,
                        WidthRequest = 3,
                        HeightRequest = radius * .75,
                        Rotation = redLineAngle,
                        Color = Colors.Red
                    });
                }

                // needle:
                int needleCenterRadius = 7 + (int)(needleWidth * 2);
                Button needleCenterBox = new Button
                {
                    TranslationX = center.X - needleCenterRadius,
                    TranslationY = center.Y - needleCenterRadius,
                    WidthRequest = needleCenterRadius * 2,
                    HeightRequest = needleCenterRadius * 2,
                    CornerRadius = needleCenterRadius,
                    BackgroundColor = needleCenterColor
                };
                absoluteLayout.Children.Add(needleCenterBox);
                needleCenterBox.Clicked += NeedleCenterBox_Clicked;

                const Double needleOffset = 0.85;
                Double needleHeight = .9 * radius;
                //needle = new BoxView // a needle é um rectangulo estreito e alto.  O centro de rotação (Anchor) é quase no o topo desse rectangulo.
                //{
                //    WidthRequest = needleWidth,
                //    HeightRequest = needleHeight,
                //    TranslationX = center.X - .5 * needleWidth,
                //    TranslationY = center.Y - needleOffset * needleHeight,
                //    AnchorX = .5,
                //    AnchorY = needleOffset,
                //    Rotation = angleMin,
                //    Color = needleColor,
                //    CornerRadius = 3
                //};
                // tive de usar Label em vez de BoxView porque o BoxView no Android 6 ignorava os Anchor e fazia a Rotation fora do lugar. 
                needle = new Label // a needle é um rectangulo estreito e alto.  O centro de rotação (Anchor) é quase no o topo desse rectangulo.
                {
                    WidthRequest = needleWidth,
                    HeightRequest = needleHeight,
                    TranslationX = center.X - .5 * needleWidth,
                    TranslationY = center.Y - needleOffset * needleHeight,
                    AnchorY = needleOffset,
                    Rotation = angleMin,
                    BackgroundColor=needleColor
                };

                absoluteLayout.Children.Add(needle);

                absoluteLayout.BatchCommit();
            }); // ...Dispatcher.Dispatch
        }


        private void drawCircle(Point center, Double radius, int pAngleMin, int pAngleMax, int pValueMin, int pValueMax, Color ticks1Color, Color ticks2Color, Color ticks3Color)
        {
            int valueBigStep;
            double degreesPerVal = (double)(pAngleMax - pAngleMin) / (double)(pValueMax - pValueMin);
            if (degreesPerVal < .8)
            {
                valueBigStep = 1000;
            }
            else if (degreesPerVal < 1.6)
            {
                valueBigStep = 20;
            }
            else
            {
                valueBigStep = 10;
            }

            Double bigStepsCount = (pValueMax - pValueMin) / valueBigStep;
            Double angleBigStep = (pAngleMax - pAngleMin) / bigStepsCount;
            double redLineAngle = GetAngle(valueRedLine); // int redLineAngle = pAngleMin + (int)((double)(valueRedLine - pValueMin) / (pValueMax - pValueMin) * (pAngleMax - pAngleMin));

            int value = pValueMin;
            for (Double angle = pAngleMin; angle <= pAngleMax + 5; angle += angleBigStep)
            {
                // big ticks:
                if (ticks1Color != Colors.Transparent)
                {
                    Double x = center.X + radius * Math.Sin(angle / 180 * Math.PI);
                    Double y = center.Y - radius * Math.Cos(angle / 180 * Math.PI);
                    absoluteLayout.Children.Add(new BoxView
                    {
                        AnchorY = 0,
                        TranslationX = x,
                        TranslationY = y,
                        WidthRequest = 3,
                        HeightRequest = 30,
                        Rotation = angle,
                        Color = value >= valueRedLine && valueRedLine != valueMax ? Colors.Red : ticks1Color
                    });
                    // text:
                    const int textSize = 25;
                    Double textX = center.X + (radius - 30 - 5 - textSize) * Math.Sin(angle / 180 * Math.PI);
                    Double textY = center.Y - (radius - 30 - 5 - textSize) * Math.Cos(angle / 180 * Math.PI);
                    absoluteLayout.Children.Add(new Label
                    {
                        TranslationX = textX - textSize * value.ToString().Length / 3.8,
                        TranslationY = textY - textSize / 1.5,
                        TextColor = value >= valueRedLine && valueRedLine != valueMax ? Colors.Red : Colors.White,
                        FontSize = textSize,
                        Text = value.ToString()
                    });
                    //absoluteLayout.Children.Add(new BoxView
                    //{
                    //    TranslationX = textX ,
                    //    TranslationY = textY ,
                    //    WidthRequest = 2,
                    //    HeightRequest = 2,
                    //    Color = Colors.Pink
                    //});
                }

                if (ticks2Color != Colors.Transparent)
                {
                    if (angle + 2 < pAngleMax)
                    {
                        // small ticks:
                        Double subAngle = angle + angleBigStep * .5;
                        Double subX = center.X + radius * Math.Sin(subAngle / 180 * Math.PI);
                        Double subY = center.Y - radius * Math.Cos(subAngle / 180 * Math.PI);
                        absoluteLayout.Children.Add(new BoxView
                        {
                            AnchorY = 0,
                            TranslationX = subX,
                            TranslationY = subY,
                            WidthRequest = 2,
                            HeightRequest = 20,
                            Rotation = subAngle,
                            Color = subAngle > redLineAngle ? Colors.Red : ticks2Color
                        });
                    }

                    if (ticks3Color != Colors.Transparent)
                    {
                        // tiny ticks:
                        Double subAngle = angle + angleBigStep * .25;
                        Double subX = center.X + radius * Math.Sin(subAngle / 180 * Math.PI);
                        Double subY = center.Y - radius * Math.Cos(subAngle / 180 * Math.PI);
                        absoluteLayout.Children.Add(new BoxView
                        {
                            AnchorY = 0,
                            TranslationX = subX,
                            TranslationY = subY,
                            WidthRequest = 1,
                            HeightRequest = 10,
                            Rotation = subAngle,
                            Color = subAngle > redLineAngle ? Colors.Red : ticks3Color
                        });
                        subAngle = angle + angleBigStep * .75;
                        subX = center.X + radius * Math.Sin(subAngle / 180 * Math.PI);
                        subY = center.Y - radius * Math.Cos(subAngle / 180 * Math.PI);
                        absoluteLayout.Children.Add(new BoxView
                        {
                            AnchorY = 0,
                            TranslationX = subX,
                            TranslationY = subY,
                            WidthRequest = 1,
                            HeightRequest = 10,
                            Rotation = subAngle,
                            Color = subAngle > redLineAngle ? Colors.Red : ticks3Color
                        });
                    }
                }

                value += valueBigStep;
            }


        }

        private void NeedleCenterBox_Clicked(object sender, EventArgs e)
        {
            PageGaugeProps detailPage = new PageGaugeProps(this);
            absoluteLayout.Navigation.PushModalAsync(detailPage);
        }

        public void needleValue(int value)
        {
            if (needle is null) return;
            needle.Rotation = GetAngle(value);
        }

        public double GetAngle(int value)
        {
            if (valueMode2 > 0)
            {
                if (value <= valueMode2)
                {
                    return angleMin + ((double)(value) / valueMode2 * (angleMax - angleMin) / 2);
                }
                else
                {
                    return angleMin + (angleMax - angleMin) / 2 + ((double)(value - valueMode2) / (valueMax - valueMode2) * (angleMax - angleMin) / 2);
                }
            }
            else
            {
                return angleMin + ((double)(value - valueMin) / (valueMax - valueMin) * (angleMax - angleMin));
            }
        }

    }
}