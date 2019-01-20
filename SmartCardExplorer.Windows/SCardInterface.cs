using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SmartCardExplorer.Windows
{
	public partial class SCardInterface : IDisposable
	{
		/// <summary>
		/// SCOPE context
		/// </summary>
		public enum SCOPE
		{
			/// <summary>
			/// The context is a user context, and any database operations are performed within the
			/// domain of the user.
			/// </summary>
			User,

			/// <summary>
			/// The context is that of the current terminal, and any database operations are performed
			/// within the domain of that terminal.  (The calling application must have appropriate
			/// access permissions for any database actions.)
			/// </summary>
			Terminal,

			/// <summary>
			/// The context is the system context, and any database operations are performed within the
			/// domain of the system.  (The calling application must have appropriate access
			/// permissions for any database actions.)
			/// </summary>
			System
		}

		/// <summary>
		/// SHARE mode enumeration
		/// </summary>
		public enum SHARE
		{
			/// <summary>
			/// This application is not willing to share this card with other applications.
			/// </summary>
			Exclusive = 1,

			/// <summary>
			/// This application is willing to share this card with other applications.
			/// </summary>
			Shared,

			/// <summary>
			/// This application demands direct control of the reader, so it is not available to other applications.
			/// </summary>
			Direct
		}


		/// <summary>
		/// PROTOCOL enumeration
		/// </summary>
		public enum PROTOCOL
		{
			/// <summary>
			/// There is no active protocol.
			/// </summary>
			Undefined = 0x00000000,

			/// <summary>
			/// T=0 is the active protocol.
			/// </summary>
			T0 = 0x00000001,

			/// <summary>
			/// T=1 is the active protocol.
			/// </summary>
			T1 = 0x00000002,

			/// <summary>
			/// Raw is the active protocol.
			/// </summary>
			Raw = 0x00010000,
			Default = unchecked((int)0x80000000),  // Use implicit PTS.

			/// <summary>
			/// T=1 or T=0 can be the active protocol
			/// </summary>
			T0orT1 = T0 | T1
		}


		/// <summary>
		/// DISCONNECT action enumeration
		/// </summary>
		public enum DISCONNECT
		{
			/// <summary>
			/// Don't do anything special on close
			/// </summary>
			Leave,

			/// <summary>
			/// Reset the card on close
			/// </summary>
			Reset,

			/// <summary>
			/// Power down the card on close
			/// </summary>
			Unpower,

			/// <summary>
			/// Eject(!) the card on close
			/// </summary>
			Eject
		}

		private UInt32 hContext = 0;

		/// <summary>
		/// Wraps the PCSC function 
		/// LONG SCardEstablishContext(
		///		IN DWORD dwScope,
		///		IN LPCVOID pvReserved1,
		///		IN LPCVOID pvReserved2,
		///		OUT LPSCARDCONTEXT phContext
		///	);
		/// </summary>
		/// <param name="Scope"></param>
		public void EstablishContext(SCOPE Scope)
		{
			IntPtr phContext = Marshal.AllocHGlobal(Marshal.SizeOf(hContext));

			int lastError = SCardEstablishContext((uint)Scope, IntPtr.Zero, IntPtr.Zero, phContext);
			if (lastError != 0)
			{
				string msg = "SCardEstablishContext error: " + lastError;

				Marshal.FreeHGlobal(phContext);
				throw new Exception(msg);
			}

			hContext = (uint)Marshal.ReadInt32(phContext);

			Marshal.FreeHGlobal(phContext);
		}


		/// <summary>
		/// Wraps the PCSC function
		/// LONG SCardReleaseContext(
		///		IN SCARDCONTEXT hContext
		///	);
		/// </summary>
		public void ReleaseContext()
		{
			if (hContext != 0)
			{
				int lastError = SCardReleaseContext(hContext);

				if (lastError != 0)
				{
					throw new Exception("SCardReleaseContext error", new Win32Exception(lastError));
				}

				hContext = 0;
			}
		}

		public string GetReaderDeviceInstanceId(string readerName)
		{
			UInt32 pcchDeviceInstanceId = 0;
			IntPtr szDeviceInstanceId = IntPtr.Zero;
			string result = null;

			try
			{
				int lastError = SCardGetReaderDeviceInstanceId(hContext, readerName, szDeviceInstanceId, out pcchDeviceInstanceId);

				// This can happen quite often, for example in an RDP session with a redirected card reader
				if (lastError == unchecked((int)ERROR_CODES.SCARD_E_READER_UNAVAILABLE))
					return "UNAVAILABLE";

				if (lastError != 0)
					throw new Win32Exception(lastError);

				szDeviceInstanceId = Marshal.AllocHGlobal((int)pcchDeviceInstanceId);

				lastError = SCardGetReaderDeviceInstanceId(hContext, readerName, szDeviceInstanceId, out pcchDeviceInstanceId);

				if (lastError != 0)
					throw new Win32Exception(lastError);

				result = Marshal.PtrToStringAnsi(szDeviceInstanceId, (int)pcchDeviceInstanceId);

			}
			catch (Win32Exception win32Exception)
			{
				throw new Exception("GetReaderDeviceInstanceId error", win32Exception);
			}
			finally
			{
				if (szDeviceInstanceId != IntPtr.Zero)
					Marshal.FreeHGlobal(szDeviceInstanceId);
			}


			return result;
		}


		/// <summary>
		/// Wraps the PCSC function
		/// LONG SCardListReaders(SCARDCONTEXT hContext, 
		///		LPCTSTR mszGroups, 
		///		LPTSTR mszReaders, 
		///		LPDWORD pcchReaders 
		///	);
		/// </summary>
		/// <returns>A string array of the readers</returns>
		public string[] ListReaders(string readerGroup = null)
		{
			string[] sListReaders = null;
			UInt32 pchReaders = 0;
			IntPtr szListReaders = IntPtr.Zero;

			try
			{
				int lastError = SCardListReaders(hContext, readerGroup, szListReaders, out pchReaders);
				if (lastError != 0)
					throw new Win32Exception(lastError);

				szListReaders = Marshal.AllocHGlobal((int)pchReaders);
				lastError = SCardListReaders(hContext, readerGroup, szListReaders, out pchReaders);
				if (lastError != 0)
					throw new Win32Exception(lastError);

				char[] caReadersData = new char[pchReaders];
				int nbReaders = 0;
				for (int nI = 0; nI < pchReaders; nI++)
				{
					caReadersData[nI] = (char)Marshal.ReadByte(szListReaders, nI);

					if (caReadersData[nI] == 0)
						nbReaders++;
				}

				// Remove last 0
				--nbReaders;

				if (nbReaders != 0)
				{
					sListReaders = new string[nbReaders];
					char[] caReader = new char[pchReaders];
					int nIdx = 0;
					int nIdy = 0;
					int nIdz = 0;
					// Get the nJ string from the multi-string

					while (nIdx < pchReaders - 1)
					{
						caReader[nIdy] = caReadersData[nIdx];
						if (caReader[nIdy] == 0)
						{
							sListReaders[nIdz] = new string(caReader, 0, nIdy);
							++nIdz;
							nIdy = 0;
							caReader = new char[pchReaders];
						}
						else
						{
							++nIdy;
						}
						++nIdx;
					}
				}
			}
			catch (Win32Exception win32Exception)
			{
				throw new Exception("GetReaderDeviceInstanceId error", win32Exception);
			}
			finally
			{
				if (szListReaders != IntPtr.Zero)
					Marshal.FreeHGlobal(szListReaders);
			}

			return sListReaders ?? (new string[0]);
		}

		public void Dispose()
		{
			ReleaseContext();
		}
	}
}
