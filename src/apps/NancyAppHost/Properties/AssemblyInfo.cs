using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NancyAppHost")]
[assembly: AssemblyDescription("Http host for NancyFX application")]
[assembly: AssemblyProduct("NancyAppHost")]
[assembly: AssemblyCopyright("Copyright makovich.great.pm © 2013")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: Guid("a78ce637-b4c6-4b3c-82f8-e6212ac1ab9b")]

// Allow tests assemblies and NSubstitute use some internal members
[assembly: InternalsVisibleTo("NancyAppHost.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]