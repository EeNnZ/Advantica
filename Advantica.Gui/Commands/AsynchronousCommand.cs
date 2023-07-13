using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Advantica.Gui.Commands
{
    /// <summary>
    /// Asynchronous implementation of ICommand. Not used.
    /// </summary>
    internal class AsynchronousCommand : ICommand
    {
        private readonly Action _action;
        private Func<bool> _canExecute;

        public AsynchronousCommand(Action action) : this(action, () => true) { }

        public AsynchronousCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute();
        }

        public void Execute(object? parameter)
        {
            Task.Run(() =>
            {
                _canExecute = () => false;
                _action();
                _canExecute = () => true;
            });
        }
    }

    /// <summary>
    /// Asynchronous parametrized implementation of ICommand
    /// </summary>
    internal class AsynchronousCommand<T> : ICommand
    {
        private readonly Action<T> _action;
        private Func<T, bool> _canExecute;

        public AsynchronousCommand(Action<T> action) : this(action, _ => true) { }

        public AsynchronousCommand(Action<T> action, Func<T, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter == null) return false;

            return _canExecute((T)parameter);
        }

        public void Execute(object? parameter)
        {
            if (parameter == null) return;

            Task.Run(() =>
            {
                _canExecute = x => false;
                _action((T)parameter);
                _canExecute = x => true;
            });
        }
    }
}
