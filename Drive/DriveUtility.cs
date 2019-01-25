using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO; // DriveInfo
using System.Runtime.InteropServices; //for Import dll, ex: createFile, DeviceIoCtrl
using Microsoft.Win32.SafeHandles;    //SafeHandle


using ULONG = System.UInt32;
using BYTE = System.Byte;


namespace EricWang
{
    class DriveUtility
    {
        //creareFile
        public const int GENERIC_ALL = 0x10000000;
        public const int GENERIC_EXECUTE = 0x20000000;
        public const uint GENERIC_READ = 0x80000000;
        public const int GENERIC_WRITE = 0x40000000;
        public const int FILE_SHARE_READ = 1;
        public const int FILE_SHARE_WRITE = 2;
        public const int OPEN_EXISTING = 3;


        private const uint IOCTL_SCSI_PASS_THROUGH_DIRECT = 0x4D014;
        private const byte SCSI_IOCTL_DATA_OUT = 0;
        private const byte SCSI_IOCTL_DATA_IN = 1;
        private const byte SCSI_IOCTL_DATA_UNSPECIFIED = 2;

        // SCSI PASS Through
        private const uint METHOD_BUFFERED = 0;
        private const uint FILE_READ_ACCESS = 0x0001;
        private const uint FILE_WRITE_ACCESS = 0x0002;
        private const uint FILE_DEVICE_CONTROLLER = 0x00000004;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void SetLastError(uint dwErrCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern SafeFileHandle CreateFile(
            String lpFileName,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            IntPtr hTemplateFile
            );

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped
            );

        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access) {
            return (((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method));
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct SCSI_PASS_THROUGH
        {
            public short Length;
            public byte ScsiStatus;
            public byte PathId;
            public byte TargetId;
            public byte Lun;
            public byte CdbLength;
            public byte SenseInfoLength;
            public byte DataIn;
            public UInt32 DataTransferLength;
            public UInt32 TimeOutValue;
            public UInt32 DataBufferOffset; // note this is now interpreted as a pointer, not an offset!!
            public UInt32 SenseInfoOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Cdb;
        };

        [StructLayout(LayoutKind.Sequential)]
        class SCSI_PASS_THROUGH_WITH_BUFFERS
        {
            internal SCSI_PASS_THROUGH spt = new SCSI_PASS_THROUGH();
            uint filter;
            // // adapt size to suit your needs!!!!!! 
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            //internal byte[] sense;
            // adapt to suit your needs!!!!!!! 
            //[MarshalAs(UnmanagedType.ByValArray)]    //changed to match CDB
            //public byte[] dataBuf;
        };

        public bool sendScsiCommand(SafeFileHandle sHandle, BYTE[] cdb, BYTE[] ioBuffer, ULONG dataLen, BYTE direction) {
            const int dataBufOffset = 0x30;

            uint IOCTL_SCSI_PASS_THROUGH = CTL_CODE(FILE_DEVICE_CONTROLLER, 0x0401, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS); //0x4D004
            SCSI_PASS_THROUGH_WITH_BUFFERS sptwb = new SCSI_PASS_THROUGH_WITH_BUFFERS();


            sptwb.spt.Cdb = new byte[16];
            sptwb.spt.Cdb = Enumerable.Repeat((byte)0, 16).ToArray();
            sptwb.spt.CdbLength = 12;
            Array.Copy(cdb, sptwb.spt.Cdb, sptwb.spt.CdbLength);

            sptwb.spt.DataTransferLength = dataLen;
            sptwb.spt.SenseInfoLength = 0;
            sptwb.spt.SenseInfoOffset = 0;

            sptwb.spt.DataBufferOffset = dataBufOffset; // fix to 0x30
            sptwb.spt.Length = (short)Marshal.SizeOf(sptwb.spt);

            sptwb.spt.PathId = 0;
            sptwb.spt.TargetId = 0;
            sptwb.spt.Lun = 0;
            sptwb.spt.TimeOutValue = 30;
            sptwb.spt.DataIn = direction;

            // call DeviceIoControl passing the buffer inpBuffer as inp buffer and/or output buffer depending on the command.
            uint Dummy = 0;
            uint inputBufLen = 0;
            uint outputBufLen = 0;

            // setup pointer of sptwb
            int sptwbLen = Marshal.SizeOf(sptwb);
            IntPtr ptrSptwb = Marshal.AllocHGlobal(sptwbLen + (int)dataLen);
            Marshal.StructureToPtr(sptwb.spt, ptrSptwb, false);

            IntPtr pDataBuf = new IntPtr(ptrSptwb.ToInt32() + dataBufOffset);

            if (dataLen != 0) {
                if (direction == SCSI_IOCTL_DATA_OUT) {
                    inputBufLen = (uint)Marshal.SizeOf(sptwb) + dataLen;
                    outputBufLen = (uint)Marshal.SizeOf(sptwb.spt);

                    //fill buffer
                    Marshal.Copy(ioBuffer, 0, pDataBuf, (int)dataLen);
                } else {
                    inputBufLen = (uint)Marshal.SizeOf(sptwb.spt);
                    outputBufLen = (uint)Marshal.SizeOf(sptwb) + dataLen;
                }
            } else {
                inputBufLen = outputBufLen = (uint)Marshal.SizeOf(sptwb.spt);
            }

            bool ret = DeviceIoControl(
                sHandle.DangerousGetHandle(),
                IOCTL_SCSI_PASS_THROUGH,
                ptrSptwb,
                inputBufLen,

                ptrSptwb,
                outputBufLen,

                out  Dummy,
                IntPtr.Zero);

            // ptr to struct
            //Marshal.PtrToStructure(ptrSptwb, sptwb);
            if (direction == SCSI_IOCTL_DATA_IN) {
                if (dataLen != 0) {
                    Marshal.Copy(pDataBuf, ioBuffer, 0, (int)dataLen);
                }
            }

            Marshal.FreeHGlobal(ptrSptwb);

            return ret;
        }

        public void getDeviceHandleAndName(List<SafeFileHandle> handleColls, List<string> devNameColls) {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives) {
                if (drive.DriveType != DriveType.Removable) {
                    continue;
                }
                if (drive.Name == "A:\\") {
                    continue;
                }

                char[] deviceName = new char[1];
                deviceName[0] = drive.Name[0];
                String dn = "\\\\.\\" + deviceName[0] + ":";
                SafeFileHandle hDevice = CreateFile
                    (
                    dn,
                    GENERIC_READ | GENERIC_WRITE,
                    FILE_SHARE_READ | FILE_SHARE_WRITE,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    0,
                    IntPtr.Zero);

                if (hDevice.IsInvalid) {
                    continue;
                }

                handleColls.Add(hDevice);
                devNameColls.Add(deviceName[0].ToString());
            }
        }
    }
}
