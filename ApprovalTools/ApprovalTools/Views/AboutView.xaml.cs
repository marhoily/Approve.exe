using ApprovalTools.Approve.ViewModels;

namespace ApprovalTools.Approve.Views
{
    public partial class AboutView 
    {
        public AboutView()
        {
            InitializeComponent();
            DataContext = new AboutViewModel();
        }
    }
}
