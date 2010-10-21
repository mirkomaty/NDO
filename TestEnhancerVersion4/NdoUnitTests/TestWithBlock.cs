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
using System.Collections;
using NDO;
using NUnit.Framework;
using TestBlocks;

namespace NdoUnitTests
{
	[TestFixture]
	public class TestWithBlock
	{
		PersistenceManager pm;
		[SetUp]
		public void Setup() 
		{
			pm = PmFactory.NewPersistenceManager();
		}

		[TearDown]
		public void TearDown() 
		{
			pm.Abort();
			IList l = pm.GetClassExtent(typeof(ClassWithBlock));
			pm.Delete(l);
			pm.Save();
			pm.Close();
			pm = null;
		}

		[Test]
		public void TestBlock()
		{
			ClassWithBlock cwb = new ClassWithBlock();
			cwb.Testfeld = "abc";
			pm.MakePersistent(cwb);
			pm.Save();
			cwb.Testfeld = "def";
			Assert.AreEqual(NDOObjectState.PersistentDirty, cwb.NDOObjectState, "Objekt sollte dirty sein");
		}
	}
}
