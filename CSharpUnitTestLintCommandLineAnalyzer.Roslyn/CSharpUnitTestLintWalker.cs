using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace CSharpUnitTestLintCommandLineAnalyzer.Roslyn
{
    public class CSharpUnitTestLintWalker : CSharpSyntaxWalker
    {
        public int NumberOfTestsFound = 0;
        public int NumberOfTestsWithIssues = 0;

        AnalyzerRules.UnitTestViolations TestViolations = new AnalyzerRules.UnitTestViolations();

        public IList<Diagnostic> FoundDiagnostics = new List<Diagnostic>();

        public string CurrentSourceCodeFile { get; set; }


        public override void VisitAttribute(AttributeSyntax node)
        {
            AttributeSyntax attribute = node as AttributeSyntax;

            string attributeName = node.Name.GetText().ToString();

            switch (attributeName)
            {
                case "Theory":
                case "Fact":
                    MethodDeclarationSyntax methodDeclaration = node.Parent.Parent as MethodDeclarationSyntax;

                    AnalyzeTest(methodDeclaration);
                    break;
            }

            base.VisitAttribute(node);
        }

        private void AnalyzeTest(MethodDeclarationSyntax methodDeclaration)
        {
            var foo = methodDeclaration.Identifier.Text;

            int beforeCount = FoundDiagnostics.Count;

            TestViolations.SearchForIfStatements(methodDeclaration, FoundDiagnostics, CurrentSourceCodeFile);
            TestViolations.SearchForForEachStatements(methodDeclaration, FoundDiagnostics, CurrentSourceCodeFile);
            TestViolations.SearchForMagicNumbers(methodDeclaration, FoundDiagnostics, CurrentSourceCodeFile);

            if (FoundDiagnostics.Count != beforeCount)
            {
                NumberOfTestsWithIssues++;
            }

            NumberOfTestsFound++;
        }
    }
}
