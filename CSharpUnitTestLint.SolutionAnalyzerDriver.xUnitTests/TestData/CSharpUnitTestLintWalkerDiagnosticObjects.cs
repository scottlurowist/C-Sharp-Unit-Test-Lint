//
// CSharpUnitTestLintWalkerDiagnosticObjects.cs
//
// Product:  CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.SolutionAnalyzerDriver.xUnitTests
//
// Description: Test data for CSharpUnitTestLintWalkerHarness.cs.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//


using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using CSharpUnitTestLint.AnalyzerRules;
using Microsoft.CodeAnalysis.Text;


namespace CSharpUnitTestLint.SolutionAnalyzerDriver.xUnitTests.TestData
{
    /// <summary>
    /// Test data for CSharpUnitTestLintWalkerHarness.cs.
    /// </summary>
    public static class CSharpUnitTestLintWalkerDiagnosticObjects
    {
        #region Private Instance Fields

        // Some pre-made diagnostics and diagnostic descriptors.
        private static readonly DiagnosticDescriptor SomeDiagnosticDescriptor =
            new DiagnosticDescriptor("someId,", "SomeTitle", "SomeMessage", "SomeCategory", 
                DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor AnotherDiagnosticDescriptor =
            new DiagnosticDescriptor("anotherId,", "Anotheritle", "AnotherMessage", "AnotherCategory",
                DiagnosticSeverity.Error, true);

        private static readonly Diagnostic SomeDiagnostic =
            Diagnostic.Create(SomeDiagnosticDescriptor, Location.None);

        private static readonly Diagnostic AnotherDiagnostic =
            Diagnostic.Create(AnotherDiagnosticDescriptor, Location.None);

        private static readonly EnhancedDiagnostic SomeEnhancedDiagnostic =
            new EnhancedDiagnostic
            {
                Diag = SomeDiagnostic,
                CodeViolationSnippet = "some snippet of code",
                MethodName = "SomeMethodName",
                ProjectName = "SomeProjectName",
                Severity = "SomeSeverity",
                SourceCodeFileName = "SomeSourceCodeFileName"
            };

        private static readonly EnhancedDiagnostic AnotherEnhancedDiagnostic =
            new EnhancedDiagnostic
            {
                Diag = SomeDiagnostic,
                CodeViolationSnippet = "some snippet of code",
                MethodName = "AnotherMethodName",
                ProjectName = "AnotherProjectName",
                Severity = "AnotherSeverity",
                SourceCodeFileName = "AnotherSourceCodeFileName"
            };

        #endregion Private Instance Fields



        #region Test Data

        /// <summary>
        /// An array of diagnostics.  The actual contents are not important since we are only
        /// interested in proving that 
        /// </summary>
        public static IEnumerable<object[]> DiagnosticObjects
        {
            get
            {
                yield return new object[]
                {
                    new List<Diagnostic> { },
                    new List<EnhancedDiagnostic> {}
                };

                yield return new object[]
                {
                    new[] { SomeDiagnostic},
                    new[] {SomeEnhancedDiagnostic}
                };

                yield return new object[]
                {
                    new[] { SomeDiagnostic, AnotherDiagnostic},
                    new[] {SomeEnhancedDiagnostic, AnotherEnhancedDiagnostic}
                };
            }
        }


        public static IEnumerable<object[]> ClassSourceAndExpectedDiagnostics
        {
            get
            {
                // Source that has no violations.
                yield return new object[]
                {
                    @"public class SomeClass
                      {
                         [Fact]
                         public void SomeTestMethod() 
                         {
                         }
                      }",
                    new List<Diagnostic>() 
                };

                // Source that has an if statement violation.
                // The location was obtained by observation.
                yield return new object[]
                {
                    @"public class SomeClass
                      {
                         [Fact]
                         public void SomeTestMethod() 
                         {
                            if (true) { /* Do Something */ }
                         }
                      }",
                    new List<Diagnostic> { Diagnostic.Create(Descriptors.IfStatementDescriptor,
                    Location.Create(String.Empty, TextSpan.FromBounds(194,226),
                        new LinePositionSpan(new LinePosition(6, 7),
                        new LinePosition(6, 39) ) ))}
                };
            }
        }

        #endregion Test Data
    }
}
