using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsWithMagicNumbersHarness
{
    public class UnitTestsWithMagicNumbersHarness
    {
        public void NonTestMethod()
        {
        }


        [Fact]
        public void EmptyTestMethodHarness()
        {
            
        }


        [Fact]
        public void MagicNumbersTestHarness()
        {
            const int x = 7;

            List<int> foo = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var range = foo.Skip(2).Take(4);
        }
    }
}
