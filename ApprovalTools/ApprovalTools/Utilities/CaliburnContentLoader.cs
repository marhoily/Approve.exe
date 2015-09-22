using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FirstFloor.ModernUI;
using FirstFloor.ModernUI.Windows;

namespace ApprovalTools.Approve.Utilities
{
    public class CaliburnContentLoader : IContentLoader
    {
        public Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                throw new InvalidOperationException();
            }

            // scheduler ensures LoadContent is executed on the current UI thread
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(() => LoadContent(uri), cancellationToken, TaskCreationOptions.None, scheduler);
        }

        protected virtual object LoadContent(Uri uri)
        {
            // don't do anything in design mode
            if (ModernUIHelper.IsInDesignMode)
            {
                return null;
            }

            var content = Application.LoadComponent(uri);
            if (content == null)
                return content;

            var vm = Caliburn.Micro.ViewModelLocator.LocateForView(content);
            if (vm == null)
                return content;

            if (content is DependencyObject)
            {
                Caliburn.Micro.ViewModelBinder.Bind(vm, content as DependencyObject, null);
            }
            return content;
        }
    }
}