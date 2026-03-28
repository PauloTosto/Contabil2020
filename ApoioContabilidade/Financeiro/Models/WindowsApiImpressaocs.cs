using System;
using System.Runtime.InteropServices;

namespace ApoioContabilidade.Financeiro.Models
{
    static public class WinApi
    {

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        //[MarshalAs(UnmanagedType.LPStr)] string
        //  [MarshalAs(UnmanagedType.LP)]
        [DllImport("gdi32.dll")]
        public static extern int Escape(IntPtr hdc, int iEscape, int cjIn, [MarshalAs(UnmanagedType.LPWStr)] string pvIn,
             IntPtr pvOut);
        
        /* 
         * devMode = (DEVMODE)Marshal.PtrToStructure(pDevMode, typeof(DEVMODE));
         * 
         * 
         * int Escape(
   HDC hdc,
   int iEscape,
   int cjIn,
   LPCSTR pvIn,
   LPVOID pvOut
 );*/
        //  function Escape(DC: HDC; p2, p3: Integer; p4: LPCSTR; p5: Pointer): Integer; stdcall;


        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        const int LOGPIXELSX = 88; // horizontal
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        const int LOGPIXELSY = 90;  // vertical

        public static int GetDeviceCapsX(IntPtr hdc)
        {
            return GetDeviceCaps(hdc, LOGPIXELSX);
        }
        public static int GetDeviceCapsY(IntPtr hdc)
        {
            return GetDeviceCaps(hdc, LOGPIXELSY);
        }
        // IntPtr hdc = GetDC(IntPtr.Zero);

        //  Console.WriteLine(GetDeviceCaps(hdc, LOGPIXELSX));
        // or
        // Console.WriteLine(GetDeviceCaps(hdc, LOGPIXELSY));
        // Console.ReadKey();

    }


