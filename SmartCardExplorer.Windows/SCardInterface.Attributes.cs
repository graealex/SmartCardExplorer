using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCardExplorer.Windows
{
	public partial class SCardInterface
	{
		public enum SCARD_ATTRIBUTES
		{
			/// <summary>
			/// Answer to reset (ATR) string.
			/// </summary>
			SCARD_ATTR_ATR_STRING = 0x00090303,

			/// <summary>
			/// DWORD encoded as 0xDDDDCCCC, where DDDD = data channel type and CCCC = channel number:
			/// The following encodings are defined for DDDD:
			/// 0x01 serial I/O; CCCC is a port number.
			/// 0x02 parallel I/O; CCCC is a port number.
			/// 0x04 PS/2 keyboard port; CCCC is zero.
			/// 0x08 SCSI; CCCC is SCSI ID number.
			/// 0x10 IDE; CCCC is device number.
			/// 0x20 USB; CCCC is device number.
			/// 0xFy vendor-defined interface with y in the range zero through 15; CCCC is vendor defined.
			/// </summary>
			SCARD_ATTR_CHANNEL_ID = 0x00020110,

			/// <summary>
			/// DWORD indicating which mechanical characteristics are supported.If zero, no special characteristics are supported.Note that multiple bits can be set:
			/// 0x00000001 Card swallowing mechanism
			/// 0x00000002 Card ejection mechanism
			/// 0x00000004 Card capture mechanism
			/// All other values are reserved for future use (RFU).
			/// </summary>
			SCARD_ATTR_CHARACTERISTICS = 0x00060150,

			/// <summary>
			/// Current block waiting time.
			/// </summary>
			SCARD_ATTR_CURRENT_BWT = 0x00080209,

			/// <summary>
			/// Current clock rate, in kHz.
			/// </summary>
			SCARD_ATTR_CURRENT_CLK = 0x00080202,

			/// <summary>
			/// Current character waiting time.
			/// </summary>
			SCARD_ATTR_CURRENT_CWT = 0x0008020A,

			/// <summary>
			/// Bit rate conversion factor.
			/// </summary>
			SCARD_ATTR_CURRENT_D = 0x00080204,

			/// <summary>
			/// Current error block control encoding.
			/// 0 = longitudinal redundancy check (LRC)
			/// 1 = cyclical redundancy check(CRC)
			/// </summary>
			SCARD_ATTR_CURRENT_EBC_ENCODING = 0x0008020B,

			/// <summary>
			/// Clock conversion factor.
			/// </summary>
			SCARD_ATTR_CURRENT_F = 0x00080203,

			/// <summary>
			/// Current byte size for information field size card.
			/// </summary>
			SCARD_ATTR_CURRENT_IFSC = 0x00080207,

			/// <summary>
			/// Current byte size for information field size device.
			/// </summary>
			SCARD_ATTR_CURRENT_IFSD = 0x00080208,

			/// <summary>
			/// Current guard time.
			/// </summary>
			SCARD_ATTR_CURRENT_N = 0x00080205,

			/// <summary>
			/// DWORD encoded as 0x0rrrpppp where rrr is RFU and should be 0x000. pppp encodes the current protocol type. Whichever bit has been set indicates which ISO protocol is currently in use. (For example, if bit zero is set, T= 0 protocol is in effect.)
			/// </summary>
			SCARD_ATTR_CURRENT_PROTOCOL_TYPE = 0x00080201,

			/// <summary>
			/// Current work waiting time.
			/// </summary>
			SCARD_ATTR_CURRENT_W = 0x00080206,

			/// <summary>
			/// Default clock rate, in kHz.
			/// </summary>
			SCARD_ATTR_DEFAULT_CLK = 0x00030121,

			/// <summary>
			/// Default data rate, in bps.
			/// </summary>
			SCARD_ATTR_DEFAULT_DATA_RATE = 0x00030123,

			/// <summary>
			/// Reader's display name.
			/// </summary>
			SCARD_ATTR_DEVICE_FRIENDLY_NAME = 0x7FFF0003,

			/// <summary>
			/// Reserved for future use.
			/// </summary>
			SCARD_ATTR_DEVICE_IN_USE = 0x7FFF0002,

			/// <summary>
			/// Reader's system name.
			/// </summary>
			SCARD_ATTR_DEVICE_SYSTEM_NAME = 0x7FFF0004,

			/// <summary>
			/// Reader's system name.
			/// </summary>
			SCARD_ATTR_DEVICE_SYSTEM_NAME_A = 0x7FFF0004,

			/// <summary>
			/// Reader's system name.
			/// </summary>
			SCARD_ATTR_DEVICE_SYSTEM_NAME_W = 0x7FFF0006,

			/// <summary>
			/// Instance of this vendor's reader attached to the computer. The first instance will be device unit 0, the next will be unit 1 (if it is the same brand of reader) and so on. Two different brands of readers will both have zero for this value.
			/// </summary>
			SCARD_ATTR_DEVICE_UNIT = 0x7FFF0001,

			/// <summary>
			/// Single byte. Zero if smart card electrical contact is not active; nonzero if contact is active.
			/// </summary>
			SCARD_ATTR_ICC_INTERFACE_STATUS = 0x00090301,

			/// <summary>
			/// Single byte indicating smart card presence:
			/// 0 = not present
			/// 1 = card present but not swallowed(applies only if reader supports smart card swallowing)
			/// 2 = card present(and swallowed if reader supports smart card swallowing)
			/// 4 = card confiscated.
			/// </summary>
			SCARD_ATTR_ICC_PRESENCE = 0x00090300,

			/// <summary>
			/// Single byte indicating smart card type:
			/// 0 = unknown type
			/// 1 = 7816 Asynchronous
			/// 2 = 7816 Synchronous
			/// Other values RFU.
			/// </summary>
			SCARD_ATTR_ICC_TYPE_PER_ATR = 0x00090304,

			/// <summary>
			/// Maximum clock rate, in kHz.
			/// </summary>
			SCARD_ATTR_MAX_CLK = 0x00030122,

			/// <summary>
			/// Maximum data rate, in bps.
			/// </summary>
			SCARD_ATTR_MAX_DATA_RATE = 0x00030124,

			/// <summary>
			/// Maximum bytes for information file size device.
			/// </summary>
			SCARD_ATTR_MAX_IFSD = 0x00030125,

			/// <summary>
			/// Zero if device does not support power down while smart card is inserted. Nonzero otherwise.
			/// </summary>
			SCARD_ATTR_POWER_MGMT_SUPPORT = 0x00040131,

			/// <summary>
			/// DWORD encoded as 0x0rrrpppp where rrr is RFU and should be 0x000. pppp encodes the supported protocol types. A '1' in a given bit position indicates support for the associated ISO protocol, so if bits zero and one are set, both T = 0 and T = 1 protocols are supported.
			/// </summary>
			SCARD_ATTR_PROTOCOL_TYPES = 0x00030120,

			/// <summary>
			/// Vendor-supplied interface device serial number.
			/// </summary>
			SCARD_ATTR_VENDOR_IFD_SERIAL_NO = 0x00010103,

			/// <summary>
			/// Vendor-supplied interface device type(model designation of reader).
			/// </summary>
			SCARD_ATTR_VENDOR_IFD_TYPE = 0x00010101,

			/// <summary>
			/// Vendor-supplied interface device version(DWORD in the form 0xMMmmbbbb where MM = major version, mm = minor version, and bbbb = build number).
			/// </summary>
			SCARD_ATTR_VENDOR_IFD_VERSION = 0x00010102,

			/// <summary>
			/// Vendor name.
			/// </summary>
			SCARD_ATTR_VENDOR_NAME = 0x00010100

		}
	}
}
