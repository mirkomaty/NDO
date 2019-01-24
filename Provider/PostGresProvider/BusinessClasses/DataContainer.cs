//
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
using System.Data;
using NDO;



namespace BusinessClasses
{
	/// <summary>
	/// Summary description for DataContainer.
	/// </summary>
	[NDOPersistent]
	public class DataContainer
	{

//		[NDOObjectId]
//		int id = new System.Random().Next();
//		public int Id
//		{
//			get { return id; }
//			set { id = value; }
//		}


		System.Guid guidVar = Guid.NewGuid();//new System.Guid(0x11111111, 0x2222, 0x3333, 4, 5, 6, 7, 8, 9, 10, 11);
		public System.Guid GuidVar
		{
			get { return guidVar; }
			set { guidVar = value; }
		}
		

		System.Byte[] byteArrVar = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16};
		public System.Byte[] ByteArrVar
		{
			get { return byteArrVar; }
			set { byteArrVar = value; }
		}
		
		
		bool boolVar = true;
		public bool BoolVar
		{
			get { return boolVar; }
			set { boolVar = value; }
		}

		byte byteVar = 0x55;
		public byte ByteVar
		{
			get { return byteVar; }
			set { byteVar = value; }
		}
		

		System.Int64 int64Var = 0x123456781234567;
		public System.Int64 Int64Var
		{
			get { return int64Var; }
			set { int64Var = value; }
		}
		
		string stringVar = "Test";
		public string StringVar
		{
			get { return stringVar; }
			set { stringVar = value; }
		}
		
		DateTime dateTimeVar = DateTime.Now;
		public DateTime DateTimeVar
		{
			get { return dateTimeVar; }
			set { dateTimeVar = value; }
		}
		decimal decVar = (decimal)1.234;
		public decimal DecVar
		{
			get { return decVar; }
			set { decVar = value; }
		}
		double doubleVar = 5.678;
		public double DoubleVar
		{
			get { return doubleVar; }
			set { doubleVar = value; }
		}
		float floatVar = 9.012F;
		public float FloatVar
		{
			get { return floatVar; }
			set { floatVar = value; }
		}
		System.Int16 int16Var = 1234;
		public System.Int16 Int16Var
		{
			get { return int16Var; }
			set { int16Var = value; }
		}
		

		System.UInt64 uint64Var = 0x765432187654321;
		public System.UInt64 Uint64Var
		{
			get { return uint64Var; }
			set { uint64Var = value; }
		}
		System.UInt32 uint32Var = 0x7654321;
		public System.UInt32 Uint32Var
		{
			get { return uint32Var; }
			set { uint32Var = value; }
		}
		System.UInt16 uint16Var = 0x765;
		public System.UInt16 Uint16Var
		{
			get { return uint16Var; }
			set { uint16Var = value; }
		}
		


		public DataContainer()
		{
		}
	}
}
