using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsWithLogicHarness
{
    public class UnitTestsWithLogicHarness
    {
        /// <summary>
        /// This is NOT a unit test. It should not trigger and diagnostics,
        /// though it has logic.
        /// </summary>
        public void NonTestMethod()
        {
            List<int> someNumbers = new List<int> { 1, 2, 3 };

            foreach (var number in someNumbers)
            {
                int y = number;
            }

            int x = 1;

            if (x == 1)
            {
                x = 3;
            }
        }


        /// <summary>
        /// This is a unit test, but it has no violations. It should
        /// not trigger any diagnsotics.
        /// </summary>
        [Fact]
        public void EmptyTestHarness()
        {
            
        }


        [Fact]
        public void ForeachTestHarness()
        {
            List<int> someNumbers = new List<int> { 1, 2, 3 };

            foreach (var number in someNumbers)
            {
                int x = number;
            }
        }


        [Fact]
        public void IfStatementHarness()
        {
            int x = 1;

            if (x == 1)
            {
                x = 3;
            }
        }
    }
}
