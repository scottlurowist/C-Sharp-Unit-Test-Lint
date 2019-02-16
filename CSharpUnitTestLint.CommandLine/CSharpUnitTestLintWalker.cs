//
// CSharpUnitTestLintWalker.cs
//
// Product:  CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.SolutionAnalyzerDriver
//
// Description: Implements a subclass of CSharpSyntaxWalker to find unit tests
//              and when found, process them. The walker analyzes a single source
//              code file at a time. It is assumed that the source code file 
//              represents a complete compilation unit.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//



using System.Collections.Generic;
using System.Linq;
using CSharpUnitTestLint.AnalyzerRules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace CSharpUnitTestLint.CommandLine
{
    public class CSharpUnitTestLintWalker : CSharpSyntaxWalker
    {
        #region Private Instance Fields

        /// <summary>
        /// The analyzer that has the actual unit test violation rules.
        /// </summary>
        private readonly IUnitTestsViolations _unitTestViolationsAnalyzer;

        /// <summary>
        /// A list of found unit test violations by the walker, over multiple
        /// invocations of the Visit method.
        /// </summary>
        private IList<Diagnostic> _foundDiagnostics = new List<Diagnostic>();

        #endregion Private Instance Fields



        #region Public Instance Properties

        /// <summary>
        /// A list of EnhancedDiagnostic found by a single invocation of the Visit
        /// method on a single source file.
        /// </summary>
        public IList<EnhancedDiagnostic> EnhancedDiagnostics { get; private set; } =
            new List<EnhancedDiagnostic>();


        public int NumberOfTestsFound;
        public int NumberOfTestsWithIssues;

        public string CurrentSourceCodeFileName { private get; set; }

        public string CurrentProject { private get; set; }

        #endregion Public Instance Properties



        #region Public Instance Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitTestViloationsAnalyzer">
        /// The analyzer that has the actual unit test violation rules.
        /// </param>
        public CSharpUnitTestLintWalker(IUnitTestsViolations unitTestViloationsAnalyzer)
        {
            _unitTestViolationsAnalyzer = unitTestViloationsAnalyzer;
        }

        #endregion Public Instance Constructors



        #region Public Overriden Methods

        /// <summary>
        /// Takes an instance of SyntaxNode that represents an attribute, and 
        /// determines if it is an attribute that indicates a unit test. If it 
        /// is, then it passes it to the unit test rules violation analyzer
        /// for analysis.
        /// </summary>
        /// <param name="node">
        /// Represents a SyntaxNode instance that is an attribute.
        /// </param>
        public override void VisitAttribute(AttributeSyntax node)
        {
            string attributeName = node.Name.GetText().ToString();

            switch (attributeName)
            {
                case "Fact":
                case "Test":
                case "TestCase":
                case "TestCaseSource":
                case "Theory": // --------> Note that both xUnit and NUnit support this attribute.

                    // The grand parent of an attribute is always an instance of MethodDeclarationSyntax.
                    // In our case, this represents a found unit test method.
                    MethodDeclarationSyntax methodDeclaration = node.Parent.Parent as MethodDeclarationSyntax;

                    AnalyzeTestMethod(methodDeclaration);
                    break;
            }

            base.VisitAttribute(node);
        }

        #endregion Public Overriden Methods



        #region Private Instance Utility Methods

        /// <summary>
        /// Analyzes a found unit test method for violations, and generates
        /// some statistics about tests found, number of violations, etc.
        /// </summary>
        /// <param name="methodDeclaration">
        /// An instance of MethodDeclarationSyntax that represents a found
        /// unit test method to analyze for violations.
        /// </param>
        private void AnalyzeTestMethod(MethodDeclarationSyntax methodDeclaration)
        {
            IList<EnhancedDiagnostic> localEnhancedDiagnostics = new List<EnhancedDiagnostic>();

            int beforeCount = _foundDiagnostics.Count;

            var rawDiagnostics = _unitTestViolationsAnalyzer.ProcessMethodDeclarationSyntax(methodDeclaration);

            foreach (var diag in rawDiagnostics)
            {
                string severity = null;

                switch(diag.Severity)
                {
                    case DiagnosticSeverity.Error:
                        severity = "Error";
                        break;
                    case DiagnosticSeverity.Hidden:
                        severity = "Hidden";
                        break;
                    case DiagnosticSeverity.Info:
                        severity = "Info";
                        break;
                    case DiagnosticSeverity.Warning:
                        severity = "Warning";
                        break;
                }

                EnhancedDiagnostic enhancedDiag = new EnhancedDiagnostic
                {
                    Diag = diag,
                    ProjectName = CurrentProject,
                    SourceCodeFileName = CurrentSourceCodeFileName,
                    MethodName = methodDeclaration.Identifier.Text,
                    Severity = severity,
                    CodeViolationSnippet = methodDeclaration.GetText().ToString()
                };

                localEnhancedDiagnostics.Add(enhancedDiag);
            }

            EnhancedDiagnostics = EnhancedDiagnostics.Union(localEnhancedDiagnostics).ToList();

            _foundDiagnostics = _foundDiagnostics
                .Union(_unitTestViolationsAnalyzer.ProcessMethodDeclarationSyntax(methodDeclaration))
                .ToList();

            if (_foundDiagnostics.Count != beforeCount)
            {
                NumberOfTestsWithIssues++;
            }

            NumberOfTestsFound++;
        }

        #endregion Private Instance Utility Methods
    }
}
