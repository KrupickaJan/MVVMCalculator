using System;
using System.CodeDom.Compiler;
using System.Windows.Input;

namespace MVVMCalculator.MVVM
{
    internal class RelayCommand : ICommand
    {
        // Action to be executed.
        // It represents a method that accepts a single parameter of type object and returns void
        // and is used to store the action that needs to be executed when the command is invoked.
        private Action<object> execute;

        // Function to determine if action can be executed.
        // It represents a method that accepts a single parameter of type object and returns a boolean value.
        // and is used to store a function that determines whether the associated command can be executed.
        // If canExecute returns true, the command is considered enabled and can be executed.If it returns false,
        // the command is disabled and cannot be executed.
        private Func<object, bool> canExecute;

        // Mandatory event handler for CanExecuteChanged event
        public event EventHandler? CanExecuteChanged;

        // Constructor for RelayCommand
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        // Determines if the command can execute
        public bool CanExecute(object? parameter)
        {
            // If canExecute is null or evaluates to true, return true; otherwise, return false
            return canExecute == null || canExecute(parameter);
        }

        // Executes the command
        public void Execute(object? parameter)
        {
            // Execute the action
            execute(parameter);
        }
    }
}