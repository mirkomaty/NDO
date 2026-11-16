# NDO

This is the source code of .NET Data Objects (NDO). See: https://www.netdataobjects.de.

The newest development happens in the main/vx.y branches, e.g. main/v6.0. The newest stable version is usually in the master branch.

## Build

Checkout this repository. Make sure, you are in the master branch. CD to the-path-of-the-repository\Make directory.

Enter 
dotnet run build.cs --all --no-consumers

This should build all packages and the .vsix extension. 
Using the .vsix simplifies development with NDO significantly. You can download the newest version of the .vsix [from here](https://www.netdataobjects.de/media/NDOPackage-VS2022.vsix). 

## Build with VS

No special preparations are required to debug NDO, as the symbols are available on the NuGet symbol server. Simply right-click on a function name—for example, `MakePersistent`—and select "Step into."

## Users Guide

See the users guide here: https://www.netdataobjects.de/en-us/how-ndo-works/ndo-users-guide/.
