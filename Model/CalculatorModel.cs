using System.Reflection.Metadata;

namespace MVVMCalculator.Model
{
    internal class CalculatorModel
    {
        // Fields to store calculator state
        private string _DisplayBottom = "0";    // Current value displayed at the bottom
        private string _DisplayTop = "";        // Expression displayed at the top
        private bool _IsExecutable = true;      // Flag to control executability


        // Properties to access calculator state
        public string DisplayBottom
        {
            get { return _DisplayBottom; }
        }
        public string DisplayTop
        {
            get { return _DisplayTop; }
        }
        public bool IsExecutable
        {
            get { return _IsExecutable; }
        }

        // List to store history of expressions displayed at the top
        private List<string> DisplayTopList = new List<string>();

        // Variables to store temporary values and state of the calculator
        private double TempNumber = 0;
        private string OperationSgn = "";
        private bool IsFirstNum = true;
        private bool IsLastAdvancedOp = false;
        private bool IsLastOpSgn = false;
        private bool IsNewTask = true;

        // Method to handle input of numeric values
        public void NumInput(object Parameter)
        {
            // Reset executable flag
            _IsExecutable = true;
            if (IsFirstNum)
            {
                // Append or set the display value based on whether it's the first number input
                _DisplayBottom = (string)Parameter;
                IsFirstNum = false;
                IsNewTask = false;
            }
            else
            {
                _DisplayBottom += (string)Parameter;
            }
            // Reset flag indicating the last operation is sign
            IsLastOpSgn = false;

            // Format the display bottom value
            _DisplayBottom = Decimal.TryParse(_DisplayBottom, out decimal number) ? number.ToString() : _DisplayBottom;
        }

        // Method to handle input of decimal point
        public void DecPointInput()
        {
            // Add a decimal point if not already present
            if (!_DisplayBottom.Contains('.'))
            {
                _DisplayBottom += '.';
                IsFirstNum = false;
                IsLastOpSgn = false;
            }
        }

        // Method to toggle the sign of the displayed number
        public void ChangeSign()
        {
            if (_DisplayBottom[0] == '-')
            {
                _DisplayBottom = _DisplayBottom.Remove(_DisplayBottom.Length - 1, 1);
            }
            else if (!_DisplayBottom.Equals("0"))
            {
                _DisplayBottom = "-" + _DisplayBottom;
            }
        }

        // Method to handle advanced operations (e.g., percentage, square root)
        public void AdvancedOperations(object Parameter)
        {
            // Handle clearing and resetting of state if starting a new task
            if (IsNewTask)
            {
                _DisplayTop = "";
                DisplayTopList.Clear();
                IsNewTask = false;
            }

            // Perform advanced operations based on the provided parameter
            if (((string)Parameter).Equals("%"))
            {
                // Calculate percentage
                _DisplayBottom = (TempNumber * 0.01 * double.Parse(_DisplayBottom)).ToString();
                DisplayTopList.Add(_DisplayBottom);
                IsFirstNum = true;
                IsLastOpSgn = false;
                IsLastAdvancedOp = true;
            }
            else if (((string)Parameter).Equals("1/x"))
            {
                // Calculate reciprocal
                DisplayTopList.Add("1/( " + _DisplayBottom + " ) ");
                _DisplayBottom = (1 / double.Parse(_DisplayBottom)).ToString();
                IsFirstNum = true;
                IsLastOpSgn = false;
                IsLastAdvancedOp = true;
            }
            else if (((string)Parameter).Equals("x^2"))
            {
                DisplayTopList.Add("sqr( " + _DisplayBottom + " ) ");
                _DisplayBottom = (Math.Pow(double.Parse(_DisplayBottom), 2)).ToString();
                IsFirstNum = true;
                IsLastOpSgn = false;
                IsLastAdvancedOp = true;
            }
            else if (((string)Parameter).Equals("√x"))
            {
                if((double.Parse(_DisplayBottom)) < 0)
                {
                    _DisplayBottom = "Invalid input";
                    TempNumber = 0;
                    OperationSgn = "";
                    IsFirstNum = true;
                    IsLastAdvancedOp = false;
                    IsLastOpSgn = false;
                    IsNewTask = true;
                    _IsExecutable = false;
                }
                else
                {
                    DisplayTopList.Add("√( " + _DisplayBottom + " ) ");
                    _DisplayBottom = (Math.Sqrt(double.Parse(_DisplayBottom))).ToString();
                    IsFirstNum = true;
                    IsLastOpSgn = false;
                    IsLastAdvancedOp = true;
                }
            }
            _DisplayTop = String.Join(" ", DisplayTopList);
        }

