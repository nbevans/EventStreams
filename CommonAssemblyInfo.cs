using System.Reflection;

[assembly: AssemblyCompany("Byteflag Ltd")]
[assembly: AssemblyProduct("EventStreams")]
[assembly: AssemblyCopyright("Copyright © MMXII, Byteflag Ltd. All rights reserved.")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion("1.0.*")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif