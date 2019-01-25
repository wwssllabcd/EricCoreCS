using System;
using System.Collections.Generic;

using ULONG = System.UInt32;
using BYTE = System.Byte;

namespace EricWang
{
    class UsbCmdSetUtility
    {
        private const BYTE SCSI_IOCTL_DATA_OUT = 0x00;
        private const BYTE SCSI_IOCTL_DATA_IN = 0x01;
        private const BYTE SCSI_IOCTL_DATA_UNSPECIFIED = 0x02; // non data

        private const BYTE FLAG_DATA_OUT = SCSI_IOCTL_DATA_OUT;
        private const BYTE FLAG_DATA_IN = SCSI_IOCTL_DATA_IN;

        public void getAllCmd(List<UsbCmdSet> colls) {
            colls.Add(inquiry());
            colls.Add(requestSense());

            colls.Add(readCapacity());
            colls.Add(readFormatCapacity());
            colls.Add(testUnitReady());
            colls.Add(read10());
            colls.Add(write10());
        }

        public UsbCmdSet inquiry() {
            UsbCmdSet cmd = new UsbCmdSet();
            cmd.cdb[0] = 0x12;
            cmd.cdb[4] = 0x24;
            cmd.length = 0x24;

            cmd.direction = FLAG_DATA_IN;
            cmd.cmdName = "UFI: Inquiry";
            cmd.desc = "Get Device Descriptor";

            return cmd;
        }

        public UsbCmdSet requestSense() {
            UsbCmdSet cmd = new UsbCmdSet();
            cmd.cdb[0] = 0x3;
            cmd.cdb[4] = 0x12;

            cmd.length = 0x12;
            cmd.direction = FLAG_DATA_IN;
            cmd.cmdName = "UFI: Request Sense";
            return cmd;
        }

        public UsbCmdSet readCapacity() {
            UsbCmdSet cmd = new UsbCmdSet();
            cmd.cdb[0] = 0x25;
            cmd.length = 8;
            cmd.direction = FLAG_DATA_IN;
            cmd.cmdName = "UFI: Read Capacity";
            cmd.desc = "How many sector in this device";
            return cmd;
        }

        public UsbCmdSet readFormatCapacity() {
            UsbCmdSet cmd = new UsbCmdSet();
            cmd.cdb[0] = 0x23;
            cmd.length = 12;
            cmd.direction = FLAG_DATA_IN;
            cmd.cmdName = "UFI: Read Format Capacity";
            return cmd;
        }

        public UsbCmdSet testUnitReady() {
            UsbCmdSet cmd = new UsbCmdSet();
            cmd.direction = FLAG_DATA_OUT;
            cmd.cmdName = "UFI: Test Unit Ready";
            return cmd;
        }

        public UsbCmdSet read10() {
            UsbCmdSet cmd = new UsbCmdSet();
            cmd.cdb[0] = 0x28;
            cmd.cdb[8] = 0x01;
            cmd.length = 512;

            cmd.direction = FLAG_DATA_IN;
            cmd.cmdName = "UFI: Read(10)";
            cmd.desc = "CDB[2]-CDB[5]: LBA address, CDB[8]: Sector Cnt";
            return cmd;
        }

        public UsbCmdSet write10() {
            UsbCmdSet cmd = new UsbCmdSet();
            cmd.cdb[0] = 0x2A;
            cmd.cdb[8] = 0x01;
            cmd.length = 512;

            cmd.direction = FLAG_DATA_OUT;
            cmd.cmdName = "UFI: Write(10)";
            cmd.desc = "CDB[2]-CDB[5]: LBA address, CDB[8]: Sector Cnt";
            return cmd;
        }
    }
}

