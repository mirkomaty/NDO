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
using NDO;

namespace DataTypeTestClasses
{
	// Keep embedded as unmutable - don't manipulate members,
	// always create a new instance on change
	public class EmbeddedWithOneMember
	{
		string member = "";
		public string Member
		{
			get { return member; }
		}
		public EmbeddedWithOneMember()
		{
		}
		public EmbeddedWithOneMember(string s)
		{
			this.member = s;
		}
	}

	[NDOPersistent]
	public class ParentWithEmbedded
	{
		string test = "";
		public string Test
		{
			get { return test; }
			set { test = value; }
		}
		// Always have embedded objects initialized
		EmbeddedWithOneMember ewom = new EmbeddedWithOneMember();
		public EmbeddedWithOneMember Ewom
		{
			get { return ewom; }
			set { ewom = value; }
		}
	}

	public struct VtPropDataContainer
	{
		public event EventHandler TestEvent;

		EnumType enumVar;
		public EnumType EnumVar
		{
			get { return enumVar; }
			set { enumVar = value; }
		}
		System.Guid guidVar;
		public System.Guid GuidVar
		{
			get { return guidVar; }
			set { guidVar = value; }
		}

		System.Guid emptyGuidVar;
		public System.Guid EmptyGuidVar
		{
			get { return emptyGuidVar; }
			set { emptyGuidVar = value; }
		}

		System.DateTime emptyDateTimeVar;
		public System.DateTime EmptyDateTimeVar
		{
			get { return emptyDateTimeVar; }
			set { emptyDateTimeVar = value; }
		}

	
		bool boolVar;
		public bool BoolVar
		{
			get { return boolVar; }
			set { boolVar = value; }
		}

		byte byteVar;
		public byte ByteVar
		{
			get { return byteVar; }
			set { byteVar = value; }
		}

		string stringVar;
		public string StringVar
		{
			get { return stringVar; }
			set { stringVar = value; }
		}
		
		DateTime dateTimeVar;
		public DateTime DateTimeVar
		{
			get { return dateTimeVar; }
			set { dateTimeVar = value; }
		}

				
		decimal decVar;
		public decimal DecVar
		{
			get { return decVar; }
			set { decVar = value; }
		}

		int int32Var;
		public int Int32Var
		{
			get { return int32Var; }
			set { int32Var = value; }
		}

		System.UInt32 uint32Var;
		public System.UInt32 Uint32Var
		{
			get { return uint32Var; }
			set { uint32Var = value; }
		}

		double doubleVar;
		public double DoubleVar
		{
			get { return doubleVar; }
			set { doubleVar = value; }
		}

		float floatVar;
		public float FloatVar
		{
			get { return floatVar; }
			set { floatVar = value; }
		}


		System.Int64 int64Var;
		public System.Int64 Int64Var
		{
			get { return int64Var; }
			set { int64Var = value; }
		}
		System.Int16 int16Var;
		public System.Int16 Int16Var
		{
			get { return int16Var; }
			set { int16Var = value; }
		}
		

		System.UInt64 uint64Var;
		public System.UInt64 Uint64Var
		{
			get { return uint64Var; }
			set { uint64Var = value; }
		}

		System.UInt16 uint16Var;
		public System.UInt16 Uint16Var
		{
			get { return uint16Var; }
			set { uint16Var = value; }
		}

		string nullString;
		public string NullString
		{
			get { return nullString; }
			set { nullString = value; }
		}

	}

	public struct VtPublicDataContainer
	{
		public event EventHandler TestEvent;

		public System.Guid EmptyGuidVar;
		public System.DateTime EmptyDateTimeVar;
		public EnumType EnumVar;
		public System.Guid GuidVar;		
		public bool BoolVar;
		public byte ByteVar;
		public string StringVar;		
		public DateTime DateTimeVar;
		public decimal DecVar;
		public int Int32Var;
		public System.UInt32 Uint32Var;
		public double DoubleVar;
		public float FloatVar;
		public System.Int64 Int64Var;
		public System.Int16 Int16Var;
		public System.UInt64 Uint64Var;
		public System.UInt16 Uint16Var;
		public string NullString;
	}


