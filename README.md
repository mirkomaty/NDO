# NDO

This is the source code of .NET Data Objects (NDO). See: https://www.netdataobjects.de.

__The latest development__ happens in the branch MergeFor4.0.

## Build

Checkout this repository and switch to the MergeFor4.0 branch. Open a Visual Studio developer console and CD to the-path-of-the-repository\make directory.

Enter 
msbuild ndo.proj

This should build the whole development infrastructure, the NDO package (in the directory NDODLL/bin/release) and the .vsix extension. 
Using the .vsix simplifies development with NDO significantly.

## Users Guide

See the users guide here: https://www.netdataobjects.de/wie-funktioniert-ndo/ndo-users-guide/.
