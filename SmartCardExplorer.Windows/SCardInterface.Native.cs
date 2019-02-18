using System;
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
		/// The following represents the current state of the smart card reader according to Smart Cards for Windows.
		/// </summary>
		enum CARDREADER_STATE
		{
			/// <summary>
			/// The current state of the reader is unknown.
			/// </summary>
			SCARD_UNKNOWN = 0x00000000,

			/// <summary>
			/// There is no card in the reader.
			/// </summary>
			SCARD_ABSENT = 0x00000001,

			/// <summary>
			/// There is a card in the reader but it has not been moved into position for use.
			/// </summary>
			SCARD_PRESENT = 0x00000002,

			/// <summary>
			/// There is a card in the reader in position for use. The card is not powered.
			/// </summary>
			SCARD_SWALLOWED = 0x00000003,

			/// <summary>
			/// There is power being applied to the card but the mode of the card is unknown.
			/// </summary>
			SCARD_POWERED = 0x00000004,

			/// <summary>
			/// The card has been reset and is awaiting PTS negotiation.
			/// </summary>
			SCARD_NEGOTIABLE = 0x00000005,

			/// <summary>
			/// The card has been reset and specific communication protocols have been established.
			/// </summary>
			SCARD_SPECIFICMODE = 0x00000006
		}

		public enum CARD_PROTOCOL : uint
		{
			/// <summary>
			/// No transmission protocol is active.
			/// </summary>
			SCARD_PROTOCOL_UNDEFINED = 0x00000000,

			/// <summary>
			/// Transmission protocol 0 (T=0) is active.
			/// It is the asynchronous half-duplex character transmission protocol.
			/// </summary>
			SCARD_PROTOCOL_T0 = 0x00000001,

			/// <summary>
			/// Transmission protocol 1 (T= 1) is active.
			/// It is the asynchronous half-duplex block transmission protocol.
			/// </summary>
			SCARD_PROTOCOL_T1 = 0x00000002,

			/// <summary>
			/// Bitwise OR combination of both of the two International Standards Organization (IS0) transmission protocols SCARD_PROTOCOL_T0 and SCARD_PROTOCOL_T1. This value can be used as a bitmask.
			/// </summary>
			SCARD_PROTOCOL_Tx = 0x00000003,

			/// <summary>
			/// Transmission protocol raw is active.The data from the smart card is raw and does not conform to any transmission protocol.
			/// </summary>
			SCARD_PROTOCOL_RAW = 0x00010000,


			/// <summary>
			/// A bitwise OR with this value forces the use of the default transmission parameters and card clock frequency.
			/// </summary>
			SCARD_PROTOCOL_DEFAULT = 0x80000000,

			/// <summary>
			/// Optimal transmission parameters and card clock frequency MUST be used. This flag is considered the default. No actual value is defined for this flag; it is there for compatibility with [PCSC5] section 3.1.3.
			/// </summary>
			SCARD_PROTOCOL_OPTIMAL = 0x00000000
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hContext"></param>
		/// <param name="szReaderName"></param>
		/// <param name="szDeviceInstanceId"></param>
		/// <param name="pcchDeviceInstanceId"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardGetReaderDeviceInstanceId(
			UInt32 hContext,
			string szReaderName,
			IntPtr szDeviceInstanceId,
			out UInt32 pcchDeviceInstanceId
			);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hContext"></param>
		/// <param name="szReaderName"></param>
		/// <param name="pbIcon"></param>
		/// <param name="pcbIcon"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardGetReaderIcon(
			UInt32 hContext,
			string szReaderName,
			IntPtr pbIcon,
			out UInt32 pcbIcon
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
			out uint phContext);

		/// <summary>
		/// Native SCardReleaseContext function from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardReleaseContext(UInt32 hContext);

		[DllImport("winscard.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern int SCardStatus(
			   uint hCard,
			   IntPtr szReaderName,
			   ref int pcchReaderLen,
			   ref int pdwState,
			   ref uint pdwProtocol,
			   byte[] pbAtr,
			   ref int pcbAtrLen);

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
			SHARE dwShareMode,
			PROTOCOL dwPreferredProtocols,
			out uint phCard,
			out uint pdwActiveProtocol);

		/// <summary>
		/// Native SCardDisconnect function from winscard.dll
		/// </summary>
		/// <param name="hCard"></param>
		/// <param name="dwDisposition"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError = true)]
		internal static extern int SCardDisconnect(UInt32 hCard,
			DISCONNECT dwDisposition);

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

		[DllImport("winscard.dll", EntryPoint= "SCardGetAttrib", SetLastError = true, CharSet = CharSet.Ansi)]
		internal static extern int SCardGetAttrib(UInt32 hCard,
			SCARD_ATTRIBUTES dwAttribId,
			[Out] byte[] pbAttr,
			out UInt32 pcbAttrLen);

		[DllImport("winscard.dll", EntryPoint = "SCardGetAttrib", SetLastError = true, CharSet = CharSet.Ansi)]
		internal static extern int SCardGetAttribDWORD(UInt32 hCard,
			SCARD_ATTRIBUTES dwAttribId,
			out UInt32 pbAttr,
			out UInt32 pcbAttrLen);

		[DllImport("winscard.dll", EntryPoint = "SCardGetAttrib", SetLastError = true, CharSet = CharSet.Ansi)]
		internal static extern int SCardGetAttribIntPtr(UInt32 hCard,
			SCARD_ATTRIBUTES dwAttribId,
			IntPtr pbAttr,
			out UInt32 pcbAttrLen);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool DeviceIoControl(
			IntPtr hDevice,
			uint dwIoControlCode,
			IntPtr lpInBuffer,
			uint nInBufferSize,
			IntPtr lpOutBuffer,
			uint nOutBufferSize,
			out uint lpBytesReturned,
			IntPtr lpOverlapped);

		[StructLayout(LayoutKind.Sequential)]
		public class SP_DEVINFO_DATA
		{
			public uint cbSize;
			public Guid classGuid;
			public uint devInst;
			public IntPtr reserved;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public uint cbSize;
			public byte[] devicePath;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class SP_DEVICE_INTERFACE_DATA
		{
			public uint cbSize;
			public Guid InterfaceClassGuid;
			public uint Flags;
			public IntPtr Reserved;
		}

		public static uint ANYSIZE_ARRAY = 1000;

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern Boolean SetupDiEnumDeviceInfo(IntPtr lpInfoSet, UInt32 dwIndex, SP_DEVINFO_DATA devInfoData);

		[DllImport(@"setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, uint memberIndex, SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

		[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, SP_DEVINFO_DATA deviceInfoData);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

		public const int DIGCF_PRESENT = 0x02;
		public const int DIGCF_DEVICEINTERFACE = 0x10;
		public const int SPDRP_DEVICEDESC = (0x00000000);
		public const long ERROR_NO_MORE_ITEMS = 259L;
		public const int ERROR_INSUFFICIENT_BUFFER = 0x0000007a;
		public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

		private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
		private static readonly Guid GuidDevinterfaceSCDevice = new Guid("50DD5230-BA8A-11D1-BF5D-0000F805F530"); // SmartCard reader devices
	}
}
