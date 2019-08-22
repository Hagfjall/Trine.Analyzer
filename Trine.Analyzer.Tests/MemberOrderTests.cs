using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trine.Analyzer.Tests.TestHelper;
using Trine.Analyzer;

namespace Trine.Analyzer.Tests
{
    [TestClass]
    public class MemberOrderTests : CodeFixVerifier
    {

        [TestMethod]
        public void NoDiagnosticsWhenEmpty()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void InvalidOrderWithFixer()
        {
            var test = @"
namespace Trine
{
    public class TestClass
    {
        public class SubClass {}

        private string PrivateProperty { get; }
        internal string InternalProperty { get; }

        int nonConstField;
        const int constField = 1;

        // Keep comment
        protected TestClass() {}

        public TestClass(string title, string details) {}

        public void Method() {}
        public static void StaticMethod() {}
    }
}
";
            VerifyCSharpDiagnostic(test, new[]{
                new DiagnosticResult
                {
                    Id = "TRINE01",
                    Message = "Property should be declared before Class",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {new DiagnosticResultLocation("Test0.cs", 8, 9)}
                },
                new DiagnosticResult
                {
                    Id = "TRINE01",
                    Message = "Internal should be declared before Private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {new DiagnosticResultLocation("Test0.cs", 9, 9)}
                },
                new DiagnosticResult
                {
                    Id = "TRINE01",
                    Message = "Field should be declared before Property",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {new DiagnosticResultLocation("Test0.cs", 11, 9)}
                },
                new DiagnosticResult
                {
                    Id = "TRINE01",
                    Message = "Constant should be declared before Field",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {new DiagnosticResultLocation("Test0.cs", 12, 9)}
                },
                new DiagnosticResult
                {
                    Id = "TRINE01",
                    Message = "Public should be declared before Protected",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {new DiagnosticResultLocation("Test0.cs", 17, 9)}
                },
                new DiagnosticResult
                {
                    Id = "TRINE01",
                    Message = "Static should be declared before NonStatic",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {new DiagnosticResultLocation("Test0.cs", 20, 9)}
                },
            });

            var fixtest = @"
namespace Trine
{
    public class TestClass
    {
        const int constField = 1;

        int nonConstField;

        public TestClass(string title, string details) {}

        // Keep comment
        protected TestClass() {}
        internal string InternalProperty { get; }

        private string PrivateProperty { get; }
        public static void StaticMethod() {}

        public void Method() {}
        public class SubClass {}
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MemberOrderCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MemberOrderAnalyzer();
        }
    }
}
