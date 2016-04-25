//
// Copyright (C) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
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


using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("NDO")]
[assembly: AssemblyDescription(".NET Data Objects Library")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else 
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Mirko Matytschak")]
[assembly: AssemblyProduct("NDO 3.0")]
[assembly: AssemblyCopyright("(c) 2002 - 2016, Mirko Matytschak, Marzling, Germany")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]

// Assembly Version
[assembly: AssemblyVersion("3.0.0.6")]

[assembly: AssemblyKeyName("")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: System.Security.SecurityRules( System.Security.SecurityRuleSet.Level1 )]

