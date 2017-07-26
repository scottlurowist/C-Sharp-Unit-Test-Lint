//
// UnitTestViolationsHarness.cs
//
// Product: CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.AnalyzerRules.xUnitTests
//
// Description: A test harness for UnitTestViolations.cs.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//



using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xunit;
// ReSharper disable SuggestVarOrType_SimpleTypes



namespace CSharpUnitTestLint.AnalyzerRules.xUnitTests
{
    /// <summary>
    /// A test harness for UnitTestViolations.cs.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UnitTestViolationsHarness
    {
        #region Constant Test Values

        /// <summary>
        /// A constant value that represents the absence of decriptors.
        /// </summary>
        private const int NoDiagnostics = 0;

        /// <summary>
        /// A constant value that represents a single decriptor.
        /// </summary>
        private const int OneDiagnostic = 1;

        /// <summary>
        /// A constant value that represents a single decriptor.
        /// </summary>
        private const int TwoDiagnostics = 2;

        #endregion Constant Test Valaues



        #region Unit Tests

        public class TheSupportedDescriptorsProperty : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that the property SupportedDescriptors contains each of
            /// the supported descriptors.
            /// </summary>
            [Fact]
            public void Must_Contain_AllSupported_DiagnosticDescriptors()
            {
                // ARRANGE
                const int numberOfSupportedDiagnostics = 7;

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT & ASSERT
                classUnderTest.SupportedDescriptors.Length.Should().Be(numberOfSupportedDiagnostics);

                classUnderTest.SupportedDescriptors.Should().Contain(Descriptors.IfStatementDescriptor);
                classUnderTest.SupportedDescriptors.Should().Contain(Descriptors.ForLoopStatementDescriptor);
                classUnderTest.SupportedDescriptors.Should().Contain(Descriptors.ForeachStatementDescriptor);
                classUnderTest.SupportedDescriptors.Should().Contain(Descriptors.MagicNumberDescriptor);
                classUnderTest.SupportedDescriptors.Should().Contain(Descriptors.TernaryOperatorStatementDescriptor);
                classUnderTest.SupportedDescriptors.Should().Contain(Descriptors.TryCatchStatementDescriptor);
                classUnderTest.SupportedDescriptors.Should().Contain(Descriptors.WhileLoopStatementDescriptor);
            }
        }

        public class TheSearchForIfStatementsMethod : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that when a test method has no if statements,
            /// that the number of generated diagnostics is zero.
            /// </summary>
            [Fact]
            public void MustNotProduce_AnIfStatementDiagnostic_When_TheMethodIsEmpty()
            {
                // ARRANGE
                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT
                foundDiagnostics.Count.Should().Be(NoDiagnostics);
            }


            /// <summary>
            /// Tests that when a test method has a single if statement,
            /// that the number of generated diagnostics is one, and that the 
            /// diagnostic is an if statement diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceASingleIfStatementDiagnostic_When_TheMethodHasOneIfStatement()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int textSpanStart = 165;
                const int textSpanEnd = 183;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        int x = 0;

