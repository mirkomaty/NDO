# NDO

This is the source code of .NET Data Objects (NDO). See: https://www.netdataobjects.de.

The newest development happens in the master branch.

## Build

Checkout this repository. Make sure, you are in the master branch. Open a Visual Studio developer console and CD to the-path-of-the-repository\make directory.

Enter 
msbuild ndo.proj

This should build the whole development infrastructure, the NDO package (in the directory NDODLL/bin/release) and the .vsix extension. 
Using the .vsix simplifies development with NDO significantly.

## Build with VS

If you need to debug the NDO.dll, open the NDO.sln in the directory /NDODLL. Recompile with DEBUG configuration. You can find the result in the bin/debug/net452 directory (or in netstandard2.0).

## Users Guide

See the users guide here: https://www.netdataobjects.de/wie-funktioniert-ndo/ndo-users-guide/.
