using MadPay724.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MadPay724.Test.Demo
{
     public class CalculatorTests
    {
       [Theory]
       [InlineData(3,4,7)]
       [InlineData(21,5.6,25)]
       [InlineData(3.25,1.75,5)]
        public void Add_ShouldCalculate(double x, double y, double expected)
        {
            // Arrange

            // Act
            double actual = Calculator.Add(x, y);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
