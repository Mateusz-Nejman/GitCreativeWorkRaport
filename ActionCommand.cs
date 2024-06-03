using System.Windows.Input;

namespace GitCreativeWorkRaport
{
    internal class AsyncCommand : ICommand
    {
        #region Fields
        private readonly Func<Task> _action;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public AsyncCommand(Func<Task> action)
        {
            this._action = action;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        #region Public Methods
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action?.Invoke();
        }
        #endregion

    }
    internal class ActionCommand<T> : ICommand
    {
        #region Fields
        private readonly Action<T> _action;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public ActionCommand(Action<T> action)
        {
            this._action = action;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        #region Public Methods
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                return;
            }

            _action?.Invoke((T)parameter);
        }
        #endregion
    }

    internal class ActionCommand : ICommand
    {
        #region Fields
        private readonly Action _action;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public ActionCommand(Action action)
        {
            this._action = action;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        #region Public Methods
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action?.Invoke();
        }
        #endregion
    }
}
