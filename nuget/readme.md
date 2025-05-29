# NDO.Build

This is a Package to enable VS Solutions to compile and enhance persistent classes.

## Release Notes

From v5.0 on NDO.Build contains the NDOEnhancer, while NDO.dll only contains the NDO framework.

For assemblies containing persistent classes, use NDO.Build _and_ NDO.dll. For assemblies using the persistent classes NDO.Build is not essential, but it helps to create the correct mapping files.

NDO.Build is controlled by a .ndoproj file provided by the NDO Data Objects plugin for Visual Studio. We recommend installing this plugin.