using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UtilitiesTool_V4.ViewModels
{
    public class AsyncRelayCommand: ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute());
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute(null))
            {
                try
                {
                    _isExecuting = true;
                    //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().ConfigureAwait(false); // Không đợi ở đây để tránh deadlock tiềm ẩn
        }
    }
}
