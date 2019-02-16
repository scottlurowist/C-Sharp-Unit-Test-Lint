//
// Descriptors.cs
//
// Product:  CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.AnalyzerRules
//
// Description: .NET Compiler Platform diagnostic descriptors describing unit test violations.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//



using Microsoft.CodeAnalysis;



namespace CSharpUnitTestLint.AnalyzerRules
{
    /// <summary>
    /// .NET Compiler Platform diagnostic descriptors decribing unit test violations.
    /// </summary>
    internal static class Descriptors
    {
        #region Private Instance Fields

        /// <summary>
        /// The category reported in diagnostic descriptors.
        /// </summary>
        private const string UnitTestingDiagnosticCategory = "Unit Testing";

        #endregion Private Instance Fields



        #region Diagnostic Descriptors

        /// <summary>
        /// The DiagnosticDescriptor of an if statement in a unit test.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static readonly DiagnosticDescriptor IfStatementDescriptor =
            new DiagnosticDescriptor("SRLCSUTL002", "Logic found in a test.",
                "An 'If Statement' was found. Logic is not permitted in a unit test.",
                UnitTestingDiagnosticCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        /// The DiagnosticDescriptor of a foreach statement in a unit test.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static readonly DiagnosticDescriptor ForeachStatementDescriptor =
            new DiagnosticDescriptor("SRLCSUTL003", "Logic found in a test.",
                "A 'foreach Statement' was found. Logic is not permitted in a unit test.",
                UnitTestingDiagnosticCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        /// The DiagnosticDescriptor of a magic number in a unit test.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static readonly DiagnosticDescriptor MagicNumberDescriptor =
            new DiagnosticDescriptor("SRLCSUTL004",
                "A Magic number was found in a test.",
                "Magic numbers decrease readability of a unit test.", UnitTestingDiagnosticCategory,
                DiagnosticSeverity.Error, true);

        /// <summary>
        /// The DiagnosticDescriptor of a try / catch statement in a unit test.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static readonly DiagnosticDescriptor TryCatchStatementDescriptor =
            new DiagnosticDescriptor("SRLCSUTL005",
                "A 'try / catch' statement was found in a test.",
                "A try / catch statement is logic and is not permitted in a unit test.", UnitTestingDiagnosticCategory,
                DiagnosticSeverity.Error, true);

        /// <summary>
        /// The DiagnosticDescriptor of a try / catch statement in a unit test.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static readonly DiagnosticDescriptor ForLoopStatementDescriptor =
            new DiagnosticDescriptor("SRLCSUTL006",
                "A 'for loop' statement was found in a test.",
                "A 'for loop' statement is logic and is not permitted in a unit test.", UnitTestingDiagnosticCategory,
                DiagnosticSeverity.Error, true);

        /// <summary>
        /// The DiagnosticDescriptor of a while loop statement in a unit test.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static readonly DiagnosticDescriptor WhileLoopStatementDescriptor =
            new DiagnosticDescriptor("SRLCSUTL007",
                "A 'while loop' statement was found in a test.",
                "A 'while loop' statement is logic and is not permitted in a unit test.", UnitTestingDiagnosticCategory,
                DiagnosticSeverity.Error, true);

        /// <summary>
        /// The DiagnosticDescriptor of a ternary operator statement in a unit test.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static readonly DiagnosticDescriptor TernaryOperatorStatementDescriptor =
            new DiagnosticDescriptor("SRLCSUTL008",
                "A 'ternary operator' statement was found in a test.",
                "A 'ternary operator' statement is logic and is not permitted in a unit test.", UnitTestingDiagnosticCategory,
                DiagnosticSeverity.Error, true);

        #endregion Diagnostic Descriptors
    }
}
