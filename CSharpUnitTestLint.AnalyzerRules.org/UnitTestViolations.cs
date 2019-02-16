//
// UnitTestViolations.cs
//
// Product:  CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.AnalyzerRules
//
// Description: Implements Analysis methods for investigating various unit test issues.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

// ReSharper disable InvocationIsSkipped



namespace CSharpUnitTestLint.AnalyzerRules
{
    /// <summary>
    /// Contains Analysis methods for investigating various unit test issues.
    /// </summary>
    public class UnitTestViolations : IUnitTestsViolations
    {
        #region Private Instance Properties

        /// <summary>
        /// Maps unit test violations to their diagnostic descriptors so that 
        /// diagnostics may be reported.
        /// </summary>
        private readonly Dictionary<SyntaxKind, DiagnosticDescriptor> _descriptorsMap;

        #endregion Private Instance Properties



        #region Public Instance Properties

        /// <summary>
        /// The list of descriptors for which this class produces diagnostics.
        /// </summary>
        public ImmutableArray<DiagnosticDescriptor> SupportedDescriptors => 
            ImmutableArray.Create(
                Descriptors.ForLoopStatementDescriptor, 
                Descriptors.ForeachStatementDescriptor,
                Descriptors.IfStatementDescriptor,
                Descriptors.MagicNumberDescriptor,
                Descriptors.TernaryOperatorStatementDescriptor, 
                Descriptors.TryCatchStatementDescriptor,
                Descriptors.WhileLoopStatementDescriptor);

        #endregion Private Instance Fields


        public UnitTestViolations()
        {
            _descriptorsMap = new Dictionary<SyntaxKind, DiagnosticDescriptor>
            {
                {SyntaxKind.ForStatement, Descriptors.ForLoopStatementDescriptor},
                {SyntaxKind.ForEachStatement, Descriptors.ForeachStatementDescriptor},
                {SyntaxKind.IfStatement, Descriptors.IfStatementDescriptor},
                {SyntaxKind.ConditionalExpression, Descriptors.TernaryOperatorStatementDescriptor},
                {SyntaxKind.TryStatement, Descriptors.TryCatchStatementDescriptor},
                {SyntaxKind.WhileStatement, Descriptors.WhileLoopStatementDescriptor}
            };
        }
        


        #region IUnitTestViolations Implementation

        /// <summary>
        /// Process a MethodDeclaratoinSyntax instance looking for unit test violations, 
        /// and reporting them if found.
        /// </summary>
        /// <param name="methodDeclSyntax">
        /// An instance of methodDeclarationSyntax that represents a unit test method. It is
        /// the responsibility of clients to pass actual unit test methods as arguments.
        /// </param>
        /// <returns>
        /// A list of .NET Compiler platform diagnostics. Each diagnostic represents a single
        /// unit test violation.
        /// </returns>
        public IList<Diagnostic> ProcessMethodDeclarationSyntax(MethodDeclarationSyntax methodDeclSyntax)
        {
            // We may assume this because _descriptorsMap is clearly set to an instance in the constructor.
            Contract.Assume(_descriptorsMap != null);

            IList<Diagnostic> foundDiagnostics = new List<Diagnostic>();

            foundDiagnostics =
                foundDiagnostics.Union(SearchForViolationsByCSharpSyntaxNode<IfStatementSyntax>(methodDeclSyntax)).ToList();
            foundDiagnostics = 
                foundDiagnostics.Union(SearchForViolationsByCSharpSyntaxNode<ForStatementSyntax>(methodDeclSyntax)).ToList();
            foundDiagnostics =
                foundDiagnostics.Union(SearchForViolationsByCSharpSyntaxNode<TryStatementSyntax>(methodDeclSyntax)).ToList();
            foundDiagnostics =
                foundDiagnostics.Union(SearchForViolationsByCSharpSyntaxNode<ForEachStatementSyntax>(methodDeclSyntax)).ToList();
            foundDiagnostics =
                foundDiagnostics.Union(SearchForViolationsByCSharpSyntaxNode<WhileStatementSyntax>(methodDeclSyntax)).ToList();
            foundDiagnostics =
                foundDiagnostics.Union(SearchForViolationsByCSharpSyntaxNode<ConditionalExpressionSyntax>(methodDeclSyntax)).ToList();

            foundDiagnostics =
                foundDiagnostics.Union(SearchForMagicNumbers(methodDeclSyntax)).ToList();

            return foundDiagnostics;
        }

