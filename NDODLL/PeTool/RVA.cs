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
using System.IO;

namespace Cli {

	/// <summary>
	/// Relative Virtual Address.
	/// </summary>
	internal struct RVA {

		public static readonly RVA Null;

		public uint value;

		static RVA()
		{
			Null = new RVA(0);
		}


		public RVA(uint val)
		{
			value = val;
		}


		public uint Value {
			get {
				return value;
			}
			set {
				this.value = value;
			}
		}

		public void Write (BinaryWriter writer)
		{
			writer.Write (value);
		}

		public static implicit operator RVA (uint val)
		{
			return new RVA(val);
		}

		public static implicit operator uint (RVA rva)
		{
			return rva.value;
		}

		public override int GetHashCode()
		{
			return (int) value;
		}

		public override bool Equals(object o)
		{
			bool res = o is RVA;
			if (res) res = (this.value == ((RVA)o).value);
			return res;
		}

		public static bool operator == (RVA rva1, RVA rva2)
		{
			return rva1.Equals(rva2);
		}

		public static bool operator != (RVA rva1, RVA rva2)
		{
			return !rva1.Equals(rva2);
		}

		public static bool operator < (RVA rva1, RVA rva2)
		{
			return (rva1.value < rva2.value);
		}

		public static bool operator > (RVA rva1, RVA rva2) {
			return (rva1.value > rva2.value);
		}

		public static bool operator <= (RVA rva1, RVA rva2)
		{
			return (rva1.value <= rva2.value);
		}

		public static bool operator >= (RVA rva1, RVA rva2)
		{
			return (rva1.value >= rva2.value);
		}

		public static RVA operator + (RVA rva, uint x)
		{
			return new RVA (rva.value + x);
		}

		public static RVA operator - (RVA rva, uint x)
		{
			return new RVA (rva.value - x);
		}


		public override string ToString()
		{
			if (this == Null) return "NULL";
			return ("0x" + value.ToString("X"));
		}

//		unsafe public static int Size {
//			get {
//				return sizeof (uint);
//			}
//		}
		public static int Size 
		{
			get 
			{
				return 2;
			}
		}

	}

}

