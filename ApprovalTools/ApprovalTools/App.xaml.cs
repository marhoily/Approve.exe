using FirstFloor.ModernUI.Presentation;

namespace ApprovalTools.Approve
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
        }
    }
}
