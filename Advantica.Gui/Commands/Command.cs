using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Advantica.Gui.Commands
{
    /// <summary>
    /// ICommand implementation. Not used.
    /// </summary>
    internal class Command : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;

        public Command(Action action)
            : this(action, () => true)
        {
        }

        public Command(Action action, Func<bool> canExecute)
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
            _action();
        }
    }

    /// <summary>
    /// Parametrized implementation of ICommand
    /// </summary>
    /// <typeparam name="T">A type that command accepts</typeparam>
    internal class Command<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _canExecute;

        public Command(Action<T> action) : this(action, _ => true) { }

        public Command(Action<T> action, Func<T, bool> canExecute)
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

            _action((T) parameter);
        }

    }
}
