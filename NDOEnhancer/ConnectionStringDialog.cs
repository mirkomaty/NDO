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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NDOEnhancer
{
	[ComImport, Guid("00000515-0000-0010-8000-00AA006D2EA4")]
	internal interface Connection15
	{
		[DispId(500)]
		object Properties { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType="", MarshalCookie="")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(500)] get; }
		[DispId(0)]
		string ConnectionString { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0)] set; }
		[DispId(2)]
		int CommandTimeout { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)] set; }
		[DispId(3)]
		int ConnectionTimeout { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)] set; }
		[DispId(4)]
		string Version { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(5)]
			/*extern*/ void Close();
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(6)]
			/*extern*/ object Execute([In, MarshalAs(UnmanagedType.BStr)] string CommandText, [Optional, MarshalAs(UnmanagedType.Struct)] out object RecordsAffected, [In, Optional] int Options /* = -1 */);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(7)]
			/*extern*/ int BeginTrans();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(8)]
			/*extern*/ void CommitTrans();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(9)]
			/*extern*/ void RollbackTrans();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(10)]
			/*extern*/ void Open([In, Optional, MarshalAs(UnmanagedType.BStr)] string ConnectionString /* = "" */, [In, Optional, MarshalAs(UnmanagedType.BStr)] string UserID /* = "" */, [In, Optional, MarshalAs(UnmanagedType.BStr)] string Password /* = "" */, [In, Optional] int Options /* = -1 */);
		[DispId(11)]
		object Errors { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType="", MarshalCookie="")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(11)] get; }
		[DispId(12)]
		string DefaultDatabase { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(12)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(12)] set; }
		[DispId(13)]
		int IsolationLevel { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(13)] set; }
		[DispId(14)]
		int Attributes { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(14)] set; }
		[DispId(15)]
		int CursorLocation { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(15)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(15)] set; }
		[DispId(0x10)]
		int Mode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x10)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x10)] set; }
		[DispId(0x11)]
		string Provider { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x11)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x11)] set; }
		[DispId(0x12)]
		int State { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x12)] get; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x13)]
			/*extern*/ object OpenSchema([In] int Schema, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Restrictions, [In, Optional, MarshalAs(UnmanagedType.Struct)] object SchemaID);
	}


	[ComImport, TypeLibType((short) 4160), Guid("2206CCB2-19C1-11D1-89E0-00C04FD7A829")]
	internal interface IDataSourceLocator
	{
		[DispId(0x60020000)]
		int hWnd { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x60020000)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x60020000)] set; }
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1610743810)]
			/*extern*/ object PromptNew();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x60020003)]
			/*extern*/ bool PromptEdit([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppADOConnection);
	}

	internal class ConnectionStringDialog
	{
		string connectionString = null;

		public string ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}

		public ConnectionStringDialog()
		{
		}

		public ConnectionStringDialog(string connectionString)
		{
			if (connectionString != null && connectionString != string.Empty)
				this.connectionString = connectionString;
		}

		public DialogResult Show()
		{
			Type t = Type.GetTypeFromCLSID(new Guid("{00000514-0000-0010-8000-00AA006D2EA4}"));			
			object oc = Activator.CreateInstance(t);
			if (oc == null)
				throw new Exception("Can't construct a Connection object");
			Connection15 cn = (Connection15) oc;
			if (cn == null)
				throw new Exception("Can't get the connection interface");
			t = Type.GetTypeFromCLSID(new Guid("{2206CDB2-19C1-11D1-89E0-00C04FD7A829}"));
			object odl = Activator.CreateInstance(t);
			if (odl == null)
				throw new Exception("Can't construct a DataLinksClass object");
			IDataSourceLocator dl = (IDataSourceLocator) odl;
			if (dl == null)
				throw new Exception("Can't get the IDataSourceLocator interface");
			cn.ConnectionString = this.connectionString;
			try
			{
				dl.PromptEdit(ref oc);
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == -2147217887)
					throw new Exception("Wrong connection string format - delete your connection string and press the edit button again.");
				else
					throw ex;
			}
			this.connectionString = cn.ConnectionString;
			if (cn.ConnectionString == null)
				return DialogResult.Cancel;
			else
				return DialogResult.OK;
			
		}

	}

}

