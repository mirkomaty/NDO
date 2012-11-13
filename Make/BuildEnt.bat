@echo Start Visual Studio!
@call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\Tools\vsvars32.bat"
@SET DEVPATH="C:\Program Files (x86)\Microsoft Visual Studio 2010\Common7\IDE\PublicAssemblies"
@SET V=ENT
"C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\bin\nmake.exe" -f c:\projekte\ndo\make\makefileent.txt
pause
