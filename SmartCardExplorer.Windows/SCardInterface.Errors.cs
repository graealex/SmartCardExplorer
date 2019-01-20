using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCardExplorer.Windows
{
	public partial class SCardInterface
	{
		enum ERROR_CODES : uint
		{
			/// <summary>
			/// No error was encountered.
			/// </summary>
			SCARD_S_SUCCESS = 0x00000000,

			/// <summary>
			/// An internal consistency check failed
			/// </summary>
			SCARD_F_INTERNAL_ERROR = 0x80100001,

			/*
#define SCARD_E_CANCELLED   ((LONG)0x80100002)
The action was cancelled by an SCardCancel request.More...

#define SCARD_E_INVALID_HANDLE   ((LONG)0x80100003)
The supplied handle was invalid. More...

#define SCARD_E_INVALID_PARAMETER   ((LONG)0x80100004)
One or more of the supplied parameters could not be properly interpreted. More...

#define SCARD_E_INVALID_TARGET   ((LONG)0x80100005)
Registry startup information is missing or invalid.More...

#define SCARD_E_NO_MEMORY   ((LONG)0x80100006)
Not enough memory available to complete this command.More...

#define SCARD_F_WAITED_TOO_LONG   ((LONG)0x80100007)
An internal consistency timer has expired.More...

#define SCARD_E_INSUFFICIENT_BUFFER   ((LONG)0x80100008)
The data buffer to receive returned data is too small for the returned data.More...

#define SCARD_E_UNKNOWN_READER   ((LONG)0x80100009)
The specified reader name is not recognized. More...

#define SCARD_E_TIMEOUT   ((LONG)0x8010000A)
The user-specified timeout value has expired.More...

#define SCARD_E_SHARING_VIOLATION   ((LONG)0x8010000B)
The smart card cannot be accessed because of other connections outstanding.More...

#define SCARD_E_NO_SMARTCARD   ((LONG)0x8010000C)
The operation requires a Smart Card, but no Smart Card is currently in the device. More...

#define SCARD_E_UNKNOWN_CARD   ((LONG)0x8010000D)
The specified smart card name is not recognized. More...

#define SCARD_E_CANT_DISPOSE   ((LONG)0x8010000E)
The system could not dispose of the media in the requested manner.More...

#define SCARD_E_PROTO_MISMATCH   ((LONG)0x8010000F)
The requested protocols are incompatible with the protocol currently in use with the smart card.More...

#define SCARD_E_NOT_READY   ((LONG)0x80100010)
The reader or smart card is not ready to accept commands.More...

#define SCARD_E_INVALID_VALUE   ((LONG)0x80100011)
One or more of the supplied parameters values could not be properly interpreted.More...

#define SCARD_E_SYSTEM_CANCELLED   ((LONG)0x80100012)
The action was cancelled by the system, presumably to log off or shut down.More...

#define SCARD_F_COMM_ERROR   ((LONG)0x80100013)
An internal communications error has been detected.More...

#define SCARD_F_UNKNOWN_ERROR   ((LONG)0x80100014)
An internal error has been detected, but the source is unknown.More...

#define SCARD_E_INVALID_ATR   ((LONG)0x80100015)
An ATR obtained from the registry is not a valid ATR string. More...

#define SCARD_E_NOT_TRANSACTED   ((LONG)0x80100016)
An attempt was made to end a non-existent transaction.More...
*/
			/// <summary>
			/// The specified reader is not currently available for use.
			/// </summary>
			SCARD_E_READER_UNAVAILABLE = 0x80100017

/*
#define SCARD_P_SHUTDOWN   ((LONG)0x80100018)
	The operation has been aborted to allow the server application to exit. More...

#define SCARD_E_PCI_TOO_SMALL   ((LONG)0x80100019)
	The PCI Receive buffer was too small.More...

#define SCARD_E_READER_UNSUPPORTED   ((LONG)0x8010001A)
	The reader driver does not meet minimal requirements for support.More...

#define SCARD_E_DUPLICATE_READER   ((LONG)0x8010001B)
	The reader driver did not produce a unique reader name. More...

#define SCARD_E_CARD_UNSUPPORTED   ((LONG)0x8010001C)
	The smart card does not meet minimal requirements for support.More...

#define SCARD_E_NO_SERVICE   ((LONG)0x8010001D)
	The Smart card resource manager is not running. More...

#define SCARD_E_SERVICE_STOPPED   ((LONG)0x8010001E)
	The Smart card resource manager has shut down. More...

#define SCARD_E_UNEXPECTED   ((LONG)0x8010001F)
	An unexpected card error has occurred. More...

#define SCARD_E_UNSUPPORTED_FEATURE   ((LONG)0x8010001F)
	This smart card does not support the requested feature.More...

#define SCARD_E_ICC_INSTALLATION   ((LONG)0x80100020)
	No primary provider can be found for the smart card.More...

#define SCARD_E_ICC_CREATEORDER   ((LONG)0x80100021)
	The requested order of object creation is not supported. More...

#define SCARD_E_DIR_NOT_FOUND   ((LONG)0x80100023)
	The identified directory does not exist in the smart card.More...

#define SCARD_E_FILE_NOT_FOUND   ((LONG)0x80100024)
	The identified file does not exist in the smart card.More...

#define SCARD_E_NO_DIR   ((LONG)0x80100025)
	The supplied path does not represent a smart card directory. More...

#define SCARD_E_NO_FILE   ((LONG)0x80100026)
	The supplied path does not represent a smart card file. More...

#define SCARD_E_NO_ACCESS   ((LONG)0x80100027)
	Access is denied to this file.More...

#define SCARD_E_WRITE_TOO_MANY   ((LONG)0x80100028)
	The smart card does not have enough memory to store the information. More...

#define SCARD_E_BAD_SEEK   ((LONG)0x80100029)
	There was an error trying to set the smart card file object pointer. More...

#define SCARD_E_INVALID_CHV   ((LONG)0x8010002A)
	The supplied PIN is incorrect.More...

#define SCARD_E_UNKNOWN_RES_MNG   ((LONG)0x8010002B)
	An unrecognized error code was returned from a layered component. More...

#define SCARD_E_NO_SUCH_CERTIFICATE   ((LONG)0x8010002C)
	The requested certificate does not exist. More...

#define SCARD_E_CERTIFICATE_UNAVAILABLE   ((LONG)0x8010002D)
	The requested certificate could not be obtained.More...

#define SCARD_E_NO_READERS_AVAILABLE   ((LONG)0x8010002E)
	Cannot find a smart card reader. More...

#define SCARD_E_COMM_DATA_LOST   ((LONG)0x8010002F)
	A communications error with the smart card has been detected. More...

#define SCARD_E_NO_KEY_CONTAINER   ((LONG)0x80100030)
	The requested key container does not exist on the smart card.More...

#define SCARD_E_SERVER_TOO_BUSY   ((LONG)0x80100031)
	The Smart Card Resource Manager is too busy to complete this operation.More...

#define SCARD_W_UNSUPPORTED_CARD   ((LONG)0x80100065)
	The reader cannot communicate with the card, due to ATR string configuration conflicts.More...

#define SCARD_W_UNRESPONSIVE_CARD   ((LONG)0x80100066)
	The smart card is not responding to a reset.More...

#define SCARD_W_UNPOWERED_CARD   ((LONG)0x80100067)
	Power has been removed from the smart card, so that further communication is not possible. More...

#define SCARD_W_RESET_CARD   ((LONG)0x80100068)
	The smart card has been reset, so any shared state information is invalid.More...

#define SCARD_W_REMOVED_CARD   ((LONG)0x80100069)
	The smart card has been removed, so further communication is not possible. More...

#define SCARD_W_SECURITY_VIOLATION   ((LONG)0x8010006A)
	Access was denied because of a security violation. More...

#define SCARD_W_WRONG_CHV   ((LONG)0x8010006B)
	The card cannot be accessed because the wrong PIN was presented.More...

#define SCARD_W_CHV_BLOCKED   ((LONG)0x8010006C)
	The card cannot be accessed because the maximum number of PIN entry attempts has been reached. More...

#define SCARD_W_EOF   ((LONG)0x8010006D)
	The end of the smart card file has been reached. More...

#define SCARD_W_CANCELLED_BY_USER   ((LONG)0x8010006E)
	The user pressed "Cancel" on a Smart Card Selection Dialog. More...

#define SCARD_W_CARD_NOT_AUTHENTICATED   ((LONG)0x8010006F)
	No PIN was presented to the smart card. More...*/

		}
	}
}