        #endregion IUnitTestViolations Implementation



        #region Rules For Syntax Kinds

        /// <summary>
        /// Looks for magic numbers within a unit test.
        /// </summary>
        /// <param name="methodDeclaration">
        /// An instance of methodDeclarationSyntax that represents a unit test method. It is
        /// the responsibility of clients to pass actual unit test methods as arguments.
        /// </param>
        /// <returns>
        /// A list of .NET Compiler platform diagnostics. Each diagnostic represents a single
        /// unit test violation.
        /// </returns>
        private IList<Diagnostic> SearchForMagicNumbers(MethodDeclarationSyntax methodDeclaration)
        {
            Contract.Requires(methodDeclaration != null);
            Contract.Ensures(Contract.Result<IList<Diagnostic>>() != null);

            // TODO: Try to make this more generic like the other rules.

            IList<Diagnostic> foundDiagnostics = new List<Diagnostic>();

            if (methodDeclaration.DescendantNodesAndTokens() != null)
            {
                IEnumerable<SyntaxNodeOrToken> childTokens = methodDeclaration
                    .DescendantNodesAndTokens()?
                    .Where(ct => ct.Kind() == SyntaxKind.NumericLiteralExpression)
                    .Select(ct => ct);

                Contract.Assume(childTokens != null);

                foreach (var syntaxToken in childTokens)
                {
                    Contract.Assume(syntaxToken.Parent?.Parent?.Parent != null);

                    // Look for magic numbers in an invocation expression.
                    if (syntaxToken.Parent.Parent.Parent.Kind() == SyntaxKind.InvocationExpression)
                    {
                        foundDiagnostics.Add(Diagnostic.Create(Descriptors.MagicNumberDescriptor, syntaxToken.GetLocation()));
                    }
                }
            }

            return foundDiagnostics;
        }

        #endregion Rules For Syntax Kinds



        #region Private Instance Utility Methods

        /// <summary>
        /// A generic method that can find certain unit test violation by
        /// SyntaxNode type
        /// </summary>
        /// <typeparam name="T">
        /// The type of SyntaxNode that if found, represents a unit test violation.
        /// </typeparam>
        /// <returns>
        /// A list of Diagnostic that indicates found unit test violations.
        /// </returns>
        // ReSharper disable once InconsistentNaming
        private IList<Diagnostic> SearchForViolationsByCSharpSyntaxNode<T>(
            MethodDeclarationSyntax methodDeclaration) 
            where T : CSharpSyntaxNode
        {
            Contract.Requires(_descriptorsMap != null);
            Contract.Requires(methodDeclaration != null);
            Contract.Ensures(Contract.Result<IList<Diagnostic>>() != null);

            IList<Diagnostic> foundDiagnostics = new List<Diagnostic>();

            foreach (T node in GetListOfSyntaxNode<T>(methodDeclaration))
            {
                // We may make this assumption, because we would not be in the body
                // of the foreach loop unless there were nodes in the collection!
                Contract.Assume(node != null);
                foundDiagnostics.Add(Diagnostic.Create(_descriptorsMap[node.Kind()], node.GetLocation()));
            }

            return foundDiagnostics;
        }

        
        /// <summary>
        /// A generic method that can find types of SyntaxNode, used to reduce the amount
        /// of boilerplate code in the rules. 
        /// </summary>
        /// <typeparam name="T">
        /// The type of SyntaxNode to find.
        /// </typeparam>
        /// <param name="methodSyntax">
        /// A method declaration syntax for which we want to find SyntaxNode of type T.
        /// </param>
        /// <returns>
        /// A list of violations found for the type T.
        /// </returns>
        private List<T> GetListOfSyntaxNode<T>(MethodDeclarationSyntax methodSyntax)
        {
            Contract.Requires(methodSyntax != null);
            Contract.Ensures(Contract.Result<List<T>>() != null);

            IEnumerable<SyntaxNode> descendantNodes = methodSyntax.DescendantNodes();

            // The null checks are needed to satisfy Code Contracts. Descendant nodes
            // will never be null for methodSyntax, since a MethodDeclarationSyntax
            // instance will always have descendants if it itself is not null.
            List<T> returnList = descendantNodes?.OfType<T>().ToList() ?? new List<T>();

            return returnList;
        }

        #endregion Private Instance Utility Methods
    }
}
