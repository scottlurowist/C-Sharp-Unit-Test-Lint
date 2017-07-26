//
// UnitTestUtilities.cs
//
// Product: CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.SolutionAnalyzerDriver.xUnitTests
//
// Description: Utilities for testing .NET Compiler Platform based code.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//



using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace CSharpUnitTestLint.SolutionAnalyzerDriver.xUnitTests
{
    internal static class UnitTestUtilities
    {
        #region Internal Static Readonly Test Values

        /// <summary>
        /// Inidicates that no EnhancedDiagnostic instances were found.
        /// </summary>
        internal static readonly int ZeroEnhancedDiagnostics = 0;

        /// <summary>
        /// A source file that has all of the various violations, but contains 
        /// no test methods. Used for testing for false positives.
        /// </summary>
        internal static readonly string SourceFileWithAllViolationsButNoTests = @"
            public class SomeClassToAnalyzeWithNoTestsButAllViolations
            {
                // This is NOT a unit test method.
                public void SomeNonTestMethodWithAllViolations()
                {
                    IList<int> integerList = new List<int> {1, 2, 3, 4, 5};
                    int x = 4;
                    int y = 2;

                    if (x == 4) { x++; }

                    do { x--; } while (x != 0);

                    for (int i = 0; i < 10; i++) { x++; }

                    foreach (int someInt in integerList) { x++; }

                    IList<int> someInts = integerList.Skip(1).Take(2).ToList();

                    int someResult = x == y ? 1 : 0;

                    try
                    {
                    }
                    catch(Exception excp)
                    {
                        throw excp;
                    }
                }
            }";

        #endregion Internal Static Readonly Test Values



        #region Internal Static Utility Methods

        /// <summary>
        /// Returns an instance of CompilationUnitSyntax, that represents a source file
        /// to be analyzed by the walker,
        /// </summary>
        /// <param name="fileToAnalyzeSourceCode">
        /// The source code of a file to analyze by the walker.
        /// </param>
        /// <returns>
        /// An instance of CompilationUnitSyntax that represents the parsed
        /// representation of fileToAnalyzeSourceCode.
        /// </returns>
        /// <remarks>
        /// This method implements a common and well known idiom in the .NET Compiler Platform.
        /// Therefore, it need not be unit tested. It is known to work.
        /// </remarks>
        internal static CompilationUnitSyntax
            GetAMethodDeclarationSyntaxFromSourceCode(string fileToAnalyzeSourceCode)
        {
            return (CompilationUnitSyntax)CSharpSyntaxTree
                .ParseText(fileToAnalyzeSourceCode)
                .GetRoot();
        }

        #endregion Internal Static Utility Methods
    }
}
