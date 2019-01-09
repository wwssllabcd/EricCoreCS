using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32.SafeHandles;    //SafeHandle

using EricWang;

using ULONG = System.UInt32;
using BYTE = System.Byte;


namespace EricWang
{
    class Drive
    {
        public EricWang.MyHandle m_HandName = new EricWang.MyHandle();
        public EricWang.DriveUtility m_driUti = new EricWang.DriveUtility();
        public EricWang.Utility m_u = new EricWang.Utility();

        public BYTE[] m_nullBuff = new BYTE[1];

        public void getDeviceColls(List<Drive> devColls) {

            List<SafeFileHandle> handleColls = new List<SafeFileHandle>();
            List<string> devNameColls = new List<string>();

            m_driUti.getDeviceHandleAndName(handleColls, devNameColls);

            for( int i=0; i<handleColls.Count; i++){
                Drive d = new Drive();
                d.m_HandName.handle = handleColls[i];
                d.m_HandName.name = devNameColls[i];
                devColls.Add(d);
            }
        }
        private void preCheck(UsbCmdSet cmd, BYTE[] buf) {
            if (cmd.length > buf.Length) {
                string msg = cmd.direction + " buffer size too small";
                throw new EricException(msg);
            }
        }

        public void sendScsiCommand_safe(UsbCmdSet cmd, BYTE[] buffer) {
            preCheck(cmd, buffer);
            bool res = sendScsiCommand(cmd.cdb, buffer, cmd.length, cmd.direction);
            if (res == false) {
                string msg = cmd.cmdName + " command fail: " + cmd.cmdName;
                throw new EricException(msg);
            }
        }

        public bool sendScsiCommand(BYTE[] cdb, BYTE[] buffer, ULONG dataLen, BYTE direction)
        {
            bool res = m_driUti.sendScsiCommand(this.m_HandName.handle, cdb, buffer, dataLen, direction);
            return res;
        }

        public void writeLba(ULONG addr, BYTE[] ioBuffer, BYTE secCnt)
        {
            UsbCmdSet cmd = new UsbCmdSet();
            UsbCmdSetUtility ucu = new UsbCmdSetUtility();

            cmd = ucu.write10();
            Utility u = new Utility();
            byte[] tmp = BitConverter.GetBytes(addr);
            for (int i = 0; i < tmp.Length; i++)
            {
                cmd.cdb[2 + i] = tmp[tmp.Length - 1 - i];
            }

            cmd.cdb[8] = secCnt;
            cmd.length = (ULONG)(secCnt * 512);

            sendScsiCommand_safe(cmd, ioBuffer);
        }

        public void readLba(ULONG addr, BYTE[] ioBuffer, BYTE secCnt)
        {
            UsbCmdSet cmd = new UsbCmdSet();
            UsbCmdSetUtility ucu = new UsbCmdSetUtility();

            cmd = ucu.read10();
            Utility u = new Utility();
            byte[] tmp = BitConverter.GetBytes(addr);
            for (int i = 0; i < tmp.Length; i++) {
                cmd.cdb[2 + i] = tmp[tmp.Length - 1 - i];
            }

            cmd.cdb[8] = secCnt;
            cmd.length = (ULONG)(secCnt * 512);

            sendScsiCommand_safe(cmd, ioBuffer);
        }

    }
}
