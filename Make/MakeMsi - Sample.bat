@"C:\Program Files (x86)\WiX Toolset v3.6\bin\candle.exe" -ext WixNetFxExtension -ext wixUtilExtension -sw1044 -out NDODevEdition21.wixobj NDOEnhancerEnterprise21.xml 
@"C:\Program Files (x86)\WiX Toolset v3.6\bin\light.exe" -ext WixNetFxExtension -ext wixUtilExtension -out ..\Deploy\Enterprise\NDODevEdition21.msi NDODevEdition21.wixobj
pause