	public class EtDataContainer
	{
		public event EventHandler TestEvent;

		EnumType enumVar;
		public EnumType EnumVar
		{
			get { return enumVar; }
			set { enumVar = value; }
		}

		System.Guid guidVar = Guid.NewGuid();
		public System.Guid GuidVar
		{
			get { return guidVar; }
			set { guidVar = value; }
		}

		System.Guid emptyGuidVar;
		public System.Guid EmptyGuidVar
		{
			get { return emptyGuidVar; }
			set { emptyGuidVar = value; }
		}

		System.DateTime emptyDateTimeVar;
		public System.DateTime EmptyDateTimeVar
		{
			get { return emptyDateTimeVar; }
			set { emptyDateTimeVar = value; }
		}

		
		bool boolVar;
		public bool BoolVar
		{
			get { return boolVar; }
			set { boolVar = value; }
		}

		byte byteVar;
		public byte ByteVar
		{
			get { return byteVar; }
			set { byteVar = value; }
		}

		string stringVar;
		public string StringVar
		{
			get { return stringVar; }
			set { stringVar = value; }
		}
		
		DateTime dateTimeVar;
		public DateTime DateTimeVar
		{
			get { return dateTimeVar; }
			set { dateTimeVar = value; }
		}

				
		decimal decVar;
		public decimal DecVar
		{
			get { return decVar; }
			set { decVar = value; }
		}

		int int32Var;
		public int Int32Var
		{
			get { return int32Var; }
			set { int32Var = value; }
		}

		System.UInt32 uint32Var;
		public System.UInt32 Uint32Var
		{
			get { return uint32Var; }
			set { uint32Var = value; }
		}

		double doubleVar;
		public double DoubleVar
		{
			get { return doubleVar; }
			set { doubleVar = value; }
		}

		float floatVar;
		public float FloatVar
		{
			get { return floatVar; }
			set { floatVar = value; }
		}


		System.Int64 int64Var;
		public System.Int64 Int64Var
		{
			get { return int64Var; }
			set { int64Var = value; }
		}
		System.Int16 int16Var;
		public System.Int16 Int16Var
		{
			get { return int16Var; }
			set { int16Var = value; }
		}
		

		System.UInt64 uint64Var;
		public System.UInt64 Uint64Var
		{
			get { return uint64Var; }
			set { uint64Var = value; }
		}

		System.UInt16 uint16Var;
		public System.UInt16 Uint16Var
		{
			get { return uint16Var; }
			set { uint16Var = value; }
		}

		string nullString;
		public string NullString
		{
			get { return nullString; }
			set { nullString = value; }
		}

	}

	[NDOPersistent]
	public class VtAndEtContainer
	{
		VtPropDataContainer propValType;
		VtPublicDataContainer pubValType;
		EtDataContainer embeddedType = new EtDataContainer();

		public event EventHandler TestEvent;

		public EtDataContainer EmbeddedType
		{
			get { return embeddedType; }
			set { embeddedType = value; }
		}

		public VtPropDataContainer PropValType
		{
			get { return propValType; }
			set { propValType = value; }
		}

		public VtPublicDataContainer PubValType
		{
			get { return pubValType; }
			set { pubValType = value; }
		}

