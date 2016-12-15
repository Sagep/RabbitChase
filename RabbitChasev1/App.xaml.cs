using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace RabbitChasev1
{
    sealed partial class App : Application
    {
        public GamePage game;
        public Launch launched;
        public string theme="Forest";
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var app = App.Current as App;
            // Create a main GamePage
            if (app.game == null) app.game = new GamePage(string.Empty);
            if (app.launched == null) app.launched = new Launch();
            // Place the GamePage in the current Window
            Window.Current.Content = app.launched;
            Window.Current.Activate();          
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
