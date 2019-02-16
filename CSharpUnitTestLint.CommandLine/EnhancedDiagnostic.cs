//
// EnhancedDiagnostic.cs
//
// Product:  CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.SolutionAnalyzerDriver
//
// Description: Contains information needed for analysis reporting. It adds
//              more information than provided by Diagnostic.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//



using Microsoft.CodeAnalysis;



namespace CSharpUnitTestLint.CommandLine
{
    /// <summary>
    /// Contains information needed for analysis reporting. It adds
    /// more information than provided by Diagnostic.
    /// </summary>
    public class EnhancedDiagnostic
    {
        /// <summary>
        /// Gets or sets an instance of Diagnostic, part of the 
        /// .NET Compiler Platform API.
        /// </summary>
        public Diagnostic Diag { get; set; }      
        
        /// <summary>
        /// Gets or sets the name of a project where a violation was found.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the name of a source code file where a violation
        /// was found.
        /// </summary>
        public string SourceCodeFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of a method where a violation was found.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets a string that indicates the severity
        /// of a found violation.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets a code snippet where a violation was found.
        /// </summary>
        public string CodeViolationSnippet { get; set; }
    }
}
