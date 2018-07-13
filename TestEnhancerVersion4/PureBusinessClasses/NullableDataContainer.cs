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
using System.Collections.Generic;
using System.Text;
using NDO;

namespace DataTypeTestClasses
{

    [NDOPersistent]
    public class NullableDataContainer : IPersistentObject
	{
        EnumType? enumVar;
        public EnumType? EnumVar
        {
            get { return enumVar; }
            set { enumVar = value; }
        }

        EnumType? enumEmptyVar;
        public EnumType? EnumEmptyVar
        {
            get { return enumEmptyVar; }
            set { enumEmptyVar = value; }
        }

        System.Guid? guidVar;
        public System.Guid? GuidVar
        {
            get { return guidVar; }
            set { guidVar = value; }
        }

        Guid? guidEmptyVar;
        public Guid? GuidEmptyVar
        {
            get { return guidEmptyVar; }
            set { guidEmptyVar = value; }
        }

        DateTime? dateTimeEmptyVar;
        public DateTime? DateTimeEmptyVar
        {
            get { return dateTimeEmptyVar; }
            set { dateTimeEmptyVar = value; }
        }

        bool? boolVar;
        public bool? BoolVar
        {
            get { return boolVar; }
            set { boolVar = value; }
        }

        bool? boolEmptyVar;
        public bool? BoolEmptyVar
        {
            get { return boolEmptyVar; }
            set { boolEmptyVar = value; }
        }

        byte? byteVar;
        public byte? ByteVar
        {
            get { return byteVar; }
            set { byteVar = value; }
        }

        byte? byteEmptyVar;
        public byte? ByteEmptyVar
        {
            get { return byteEmptyVar; }
            set { byteEmptyVar = value; }
        }


        DateTime? dateTimeVar;
        public DateTime? DateTimeVar
        {
            get { return dateTimeVar; }
            set { dateTimeVar = value; }
        }


        decimal? decVar;
        public decimal? DecVar
        {
            get { return decVar; }
            set { decVar = value; }
        }

        decimal? decEmptyVar;
        public decimal? DecEmptyVar
        {
            get { return decEmptyVar; }
            set { decEmptyVar = value; }
        }

        int? int32Var;
        public int? Int32Var
        {
            get { return int32Var; }
            set { int32Var = value; }
        }

        int? int32EmptyVar;
        public int? Int32EmptyVar
        {
            get { return int32EmptyVar; }
            set { int32EmptyVar = value; }
        }

        System.UInt32? uint32Var;
        public System.UInt32? Uint32Var
        {
            get { return uint32Var; }
            set { uint32Var = value; }
        }


        System.UInt32? uint32EmptyVar;
        public System.UInt32? Uint32EmptyVar
        {
            get { return uint32EmptyVar; }
            set { uint32EmptyVar = value; }
        }

        double? doubleEmptyVar;
        public double? DoubleEmptyVar
        {
            get { return doubleEmptyVar; }
            set { doubleEmptyVar = value; }
        }


        double? doubleVar;
        public double? DoubleVar
        {
            get { return doubleVar; }
            set { doubleVar = value; }
        }

        float? floatVar;
        public float? FloatVar
        {
            get { return floatVar; }
            set { floatVar = value; }
        }

        float? floatEmptyVar;
        public float? FloatEmptyVar
        {
            get { return floatEmptyVar; }
            set { floatEmptyVar = value; }
        }



        System.Int64? int64Var;
        public System.Int64? Int64Var
        {
            get { return int64Var; }
            set { int64Var = value; }
        }

        System.Int64? int64EmptyVar;
        public System.Int64? Int64EmptyVar
        {
            get { return int64EmptyVar; }
            set { int64EmptyVar = value; }
        }


        System.Int16? int16Var;
        public System.Int16? Int16Var
        {
            get { return int16Var; }
            set { int16Var = value; }
        }


        System.Int16? int16EmptyVar;
        public System.Int16? Int16EmptyVar
        {
            get { return int16EmptyVar; }
            set { int16EmptyVar = value; }
        }


        System.UInt64? uint64Var;
        public System.UInt64? Uint64Var
        {
            get { return uint64Var; }
            set { uint64Var = value; }
        }

        System.UInt64? uint64EmptyVar;
        public System.UInt64? Uint64EmptyVar
        {
            get { return uint64EmptyVar; }
            set { uint64EmptyVar = value; }
        }


        System.UInt16? uint16Var;
        public System.UInt16? Uint16Var
        {
            get { return uint16Var; }
            set { uint16Var = value; }
        }

        System.UInt16? uint16EmptyVar;
        public System.UInt16? Uint16EmptyVar
        {
            get { return uint16EmptyVar; }
            set { uint16EmptyVar = value; }
        }

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Init()
        {
            this.boolVar = true;
            this.byteVar = 127;
            this.dateTimeVar = DateTime.Now;
            this.guidVar = new Guid("12341234-1234-1234-1234-123412341234");
            this.decVar = 1231.12m;
            this.int32Var = int.MaxValue;
            this.uint32Var = (uint)int.MaxValue;
            this.doubleVar = 1E28;
            this.floatVar = 1E14F;
            this.int64Var = 0x1ffffffff;
            this.int16Var = short.MaxValue;
            this.uint16Var = (ushort)short.MaxValue;
            this.uint64Var = 0x1ffffffff;
            this.enumVar = EnumType.drei;
        }

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}

		public NullableDataContainer()
        {
        }
    }

    [NDOPersistent]
    public class NullableDataContainerDerived : NullableDataContainer
    {
        public NullableDataContainerDerived()
        {
        }
    }

}
