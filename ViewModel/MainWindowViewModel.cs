using MVVMCalculator.Model;
using MVVMCalculator.MVVM;
using System.ComponentModel;
using System.Windows;

namespace MVVMCalculator.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private CalculatorModel CM = new CalculatorModel(); // Instance of the Calculator Model


        private bool IsMaximized = false; // Flag to track if the window is maximized
        private bool ExecutableButton = true; // Flag to control button executability

        // Properties for the display
        public string BottomRow { get; set; } = "0";// Displayed in the bottom row
        public string TopRow { get; set; } = "";// Displayed in the top row

        // Binding for button opacity based on ExecutableButton state
        public double ButtonOpacity => ExecutableButton ? 1 : 0.5;

        // Visibility of maximize icon based on window state
        public Visibility MaximizeIconVisibility => IsMaximized ? Visibility.Collapsed : Visibility.Visible;
        
        // Visibility of restore down icon based on window state
        public Visibility RestoreDownIconVisibility => (!IsMaximized) ? Visibility.Collapsed : Visibility.Visible;

        // Commands for various actions
        public RelayCommand CloseCommand => new RelayCommand(obj => WmCloseApp());
        public RelayCommand MinimizeCommand => new RelayCommand(obj => WmMinimize());
        public RelayCommand MaximizeCommand => new RelayCommand(obj => WmMaximize());
        public RelayCommand NumCommand => new RelayCommand(WmAddNumber);
        public RelayCommand ClrCommand => new RelayCommand(WmClearDisplay);
        public RelayCommand DecPointCommand => new RelayCommand(obj => WmAddDecimalPoint(), obj => WmIsExecutable());
        public RelayCommand EqualCommand => new RelayCommand(obj => WmEqualitySymbol(), obj => WmIsExecutable());
        public RelayCommand ArtCommand => new RelayCommand(WmSetOperator, obj => WmIsExecutable());
        public RelayCommand AdvOpCommand => new RelayCommand(WmAdvOperation, obj => WmIsExecutable());
        public RelayCommand ChangeSignCommand => new RelayCommand(obj => WmChangeSign(), obj => WmIsExecutable());

        // Check if the button is executable
        private bool WmIsExecutable()
        {
            return CM.IsExecutable;
        }

        // Add a number to the calculator display
        private void WmAddNumber(object b)
        {
            CM.NumInput(b);
            BottomRow = CM.DisplayBottom;
            ExecutableButton = CM.IsExecutable;
            OnPropertyChanged(nameof(BottomRow));
            OnPropertyChanged(nameof(ButtonOpacity));
        }

        // Add a decimal point to the calculator display
        private void WmAddDecimalPoint()
        {
            CM.DecPointInput();
            BottomRow = CM.DisplayBottom;
            OnPropertyChanged(nameof(BottomRow));
        }

        // Change the sign of the number displayed
        private void WmChangeSign()
        {
            CM.ChangeSign();
            BottomRow = CM.DisplayBottom;
            OnPropertyChanged(nameof(BottomRow));
        }

        // Set an arithmetic operator
        private void WmSetOperator(object b)
        {
            CM.ArithmeticOperatorInput(b);
            BottomRow = CM.DisplayBottom;
            TopRow = CM.DisplayTop;
            OnPropertyChanged(nameof(BottomRow));
            OnPropertyChanged(nameof(TopRow));

        }

        // Perform an advanced operation
        private void WmAdvOperation(object b)
        {
            CM.AdvancedOperations(b);
            BottomRow = CM.DisplayBottom;
            TopRow = CM.DisplayTop;
            ExecutableButton = CM.IsExecutable;
            OnPropertyChanged(nameof(BottomRow));
            OnPropertyChanged(nameof(TopRow));
            OnPropertyChanged(nameof(ButtonOpacity));
        }

        // Perform the equality operation
        private void WmEqualitySymbol()
        {
            CM.EqualityFc();
            BottomRow = CM.DisplayBottom;
            TopRow = CM.DisplayTop;
            ExecutableButton = CM.IsExecutable;
            OnPropertyChanged(nameof(BottomRow));
            OnPropertyChanged(nameof(TopRow));
            OnPropertyChanged(nameof(ButtonOpacity));
        }

        // Clear the calculator display
        private void WmClearDisplay(object b)
        {
            CM.ClearFc(b);
            BottomRow = CM.DisplayBottom;
            TopRow = CM.DisplayTop;
            ExecutableButton = CM.IsExecutable;
            OnPropertyChanged(nameof(BottomRow));
            OnPropertyChanged(nameof(TopRow));
            OnPropertyChanged(nameof(ButtonOpacity));
        }

        // Close the application
        private void WmCloseApp()
        {
            Application.Current.Shutdown();
        }

        // Minimize the window
        private void WmMinimize()
        {
            Application.Current.Windows[0].WindowState = WindowState.Minimized;
        }

        // Maximize or restore down the window
        private void WmMaximize()
        {
            if (Application.Current.Windows[0].WindowState == WindowState.Maximized)
            {
                Application.Current.Windows[0].WindowState = WindowState.Normal;
                IsMaximized = false;
            }
            else
            {
                Application.Current.Windows[0].WindowState = WindowState.Maximized;
                IsMaximized = true;
            }
            OnPropertyChanged(nameof(MaximizeIconVisibility));
            OnPropertyChanged(nameof(RestoreDownIconVisibility));
        }

        // PropertyChanged event handler
        public event PropertyChangedEventHandler? PropertyChanged;

        // Notify property changed
        protected void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
