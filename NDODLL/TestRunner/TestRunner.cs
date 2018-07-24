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


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using QueryTests;
using NDO;
using NUnit.Framework;
using System.Reflection;

namespace NDO
{
    class TestRunner
    {
        static void Main(string[] args)
        {
            PersistenceManager pm = new PersistenceManager();
#if true
            //return;
            //PersistenceManager pm = new PersistenceManager();
            foreach (string s in pm.BuildDatabase())
            {
                Console.WriteLine(s);
            }
#endif
            DateTime startTime = DateTime.Now;

#if true
            NDOQueryTests t = new NDOQueryTests();
            try
            {
                t.SetUp();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
			try
			{
	            t.TestIfLinqQueryWithOidParameterWorks();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			//try
			//{
                t.TearDown();
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.ToString());
            //}

#else
            Type[] allTypes = typeof(NDOQueryTests).Assembly.GetTypes();
            Console.WriteLine("Number of types: " + allTypes.Length);
            int passed = 0;
            int failed = 0;
            foreach (Type t in allTypes)
            {
                if (t.IsClass)
                {
                    if (t.GetCustomAttributes(typeof(NUnit.Framework.TestFixtureAttribute), false).Length > 0)
                    {
                        MethodInfo setUp = null;
                        MethodInfo tearDown = null;
                        object theTest = Activator.CreateInstance(t);
                        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        //                        methods.AddRange(t.GetMethods(BindingFlags.NonPublic));
                        foreach (MethodInfo mi in methods)
                        {
                            if (mi.GetCustomAttributes(typeof(NUnit.Framework.SetUpAttribute), false).Length > 0)
                                setUp = mi;
                            if (mi.GetCustomAttributes(typeof(NUnit.Framework.TearDownAttribute), false).Length > 0)
                                tearDown = mi;
                        }
                        foreach (MethodInfo mi in methods)
                        {
                            if (mi.GetCustomAttributes(typeof(NUnit.Framework.TestAttribute), false).Length > 0)
                            {
                                string testName = t.FullName + "." + mi.Name;
                                if (mi.GetCustomAttributes(typeof(NUnit.Framework.IgnoreAttribute), false).Length > 0)
                                {
                                    Console.WriteLine("\nIgnore: " + testName);
                                }
                                else
                                {
                                    Type expectedException = null;

                                    //object[] attrs;
                                    //if ((attrs = mi.GetCustomAttributes(typeof(NUnit.Framework.ExpectedExceptionAttribute), false)).Length > 0)
                                    //{
                                    //    expectedException = ((ExpectedExceptionAttribute)attrs[0]).ExpectedException;
                                    //}
                                    if (setUp != null)
                                    {
                                        try
                                        {
                                            setUp.Invoke(theTest, new object[] { });
                                        }
                                        catch (TargetInvocationException ex)
                                        {
                                            Console.WriteLine("\n===Setup Failure===\n" + testName + "\n" + ex.InnerException.ToString());
                                            //Console.Write('-');
                                        }
                                    }
                                    try
                                    {
                                        mi.Invoke(theTest, new object[] { });
                                        Console.Write('.');
                                        passed++;
                                    }
                                    catch (TargetInvocationException ex)
                                    {
                                        if (ex.InnerException.GetType() == expectedException)
                                        {
                                            Console.Write('.');
                                            passed++;
                                        }
                                        else
                                        {
                                            //Console.Write('-');
                                            failed++;
                                            Console.WriteLine("\n===Test Failure===\n" + testName + "\n" + ex.InnerException.ToString());
                                        }
                                    }
                                    if (tearDown != null)
                                    {
                                        try
                                        {
                                            tearDown.Invoke(theTest, new object[] { });
                                        }
                                        catch (TargetInvocationException ex)
                                        {
                                            Console.WriteLine("\n===Tear Down Failure===\n" + testName + "\n" + ex.InnerException.ToString());
                                            Console.Write("Trying to call BuildDatabase");
                                            try
                                            {
                                                pm.BuildDatabase();
                                            }
                                            catch (Exception ex2)
                                            {
                                                Console.WriteLine("\n===BuildDatabase Failure===\n" + testName + "\n" + ex2.ToString());
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

            }
            Console.WriteLine("\nPassed: " + passed + ", Failed: " + failed);
#endif
            TimeSpan ts = DateTime.Now - startTime;
            Console.WriteLine("Ready (" + ts + ") - Press the Return key");
            Console.ReadLine();

        }

    }
}
