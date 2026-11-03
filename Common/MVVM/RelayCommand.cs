using System;
using System.Windows.Input;

namespace Common.MVVM
{
    /// <summary>
    /// Standard ICommand implementation used by every ViewModel command
    /// (SaveCommand, DeleteCommand, AddCommand, etc.), per rulebook Rule 6.3.
    /// Commands are instantiated lazily inside their public property getter.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Forces WPF to re-query CanExecute for all commands bound in the UI.
        /// Call after state changes that affect command availability
        /// (e.g. after IsRowSelected changes in a List ViewModel).
        /// </summary>
        public static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// Generic variant of RelayCommand for commands that need a strongly typed parameter.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute(parameter is T t ? t : default);
        }

        public void Execute(object parameter)
        {
            _execute(parameter is T t ? t : default);
        }
    }
}
