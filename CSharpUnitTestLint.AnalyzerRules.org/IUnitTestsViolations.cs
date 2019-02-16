//
// IUnitTestViolations.cs
//
// Product:  CSharp Unit Test Lint
//
// Component: CSharpUnitTestLint.AnalyzerRules
//
// Description: Defines Analysis methods for investigating various unit test issues.
//
// Author: Scott Lurowist
//
// Copyright © 2017
//


using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
// ReSharper disable InvocationIsSkipped



namespace CSharpUnitTestLint.AnalyzerRules
{
    /// <summary>
    /// Defines analysis methods for investigating various unit test issues.
    /// </summary>
    [ContractClass(typeof(IUnitTestViolationsContract))]
    public interface IUnitTestsViolations
    {
        /// <summary>
        /// Process a MethodDeclarationSyntax instance looking for unit test violations, 
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
        /// <remarks>
        /// DESIGN BY CONTRACT:
        /// 
        /// 1. MethodDeclarationSyntax cannot be null.
        /// 2. MethodDeclarationSyntax must represent a unit test method, otherwise false
        ///    diagnostics will be generated.
        /// 
        /// </remarks>
        IList<Diagnostic> ProcessMethodDeclarationSyntax(MethodDeclarationSyntax methodDeclSyntax);
    }


    /// <summary>
    /// A Code Contracts buddy class for IUnitTestViolations.
    /// </summary>
    [ContractClassFor(typeof(IUnitTestsViolations))]
    // ReSharper disable once InconsistentNaming
    abstract class IUnitTestViolationsContract : IUnitTestsViolations
    {
        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            // TODO: Implement if needed.     
        }


        IList<Diagnostic> IUnitTestsViolations.ProcessMethodDeclarationSyntax(MethodDeclarationSyntax methodDeclSyntax)
        {
            Contract.Requires(methodDeclSyntax != null);
            Contract.Ensures(Contract.Result<IList<Diagnostic>>() != null);

            throw new NotImplementedException();
        }
    }
}
