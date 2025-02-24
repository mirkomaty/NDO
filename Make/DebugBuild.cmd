@echo off
msbuild -p:Configuration=Debug -p:RestoreConfigFile=.\nuget.config test.proj
