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


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using NDO;
using NUnit.Framework;
using System.Reflection;
using RelationUnitTests;

namespace UnitTests
{
	class TestRunner
	{
		static void Main( string[] args )
		{

#if true
			Console.WriteLine( "BuildDatabase:" );
			PersistenceManager pm = new PersistenceManager();
			foreach (string s in pm.BuildDatabase())
			{
				Console.WriteLine( s );
			}
			Console.WriteLine( "BuildDatabase ready." );
			return;
#endif
			DateTime startTime = DateTime.Now;

#if false
			TestAgrBi1nOwnpconTblAuto test = new TestAgrBi1nOwnpconTblAuto();
			test.Setup();
//			try 
//			{
			test.TestSaveReload();
//			} 
//			catch (Exception ex){Debug.WriteLine(ex);}
			test.TearDown();

#else
			Type[] allTypes = typeof( TestCmpBin1TblAuto ).Assembly.GetTypes();
			Console.WriteLine( "Number of types: " + allTypes.Length );
			int passed = 0;
			int failed = 0;
			foreach (Type t in allTypes)
			{
				if (t.IsClass)
				{
					if (t.GetCustomAttributes( typeof( NUnit.Framework.TestFixtureAttribute ), false ).Length > 0)
					{
						MethodInfo setUp = null;
						MethodInfo tearDown = null;
						object theTest = Activator.CreateInstance( t );
						MethodInfo[] methods = t.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
						//                        methods.AddRange(t.GetMethods(BindingFlags.NonPublic));
						foreach (MethodInfo mi in methods)
						{
							if (mi.GetCustomAttributes( typeof( NUnit.Framework.SetUpAttribute ), false ).Length > 0)
								setUp = mi;
							if (mi.GetCustomAttributes( typeof( NUnit.Framework.TearDownAttribute ), false ).Length > 0)
								tearDown = mi;
						}
						foreach (MethodInfo mi in methods)
						{
							if (mi.GetCustomAttributes( typeof( NUnit.Framework.TestAttribute ), false ).Length > 0)
							{
								string testName = t.FullName + "." + mi.Name;
								if (mi.GetCustomAttributes( typeof( NUnit.Framework.IgnoreAttribute ), false ).Length > 0)
								{
									Console.WriteLine( "\nIgnore: " + testName );
								}
								else
								{
									object[] attrs;

									if (setUp != null)
									{
										try
										{
											setUp.Invoke( theTest, new object[] { } );
										}
										catch (TargetInvocationException ex)
										{
											Console.WriteLine( "\n===Setup Failure===\n" + testName + "\n" + ex.InnerException.ToString() );
											Debug.WriteLine( ex.ToString() );
											//Console.Write('-');
										}
									}
									try
									{
										mi.Invoke( theTest, new object[] { } );
										Console.Write( '.' );
										passed++;
									}
									catch (TargetInvocationException ex)
									{
										failed++;
										Console.WriteLine( "\n===Test Failure===\n" + testName + "\n" + ex.InnerException.ToString() );
										Debug.WriteLine( "\n===Test Failure===\n" + testName + "\n" + ex.InnerException.ToString() );
									}
									if (tearDown != null)
									{
										try
										{
											tearDown.Invoke( theTest, new object[] { } );
										}
										catch (TargetInvocationException ex)
										{
											Console.WriteLine( "\n===Tear Down Failure===\n" + testName + "\n" + ex.InnerException.ToString() );
											Debug.WriteLine( "\n===Tear Down Failure===\n" + testName + "\n" + ex.InnerException.ToString() );
											Console.Write( "Trying to call BuildDatabase" );
											Debug.Write( "Trying to call BuildDatabase" );
											try
											{
												pm.BuildDatabase();
											}
											catch (Exception ex2)
											{
												Console.WriteLine( "\n===BuildDatabase Failure===\n" + testName + "\n" + ex2.ToString() );
												Debug.WriteLine( "\n===BuildDatabase Failure===\n" + testName + "\n" + ex2.ToString() );
											}
											Debug.WriteLine( "" );
											Console.WriteLine();
										}
									}

								}
							}
						}
					}
				}

			}
			Console.WriteLine( "\nPassed: " + passed + ", Failed: " + failed );
#endif
			TimeSpan ts = DateTime.Now - startTime;
			Console.WriteLine( "Ready (" + ts + ") - Press the Return key" );
			Console.ReadLine();

		}
	}
}
