//
// Utilities.cs
//
// Product: CSharp Unit Test Lint
//
// Component: CSharpUnitTestLintRoslyn
//
// Description: Utilities for Analyzers and CodeFixProviders.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//



using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace CSharpUnitTestLint.VisualStudio
{
    /// <summary>
    /// Utilities for Analyzers and CodeFixProviders.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Finds methods that are unit test methods. It currently only
        /// support xUnit.
        /// </summary>
        /// <param name="method">
        /// An instance of MethodDeclarationSyntax to be checked if it is
        /// an xUnit [Fact] test method.
        /// </param>
        /// <param name="model">An instance of the semantic model for the analysis.
        /// </param>
        /// <returns>
        /// true if the method is a unit test method; otherwise, false. Currently,
        /// only xUnit is supported.
        /// </returns>
        public bool IsTheMethodAUnitTest(MethodDeclarationSyntax method, SemanticModel model)
        {
            bool returnValue = false;

            IMethodSymbol methodSymbol = model.GetDeclaredSymbol(method);

            foreach (var attribute in methodSymbol.GetAttributes())
            {
                if (attribute.AttributeClass.Name == "FactAttribute")
                {
                    returnValue = true;
                }
            }

            return returnValue;
        }
    }
}
