using System;
using System.Windows.Input;

namespace Log2Html.ViewModel
{
    internal class RelayCommand : ICommand
    {
        private Action<object?> _execute;

        private Predicate<object?> _canExecute;

        private event EventHandler? _canExecuteChangedInternal;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="execute">Execute action</param>
        public RelayCommand(Action<object?> execute) : this(execute, DefaultCanExecute)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="execute">execute action</param>
        /// <param name="canExecute">canExecute action</param>
        /// <exception cref="ArgumentNullException">ArgumentNullException when the action is null</exception>
        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            this._execute = execute ?? throw new ArgumentNullException("execute");
            this._canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
        }

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this._canExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this._canExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object? parameter)
        {
            return this._canExecute != null && this._canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            this._execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            EventHandler? handler = this._canExecuteChangedInternal;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()
        {
            this._canExecute = _ => false;
            this._execute = _ => { return; };
        }

        private static bool DefaultCanExecute(object? parameter)
        {
            return true;
        }
    }
}
