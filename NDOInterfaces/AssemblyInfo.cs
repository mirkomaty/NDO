//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System;

//
// Allgemeine Informationen �ber eine Assembly werden �ber folgende Attribute 
// gesteuert. �ndern Sie diese Attributswerte, um die Informationen zu modifizieren,
// die mit einer Assembly verkn�pft sind.
//
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		
[assembly: AllowPartiallyTrustedCallers]
[assembly: CLSCompliant(true)]


//
// Versionsinformationen f�r eine Assembly bestehen aus folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie k�nnen alle Werte oder die standardm��ige Revision und Buildnummer 
// mit '*' angeben:

#if NDO11
[assembly: AssemblyVersion("1.1.0.0")]
#endif
#if NDO12
[assembly: AssemblyVersion("1.2.0.0")]
#endif
#if NDO20
[assembly: AssemblyVersion("2.1.0.0")]
#endif

//
// Um die Assembly zu signieren, m�ssen Sie einen Schl�ssel angeben. Weitere Informationen 
// �ber die Assemblysignierung finden Sie in der Microsoft .NET Framework-Dokumentation.
//
// Mit den folgenden Attributen k�nnen Sie festlegen, welcher Schl�ssel f�r die Signierung verwendet wird. 
//
// Hinweise: 
//   (*) Wenn kein Schl�ssel angegeben ist, wird die Assembly nicht signiert.
//   (*) KeyName verweist auf einen Schl�ssel, der im CSP (Crypto Service
//       Provider) auf Ihrem Computer installiert wurde. KeyFile verweist auf eine Datei, die einen
//       Schl�ssel enth�lt.
//   (*) Wenn die Werte f�r KeyFile und KeyName angegeben werden, 
//       werden folgende Vorg�nge ausgef�hrt:
//       (1) Wenn KeyName im CSP gefunden wird, wird dieser Schl�ssel verwendet.
//       (2) Wenn KeyName nicht vorhanden ist und KeyFile vorhanden ist, 
//           wird der Schl�ssel in KeyFile im CSP installiert und verwendet.
//   (*) Um eine KeyFile zu erstellen, k�nnen Sie das Programm sn.exe (Strong Name) verwenden.
//       Wenn KeyFile angegeben wird, muss der Pfad von KeyFile
//       relativ zum Projektausgabeverzeichnis sein:
//       %Project Directory%\obj\<configuration>. Wenn sich KeyFile z.B.
//       im Projektverzeichnis befindet, geben Sie das AssemblyKeyFile-Attribut 
//       wie folgt an: [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Das verz�gern der Signierung ist eine erweiterte Option. Weitere Informationen finden Sie in der
//       Microsoft .NET Framework-Dokumentation.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\..\NDOInterfaces.snk")]
[assembly: AssemblyKeyName("")]
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
