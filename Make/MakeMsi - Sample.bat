@"C:\Program Files (x86)\Windows Installer XML v3.5\bin\candle.exe" -ext WixNetFxExtension -sw1044 -out NDOEnhancerEnterprise21.wixobj NDOEnhancerEnterprise21.xml > wixerrors.txt
@"C:\Program Files (x86)\Windows Installer XML v3.5\bin\light.exe" -ext WixNetFxExtension -out ..\Deploy\Enterprise\NDOEnhancerEnterprise21.msi NDOEnhancerEnterprise21.wixobj >>wixerrors.txt
pause
