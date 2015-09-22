using System;
using Caliburn.Micro;
using FirstFloor.ModernUI.Presentation;
using JetBrains.Annotations;

namespace ApprovalTools.Approve.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class SettingsViewModel : Screen
    {
        private Uri _selectedSource = new Uri("/Views/SettingsAppearanceView.xaml", UriKind.Relative);
        private string _statusMessage;

        public SettingsViewModel()
        {
            TabLinks = new LinkCollection(new[]
            {
                new Link
                {
                    DisplayName = "appearance",
                    Source = new Uri("/Views/SettingsAppearanceView.xaml", UriKind.Relative)
                },
                new Link
                {
                    DisplayName = "about",
                    Source = new Uri("/Views/AboutView.xaml", UriKind.Relative)
                }
            });
        }

        public LinkCollection TabLinks { get; set; }

        public Uri SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                if (_selectedSource != value)
                {
                    _selectedSource = value;
                    NotifyOfPropertyChange(() => SelectedSource);
                }
            }
        }
    }
}