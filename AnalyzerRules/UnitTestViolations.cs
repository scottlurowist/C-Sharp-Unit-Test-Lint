using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;


namespace AnalyzerRules
{
    public class UnitTestViolations
    {
        public void SearchForIfStatements(MethodDeclarationSyntax methodDeclaration, IList<Diagnostic> foundDiagnostics, string currentSourceCodeFile)
        {
            List<IfStatementSyntax> ifStatentList = methodDeclaration.DescendantNodes().OfType<IfStatementSyntax>().ToList();

            foreach(IfStatementSyntax ifStatement in ifStatentList)
            {
                Location violationLocation = ifStatement.GetLocation();

                string methodName = methodDeclaration.Identifier.Text;

                DiagnosticDescriptor descriptor = new DiagnosticDescriptor("SRLCSUTL002", "Logic found in a test.",
                    $"Found If Statement in {currentSourceCodeFile} - method {methodName}. Logic is not permitted in a unit test.", "Unit Testing",
                    DiagnosticSeverity.Error, true);

                Diagnostic.Create(descriptor, ifStatement.GetLocation());

                if (foundDiagnostics != null)
                {
                    foundDiagnostics.Add(Diagnostic.Create(descriptor, ifStatement.GetLocation()));
                }
            }
        }


        public void SearchForForEachStatements(MethodDeclarationSyntax methodDeclaration, IList<Diagnostic> foundDiagnostics, string currentSourceCodeFile)
        {
            List<ForEachStatementSyntax> ifStatentList = methodDeclaration.DescendantNodes().OfType<ForEachStatementSyntax>().ToList();

            foreach (ForEachStatementSyntax ifStatement in ifStatentList)
            {
                Location violationLocation = ifStatement.GetLocation();

                string methodName = methodDeclaration.Identifier.Text;

                DiagnosticDescriptor descriptor = new DiagnosticDescriptor("SRLCSUTL003", "Logic found in a test.",
                    $"Found foreach Statement in {currentSourceCodeFile} - method {methodName}. Logic is not permitted in a unit test.", "Unit Testing",
                    DiagnosticSeverity.Error, true);

                Diagnostic.Create(descriptor, ifStatement.GetLocation());

                if (foundDiagnostics != null)
                {
                    foundDiagnostics.Add(Diagnostic.Create(descriptor, ifStatement.GetLocation()));
                }
            }
        }


        public void SearchForMagicNumbers(MethodDeclarationSyntax methodDeclaration, IList<Diagnostic> foundDiagnostics, string currentSourceCodeFile)
        {
            List<SyntaxNodeOrToken> magicNumberList = methodDeclaration
                .DescendantNodesAndTokensAndSelf()
                .Where(ct => ct.Kind() == SyntaxKind.NumericLiteralExpression)
                .Select(ct => ct).ToList();

            foreach (SyntaxNodeOrToken syntaxNodeToken in magicNumberList)
            {
                if (syntaxNodeToken.Parent.Parent.Parent.Kind() == SyntaxKind.InvocationExpression)
                {
                    Location violationLocation = syntaxNodeToken.GetLocation();

                    string magicNumberValue = syntaxNodeToken.AsNode().GetText().ToString();

                    string methodName = methodDeclaration.Identifier.Text;

                    DiagnosticDescriptor descriptor = new DiagnosticDescriptor("SRLCSUTL004", "Magic number found in a test.",
                        $"Found magic number '{magicNumberValue}' in {currentSourceCodeFile} - method {methodName}." +
                        " Magic numbers not permitted in a unit tests because their meaning is unclear.", "Unit Testing",
                        DiagnosticSeverity.Error, true);

                    Diagnostic.Create(descriptor, syntaxNodeToken.GetLocation());

                    if (foundDiagnostics != null)
                    {
                        foundDiagnostics.Add(Diagnostic.Create(descriptor, syntaxNodeToken.GetLocation()));
                    }
                }

            }
        }
    }
}
