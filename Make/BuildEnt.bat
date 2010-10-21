@echo Start Visual Studio!
@call "C:\Program Files\Microsoft Visual Studio 2010\Common7\Tools\vsvars32.bat"
@SET DEVPATH="C:\Program Files\Microsoft Visual Studio 2010\Common7\IDE\PublicAssemblies"
@SET V=ENT
nmake -f c:\projekte\ndo\make\makefileent.txt
pause
