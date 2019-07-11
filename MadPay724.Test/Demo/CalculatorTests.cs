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
       [InlineData(3.25,1.75,5)]
        public void Add_ShouldCalculate(double x, double y, double expected)
        {
            // Arrange

            // Act
            double actual = Calculator.Add(x, y);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(12, 6, 2)]
        public void Divide_ShouldCalculate(double x, double y, double expected)
        {
            // Arrange

            // Act
            double actual = Calculator.Divide(x, y);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Divide_DivideByZero()
        {
            // Arrange
            double expected = 0;
            // Act
            double actual = Calculator.Divide(15, 0);

            // Assert
            Assert.Equal(expected, actual);
        }

    }
}
