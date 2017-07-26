//
// CSharpUnitTestLintWalkerFalsePositivesHarness.cs
//
// Product: CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.SolutionAnalyzerDriver.xUnitTests
//
// Description: A test harness for CSharpUnitTestLintWalker.cs that test for false positives.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//



using CSharpUnitTestLint.AnalyzerRules;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;



namespace CSharpUnitTestLint.SolutionAnalyzerDriver.xUnitTests
{
    /// <summary>
    /// A test harness for CSharpUnitTestLintWalker.cs that test for false positives.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CSharpUnitTestLintWalkerFalsePositivesHarness
    {
        #region Unit Tests

        public class TheEnhancedDiagnosticsPropertyMustNotContainEnhancedDiagnostics : CSharpUnitTestLintWalkerFalsePositivesHarness
        {
            /// <summary>
            /// Tests that the EnhancedDiagnostics property has no EnhancedDiagnostics
            /// when the method to analyze is NOT a test method, but it has rule violations.
            /// This tests that false positives are not generated. 
            /// </summary>
            [Fact]
            public void When_TheVisitMethod_Is_Invoked_And_TheMethodToAnalyze_IsNotATestMethod_ButHasRuleViolations()
            {
                // ARRANGE
                CompilationUnitSyntax compUnitSyntax =
                    UnitTestUtilities.GetAMethodDeclarationSyntaxFromSourceCode(
                        UnitTestUtilities.SourceFileWithAllViolationsButNoTests);

                CSharpUnitTestLintWalker classUnderTest = new CSharpUnitTestLintWalker(new UnitTestViolations());


                // ACT 
                classUnderTest.Visit(compUnitSyntax);


                // ASSERT 
                classUnderTest.EnhancedDiagnostics
                    .Count
                    .Should().Be(UnitTestUtilities.ZeroEnhancedDiagnostics);
            }


            /// <summary>
            /// Tests that the EnhancedDiagnostics property has no EnhancedDiagnostics
            /// when the xUnit Fact method to analyze is empty.
            /// </summary>
            [Fact]
            public void When_TheVisitMethod_Is_Invoked_And_TheXUnitFactToAnalyze_IsEmpty()
            {
                // ARRANGE
                const string sourceCodeFileToAnalyze = @"
                    public class SomeClassToAnalyze
                    {
                        [Fact]
                        public void SomeTestMethod()
                        {
                        }
                    }";

                CompilationUnitSyntax compUnitSyntax =
                    UnitTestUtilities.GetAMethodDeclarationSyntaxFromSourceCode(sourceCodeFileToAnalyze);

                CSharpUnitTestLintWalker classUnderTest = new CSharpUnitTestLintWalker(new UnitTestViolations());


                // ACT 
                classUnderTest.Visit(compUnitSyntax);


                // ASSERT 
                classUnderTest.EnhancedDiagnostics
                    .Count
                    .Should().Be(UnitTestUtilities.ZeroEnhancedDiagnostics);
            }


            /// <summary>
            /// Tests that the EnhancedDiagnostics property has no EnhancedDiagnostics
            /// when the xUnit Theory method to analyze is empty.
            /// </summary>
            [Fact]
            public void When_TheVisitMethod_Is_Invoked_And_TheXUnitTheoryToAnalyze_IsEmpty()
            {
                // ARRANGE
                const string sourceCodeFileToAnalyze = @"
                    public class SomeClassToAnalyze
                    {
                        [Theory]
                        [InlineData(1)]
                        public void SomeTestMethod(int someInlineIntegerData)
                        {
                        }
                    }";

                CompilationUnitSyntax compUnitSyntax =
                    UnitTestUtilities.GetAMethodDeclarationSyntaxFromSourceCode(sourceCodeFileToAnalyze);

                CSharpUnitTestLintWalker classUnderTest = new CSharpUnitTestLintWalker(new UnitTestViolations());


                // ACT 
                classUnderTest.Visit(compUnitSyntax);


                // ASSERT 
                classUnderTest.EnhancedDiagnostics
                    .Count
                    .Should().Be(UnitTestUtilities.ZeroEnhancedDiagnostics);
            }


            /// <summary>
            /// Tests that the EnhancedDiagnostics property has no EnhancedDiagnostics
            /// when the NUnit Test method to analyze is empty.
            /// </summary>
            [Fact]
            public void When_TheVisitMethod_Is_Invoked_And_TheNUnitTestToAnalyze_IsEmpty()
            {
                // ARRANGE
                const string sourceCodeFileToAnalyze = @"
                    public class SomeClassToAnalyze
                    {
                        [Test]
                        public void SomeTestMethod()
                        {
                        }
                    }";

                CompilationUnitSyntax compUnitSyntax =
                    UnitTestUtilities.GetAMethodDeclarationSyntaxFromSourceCode(sourceCodeFileToAnalyze);

                CSharpUnitTestLintWalker classUnderTest = new CSharpUnitTestLintWalker(new UnitTestViolations());


                // ACT 
                classUnderTest.Visit(compUnitSyntax);


                // ASSERT 
                classUnderTest.EnhancedDiagnostics
                    .Count
                    .Should().Be(UnitTestUtilities.ZeroEnhancedDiagnostics);
            }


            /// <summary>
            /// Tests that the EnhancedDiagnostics property has no EnhancedDiagnostics
            /// when the NUnit TestCase method to analyze is empty.
            /// </summary>
            [Fact]
            public void When_TheVisitMethod_Is_Invoked_And_TheNUnitTestCaseToAnalyze_IsEmpty()
            {
                // ARRANGE
                const string sourceCodeFileToAnalyze = @"
                    public class SomeClassToAnalyze
                    {
                        [TestCase(1)]
                        public void SomeTestMethod(int someInt)
                        {
                        }
                    }";

                CompilationUnitSyntax compUnitSyntax =
                    UnitTestUtilities.GetAMethodDeclarationSyntaxFromSourceCode(sourceCodeFileToAnalyze);

                CSharpUnitTestLintWalker classUnderTest = new CSharpUnitTestLintWalker(new UnitTestViolations());


                // ACT 
                classUnderTest.Visit(compUnitSyntax);


                // ASSERT 
                classUnderTest.EnhancedDiagnostics
                    .Count
                    .Should().Be(UnitTestUtilities.ZeroEnhancedDiagnostics);
            }


            /// <summary>
            /// Tests that the EnhancedDiagnostics property has no EnhancedDiagnostics
            /// when the NUnit TestCaseSource method to analyze is empty.
            /// </summary>
            [Fact]
            public void When_TheVisitMethod_Is_Invoked_And_TheNUnitTestCaseSourceToAnalyze_IsEmpty()
            {
                // ARRANGE
                const string sourceCodeFileToAnalyze = @"
                    public class SomeClassToAnalyze
                    {
                        [TestCaseSource(""SomeCases"")]
                        public void SomeTestMethod(int someInt)
                        {
                        }

                        static object[] SomeCases = {
                            new object[] { 1},
                            new object[] { 2 },
                            new object[] { 3 }
                        };
                    }";

                CompilationUnitSyntax compUnitSyntax =
                    UnitTestUtilities.GetAMethodDeclarationSyntaxFromSourceCode(sourceCodeFileToAnalyze);

                CSharpUnitTestLintWalker classUnderTest = new CSharpUnitTestLintWalker(new UnitTestViolations());


                // ACT 
                classUnderTest.Visit(compUnitSyntax);


                // ASSERT 
                classUnderTest.EnhancedDiagnostics
                    .Count
                    .Should().Be(UnitTestUtilities.ZeroEnhancedDiagnostics);
            }


            /// <summary>
            /// Tests that the EnhancedDiagnostics property has no EnhancedDiagnostics
            /// when the NUnit Theory method to analyze is empty.
            /// </summary>
            [Fact]
            public void When_TheVisitMethod_Is_Invoked_And_TheNUnitTheoryToAnalyze_IsEmpty()
            {
                // ARRANGE
                const string sourceCodeFileToAnalyze = @"
                    public class SomeClassToAnalyze
                    {
                        [DatapointSource]
                        public double[] values = new double[] { 0.0, 1.0, -1.0, 42.0 };


                        [Theory]
                        public void SomeTestMethod(int someDouble)
                        {
                        }
                    }";

                CompilationUnitSyntax compUnitSyntax =
                    UnitTestUtilities.GetAMethodDeclarationSyntaxFromSourceCode(sourceCodeFileToAnalyze);

                CSharpUnitTestLintWalker classUnderTest = new CSharpUnitTestLintWalker(new UnitTestViolations());


                // ACT 
                classUnderTest.Visit(compUnitSyntax);


                // ASSERT 
                classUnderTest.EnhancedDiagnostics
                    .Count
                    .Should().Be(UnitTestUtilities.ZeroEnhancedDiagnostics);
            }
        }

        #endregion Unit Tests
    }
}
