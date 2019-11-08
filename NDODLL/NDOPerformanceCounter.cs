using System;
using System.Collections.Generic;
using System.Text;

namespace NDO
{
	/// <summary>
	/// 
	/// </summary>
	public class NDOPerformanceCounter
	{
		static object lockObject = new object();
		static int createTransactionCount;
		static int readCount;
		static int writeCount;
		static int releaseTransactionCount;
		static int openTransactions;

		/// <summary>
		/// 
		/// </summary>
		public static int OpenTransactions
		{
			get
			{
				lock(lockObject)
				return openTransactions;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void IncrementOpenTransactions()
		{
			lock (lockObject)
				openTransactions++;
		}

		/// <summary>
		/// 
		/// </summary>
		public static void DecrementOpenTransactions()
		{
			lock (lockObject)
				openTransactions--;
		}

		/// <summary>
		/// 
		/// </summary>
		public static int CreateTransactionCount
		{
			get
			{
				lock(lockObject)
				return createTransactionCount;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void IncrementCreateTransactionCount()
		{
			lock (lockObject)
			createTransactionCount++;
		}

		/// <summary>
		/// 
		/// </summary>
		public static int ReadCount
		{
			get
			{
				lock (lockObject)
					return readCount;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void IncrementReadCount()
		{
			lock (lockObject)
				readCount++;
		}

		/// <summary>
		/// 
		/// </summary>
		public static int ReleaseTransactionCount
		{
		
			get
			{
				lock (lockObject)
				return releaseTransactionCount;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void IncrementReleaseTransactionCount()
		{
			lock (lockObject)
				releaseTransactionCount++;
		}

		/// <summary>
		/// 
		/// </summary>
		public static int WriteCount
		{
			get
			{
				lock (lockObject)
				return writeCount;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void IncrementWriteCount()
		{
			lock (lockObject)
				writeCount++;
		}
	}
}
