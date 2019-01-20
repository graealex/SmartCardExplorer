﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SmartCardExplorer.Windows
{
	public partial class SCardInterface
	{
		/// <summary>
		/// CARD_STATE enumeration, used by the PC/SC function SCardGetStatusChanged
		/// </summary>
		enum CARD_STATE
		{
			UNAWARE = 0x00000000,
			IGNORE = 0x00000001,
			CHANGED = 0x00000002,
			UNKNOWN = 0x00000004,
			UNAVAILABLE = 0x00000008,
			EMPTY = 0x00000010,
			PRESENT = 0x00000020,
			ATRMATCH = 0x00000040,
			EXCLUSIVE = 0x00000080,
			INUSE = 0x00000100,
			MUTE = 0x00000200,
			UNPOWERED = 0x00000400
		}

		/// <summary>
		/// Wraps the SCARD_IO_STRUCTURE
		///  
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct SCard_IO_Request
		{
			public UInt32 m_dwProtocol;
			public UInt32 m_cbPciLength;
		}


		/// <summary>
		/// Wraps theSCARD_READERSTATE structure of PC/SC
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SCard_ReaderState
		{
			public string m_szReader;
			public IntPtr m_pvUserData;
			public UInt32 m_dwCurrentState;
			public UInt32 m_dwEventState;
			public UInt32 m_cbAtr;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] m_rgbAtr;
		}

		/// <summary>
		/// Native SCardGetStatusChanged from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <param name="dwTimeout"></param>
		/// <param name="rgReaderStates"></param>
		/// <param name="cReaders"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardGetStatusChange(UInt32 hContext,
			UInt32 dwTimeout,
			[In, Out] SCard_ReaderState[] rgReaderStates,
			UInt32 cReaders);

		/// <summary>
		/// Native SCardListReaders function from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <param name="mszGroups"></param>
		/// <param name="mszReaders"></param>
		/// <param name="pcchReaders"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardListReaders(UInt32 hContext,
			string mszGroups,
			IntPtr mszReaders,
			out UInt32 pcchReaders);

		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardGetReaderDeviceInstanceId(
			UInt32 hContext,
			string szReaderName,
			IntPtr szDeviceInstanceId,
			out UInt32 pcchDeviceInstanceId
			);

		/// <summary>
		/// Native SCardEstablishContext function from winscard.dll
		/// </summary>
		/// <param name="dwScope"></param>
		/// <param name="pvReserved1"></param>
		/// <param name="pvReserved2"></param>
		/// <param name="phContext"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardEstablishContext(UInt32 dwScope,
			IntPtr pvReserved1,
			IntPtr pvReserved2,
			IntPtr phContext);

		/// <summary>
		/// Native SCardReleaseContext function from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardReleaseContext(UInt32 hContext);

		/// <summary>
		/// Native SCardConnect function from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <param name="szReader"></param>
		/// <param name="dwShareMode"></param>
		/// <param name="dwPreferredProtocols"></param>
		/// <param name="phCard"></param>
		/// <param name="pdwActiveProtocol"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern int SCardConnect(UInt32 hContext,
			[MarshalAs(UnmanagedType.LPTStr)] string szReader,
			UInt32 dwShareMode,
			UInt32 dwPreferredProtocols,
			IntPtr phCard,
			IntPtr pdwActiveProtocol);

		/// <summary>
		/// Native SCardDisconnect function from winscard.dll
		/// </summary>
		/// <param name="hCard"></param>
		/// <param name="dwDisposition"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardDisconnect(UInt32 hCard,
			UInt32 dwDisposition);

		/// <summary>
		/// Native SCardTransmit function from winscard.dll
		/// </summary>
		/// <param name="hCard"></param>
		/// <param name="pioSendPci"></param>
		/// <param name="pbSendBuffer"></param>
		/// <param name="cbSendLength"></param>
		/// <param name="pioRecvPci"></param>
		/// <param name="pbRecvBuffer"></param>
		/// <param name="pcbRecvLength"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardTransmit(UInt32 hCard,
			[In] ref SCard_IO_Request pioSendPci,
			byte[] pbSendBuffer,
			UInt32 cbSendLength,
			IntPtr pioRecvPci,
			[Out] byte[] pbRecvBuffer,
			out UInt32 pcbRecvLength
			);

		/// <summary>
		/// Native SCardBeginTransaction function of winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardBeginTransaction(UInt32 hContext);

		/// <summary>
		/// Native SCardEndTransaction function of winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardEndTransaction(UInt32 hContext, UInt32 dwDisposition);

		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardGetAttrib(UInt32 hCard,
			UInt32 dwAttribId,
			[Out] byte[] pbAttr,
			out UInt32 pcbAttrLen);
	}
}
