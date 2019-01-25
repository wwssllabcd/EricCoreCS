
using u32 = System.UInt32;
using u8 = System.Byte;

using EricCore.Utilitys;

namespace EricCore.EricCoreCs.UsbCmder {
    public class SeqWriteParam {
        public u32 startLba;
        public u32 endLba;
        public u32 step;
        public u32 secLen;
        public bool isNoWrite;
        public bool isNoRead;
    }
    public class SeqWrite {
        public delegate bool UiStop();
        public delegate void LbaReadWrite(u32 lba, u32 secLen, u8[] buffer);
        public delegate void Reboot();
        public delegate void ShowMsg(string msg, bool isAppend);

        public void start(SeqWriteParam param, UiStop stop, LbaReadWrite lba_write, LbaReadWrite lba_read, Reboot reboot, ShowMsg show_msg) {
            u32 endLba = param.endLba;
            u32 stepLba = param.step;
            u32 secLen = param.secLen;
            bool isNoWrite = param.isNoWrite;
            bool isNoRead = param.isNoRead;

            u8[] writeBuffer = new byte[128 * 1024];
            u8[] readBuffer = new byte[128 * 1024];
            Utility util = new Utility();

            for (u32 startLba = param.startLba; startLba < endLba; startLba += stepLba) {
                if (stop()) {
                    return;
                }

                if ((startLba + secLen) > endLba) {
                    return;
                }

                if (isNoWrite == false) {
                    lba_write(startLba, secLen, writeBuffer);
                    reboot();
                }

                if (isNoRead) {
                    continue;
                }
                lba_read(startLba, secLen, readBuffer);
                bool res = util.memcmp(writeBuffer, readBuffer);
                if (res == false) {
                    string msg = util.show_buf_diff(writeBuffer, readBuffer);
                    show_msg(msg, false);
                }
            }
        }
    }
}
