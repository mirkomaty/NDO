@SET V=%1
@if "%2"=="" goto NOGUID
@SET USEGUIDS=USEGUIDS
@goto GUIDREADY
:NOGUID
@SET USEGUIDS=
:GUIDREADY
@nmake -f conftests.mak 
@if errorlevel 2 if not errorlevel 3 goto ERR1
@echo OK
@goto EXIT
:ERR1
@echo ****** Error in Configuration ******
:EXIT