		public void Init()
		{
			pubValType.EnumVar = EnumType.zwei;
			pubValType.BoolVar = true;
			pubValType.ByteVar = 0x55;
			pubValType.DateTimeVar = new DateTime(2002, 12, 1, 1, 0, 20);
			pubValType.DecVar = 12.34m;
			pubValType.DoubleVar = 12345.123456;
			pubValType.FloatVar = 12345.1f;
			pubValType.GuidVar = new Guid("12341234-1234-1234-1234-123412341234");
			pubValType.Int16Var = 0x1234;
			pubValType.Int32Var = 0x12341234;
			pubValType.Int64Var = 0x143214321;
			pubValType.StringVar = "Teststring";
			pubValType.Uint16Var = 0xabc;
			pubValType.Uint32Var = 0x12341234;
			pubValType.Uint64Var = 0x143214321;
			pubValType.EmptyDateTimeVar = DateTime.MinValue;
			pubValType.EmptyGuidVar = Guid.Empty;
			pubValType.NullString = null;

			propValType.EnumVar = EnumType.zwei;
			propValType.BoolVar = true;
			propValType.ByteVar = 0x55;
			propValType.DateTimeVar = new DateTime(2002, 12, 1, 1, 0, 20);
			propValType.DecVar = 12.34m;
			propValType.DoubleVar = 12345.123456;
			propValType.FloatVar = 12345.1f;
			propValType.GuidVar = new Guid("12341234-1234-1234-1234-123412341234");
			propValType.Int16Var = 0x1234;
			propValType.Int32Var = 0x12341234;
			propValType.Int64Var = 0x143214321;
			propValType.StringVar = "Teststring";
			propValType.Uint16Var = 0xabc;
			propValType.Uint32Var = 0x12341234;
			propValType.Uint64Var = 0x143214321;
			propValType.EmptyDateTimeVar = DateTime.MinValue;
			propValType.EmptyGuidVar = Guid.Empty;
			propValType.NullString = null;

			embeddedType.EnumVar = EnumType.drei;
			embeddedType.BoolVar = true;
			embeddedType.ByteVar = 0x55;
			embeddedType.DateTimeVar = new DateTime(2002, 12, 1, 1, 0, 20);
			embeddedType.DecVar = 12.34m;
			embeddedType.DoubleVar = 12345.123456;
			embeddedType.FloatVar = 12345.1f;
			embeddedType.GuidVar = new Guid("12341234-1234-1234-1234-123412341234");
			embeddedType.Int16Var = 0x1234;
			embeddedType.Int32Var = 0x12341234;
			embeddedType.Int64Var = 0x143214321;
			embeddedType.StringVar = "Teststring";
			embeddedType.Uint16Var = 0xabc;
			embeddedType.Uint32Var = 0x12341234;
			embeddedType.Uint64Var = 0x143214321;
			embeddedType.EmptyDateTimeVar = DateTime.MinValue;
			embeddedType.EmptyGuidVar = Guid.Empty;
			embeddedType.NullString = null;
		}
	}


	
	public enum EnumType
	{
		eins,
		zwei,
		drei
	}


	[NDOPersistent]
	public class DataContainer
	{		
		public event EventHandler TestEvent;
		private EventHandler shitTest;  // Just to test, that delegates are not persistent

		EnumType enumVar;
		public EnumType EnumVar
		{
			get { return enumVar; }
			set { enumVar = value; }
		}

		System.Guid guidVar = Guid.NewGuid();//new System.Guid(0x11111111, 0x2222, 0x3333, 4, 5, 6, 7, 8, 9, 10, 11);
		public System.Guid GuidVar
		{
			get { return guidVar; }
			set { guidVar = value; }
		}

		Guid emptyGuidVar;
		public Guid EmptyGuidVar
		{
			get { return emptyGuidVar; }
			set { emptyGuidVar = value; }
		}

		DateTime emptyDateTimeVar;
		public DateTime EmptyDateTimeVar
		{
			get { return emptyDateTimeVar; }
			set { emptyDateTimeVar = value; }
		}

        System.Byte[] byteArrVar = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        public System.Byte[] ByteArrVar
        {
            get { return byteArrVar; }
            set { byteArrVar = value; }
        }
		
		
		bool boolVar;
		public bool BoolVar
		{
			get { return boolVar; }
			set { boolVar = value; }
		}

