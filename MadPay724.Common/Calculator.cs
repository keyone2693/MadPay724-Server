using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Common
{
    public static class Calculator
    {
        public static double Add(double x, double y)
        {
            return x + y;
        }

        public static double Substract(double x, double y)
        {
            return x - y;
        }

        public static double Divide(double x, double y)
        {
            if (y!=0)
            {
                return x / y;
            }
            else
            {
                return 0;
            }
        }
    }
}
