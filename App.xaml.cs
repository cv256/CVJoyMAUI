
namespace CVJoyMAUI
{

    public delegate void DebugPrintsHandler(string debugPrints);

    public partial class App : Application
    {
        public event DebugPrintsHandler DebugPrintsUpdated;

        public BaseUdpReceiver udpReceiver;
        public string DebugPrints = "";
        public string ButtonboxConfiguration = "";
        public int WidthPercentage = 100;
        public int HeightPercentage = 100;

        public App()
        {
            InitializeComponent();

            udpReceiver = new BaseUdpReceiver();
            udpReceiver.Start(); //            udpReceiver.StartDebug();

            MainPage = new PageDigital();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public void DebugPrint(string textToAdd)
        {
            DebugPrints = DateTime.Now.ToString("mm:ss:ff") + "  " + textToAdd + Environment.NewLine + DebugPrints;
            if (DebugPrints.Length > 5000) DebugPrints = DebugPrints.Substring(4000);

            DebugPrintsUpdated?.Invoke(DebugPrints);
        }

        public async void AskForPage()
        {
            Application.Current.MainPage = new PageAskForPage();
        }
    }
}