   /* https://www.pinvoke.net/default.aspx/Structures/DEVMODE.html
    internal enum DMCOLLATE : short
    {
        /// <summary>
        /// Do not collate when printing multiple copies.
        /// </summary>
        DMCOLLATE_FALSE = 0,

        /// <summary>
        /// Collate when printing multiple copies.
        /// </summary>
        DMCOLLATE_TRUE = 1
    }
    // <summary>
    /// Selects duplex or double-sided printing for printers capable of duplex printing.
    /// </summary>
    internal enum DMDUP : short
    {
        /// <summary>
        /// Unknown setting.
        /// </summary>
        DMDUP_UNKNOWN = 0,

        /// <summary>
        /// Normal (nonduplex) printing.
        /// </summary>
        DMDUP_SIMPLEX = 1,

        /// <summary>
        /// Long-edge binding, that is, the long edge of the page is vertical.
        /// </summary>
        DMDUP_VERTICAL = 2,

        /// <summary>
        /// Short-edge binding, that is, the long edge of the page is horizontal.
        /// </summary>
        DMDUP_HORIZONTAL = 3,
    }
    /// <summary>
    /// Switches between color and monochrome on color printers.
    /// </summary>
    internal enum DMCOLOR : short
    {
        DMCOLOR_UNKNOWN = 0,

        DMCOLOR_MONOCHROME = 1,

        DMCOLOR_COLOR = 2
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
    internal struct DM_DOTNET
    {
        private const int CCHDEVICENAME2 = 32;
        private const int CCHFORMNAME2 = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME2)]
        public string dmDeviceName;
        public Int16 dmSpecVersion;
        public Int16 dmDriverVersion;
        public Int16 dmSize;
        public Int16 dmDriverExtra;
        public DM dmFields;

        public Int16 dmOrientation;
        public Int16 dmPaperSize;
        public Int16 dmPaperLength;
        public Int16 dmPaperWidth;
        public Int16 dmScale;
        public Int16 dmCopies;
        public Int16 dmDefaultSource;
        public Int16 dmPrintQuality;

        public POINTL dmPosition;
        public Int32 dmDisplayOrientation;
        public Int32 dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME2)]
        public string dmFormName;
        public Int16 dmLogPixels;
        public Int32 dmBitsPerPel;
        public Int32 dmPelsWidth;
        public Int32 dmPelsHeight;
        public Int32 dmDisplayFlags;
        //public Int32 dmNup;
        public Int32 dmDisplayFrequency;
    }




    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        [System.Runtime.InteropServices.FieldOffset(0)]
        public string dmDeviceName;
        [System.Runtime.InteropServices.FieldOffset(32)]
        public Int16 dmSpecVersion;
        [System.Runtime.InteropServices.FieldOffset(34)]
        public Int16 dmDriverVersion;
        [System.Runtime.InteropServices.FieldOffset(36)]
        public Int16 dmSize;
        [System.Runtime.InteropServices.FieldOffset(38)]
        public Int16 dmDriverExtra;
        [System.Runtime.InteropServices.FieldOffset(40)]
        public DM dmFields;

        [System.Runtime.InteropServices.FieldOffset(44)]
        Int16 dmOrientation;
        [System.Runtime.InteropServices.FieldOffset(46)]
        Int16 dmPaperSize;
        [System.Runtime.InteropServices.FieldOffset(48)]
        Int16 dmPaperLength;
        [System.Runtime.InteropServices.FieldOffset(50)]
        Int16 dmPaperWidth;
        [System.Runtime.InteropServices.FieldOffset(52)]
        Int16 dmScale;
        [System.Runtime.InteropServices.FieldOffset(54)]
        Int16 dmCopies;
        [System.Runtime.InteropServices.FieldOffset(56)]
        Int16 dmDefaultSource;
        [System.Runtime.InteropServices.FieldOffset(58)]
        Int16 dmPrintQuality;

        [System.Runtime.InteropServices.FieldOffset(44)]
        public POINTL dmPosition;
        [System.Runtime.InteropServices.FieldOffset(52)]
        public Int32 dmDisplayOrientation;
        [System.Runtime.InteropServices.FieldOffset(56)]
        public Int32 dmDisplayFixedOutput;

        [System.Runtime.InteropServices.FieldOffset(60)]
        public short dmColor; // See note below!
        [System.Runtime.InteropServices.FieldOffset(62)]
        public short dmDuplex; // See note below!
        [System.Runtime.InteropServices.FieldOffset(64)]
        public short dmYResolution;
        [System.Runtime.InteropServices.FieldOffset(66)]
        public short dmTTOption;
        [System.Runtime.InteropServices.FieldOffset(68)]
        public short dmCollate; // See note below!
        [System.Runtime.InteropServices.FieldOffset(70)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        [System.Runtime.InteropServices.FieldOffset(102)]
        public Int16 dmLogPixels;
        [System.Runtime.InteropServices.FieldOffset(104)]
        public Int32 dmBitsPerPel;
        [System.Runtime.InteropServices.FieldOffset(108)]
        public Int32 dmPelsWidth;
        [System.Runtime.InteropServices.FieldOffset(112)]
        public Int32 dmPelsHeight;
        [System.Runtime.InteropServices.FieldOffset(116)]
        public Int32 dmDisplayFlags;
        [System.Runtime.InteropServices.FieldOffset(116)]
        public Int32 dmNup;
        [System.Runtime.InteropServices.FieldOffset(120)]
        public Int32 dmDisplayFrequency;
    }
    public struct POINTL
    {
        public Int32 x;
        public Int32 y;
    }
    public class PrinterSettings
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class PRINTERDEFAULTSClass
        {
            public IntPtr pDatatype;
            public IntPtr pDevMode;
            public int DesiredAccess;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, PRINTERDEFAULTSClass pdc);

        public DEVMODE GetPrinterSettings(string PrinterName)
        {
            DEVMODE dm;

            var pdc = new PRINTERDEFAULTSClass
            {
                pDatatype = new IntPtr(0),
                pDevMode = new IntPtr(0),
                DesiredAccess = PRINTER_ALL_ACCESS
            };
            IntPtr hPrinter;
            var nRet = Convert.ToInt32(OpenPrinter(PrinterName,
                    out hPrinter, pdc));
            return dm;
        }
    }
   */
}