                        if (x == 0) x = 1;
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have a single diagnostic that is actually a proper if statement diagnostic.
                foundDiagnostics.Count.Should().Be(OneDiagnostic);

                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(textSpanStart, textSpanEnd));
                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.IfStatementDescriptor);
            }


            /// <summary>
            /// Tests that when a test method has two if statements,
            /// that the number of generated diagnostics is two, and that each 
            /// diagnostic is an if statement diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceTwoIfStatementDiagnostics_When_TheMethodHasTwoIfStatements()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int firstTextSpanStart = 201;
                const int firstTextSpanEnd = 219;
                const int secondTextSpanStart = 245;
                const int secondTextSpanEnd = 263;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        int x = 0;
                        int y = 1;

                        if (x == 0) x = 1;
                        if (x == 1) y = 0;
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have two diagnostics that are each a proper if statement diagnostics.
                foundDiagnostics.Count.Should().Be(TwoDiagnostics);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.IfStatementDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(firstTextSpanStart, firstTextSpanEnd));

                foundDiagnostics.Last().Descriptor.Should().Be(Descriptors.IfStatementDescriptor);
                foundDiagnostics.Last().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(secondTextSpanStart, secondTextSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has no if statements, but other test
            /// violations, that the number of generated diagnostics for if statements is zero.
            /// </summary>
            /// <param name="methodToParseAsString">
            /// A chunk of C# method code to be parsed and analyzed for diagnostics.
            /// </param>
            /// <param name="friendlyDescription">
            /// A friendly description of the code to parse. It exists only for readability
            /// for developers reading this test method. It is not used by the test.
            /// </param>
            [Theory]
            [InlineData("[Fact]SomeTestMethod(){ foreach(int val in new int[] {1,2,3}) { /* Do something */ }  }",
                "foreach loop")]
            [InlineData("[Fact]SomeTestMethod(){ for(int i = 0; i < 10; i++) { /* Do something */  };  }",
                "for loop")]
            [InlineData("[Fact]SomeTestMethod(){ do { /* Do something */  } while(true);  }",
                "do while loop")]
            public void MustNotProduce_IfStatementDiagnostics_When_TheMethod_HasNoIfStatements_ButHasOtherViolations(
                string methodToParseAsString, string friendlyDescription)
            {
                // ARRANGE
                IUnitTestsViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToParseAsString));


                // ASSERT
                foundDiagnostics.Count(diag => diag.Id == Descriptors.IfStatementDescriptor.Id)
                    .Should().Be(NoDiagnostics);
            }
        }


        public class TheSearchForForEachStatementsMethod : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that when a test method has no foreach statements,
            /// that the number of generated diagnostics is zero.
            /// </summary>
            [Fact]
            public void MustNotProduce_AForeachStatementDiagnostic_When_TheMethodIsEmpty()
            {
                // ARRANGE
                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT
                foundDiagnostics.Count.Should().Be(NoDiagnostics);
            }


            /// <summary>
            /// Tests that when a test method has a single foreach statement,
            /// that the number of generated diagnostics is one, and that the 
            /// diagnostic is a foreach statement diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceASingleForeachStatementDiagnostic_When_TheMethodHasOneForeachStatement()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int textSpanStart = 204;
                const int textSpanEnd = 337;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        IList<int> listOfInt = new List<int> {1,2,3,4,5};

                        foreach (var item in listOfInt)
                        {
                            /* Do something */
                        }
                    }";


                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have a single diagnostic that is actually a proper foreach statement diagnostic.
                foundDiagnostics.Count.Should().Be(OneDiagnostic);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.ForeachStatementDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(textSpanStart, textSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has two foreach statements,
            /// that the number of generated diagnostics is two, and that each 
            /// diagnostic is a foreach statement diagnostic.
            /// 
            /// </summary>
            [Fact]
            public void MustProduceTwoForeachStatementDiagnostics_When_TheMethodHasTwoForeachStatements()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int firstTextSpanStart = 204;
                const int firstTextSpanEnd = 337;
                const int secondTextSpanStart = 365;
                const int secondTextSpanEnd = 498;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        IList<int> listOfInt = new List<int> {1,2,3,4,5};

                        foreach (var item in listOfInt)
                        {
                            /* Do something */
                        }

                        foreach (var item in listOfInt)
                        {
                            /* Do something */
                        }
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have two diagnostics that are each a proper foreach statement diagnostics.
                foundDiagnostics.Count.Should().Be(TwoDiagnostics);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.ForeachStatementDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(firstTextSpanStart, firstTextSpanEnd));

                foundDiagnostics.Last().Descriptor.Should().Be(Descriptors.ForeachStatementDescriptor);
                foundDiagnostics.Last().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(secondTextSpanStart, secondTextSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has no foreach statements, but other test
            /// violations, that the number of generated diagnostics for foreach statements is zero.
            /// </summary>
            /// <param name="methodToParseAsString">
            /// A chunk of C# method code to be parsed and analyzed for diagnostics.
            /// </param>
            /// <param name="friendlyDescription">
            /// A friendly description of the code to parse. It exists only for readability
            /// for developers reading this test method. It is not used by the test.
            /// </param>
            [Theory]
            [InlineData("[Fact]SomeTestMethod(){ if(true) { /* Do something */ }  }",
                "foreach loop")]
            [InlineData("[Fact]SomeTestMethod(){ for(int i = 0; i < 10; i++) { /* Do something */  };  }",
                "for loop")]
            [InlineData("[Fact]SomeTestMethod(){ do { /* Do something */  } while(true);  }",
                "do while loop")]
            public void MustNotProduce_ForeachStatementDiagnostics_When_TheMethod_HasNoForeachStatements_ButHasOtherViolations(
                string methodToParseAsString, string friendlyDescription)
            {
                // ARRANGE
                IUnitTestsViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToParseAsString));


                // ASSERT
                foundDiagnostics.Count(diag => diag.Id == Descriptors.ForeachStatementDescriptor.Id)
                    .Should().Be(NoDiagnostics);
            }
        }


        public class TheSearchForMagicNumbersMethod : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that when a test method has no magic numbers,
            /// that the number of generated diagnostics is zero.
            /// </summary>
            [Fact]
            public void MustNotProduce_AMagicNumberDiagnostic_When_TheMethodIsEmpty()
            {
                // ARRANGE
                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT
                foundDiagnostics.Count.Should().Be(NoDiagnostics);
            }


            /// <summary>
            /// Tests that when a test method has a single magic number,
            /// that the number of generated diagnostics is one, and that the 
            /// diagnostic is a magic number diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceASingleMagicNumberDiagnostic_When_TheMethodHasOneMagicNumber()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int textSpanStart = 246;
                const int textSpanEnd = 247;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        IList<int> listOfInt = new List<int> {1,2,3,4,5};
    
                        IEnumerable<int> foo = listOfInt.Skip(1);
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have a single diagnostic that is actually a proper magic number diagnostic.
                foundDiagnostics.Count.Should().Be(OneDiagnostic);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.MagicNumberDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(textSpanStart, textSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has two magic numbers,
            /// that the number of generated diagnostics is two, and that each 
            /// diagnostic is a magic number diagnostic.
            /// 
            /// </summary>
            [Fact]
            public void MustProduceTwoMagicNumberDiagnostics_When_TheMethodHasTwoMagicNumbers()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int firstTextSpanStart = 247;
                const int firstTextSpanEnd = 248;
                const int secondTextSpanStart = 255;
                const int secondTextSpanEnd = 256;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        IList<int> listOfInt = new List<int> {1,2,3,4,5}; 
    
                        IEnumerable<int> foo = listOfInt.Skip(1).Take(1);
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have two diagnostics that are each a proper magic number diagnostics.
                foundDiagnostics.Count.Should().Be(TwoDiagnostics);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.MagicNumberDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(firstTextSpanStart, firstTextSpanEnd));

                foundDiagnostics.Last().Descriptor.Should().Be(Descriptors.MagicNumberDescriptor);
                foundDiagnostics.Last().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(secondTextSpanStart, secondTextSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has no magic numbers, but other test
            /// violations, that the number of generated diagnostics for magic numbers is zero.
            /// </summary>
            /// <param name="methodToParseAsString">
            /// A chunk of C# method code to be parsed and analyzed for diagnostics.
            /// </param>
            /// <param name="friendlyDescription">
            /// A friendly description of the code to parse. It exists only for readability
            /// for developers reading this test method. It is not used by the test.
            /// </param>
            [Theory]
            [InlineData("[Fact]SomeTestMethod(){ if(true) { /* Do something */ }  }",
                "foreach loop")]
            [InlineData("[Fact]SomeTestMethod(){ for(int i = 0; i < 10; i++) { /* Do something */  };  }",
                "for loop")]
            [InlineData("[Fact]SomeTestMethod(){ do { /* Do something */  } while(true);  }",
                "do while loop")]
            public void MustNotProduce_MagicNumberDiagnostics_When_TheMethod_HasNoMagicNumbers_ButHasOtherViolations(
                string methodToParseAsString, string friendlyDescription)
            {
                // ARRANGE
                IUnitTestsViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToParseAsString));


                // ASSERT
                foundDiagnostics.Count(diag => diag.Id == Descriptors.MagicNumberDescriptor.Id)
                    .Should().Be(NoDiagnostics);
            }
        }


        public class TheSearchForTryCatchStatementsMethod : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that when a test method has no try/catch statements,
            /// that the number of generated diagnostics is zero.
            /// </summary>
            [Fact]
            public void MustNotProduce_ATryCatchStatementDiagnostic_When_TheMethodIsEmpty()
            {
                // ARRANGE
                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT
                foundDiagnostics.Count.Should().Be(NoDiagnostics);
            }


            /// <summary>
            /// Tests that when a test method has a single try/catch statement,
            /// that the number of generated diagnostics is one, and that the 
            /// diagnostic is a try/catch statement diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceASingleTryCatchStatementDiagnostic_When_TheMethodHasOneTryCatchStatement()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int textSpanStart = 127;
                const int textSpanEnd = 381;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        try
                        {
                            /* Do Something */
                        }
                        catch(Exception excp)
                        {
                            /* Do Something */
                        }
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have a single diagnostic that is actually a proper try/catch statement diagnostic.
                foundDiagnostics.Count.Should().Be(OneDiagnostic);

                foundDiagnostics.First().Location.SourceSpan.Should().Be(TextSpan.FromBounds(textSpanStart, textSpanEnd));
                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.TryCatchStatementDescriptor);
            }


            /// <summary>
            /// Tests that when a test method has two try/catch statements,
            /// that the number of generated diagnostics is two, and that each 
            /// diagnostic is a try/catch statement diagnostic.
            /// 
            /// </summary>
            [Fact]
            public void MustProduceTwoTryCatchStatementDiagnostics_When_TheMethodHasTwoTryCatchStatements()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int firstTextSpanStart = 127;
                const int firstTextSpanEnd = 381;
                const int secondTextSpanStart = 409;
                const int secondTextSpanEnd = 663;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        try
                        {
                            /* Do Something */
                        }
                        catch(Exception excp)
                        {
                            /* Do Something */
                        }

                        try
                        {
                            /* Do Something */
                        }
                        catch(Exception excp)
                        {
                            /* Do Something */
                        }
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have two diagnostics that are each a proper try/catch statement diagnostics.
                foundDiagnostics.Count.Should().Be(TwoDiagnostics);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.TryCatchStatementDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(firstTextSpanStart, firstTextSpanEnd));

                foundDiagnostics.Last().Descriptor.Should().Be(Descriptors.TryCatchStatementDescriptor);
                foundDiagnostics.Last().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(secondTextSpanStart, secondTextSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has no try/catch statements, but other test
            /// violations, that the number of generated diagnostics for try/catch statements is zero.
            /// </summary>
            /// <param name="methodToParseAsString">
            /// A chunk of C# method code to be parsed and analyzed for diagnostics.
            /// </param>
            /// <param name="friendlyDescription">
            /// A friendly description of the code to parse. It exists only for readability
            /// for developers reading this test method. It is not used by the test.
            /// </param>
            [Theory]
            [InlineData("[Fact]SomeTestMethod(){ if(true) { /* Do something */ }  }",
                "foreach loop")]
            [InlineData("[Fact]SomeTestMethod(){ for(int i = 0; i < 10; i++) { /* Do something */  };  }",
                "for loop")]
            [InlineData("[Fact]SomeTestMethod(){ do { /* Do something */  } while(true);  }",
                "do while loop")]
            public void MustNotProduce_TryCatchStatementDiagnostics_When_TheMethod_HasNoTryCatchStatements_ButHasOtherViolations(
                string methodToParseAsString, string friendlyDescription)
            {
                // ARRANGE
                IUnitTestsViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToParseAsString));


                // ASSERT
                foundDiagnostics.Count(diag => diag.Id == Descriptors.TryCatchStatementDescriptor.Id)
                    .Should().Be(NoDiagnostics);
            }
        }


        public class TheSearchForForLoopsMethod : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that when a test method has no for loop statements,
            /// that the number of generated diagnostics is zero.
            /// </summary>
            [Fact]
            public void MustNotProduce_AForLoopStatementDiagnostic_When_TheMethodIsEmpty()
            {
                // ARRANGE
                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT
                foundDiagnostics.Count.Should().Be(NoDiagnostics);
            }


            /// <summary>
            /// Tests that when a test method has a for loop statement,
            /// that the number of generated diagnostics is one, and that the 
            /// diagnostic is a for loop statement diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceASingleForLoopStatementDiagnostic_When_TheMethodHasOneForLoopStatement()
            {
                // ARRANGE                
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int textSpanStart = 127;
                const int textSpanEnd = 273;


                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        for (int i; i < 10; i++)
                        {
                            /* Do something */                    
                        }
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have a single diagnostic that is actually a proper for loop statement diagnostic.
                foundDiagnostics.Count.Should().Be(OneDiagnostic);

                foundDiagnostics.First().Location.SourceSpan.Should().Be(TextSpan.FromBounds(textSpanStart, textSpanEnd));
                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.ForLoopStatementDescriptor);
            }


            /// <summary>
            /// Tests that when a test method has two for loop statements,
            /// that the number of generated diagnostics is two, and that each 
            /// diagnostic is a for loop statement diagnostic.
            /// 
            /// </summary>
            // TODO: Write another unit test with nested loops. Do this for all logic tests.
            [Fact]
            public void MustProduceTwoForLoopStatementDiagnostics_When_TheMethodHasTwoForLoopStatements()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int firstTextSpanStart = 127;
                const int firstTextSpanEnd = 272;
                const int secondTextSpanStart = 300;
                const int secondTextSpanEnd = 446;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        for (int i; i < 10; i++)
                        {
                            /* Do something */                   
                        }

                        for (int i; i < 10; i++)
                        {
                            /* Do something */                    
                        }  
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have two diagnostics that are each a proper for loop statement diagnostics.
                foundDiagnostics.Count.Should().Be(TwoDiagnostics);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.ForLoopStatementDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(firstTextSpanStart, firstTextSpanEnd));

                foundDiagnostics.Last().Descriptor.Should().Be(Descriptors.ForLoopStatementDescriptor);
                foundDiagnostics.Last().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(secondTextSpanStart, secondTextSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has no for loop statements, but other test
            /// violations, that the number of generated diagnostics for for loop statements is zero.
            /// </summary>
            /// <param name="methodToParseAsString">
            /// A chunk of C# method code to be parsed and analyzed for diagnostics.
            /// </param>
            /// <param name="friendlyDescription">
            /// A friendly description of the code to parse. It exists only for readability
            /// for developers reading this test method. It is not used by the test.
            /// </param>
            // TODO: Each instance of this type of test must include all diagnostics.
            [Theory]
            [InlineData("[Fact]SomeTestMethod(){ if(true) { /* Do something */ }  }",
                "foreach loop")]
            [InlineData("[Fact]SomeTestMethod(){ do { /* Do something */  } while(true);  }",
                "do while loop")]
            public void MustNotProduce_ForLoopStatementDiagnostics_When_TheMethod_HasNoForLoopStatements_ButHasOtherViolations(
                string methodToParseAsString, string friendlyDescription)
            {
                // ARRANGE
                IUnitTestsViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToParseAsString));


                // ASSERT
                foundDiagnostics.Count(diag => diag.Id == Descriptors.ForLoopStatementDescriptor.Id)
                    .Should().Be(NoDiagnostics);
            }
        }


        public class TheSearchForWhileLoopsMethod : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that when a test method has no while loop statements,
            /// that the number of generated diagnostics is zero.
            /// </summary>
            [Fact]
            public void MustNotProduce_AWhileLoopStatementDiagnostic_When_TheMethodIsEmpty()
            {
                // ARRANGE
                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT
                foundDiagnostics.Count.Should().Be(NoDiagnostics);
            }


            /// <summary>
            /// Tests that when a test method has a while loop statement,
            /// that the number of generated diagnostics is one, and that the 
            /// diagnostic is a while loop statement diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceASingleWhileLoopStatementDiagnostic_When_TheMethodHasOneWhileLoopStatement()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int textSpanStart = 127;
                const int textSpanEnd = 242;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        while (true)
                        {
                             /* Do Something */
                        }
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have a single diagnostic that is actually a proper while loop statement diagnostic.
                foundDiagnostics.Count.Should().Be(OneDiagnostic);

                foundDiagnostics.First().Location.SourceSpan.Should().Be(TextSpan.FromBounds(textSpanStart, textSpanEnd));
                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.WhileLoopStatementDescriptor);
            }


            /// <summary>
            /// Tests that when a test method has two while loop statements,
            /// that the number of generated diagnostics is two, and that each 
            /// diagnostic is a while loop statement diagnostic.
            /// 
            /// </summary>
            // TODO: Write another unit test with nested loops. Do this for all logic tests.
            [Fact]
            public void MustProduceTwoWhileLoopStatementDiagnostics_When_TheMethodHasTwoWhileLoopStatements()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int firstTextSpanStart = 127;
                const int firstTextSpanEnd = 242;
                const int secondTextSpanStart = 270;
                const int secondTextSpanEnd = 385;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        while (true)
                        {
                             /* Do Something */
                        }

                        while (true)
                        {
                             /* Do Something */
                        }  
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have two diagnostics that are each a proper while loop statement diagnostics.
                foundDiagnostics.Count.Should().Be(TwoDiagnostics);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.WhileLoopStatementDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(firstTextSpanStart, firstTextSpanEnd));

                foundDiagnostics.Last().Descriptor.Should().Be(Descriptors.WhileLoopStatementDescriptor);
                foundDiagnostics.Last().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(secondTextSpanStart, secondTextSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has no while loop statements, but other test
            /// violations, that the number of generated diagnostics for while loop statements is zero.
            /// </summary>
            /// <param name="methodToParseAsString">
            /// A chunk of C# method code to be parsed and analyzed for diagnostics.
            /// </param>
            /// <param name="friendlyDescription">
            /// A friendly description of the code to parse. It exists only for readability
            /// for developers reading this test method. It is not used by the test.
            /// </param>
            // TODO: Each instance of this type of test must include all diagnostics.
            [Theory]
            [InlineData("[Fact]SomeTestMethod(){ if(true) { /* Do something */ }  }",
                "foreach loop")]
            [InlineData("[Fact]SomeTestMethod(){ do { /* Do something */  } while(true);  }",
                "do while loop")]
            public void MustNotProduce_WhileLoopStatementDiagnostics_When_TheMethod_HasNoWhileLoopStatements_ButHasOtherViolations(
                string methodToParseAsString, string friendlyDescription)
            {
                // ARRANGE
                IUnitTestsViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToParseAsString));


                // ASSERT
                foundDiagnostics.Count(diag => diag.Id == Descriptors.WhileLoopStatementDescriptor.Id)
                    .Should().Be(NoDiagnostics);
            }
        }


        public class TheSearchForTernaryOperatorsMethod : UnitTestViolationsHarness
        {
            /// <summary>
            /// Tests that when a test method has no ternary operators,
            /// that the number of generated diagnostics is zero.
            /// </summary>
            [Fact]
            public void MustNotProduce_ATernaryOperatorDiagnostic_When_TheMethodIsEmpty()
            {
                // ARRANGE
                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT
                foundDiagnostics.Count.Should().Be(NoDiagnostics);
            }


            /// <summary>
            /// Tests that when a test method has a ternary operator,
            /// that the number of generated diagnostics is one, and that the 
            /// diagnostic is a ternary operator diagnostic.
            /// </summary>
            [Fact]
            public void MustProduceASingleTernaryOperatorDiagnostic_When_TheMethodHasOneTernaryOperator()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int textSpanStart = 145;
                const int textSpanEnd = 168;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        bool someResult = (1 == 2) ? true : false;
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have a single diagnostic that is actually a proper ternary operator diagnostic.
                foundDiagnostics.Count.Should().Be(OneDiagnostic);

                foundDiagnostics.First().Location.SourceSpan.Should().Be(TextSpan.FromBounds(textSpanStart, textSpanEnd));
                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.TernaryOperatorStatementDescriptor);
            }


            /// <summary>
            /// Tests that when a test method has two ternary operators,
            /// that the number of generated diagnostics is two, and that each 
            /// diagnostic is a ternary operator diagnostic.
            /// </summary>
            // TODO: Write another unit test with nested loops. Do this for all logic tests.
            [Fact]
            public void MustProduceTwoTernaryOperatorDiagnostics_When_TheMethodHasTwoTernaryOperators()
            {
                // ARRANGE
                // These values were obtained empirically in Notepad++ by copying
                // "methodToAnalyzeAsString" to it and obtaining the text spans.
                const int firstTextSpanStart = 145;
                const int firstTextSpanEnd = 168;
                const int secondTextSpanStart = 220;
                const int secondTextSpanEnd = 243;

                const string methodToAnalyzeAsString = @"
                    [Fact]
                    public void SomeTestMethod()
                    {
                        bool someResult = (1 == 2) ? true : false;  

                        bool anotherResult = (1 == 2) ? true : false;  
                    }";

                UnitTestViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToAnalyzeAsString));


                // ASSERT - we should have two diagnostics that are each a proper ternary operator diagnostics.
                foundDiagnostics.Count.Should().Be(TwoDiagnostics);

                foundDiagnostics.First().Descriptor.Should().Be(Descriptors.TernaryOperatorStatementDescriptor);
                foundDiagnostics.First().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(firstTextSpanStart, firstTextSpanEnd));

                foundDiagnostics.Last().Descriptor.Should().Be(Descriptors.TernaryOperatorStatementDescriptor);
                foundDiagnostics.Last().Location.SourceSpan.Should().Be(
                    TextSpan.FromBounds(secondTextSpanStart, secondTextSpanEnd));
            }


            /// <summary>
            /// Tests that when a test method has no ternary operators, but other test
            /// violations, that the number of generated diagnostics for ternary operators is zero.
            /// </summary>
            /// <param name="methodToParseAsString">
            /// A chunk of C# method code to be parsed and analyzed for diagnostics.
            /// </param>
            /// <param name="friendlyDescription">
            /// A friendly description of the code to parse. It exists only for readability
            /// for developers reading this test method. It is not used by the test.
            /// </param>
            // TODO: Each instance of this type of test must include all diagnostics.
            [Theory]
            [InlineData("[Fact]SomeTestMethod(){ if(true) { /* Do something */ }  }",
                "foreach loop")]
            [InlineData("[Fact]SomeTestMethod(){ do { /* Do something */  } while(true);  }",
                "do while loop")]
            public void MustNotProduce_TernaryOperatorDiagnostics_When_TheMethod_HasNoTernaryOperrators_ButHasOtherViolations(
                string methodToParseAsString, string friendlyDescription)
            {
                // ARRANGE
                IUnitTestsViolations classUnderTest = new UnitTestViolations();


                // ACT
                IList<Diagnostic> foundDiagnostics = classUnderTest
                    .ProcessMethodDeclarationSyntax(GetMethodDeclarationSyntaxFromParser(methodToParseAsString));


                // ASSERT
                foundDiagnostics.Count(diag => diag.Id == Descriptors.TernaryOperatorStatementDescriptor.Id)
                    .Should().Be(NoDiagnostics);
            }
        }


        public class TheGetMethodDeclarationSyntaxFromParserUtilityMethod
        {
            /// <summary>
            /// Tests that when the code to parse does not contain a method, that it 
            /// throws an InvalidOperationException. This occurs, because the utility
            /// method assumes that we are giving it a single method to parse. Because there
            /// is no method, and it tries to return the first and only method, the
            /// LINQ query will fail.
            /// </summary>
            [Fact]
            public void MustThrow_InvalidOperationException_When_TheCodeToParse_DoesNotContain_AMethod()
            {
                // ARRANGE
                const string codeToParse = @"
                    public class SomeEmptyClass()
                    {
                    }";


                // ACT
                Action act = () => GetMethodDeclarationSyntaxFromParser(codeToParse);


                // ASSERT
                act.ShouldThrow<InvalidOperationException>();
            }


            /// <summary>
            /// Tests that when the code to parse contains a single method, with method name
            /// 'SomeEmptyMethod', that the MethodDeclarationSyntax returned, contains
            /// the method name 'SomeEmptyMethod'. Note that we are only testing that
            /// we get the correct MethodDeclarationSyntax. We are not testing the
            /// .NET Compiler Platform itself.
            /// </summary>
            [Fact]
            public void MustReturn_MethodDeclarationSyntax_WithMethodName_SomeEmptyMethod_When_TheCodeToParse_Contains_ASingleMethod()
            {
                const string expectedMethodName = "SomeEmptyMethod";

                // ARRANGE
                const string codeToParse = @"
                    public void SomeEmptyMethod()
                    {
                    }";


                // ACT
                MethodDeclarationSyntax actualMethodDeclarationSyntax =
                    GetMethodDeclarationSyntaxFromParser(codeToParse);


                // ASSERT
                actualMethodDeclarationSyntax.Identifier.Text.Should().Be(expectedMethodName);
            }


            /// <summary>
            /// Tests that when the code to parse contains two methods, with method names
            /// 'SomeEmptyMethod' and 'AnotherEmptyMethod, that the MethodDeclarationSyntax 
            /// returned, contains the method name 'SomeEmptyMethod'. This is because
            /// it is the first method defined in the source code to be parsed.
            /// </summary>
            [Fact]
            public void MustReturn_MethodDeclarationSyntax_WithMethodName_SomeEmptyMethod_When_TheCodeToParse_Contains_TwoMethods()
            {
                const string expectedMethodName = "SomeEmptyMethod";

                // ARRANGE
                const string codeToParse = @"
                    public void SomeEmptyMethod()
                    {
                    }

                    public void AnotherEmptyMethod()
                    {
                    }";


                // ACT
                MethodDeclarationSyntax actualMethodDeclarationSyntax =
                    GetMethodDeclarationSyntaxFromParser(codeToParse);


                // ASSERT
                actualMethodDeclarationSyntax.Identifier.Text.Should().Be(expectedMethodName);
            }
        }

        #endregion Unit Tests



        #region Private Instance Helper Methods

        /// <summary>
        /// Converts a piece of source code that represents a method, and 
        /// returns an instance of MethodDeclarationSyntax that is the parsed
        /// representation of the source code.
        /// </summary>
        /// <param name="methodSourceCodeToParse">
        /// The source code to parse.
        /// </param>
        /// <returns>
        /// An instance of MethodDeclarationSyntax that represents the source code.
        /// </returns>
        /// <remarks>
        /// This is a standard idiom in the .NET Compiler Platform. Therefore, 
        /// this utility method need not be unit tested, but it is.
        /// </remarks>
        private static MethodDeclarationSyntax GetMethodDeclarationSyntaxFromParser(string methodSourceCodeToParse)
        {
            // We can use .First(), because we know that there is only one method
            // to analyze.
            return CSharpSyntaxTree
                .ParseText(methodSourceCodeToParse)
                .GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .First();
        }

        #endregion Private Instance Helper Methods
    }
}
