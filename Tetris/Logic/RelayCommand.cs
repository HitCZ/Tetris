using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tetris.Logic
{
    public class RelayCommand : ICommand
    {
        #region Properties

        /// <summary>
        /// Execution logic.
        /// </summary>
        public Action ExecuteAction { get; set; }

        /// <summary>
        /// Determines whether the execution can proceed.
        /// </summary>
        public Func<bool> CanExecuteAction { get; set; }

        #endregion Properties

        #region Constructor

        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException("execute");
            CanExecuteAction = canExecute;
        }

        #endregion Constructor

        public bool CanExecute(object parameter) => CanExecuteAction is null || CanExecuteAction();


        public void Execute(object parameter) => Execute();

        public void Execute() => ExecuteAction();

        public async Task ExecuteAsync() => await Task.Run(() => ExecuteAction());

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}