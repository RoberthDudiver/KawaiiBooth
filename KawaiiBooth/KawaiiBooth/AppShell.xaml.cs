using KawaiiBooth.Pages;

namespace KawaiiBooth
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(CameraPage), typeof(CameraPage));

        }
    }
}
