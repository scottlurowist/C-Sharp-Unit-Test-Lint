




using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using CSharpUnitTestLint.AnalyzerRules;
using CSharpUnitTestLint.SolutionAnalyzerDriver;
using Microsoft.CodeAnalysis.CSharp.Syntax;
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression


namespace CSharpUnitTestLint.CommandLineAnalyzer
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        static void Main(string[] args)
        {
            string solutionPath; 

            if (args.Length == 1)
            {
                solutionPath = args[0];
            }
            else
            {
                // The solution that we are going to analyze... This will be passed as a command line argument in production.
                solutionPath = @"..\..\..\CSharpUnitTestLint.sln";
                //solutionPath = @"C:\GitRepos\CMGR_Trunk\ContentManager.sln";
                //solutionPath = @"C:\TPP_Trunk\McKesson.TPP.sln";
            }

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            //workspace.Options.WithChangedOpC:\GitRepos\CMGR_Trunktion()
            Solution solutionToAnalyze = workspace.OpenSolutionAsync(solutionPath).Result;

            IUnitTestsViolations violations = new UnitTestViolations();

            IList<EnhancedDiagnostic> enhancedDiagnostics = new List<EnhancedDiagnostic>();

            int numberOfTestsFound = 0;
            int numberOfTestsWithIssues = 0;


            foreach (var project in solutionToAnalyze.Projects)
            {
                foreach (var document in project.Documents)
                {
                    SyntaxTree currentSyntaxTree = document.GetSyntaxTreeAsync().Result;

                    CSharpUnitTestLintWalker walker =
                        new CSharpUnitTestLintWalker(violations)
                        {
                            CurrentProject = project.Name,
                            CurrentSourceCodeFileName = document.Name
                        };

                    // Our walker assumes that its SyntaxNode is an entire source code file.
                    // Therefore, our visitor requires that the SyntaxNode's IsCompilationUnit
                    // attribute is true. We know that it is true, because we pass
                    // an entirely parsed source code file.
                    walker.Visit((CompilationUnitSyntax)currentSyntaxTree.GetRoot());

                    enhancedDiagnostics = enhancedDiagnostics.Union(walker.EnhancedDiagnostics).ToList();

                    numberOfTestsFound += walker.NumberOfTestsFound;
                    numberOfTestsWithIssues += walker.NumberOfTestsWithIssues;
                }
            }

            var violationsByIdAndSeverity = enhancedDiagnostics
                .GroupBy(ed => ed.Diag.Id  + " - " + ed.Severity)
                .Select(group => new
                {
                    group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(edgrp => edgrp.Count);

            var topTestsWithIssues = enhancedDiagnostics
                .GroupBy(ed => ed.MethodName)
                .Select(group => new
                {
                    MethodName = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(newGroups => newGroups.Count)
                .Take(10);

          

            // TODO: Do not hard code this. The walker should return a collection.
            Console.WriteLine("Rules Used In Analysis:");
            Console.WriteLine("     SRLCDUTL002: An 'if' statement found in a test.");
            Console.WriteLine("     SRLCDUTL003: A 'foreach' statement found in a test.");
            Console.WriteLine("     SRLCDUTL004: A 'magic number' found in a test.");
            Console.WriteLine("     SRLCDUTL005: A 'try / catch statement' found in a test.");
            Console.WriteLine("     SRLCDUTL006: A 'for loop statement' found in a test.");
            Console.WriteLine("     SRLCDUTL007: A 'while loop statement' found in a test.");
            Console.WriteLine("     SRLCDUTL008: An 'ternary operator statement' found in a test.");
            Console.WriteLine();
            Console.WriteLine("Number of tests found: {0}", numberOfTestsFound);
            Console.WriteLine();
            Console.WriteLine("Number of issues found: {0}", enhancedDiagnostics.Count);
            Console.WriteLine();
            Console.WriteLine("Number of tests with issues: {0} - {1}%", numberOfTestsWithIssues, 
                numberOfTestsWithIssues / numberOfTestsFound);
            Console.WriteLine();
            Console.WriteLine("Violations by Rule And Severity:");

            foreach(var violationGroup in violationsByIdAndSeverity)
            {
                Console.WriteLine("     {0} : {1}", 
                    violationGroup.Key,
                    violationGroup.Count);
            }

            Console.WriteLine();
            Console.WriteLine("Top Ten Tests By Violations Count:");

            // TODO: Magic Numbers seems to be reporting violation counts incorrectly.
            //       See Content Manager -
            //       Execute_ReturnsCorrectValues_IfColumnTypesContainsColumnsThatShouldBeStripped
            foreach (var methodWithIssues in topTestsWithIssues)
            {
                Console.WriteLine("     {0} : {1}",
                    methodWithIssues.MethodName,
                    methodWithIssues.Count);
            }

            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Violations Details:");
            Console.WriteLine();
            Console.WriteLine("----------------------------------------");

            foreach (EnhancedDiagnostic enhancedDiag in enhancedDiagnostics)
            { 
                Console.WriteLine("Project Name      - {0}:", enhancedDiag.ProjectName);
                Console.WriteLine("File Name         - {0} ", enhancedDiag.SourceCodeFileName);
                Console.WriteLine("Test Method Name  - {0} ", enhancedDiag.MethodName);
                Console.WriteLine("Rule violation:");
                Console.WriteLine("     ID       - {0}:", enhancedDiag.Diag.Id);
                Console.WriteLine("     Severity - {0} ", enhancedDiag.Diag.Severity);
                Console.WriteLine("     Title    - {0} ", enhancedDiag.Diag.Descriptor.Title);
                Console.WriteLine("     Message  - {0} ", enhancedDiag.Diag.GetMessage());
                Console.WriteLine("     Source   - {0} ", enhancedDiag.CodeViolationSnippet);
                Console.WriteLine();
                Console.WriteLine("----------------------------------------");
                Console.WriteLine();
            }
        }
    }
}