		byte byteVar;
		public byte ByteVar
		{
			get { return byteVar; }
			set { byteVar = value; }
		}

		string stringVar;
		public string StringVar
		{
			get { return stringVar; }
			set { stringVar = value; }
		}
		
		DateTime dateTimeVar;
		public DateTime DateTimeVar
		{
			get { return dateTimeVar; }
			set { dateTimeVar = value; }
		}

				
		decimal decVar;
		public decimal DecVar
		{
			get { return decVar; }
			set { decVar = value; }
		}

		int int32Var;
		public int Int32Var
		{
			get { return int32Var; }
			set { int32Var = value; }
		}

		System.UInt32 uint32Var;
		public System.UInt32 Uint32Var
		{
			get { return uint32Var; }
			set { uint32Var = value; }
		}
		double doubleVar;
		public double DoubleVar
		{
			get { return doubleVar; }
			set { doubleVar = value; }
		}
		float floatVar;
		public float FloatVar
		{
			get { return floatVar; }
			set { floatVar = value; }
		}


		System.Int64 int64Var;
		public System.Int64 Int64Var
		{
			get { return int64Var; }
			set { int64Var = value; }
		}
		System.Int16 int16Var;
		public System.Int16 Int16Var
		{
			get { return int16Var; }
			set { int16Var = value; }
		}
		

		System.UInt64 uint64Var;
		public System.UInt64 Uint64Var
		{
			get { return uint64Var; }
			set { uint64Var = value; }
		}
		System.UInt16 uint16Var;
		public System.UInt16 Uint16Var
		{
			get { return uint16Var; }
			set { uint16Var = value; }
		}

		string nullString;
		public string NullString
		{
			get { return nullString; }
			set { nullString = value; }
		}

		public void Init()
		{
			this.BoolVar = true;
			this.ByteVar = 127;
			this.DateTimeVar = DateTime.Now;
			this.GuidVar = new Guid("12341234-1234-1234-1234-123412341234");
			this.StringVar = "Test";
			this.DecVar = 1231.12m;   
			this.Int32Var = int.MaxValue;
			this.Uint32Var = (uint) int.MaxValue;
			this.DoubleVar = 1E28;
			this.FloatVar = 1E14F;
			this.Int64Var = 0x1ffffffff;
			this.Int16Var = short.MaxValue;
			this.Uint16Var = (ushort) short.MaxValue;
			this.Uint64Var = 0x1ffffffff;
			this.EnumVar = EnumType.drei;
			//this.EmptyDateTimeVar = DateTime.MinValue;
			//this.EmptyGuidVar = Guid.Empty;
			this.NullString = null;
		}

		public DataContainer()
		{
		}
	}

	[NDOPersistent]
	public class VtAndEtContainerDerived : VtAndEtContainer
	{
	}

	[NDOPersistent]
	public class DataContainerDerived : DataContainer
	{
	}

    [NDOPersistent]
    public class PrimitiveTypeMethodCaller
    {
        int intVar = 0;
        bool boolVar = false;
        double doubleVar = 1.0;
        string stringVar = "test";
		DateTime dtVar = new DateTime( 2008, 10, 30, 11, 3, 33, 123 );

        public int IntTest(PrimitiveTypeMethodCaller obj)
        {
            return intVar.CompareTo(obj.intVar);
        }
        public int BoolTest(PrimitiveTypeMethodCaller obj)
        {
            return boolVar.CompareTo(obj.boolVar);
        }
        public int DoubleTest(PrimitiveTypeMethodCaller obj)
        {
            return doubleVar.CompareTo(obj.doubleVar);
        }
        public int StringTest(PrimitiveTypeMethodCaller obj)
        {
            return stringVar.CompareTo(obj.stringVar);
        }
        public int DateTimeTest(PrimitiveTypeMethodCaller obj)
        {
            return dtVar.CompareTo(obj.dtVar);
        }
    }


}
