using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using Microsoft.Win32.SafeHandles;

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
			None = 0,

			/// This application is not willing to share this card with other applications.
			/// </summary>
			Exclusive = 1,

			/// <summary>
			/// This application is willing to share this card with other applications.
			/// </summary>
			Shared = 2,

			/// <summary>
			/// This application demands direct control of the reader, so it is not available to other applications.
			/// </summary>
			Direct = 3
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
		private UInt32 hCard = 0;
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
			try
			{
				int lastError = SCardEstablishContext((uint)Scope, IntPtr.Zero, IntPtr.Zero, out hContext);
				if (lastError != 0)
					throw new Win32Exception(lastError);
			}
			catch (Win32Exception win32Exception)
			{
				throw new Exception("EstablishContext error", win32Exception);
			}
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

		public bool ConnectCard(string readerName, bool throwOnNoCard = false, bool connectNoCardPresent = false, SHARE share = SHARE.Shared)
		{
			int lastError = 0;
			uint dwActiveProtocol;
			try
			{
				// Try to connect in shared mode if card is available
				lastError = SCardConnect(hContext, readerName, share, PROTOCOL.T0orT1, out hCard, out dwActiveProtocol);

				// If no connection is allowed without a card present, don't go further if connection failed
				if ((lastError != 0) && (!connectNoCardPresent))
				{
					if ((!throwOnNoCard) && (
						(lastError == unchecked((int)SCARD_ERRORS.SCARD_E_NO_SMARTCARD)) ||
						(lastError == unchecked((int)SCARD_ERRORS.SCARD_E_NOT_READY))
						))
						return false;

					throw new Win32Exception(lastError);
				}

				if (lastError != 0)
					lastError = SCardConnect(hContext, readerName, SHARE.Direct, PROTOCOL.Undefined, out hCard, out dwActiveProtocol);

				if (lastError != 0)
					throw new Win32Exception(lastError);

			}
			catch (Win32Exception win32Exception)
			{
				throw new Exception("ConnectCard error", win32Exception);
			}
			return true;
		}

		public void DisconnectCard(DISCONNECT disconnect = DISCONNECT.Leave)
		{
			if (hCard != 0)
			{
				SCardDisconnect(hContext, disconnect);
				hCard = 0;
			}
		}

		public string GetReaderDeviceInstanceId(string readerName)
		{
			UInt32 pcchDeviceInstanceId = 0;
			IntPtr szDeviceInstanceId = IntPtr.Zero;
			string result = null;
			int lastError = 0;

			try
			{
				lastError = SCardGetReaderDeviceInstanceId(hContext, readerName, szDeviceInstanceId, out pcchDeviceInstanceId);

				// This can happen quite often, for example in an RDP session with a redirected card reader
				if (lastError == unchecked((int)SCARD_ERRORS.SCARD_E_READER_UNAVAILABLE))
					return null;

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

		public Image GetReaderIcon(string readerName)
		{
			UInt32 pcbIcon = 0;
			IntPtr pbIcon = IntPtr.Zero;
			string result = null;

			try
			{
				int lastError = SCardGetReaderIcon(hContext, readerName, pbIcon, out pcbIcon);

				if (lastError == unchecked((int)SCARD_ERRORS.SCARD_E_READER_UNAVAILABLE))
					return null;

				if (lastError != 0)
					throw new Win32Exception(lastError);

				pbIcon = Marshal.AllocHGlobal((int)pcbIcon);

				lastError = SCardGetReaderIcon(hContext, readerName, pbIcon, out pcbIcon);

				if (lastError != 0)
					throw new Win32Exception(lastError);

				// Using UnmanagedMemoryStream would not be CLS-compliant
				byte[] bufferIcon = new byte[pcbIcon];
				Marshal.Copy(pbIcon, bufferIcon, 0, bufferIcon.Length);

				// MemoryStream cannot be disposed because Bitmap object depends on it
				MemoryStream streamIcon = new MemoryStream(bufferIcon);
				return Bitmap.FromStream(streamIcon);
			
			}
			catch (Win32Exception win32Exception)
			{
				throw new Exception("GetReaderIcon error", win32Exception);
			}
			finally
			{
				if (pbIcon != IntPtr.Zero)
					Marshal.FreeHGlobal(pbIcon);
			}
		}

		public Dictionary<string, string> GetReaderProperties(string readerName)
		{
			Dictionary<string, string> readerProperties = new Dictionary<string, string>();

			int lastError = 0;
			uint hCard = 0;
			uint dwActiveProtocol;
			try
			{
				// Try to connect in shared mode to avoid card ejection event to propagate to Windows
				lastError = SCardConnect(hContext, readerName, SHARE.Shared, PROTOCOL.T0orT1, out hCard, out dwActiveProtocol);

				if (lastError != 0)
					lastError = SCardConnect(hContext, readerName, SHARE.Direct, PROTOCOL.Undefined, out hCard, out dwActiveProtocol);

				if (lastError != 0)
					throw new Win32Exception(lastError);

				readerProperties["VendorName"] = GetStringAttribute(hCard, SCARD_ATTRIBUTES.SCARD_ATTR_VENDOR_NAME);
				readerProperties["FriendlyName"] = GetStringAttribute(hCard, SCARD_ATTRIBUTES.SCARD_ATTR_DEVICE_FRIENDLY_NAME);

				readerProperties["DefaultDataRate"] = $"{GetDWORDAttribute(hCard, SCARD_ATTRIBUTES.SCARD_ATTR_DEFAULT_DATA_RATE)} bps";
				readerProperties["MaxDataRate"] = $"{GetDWORDAttribute(hCard, SCARD_ATTRIBUTES.SCARD_ATTR_MAX_DATA_RATE)} bps";

				readerProperties["DefaultClockRate"] = $"{GetDWORDAttribute(hCard, SCARD_ATTRIBUTES.SCARD_ATTR_DEFAULT_CLK)}";
				readerProperties["MaxClockRate"] = $"{GetDWORDAttribute(hCard, SCARD_ATTRIBUTES.SCARD_ATTR_MAX_CLK)}";
			}
			finally
			{
				if (hCard != 0)
				{
					SCardDisconnect(hContext, DISCONNECT.Leave);
					hCard = 0;
				}
			}

			return readerProperties;
		}

		public UInt32 GetDWORDAttribute(uint hCard, SCARD_ATTRIBUTES attribute)
		{
			UInt32 pbAttr = 0;
			UInt32 pcbAttrLen = (UInt32)Marshal.SizeOf(pbAttr);
			int lastError = SCardGetAttribDWORD(hCard, attribute, out pbAttr, out pcbAttrLen);

			if (pcbAttrLen != (UInt32)Marshal.SizeOf(pbAttr))
				throw new InvalidDataException();

			if (lastError != 0)
				throw new Win32Exception(lastError);

			return pbAttr;
		}

		public string GetStringAttribute(uint hCard, SCARD_ATTRIBUTES attribute)
		{
			IntPtr pbAttr = IntPtr.Zero;
			UInt32 pcbAttrLen = 0;
			int lastError = SCardGetAttribIntPtr(hCard, attribute, pbAttr, out pcbAttrLen);
			if (lastError != 0)
				throw new Win32Exception(lastError);

			try
			{
				pbAttr = Marshal.AllocHGlobal((int)pcbAttrLen);

				lastError = SCardGetAttribIntPtr(hCard, attribute, pbAttr, out pcbAttrLen);
				if (lastError != 0)
					throw new Win32Exception(lastError);

				return Marshal.PtrToStringAnsi(pbAttr, (int)pcbAttrLen);

			}
			finally
			{
				if (pbAttr != IntPtr.Zero)
				{
					if (pbAttr != IntPtr.Zero)
						Marshal.FreeHGlobal(pbAttr);
				}
			}
		}

		public string[] ListReadersSetupApi()
		{
			List<string> readers = new List<string>();
			Guid classGuid = GuidDevinterfaceSCDevice;
			int lastError;
			bool result;
			IntPtr hDevInfo = IntPtr.Zero;

			try
			{
				hDevInfo = SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);
				if (hDevInfo == INVALID_HANDLE_VALUE)
				{
					lastError = Marshal.GetLastWin32Error();
					throw new Win32Exception(lastError);
				}

				for (uint uiIndex = 0; true; ++uiIndex)
				{
					SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();
					devInfoData.cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
					devInfoData.classGuid = Guid.Empty;
					devInfoData.devInst = 0;
					devInfoData.reserved = IntPtr.Zero;
					result = SetupDiEnumDeviceInfo(hDevInfo, uiIndex, devInfoData);

					if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
						break;

					if (result == false)
						continue;

					SP_DEVICE_INTERFACE_DATA ifData = new SP_DEVICE_INTERFACE_DATA();
					ifData.cbSize = (uint)Marshal.SizeOf(ifData);
					ifData.Flags = 0;
					ifData.InterfaceClassGuid = Guid.Empty;
					ifData.Reserved = IntPtr.Zero;

					result = SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref classGuid, uiIndex, ifData);
					if (result == false)
					{
						lastError = Marshal.GetLastWin32Error();
						if (lastError != ERROR_NO_MORE_ITEMS)
							throw new Win32Exception(lastError);
					}
					else
					{
						uint requiredSize = 0;
						result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ifData, IntPtr.Zero, 0, out requiredSize, null);
						if (result == false)
						{
							lastError = Marshal.GetLastWin32Error();

							if (lastError != ERROR_INSUFFICIENT_BUFFER)
								throw new Win32Exception(lastError);
						}

						IntPtr detailDataBuffer = IntPtr.Zero;
						try
						{
							detailDataBuffer = Marshal.AllocHGlobal((int)requiredSize);
							Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
							result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ifData, detailDataBuffer, requiredSize, out requiredSize, null);
							if (result == false)
							{
								lastError = Marshal.GetLastWin32Error();
								if (lastError != ERROR_NO_MORE_ITEMS)
									throw new Win32Exception(lastError);
							}

							IntPtr pDevicePathName = new IntPtr(detailDataBuffer.ToInt32() + 4);
							String devicePathName = Marshal.PtrToStringAuto(pDevicePathName);
							readers.Add(devicePathName);
						}
						finally
						{
							if (detailDataBuffer != IntPtr.Zero)
							{
								Marshal.FreeHGlobal(detailDataBuffer);
								detailDataBuffer = IntPtr.Zero;
							}
						}
					}
				}

			}
			finally
			{
				if ((hDevInfo != IntPtr.Zero) && (hDevInfo != INVALID_HANDLE_VALUE))
					SetupDiDestroyDeviceInfoList(hDevInfo);
			}

			return readers.ToArray();

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
