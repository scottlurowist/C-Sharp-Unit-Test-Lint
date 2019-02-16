using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using CSharpUnitTestLint.AnalyzerRules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


namespace CSharpUnitTestLint.VisualStudio
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CSharpUnitTestLintVisualStudioAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSharpUnitTestLintVisualStudio";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private UnitTestViolations _unitTestViolations = new UnitTestViolations(); 

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        //private static readonly DiagnosticDescriptor RuleDescriptor =
        //    new DiagnosticDescriptor(Constants.RuleDescriptorIds.FindXUnitFactAttributes,
        //        Title, MessageFormat, Constants.UnitTestingCategory,
        //        DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return _unitTestViolations.SupportedDescriptors; }
        }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterSyntaxNodeAction(FindAndAnalyzeUnitTestMethods, SyntaxKind.MethodDeclaration);
        }



        /// <summary>
        /// Performs analysis of unit tests for bad practices.
        /// </summary>
        /// <param name="context">
        /// Context for a syntax node action. A syntax node action can use a 
        /// Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext
        /// to report Microsoft.CodeAnalysis.Diagnostics for a Microsoft.CodeAnalysis.SyntaxNode.
        /// </param>
        private void FindAndAnalyzeUnitTestMethods(SyntaxNodeAnalysisContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            MethodDeclarationSyntax methodDeclarationSyntax = context.Node as MethodDeclarationSyntax;

            // For performance reasons, grab the model only once, and reuse it.
            SemanticModel model = context.SemanticModel;

            Utilities utils = new Utilities();

            if (utils.IsTheMethodAUnitTest(methodDeclarationSyntax, model))
            {
                //var diagnostic = Diagnostic.Create(SupportedDiagnostics,
                //    // ReSharper disable once PossibleNullReferenceException
                //    // This cannot be null, because all of our analyzers 
                //    // look for methods.
                //    methodDeclarationSyntax.Identifier.GetLocation(), "[Fact]");

                //context.ReportDiagnostic(diagnostic);
                IList<Diagnostic> diagnostics = _unitTestViolations.ProcessMethodDeclarationSyntax(methodDeclarationSyntax);

                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }


                var x = 4;
            }
        }

        //private static void AnalyzeSymbol(SymbolAnalysisContext context)
        //{
        //    // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
        //    var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        //    // Find just those named type symbols with names containing lowercase letters.
        //    if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
        //    {
        //        // For all such symbols, produce a diagnostic.
        //        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

        //        context.ReportDiagnostic(diagnostic);
        //    }
        //}
    }
}
