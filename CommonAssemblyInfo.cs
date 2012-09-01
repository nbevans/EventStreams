using System.Reflection;
using System.Runtime.CompilerServices;

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

[assembly: InternalsVisibleTo(
    "EventStreams.Tests, PublicKey=" +
    "00240000048000009400000006020000002400005253413100040000010001001700b2cd698b1d" +
    "451273623856b339c05e72073013952930b55f673fcbec145cf1092e51ec552cc58dcb1a03c2ad" +
    "f3e769542837c483b296f10189aee8cf6d9e0e096dfcaad367fde01ae09428a0192e97ab4e8040" +
    "4ec02076a48731fc6d0c86471eb9342959bcae2bf04e8b5689c2f4ad4a3c1ba9ce41dd22288cf0" +
    "34bdf1ad")]