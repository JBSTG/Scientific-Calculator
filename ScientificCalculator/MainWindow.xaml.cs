/*
CMPS 3500
Spring 2020
Joel Staggs
DATE: 5/18/20
A scientific calculator written in C#.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScientificCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Operands
        double leftOperand = double.NaN;
        double centerOperand = double.NaN;
        double rightOperand = double.NaN;

        //State variables
        bool leftConfirmed = false;
        bool centerConfirmed = false;

        //variables for exp
        bool selectingPower = false;
        double storedPowerBase = double.NaN;

        //operations
        char lop = char.MinValue;
        char rop = char.MinValue;

        //Needed to check if we have a decimal stored
        bool addingDecimal = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        //Number entry
        public void num(Object sender, RoutedEventArgs e) {
            error.Content = "";
            double newDigit = double.Parse((sender as Button).Content.ToString());
            if (!leftConfirmed) {
                double newNumber;
                if (Double.IsNaN(leftOperand))
                {
                    newNumber = double.Parse(newDigit.ToString());
                }
                else if (addingDecimal)
                {
                    newNumber = double.Parse(leftOperand.ToString()+"."+ newDigit.ToString());
                    addingDecimal = false;
                }
                else {
                    newNumber = double.Parse(leftOperand.ToString() + newDigit.ToString());
                }
                info.Content = newNumber.ToString();
                leftOperand = newNumber;
            } else if (!centerConfirmed) {
                double newNumber;
                if (Double.IsNaN(centerOperand))
                {
                    newNumber = double.Parse(newDigit.ToString());
                }
                else if (addingDecimal)
                {
                    newNumber = double.Parse(centerOperand.ToString() + "." + newDigit.ToString());
                    addingDecimal = false;
                }
                else
                {
                    newNumber = double.Parse(centerOperand.ToString() + newDigit.ToString());
                }
                info.Content = newNumber.ToString();
                centerOperand = newNumber;
            } else if (leftConfirmed && centerConfirmed) {
                double newNumber;
                if (Double.IsNaN(rightOperand))
                {
                    newNumber = double.Parse(newDigit.ToString());
                }
                else if (addingDecimal)
                {
                    newNumber = double.Parse(rightOperand.ToString() + "." + newDigit.ToString());
                    addingDecimal = false;
                }
                else
                {
                    newNumber = double.Parse(rightOperand.ToString() + newDigit.ToString());
                }
                info.Content = newNumber.ToString();
                rightOperand = newNumber;
            }
        }
        //When we press '+' or other arith operation.
        public void arith(Object sender, RoutedEventArgs e) {
            char newOp = ((sender as Button).Content.ToString()[0]);
            if (!leftConfirmed) {
                if (double.IsNaN(leftOperand)) {
                    return;
                }
                leftConfirmed = true;
                info.Content = "";
                history.Content = leftOperand.ToString() + newOp;
            } else if (!centerConfirmed && !(Double.IsNaN(centerOperand))) {
                centerConfirmed = true;
                string h = history.Content.ToString();
                history.Content = h + centerOperand.ToString();
                info.Content = "";
            }
            if (lop == char.MinValue && !double.IsNaN(leftOperand)) {
                lop = newOp;
            } else if (rop == char.MinValue && centerConfirmed) {
                rop = newOp;
                string h = history.Content.ToString();
                history.Content = h + rop.ToString();
            }
            else if (!(lop == '\0') && !(rop == '\0') && !double.IsNaN(rightOperand)) {
                partialCompute(newOp);
            }
        }
        //Actually do the math.
        public double operate(double a, double b, char op) {
            if (op == '+') {
                return a + b;
            }
            if (op == '-')
            {
                return a - b;
            }
            if (op == '*')
            {
                return a * b;
            }
            if (op == '/')
            {
                if (b==0||double.IsNaN(b)) {
                    clear();
                    error.Content = "Error - Cannot divide by zero.";
                    return double.NaN;
                }
                return a / b;
            }
            return double.NaN;
        }
        //When we receive a new operation when full.
        public void partialCompute(char newOp) {
            int lw = (lop == '+' || lop == '-') ? 1 : 2;
            int rw = (rop == '+' || rop == '-') ? 1 : 2;

            if (rw > lw) {
                double ans = operate(centerOperand, rightOperand, rop);
                centerOperand = ans;
                info.Content = operate(leftOperand,centerOperand,lop).ToString();
                string h = history.Content.ToString();
                history.Content = h + rightOperand.ToString() + newOp.ToString();
                rop = newOp;
                rightOperand = double.NaN;
            }
            else {
                double ans = operate(leftOperand, centerOperand, lop);
                leftOperand = ans;
                centerOperand = rightOperand;
                info.Content = operate(leftOperand, centerOperand, rop).ToString();
                rightOperand = double.NaN;
                lop = rop;
                rop = newOp;
                string h = history.Content.ToString();
                history.Content = h + centerOperand + newOp;
            }
        }

        //run all operations and clear variables.
        public void compute(Object sender, RoutedEventArgs e) {
            double ans;
            if (rop == Char.MinValue)
            {
                if (double.IsNaN(leftOperand)||double.IsNaN(centerOperand)) {
                    return;
                }

                ans = operate(leftOperand, centerOperand, lop);
                string h = history.Content.ToString();
                history.Content = h + centerOperand.ToString();
                info.Content = ans;
            }
            else {

                if (double.IsNaN(rightOperand) || double.IsNaN(centerOperand))
                {
                    return;
                }

                int lw = (lop == '+' || lop == '-') ? 1 : 2;
                int rw = (rop == '+' || rop == '-') ? 1 : 2;


                if (rw > lw)
                {
                    ans = operate(centerOperand, rightOperand, rop);
                    ans = operate(leftOperand, ans, lop);
                }
                else {
                    ans = operate(leftOperand, centerOperand, lop);
                    ans = operate(ans, rightOperand, rop);
                }
            }
            if (!(lop == char.MinValue) || !(rop == char.MinValue)) {
                leftOperand = ans;
                leftConfirmed = false;
                centerConfirmed = false;
                centerOperand = Double.NaN;
                rightOperand = Double.NaN;
                lop = char.MinValue;
                rop = char.MinValue;
                history.Content = "";
                info.Content = ans.ToString();
            }
        }
        //Initialize all variables.
        public void clear(Object sender, RoutedEventArgs e) {
            history.Content = "";
            info.Content = "";
            error.Content = "";
            lop = Char.MinValue;
            rop = Char.MinValue;

            leftOperand = double.NaN;
            centerOperand = double.NaN;
            rightOperand = double.NaN;

            leftConfirmed = false;
            centerConfirmed = false;
            selectingPower = false;

            storedPowerBase = double.NaN;
        }

        public void clear()
        {
            history.Content = "";
            info.Content = "";
            error.Content = "";
            lop = Char.MinValue;
            rop = Char.MinValue;

            leftOperand = double.NaN;
            centerOperand = double.NaN;
            rightOperand = double.NaN;

            leftConfirmed = false;
            centerConfirmed = false;
            selectingPower = false;

            storedPowerBase = double.NaN;
        }

        //Operations
        public void sin(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed) {
                double rad = leftOperand * Math.PI / 180;
                leftOperand = Math.Sin(rad);
                info.Content = leftOperand;
            } else if (!centerConfirmed) {
                double rad = centerOperand * Math.PI / 180;
                centerOperand = Math.Sin(rad);
                info.Content = centerOperand;
            } else if (leftConfirmed&&centerConfirmed) {
                double rad = rightOperand * Math.PI / 180;
                rightOperand = Math.Sin(rad);
                info.Content = rightOperand;
            }
        }
        public void cos(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                double rad = leftOperand * Math.PI / 180;
                leftOperand = Math.Cos(rad);
                info.Content = leftOperand;
            }
            else if (!centerConfirmed)
            {
                double rad = centerOperand * Math.PI / 180;
                centerOperand = Math.Cos(rad);
                info.Content = centerOperand;
            }
            else if (leftConfirmed && centerConfirmed)
            {
                double rad = rightOperand * Math.PI / 180;
                rightOperand = Math.Cos(rad);
                info.Content = rightOperand;
            }
        }
        public void tan(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                if (leftOperand%180==90) {
                    error.Content = "Error - tangent undefined.";
                    return;
                }
                double rad = leftOperand * Math.PI / 180;
                leftOperand = Math.Tan(rad);
                info.Content = leftOperand;
            }
            else if (!centerConfirmed)
            {
                if (centerOperand % 180 == 90)
                {
                    error.Content = "Error - tangent undefined.";
                    return;
                }
                double rad = centerOperand * Math.PI / 180;
                centerOperand = Math.Tan(rad);
                info.Content = centerOperand;
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (rightOperand % 180 == 90)
                {
                    error.Content = "Error - tangent undefined.";
                    return;
                }
                double rad = rightOperand * Math.PI / 180;
                rightOperand = Math.Tan(rad);
                info.Content = rightOperand;
            }
        }

        public void exp(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                if (selectingPower)
                {
                    if (!double.IsNaN(leftOperand)) {
                        leftOperand = Math.Pow(storedPowerBase, leftOperand);
                        storedPowerBase = double.NaN;
                        selectingPower = false;
                        info.Content = leftOperand;
                    }
                }
                else {
                    if (!double.IsNaN(leftOperand)) {
                        selectingPower = true;
                        storedPowerBase = leftOperand;
                        leftOperand = double.NaN;
                    }
                }
            }
            else if (!centerConfirmed)
            {
                if (selectingPower)
                {
                    if (!double.IsNaN(centerOperand))
                    {
                        centerOperand = Math.Pow(storedPowerBase, centerOperand);
                        storedPowerBase = double.NaN;
                        selectingPower = false;
                        info.Content = centerOperand;
                    }
                }
                else
                {
                    if (!double.IsNaN(centerOperand))
                    {
                        selectingPower = true;
                        storedPowerBase = centerOperand;
                        centerOperand = double.NaN;
                    }
                }
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (selectingPower)
                {
                    if (!double.IsNaN(rightOperand))
                    {
                        rightOperand = Math.Pow(storedPowerBase, rightOperand);
                        storedPowerBase = double.NaN;
                        selectingPower = false;
                        info.Content = rightOperand;
                    }
                }
                else
                {
                    if (!double.IsNaN(rightOperand))
                    {
                        selectingPower = true;
                        storedPowerBase = rightOperand;
                        rightOperand = double.NaN;
                    }
                }
            }
        }
        //Natural log
        public void ln(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                if (!double.IsNaN(leftOperand)) {
                    if (leftOperand<0) {
                        error.Content = "Error - Operand must be non-negative.";
                        return;
                    }
                    leftOperand = Math.Log(leftOperand);
                    info.Content = leftOperand;
                }
            }
            else if (!centerConfirmed)
            {
                if (!double.IsNaN(centerOperand))
                {
                    if (centerOperand < 0)
                    {
                        error.Content = "Error - Operand must be non-negative.";
                        return;
                    }
                    centerOperand = Math.Log(centerOperand);
                    info.Content = centerOperand;
                }
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (!double.IsNaN(rightOperand))
                {
                    if (rightOperand < 0)
                    {
                        error.Content = "Error - Operand must be non-negative.";
                        return;
                    }
                    rightOperand = Math.Log(rightOperand);
                    info.Content = rightOperand;
                }
            }
        }
        //2^X
        public void pow2(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                if (!Double.IsNaN(leftOperand)) {
                    leftOperand = Math.Pow(2,leftOperand);
                    info.Content = leftOperand;
                }
            }
            else if (!centerConfirmed)
            {
                if (!Double.IsNaN(centerOperand))
                {
                    centerOperand = Math.Pow(2, centerOperand);
                    info.Content = centerOperand;
                }
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (!Double.IsNaN(rightOperand))
                {
                    rightOperand = Math.Pow(2, rightOperand);
                    info.Content = rightOperand;
                }
            }
        }
        //3^X
        public void pow3(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                if (!double.IsNaN(leftOperand)) {
                    leftOperand = Math.Pow(3,leftOperand);
                    info.Content = leftOperand.ToString();
                }
            }
            else if (!centerConfirmed)
            {
                if (!double.IsNaN(centerOperand))
                {
                    centerOperand = Math.Pow(3, centerOperand);
                    info.Content = centerOperand.ToString();
                }
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (!double.IsNaN(rightOperand))
                {
                    rightOperand = Math.Pow(3, rightOperand);
                    info.Content = rightOperand.ToString();
                }
            }
        }
        //sqrt, positive only.
        public void sqrt(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                if (leftOperand<0) {
                    error.Content = "Error - Operand must be non-negative.";
                    return;
                }
                leftOperand = Math.Sqrt(leftOperand);
                info.Content = leftOperand.ToString();
            }
            else if (!centerConfirmed)
            {
                if (centerOperand < 0)
                {
                    error.Content = "Error - Operand must be non-negative.";
                    return;
                }
                centerOperand = Math.Sqrt(centerOperand);
                info.Content = centerOperand.ToString();
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (rightOperand < 0)
                {
                    error.Content = "Error - Operand must be non-negative.";
                    return;
                }
                rightOperand = Math.Sqrt(rightOperand);
                info.Content = rightOperand.ToString();
            }
        }
        //cb root
        public void cbroot(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                leftOperand = Math.Cbrt(leftOperand);
                info.Content = leftOperand.ToString();
            }
            else if (!centerConfirmed)
            {
                centerOperand = Math.Cbrt(centerOperand);
                info.Content = centerOperand.ToString();
            }
            else if (leftConfirmed && centerConfirmed)
            {
                rightOperand = Math.Cbrt(rightOperand);
                info.Content = rightOperand.ToString();
            }
        }
        //tell number() to add a decimal point when it is next called.
        public void dec(Object sender, RoutedEventArgs e)
        {
            if (!leftConfirmed)
            {
                if (!double.IsNaN(leftOperand))
                {
                    if (leftOperand.ToString().IndexOf('.') == -1&&!addingDecimal) {
                        addingDecimal = true;
                        string h = info.Content.ToString();
                        info.Content = h + ".";
                    }
                }
                else {
                    if (!addingDecimal) {
                        leftOperand = 0;
                        addingDecimal = true;
                        info.Content = "0.";
                    }
                }
            }
            else if (!centerConfirmed)
            {
                if (!double.IsNaN(centerOperand))
                {
                    if (centerOperand.ToString().IndexOf('.') == -1 && !addingDecimal)
                    {
                        addingDecimal = true;
                        string h = info.Content.ToString();
                        info.Content = h + ".";
                    }
                }
                else
                {
                    if (!addingDecimal)
                    {
                        centerOperand = 0;
                        addingDecimal = true;
                        info.Content = "0.";
                    }
                }
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (!double.IsNaN(rightOperand))
                {
                    if (rightOperand.ToString().IndexOf('.') == -1 && !addingDecimal)
                    {
                        addingDecimal = true;
                        string h = info.Content.ToString();
                        info.Content = h + ".";
                    }
                }
                else
                {
                    if (!addingDecimal)
                    {
                        rightOperand = 0;
                        addingDecimal = true;
                        info.Content = "0.";
                    }
                }
            }
        }
        //Flip sign.
        public void sign(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                if (!Double.IsNaN(leftOperand)) {
                    leftOperand *= -1;
                    info.Content = leftOperand.ToString();
                }
            }
            else if (!centerConfirmed)
            {
                if (!Double.IsNaN(centerOperand))
                {
                    centerOperand *= -1;
                    info.Content = centerOperand.ToString();
                }
            }
            else if (leftConfirmed && centerConfirmed)
            {
                if (!Double.IsNaN(rightOperand))
                {
                    rightOperand *= -1;
                    info.Content = rightOperand.ToString();
                }
            }
        }
        //Clear immediate entry.
        public void ce(Object sender, RoutedEventArgs e) {
            if (!leftConfirmed)
            {
                leftOperand = double.NaN;
                info.Content = "";
            }
            else if (!centerConfirmed)
            {
                centerOperand = double.NaN;
                info.Content = "";
            }
            else if (leftConfirmed && centerConfirmed)
            {
                rightOperand = double.NaN;
                info.Content = "";
            }
        }
        public void printDiag() {
            Debug.WriteLine(leftOperand);
            Debug.WriteLine(lop);
            Debug.WriteLine(centerOperand);
            Debug.WriteLine(rop);
            Debug.WriteLine(rightOperand);
        }
    }
}
