using MadPay724.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace MadPay724.Test.Demo
{
    public class LoadFileTests
    {
        [Fact]
        public void LoadTextFile_ValidFileLenth()
        {
            string actual = LoadFile.LoadTextFile("aasdas as das asd");

            Assert.True(actual.Length > 0);
        }

        [Fact]
        public void LoadTextFile_InValidFileFail()
        {
            Assert.Throws<ArgumentException>("file5",() => LoadFile.LoadTextFile(""));
        }

    }
}
