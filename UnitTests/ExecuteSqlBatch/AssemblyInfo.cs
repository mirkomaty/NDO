﻿//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System.Reflection;
using System.Runtime.CompilerServices;

//
// Allgemeine Informationen ï¿½ber eine Assembly werden ï¿½ber folgende Attribute 
// gesteuert. ï¿½ndern Sie diese Attributswerte, um die Informationen zu modifizieren,
// die mit einer Assembly verknï¿½pft sind.
//
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Versionsinformationen fï¿½r eine Assembly bestehen aus folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie kï¿½nnen alle Werte oder die standardmï¿½ï¿½ige Revision und Buildnummer 
// mit '*' angeben:

[assembly: AssemblyVersion("1.0.*")]

//
// Um die Assembly zu signieren, mï¿½ssen Sie einen Schlï¿½ssel angeben. Weitere Informationen 
// ï¿½ber die Assemblysignierung finden Sie in der Microsoft .NET Framework-Dokumentation.
//
// Mit den folgenden Attributen kï¿½nnen Sie festlegen, welcher Schlï¿½ssel fï¿½r die Signierung verwendet wird. 
//
// Hinweise: 
//   (*) Wenn kein Schlï¿½ssel angegeben ist, wird die Assembly nicht signiert.
//   (*) KeyName verweist auf einen Schlï¿½ssel, der im CSP (Crypto Service
//       Provider) auf Ihrem Computer installiert wurde. KeyFile verweist auf eine Datei, die einen
//       Schlï¿½ssel enthï¿½lt.
//   (*) Wenn die Werte fï¿½r KeyFile und KeyName angegeben werden, 
//       werden folgende Vorgï¿½nge ausgefï¿½hrt:
//       (1) Wenn KeyName im CSP gefunden wird, wird dieser Schlï¿½ssel verwendet.
//       (2) Wenn KeyName nicht vorhanden ist und KeyFile vorhanden ist, 
//           wird der Schlï¿½ssel in KeyFile im CSP installiert und verwendet.
//   (*) Um eine KeyFile zu erstellen, kï¿½nnen Sie das Programm sn.exe (Strong Name) verwenden.
//       Wenn KeyFile angegeben wird, muss der Pfad von KeyFile
//       relativ zum Projektausgabeverzeichnis sein:
//       %Project Directory%\obj\<configuration>. Wenn sich KeyFile z.B.
//       im Projektverzeichnis befindet, geben Sie das AssemblyKeyFile-Attribut 
//       wie folgt an: [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Das verzï¿½gern der Signierung ist eine erweiterte Option. Weitere Informationen finden Sie in der
//       Microsoft .NET Framework-Dokumentation.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
