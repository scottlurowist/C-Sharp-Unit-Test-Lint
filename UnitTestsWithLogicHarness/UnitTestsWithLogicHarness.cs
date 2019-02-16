


using System.Collections.Generic;
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
        public void EmptyTestMethod()
        {
            
        }


        [Fact]
        public void ForeachTestMethod()
        {
            List<int> someNumbers = new List<int> { 1, 2, 3 };

            foreach (var number in someNumbers)
            {
                int x = number;
            }
        }


        [Fact]
        public void IfStatementMethod()
        {
            int x = 1;

            if (x == 1)
            {
                x = 3;
            }
        }


        /// <summary>
        /// A test method that multiple logic statements. 
        /// </summary>
        [Fact]
        public void MultipleLogicIssuesMethod_ButNoIfStatement()
        {
            int counter = 0;

            IList<int> intList = new List<int> { 1, 2, 3 };

            foreach (int val in intList)
            {
                /* Do something */
            }

            do
            {
                counter++;
            } while (counter != 5);

            counter = 0;

            while (counter != 5)
            {
                counter++;
            }

            counter = counter == 5 ? counter = 0 : counter = 1;
        }
    }
}
