using System.Windows.Input;

namespace LLC_MOD_Toolbox.Infrastructure
{
    public sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter is null)
            {
                return _canExecute?.Invoke(default) ?? true;
            }

            if (parameter is T typedParameter)
            {
                return _canExecute?.Invoke(typedParameter) ?? true;
            }

            return false;
        }

        public void Execute(object? parameter)
        {
            if (parameter is null)
            {
                _execute(default);
                return;
            }

            if (parameter is T typedParameter)
            {
                _execute(typedParameter);
            }
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
