namespace CVJoyMAUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageButtonBox : ContentPage
    {

        public PageButtonBox()
        {
            InitializeComponent();

            string[] confs = (Application.Current as CVJoyMAUI.App).ButtonboxConfiguration.Split('|');
            int rows = -1; int columns = -1;
            foreach (string x in confs)
            {
                if (x == "") continue;
                string[] conf = x.Split('\t');
                int row = int.Parse(conf[0]);
                int column = int.Parse(conf[1]);
                if (row > rows) rows = row;
                if (column > columns) columns = column;
            }

            for (int i = 0; i < rows; i++)
            {
                grid1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < columns; i++)
            {
                grid1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            foreach (string x in confs)
            {
                if (x == "") continue;
                string[] conf = x.Split('\t');
                if (conf.Length != 5) continue;
                int pRow = int.Parse(conf[0]);
                int pColumn = int.Parse(conf[1]);
                Button b = new Button
                {
                    Text = conf[2],
                    FontSize = 28,
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Colors.LightSteelBlue,
                    BorderColor = Colors.DarkBlue,
                    BorderWidth = 9,
                    CornerRadius = 20,
                    Padding = new Thickness(0, -17, 0, 0),
                    ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, -2),
                    ImageSource = conf[4]
                }; // <Button Text="Calibrate"  Grid.Row="0" Grid.Column="4"   ContentLayout="Top,-2" Padding="0,-17,0,0"    FontSize="28" FontAttributes="Bold" ImageSource="steer.png" BackgroundColor = "LightSteelBlue" BorderColor = "DarkBlue" BorderWidth=" 9" CornerRadius="20" />
                b.Clicked += ButtonBox_Clicked;
                grid1.Add(b, pColumn, pRow);
            }

            Grid.SetRow(button1, 2);
            Grid.SetColumn(button1, 4);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            (Application.Current as CVJoyMAUI.App).AskForPage();
        }

        private void ButtonBox_Clicked(object sender, EventArgs e)
        {
            (Application.Current as CVJoyMAUI.App).udpReceiver.SendButtonBoxCommand((sender as Button).Text);
        }

    }
}