        // Method to clear the display or perform backspace operation
        public void ClearFc(object Parameter)
        {
            // Handle different types of clear operations
            if (((string)Parameter).Equals("C") || !_IsExecutable)
            {
                // Clear all
                _DisplayBottom = "0";
                _DisplayTop = "";
                DisplayTopList.Clear();
                IsFirstNum = true;
                IsNewTask = true;
                TempNumber = 0;
                OperationSgn = "";
                IsLastAdvancedOp = false;
                IsLastOpSgn = false;
                _IsExecutable = true;
            }
            else if (((string)Parameter).Equals("CE"))
            {
                // Clear entry
                _DisplayBottom = "0";
                IsFirstNum = true;
            }
            else
            {
                // Perform backspace operation
                try
                {
                    _DisplayBottom = _DisplayBottom.Remove(_DisplayBottom.Length - 1, 1);
                }
                catch (ArgumentOutOfRangeException) { }
            }

            // Ensure a valid display value
            if (_DisplayBottom.Equals(""))
            {
                _DisplayBottom = "0";
                IsFirstNum = true;
            }
        }

        // Method to handle the equality operation
        public void EqualityFc()
        {
            // Add the current display value to the top expression list if not part of an advanced operation
            if (!IsLastAdvancedOp)
            {
                DisplayTopList.Add(_DisplayBottom);
            }

            // Add the equality sign to the top expression list
            DisplayTopList.Add("=");

            // Perform arithmetic operation and update state
            ArithmeticOperation();
            IsFirstNum = true;
            IsNewTask = true;
            TempNumber = 0;
            OperationSgn = "";
            IsLastAdvancedOp = false;
            IsLastOpSgn = false;

            // Update the top display
            _DisplayTop = String.Join(" ", DisplayTopList);
            DisplayTopList.Clear();
        }

        // Method to handle input of arithmetic operators
        public void ArithmeticOperatorInput(object Parameter)
        {
            // Handle input of arithmetic operators
            if (IsLastOpSgn)
            {
                // Replace the last operator in the top expression list if already present
                OperationSgn = (string)Parameter;
                DisplayTopList.RemoveAt(DisplayTopList.Count - 1);
                DisplayTopList.Add((string)Parameter);

            }
            else if (IsLastAdvancedOp)
            {
                // Perform arithmetic operation and update state if it's an advanced operation
                ArithmeticOperation();
                TempNumber = double.Parse(_DisplayBottom);
                OperationSgn = (string)Parameter;
                DisplayTopList.Add((string)Parameter);
                IsFirstNum = true;
                IsLastOpSgn = true;
                IsLastAdvancedOp = false;
            }
            else
            {
                // Perform arithmetic operation and update state
                ArithmeticOperation();
                TempNumber = double.Parse(_DisplayBottom);
                OperationSgn = (string)Parameter;
                DisplayTopList.Clear();
                DisplayTopList.Add(_DisplayBottom);
                DisplayTopList.Add((string)Parameter);
                IsFirstNum = true;
                IsLastOpSgn = true;
                IsLastAdvancedOp = false;
            }

            // Update the top display
            _DisplayTop = String.Join(" ", DisplayTopList);
        }

        // Method to perform arithmetic operation based on the stored operator
        private void ArithmeticOperation()
        {
            // Retrieve the numbers involved in the operation
            double num1 = TempNumber;
            double num2 = double.Parse(_DisplayBottom);

            switch (OperationSgn)
            {
                case "+":
                    _DisplayBottom = (num1 + num2).ToString();
                    break;
                case "-":
                    _DisplayBottom = (num1 - num2).ToString();
                    break;
                case "×":
                    _DisplayBottom = (num1 * num2).ToString();
                    break;
                case "÷":
                    if (num2 != 0)
                    {
                        _DisplayBottom = (num1 / num2).ToString();
                        break;
                    }
                    else
                    {
                        // Handle division by zero case
                        _DisplayBottom = "Cannot divide by zero";

                        // Reset calculator state
                        TempNumber = 0;
                        OperationSgn = "";
                        IsFirstNum = true;
                        IsLastAdvancedOp = false;
                        IsLastOpSgn = false;
                        IsNewTask = true;
                        _IsExecutable = false;
                        break;
                    }
                default:
                    _DisplayBottom = num2.ToString();
                    break;
            }
        }
    }
}
