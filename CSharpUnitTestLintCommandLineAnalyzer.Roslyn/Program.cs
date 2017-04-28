using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CSharpUnitTestLintCommandLineAnalyzer.Roslyn
{
    class Program
    {
        static void Main(string[] args)
        {
            // The solution that we are going to analyze... This will be passed as a command line argument in production.
            string solutionPath =
                @"..\..\..\CSharpUnitTestLintCommandLineAnalyzer.Roslyn.sln";

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Solution solutionToAnalyze = workspace.OpenSolutionAsync(solutionPath).Result;

            CSharpUnitTestLintWalker walker = new CSharpUnitTestLintWalker();

            foreach (var project in solutionToAnalyze.Projects)
            {
                foreach (var document in project.Documents)
                {
                    SyntaxTree currentSyntaxTree = document.GetSyntaxTreeAsync().Result;

                    walker.CurrentSourceCodeFile = document.Name;
                    walker.Visit(currentSyntaxTree.GetRoot());
                }
            }

            Console.WriteLine();
            Console.WriteLine("Number of tests found: {0}", walker.NumberOfTestsFound);
            Console.WriteLine();
            Console.WriteLine("Number of issues found: {0}", walker.FoundDiagnostics.Count);
            Console.WriteLine();
            Console.WriteLine("Number of tests with issues: {0} - {1}%", walker.NumberOfTestsWithIssues, 
                (float)walker.NumberOfTestsWithIssues / (float)walker.NumberOfTestsFound);
            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();

            foreach (Diagnostic diagnostic in walker.FoundDiagnostics)
            {
                Console.WriteLine("Rule violation ID - {0}:",diagnostic.Id);
                Console.WriteLine("     Severity - {0} ", diagnostic.Severity.ToString());
                Console.WriteLine("     Title - {0} ", diagnostic.Descriptor.Title);
                Console.WriteLine("     Message - {0} ", diagnostic.GetMessage());
                Console.WriteLine();
                Console.WriteLine("----------------------------------------");
                Console.WriteLine();
            }

            Console.WriteLine("\nPress any key to continue...");
            //Console.ReadKey();

        }


        private static void Foo()
        {
            //EnvDTE80.DTE2 dte2;
            //dte2 = (DTE2)System.Runtime.InteropServices.Marshal.
            //    GetActiveObject("VisualStudio.DTE.14.0");

            //dte2.SuppressUI = true;

            ////System.Type dteType = Type.GetTypeFromProgID("VisualStudio.DTE.14.0", true);
            ////EnvDTE.DTE dte = (EnvDTE.DTE)System.Activator.CreateInstance(dteType);
            //dte2.
            //dte2.Solution.Open(@"C:\GitRepos\SampleUnitTestProjectsForRoslynAnalysis\SampleUnitTestProjectsForRoslynAnalysis.sln");
        }
    }
}
