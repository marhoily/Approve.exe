using System;

namespace ApprovalTools.Approve.ViewModels
{
    public class AutoDisposable : IDisposable
    {
        private readonly Action _action;
        public AutoDisposable(Action action)
        {
            _action = action;
        }

        public void Dispose() { _action(); }
    }
}