using System.Collections.Generic;


using u32 = System.UInt32;
using u8 = System.Byte;

namespace EricCore.Scsi {
    public class CdbCmd {
        public List<byte> cdb = new List<byte> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public u32 len;
        public string desc;
        public u8 direction;

        public const u8 DATA_IN = 2;
        public const u8 DATA_OUT = 4;

        public CdbCmd() { }
        public override string ToString() {
            return desc;
        }
    }

    public class CmdSetUfi {
        public List<CdbCmd> get_cmd_colls() {
            List<CdbCmd> cmdColls = new List<CdbCmd>();
            cmdColls.Add(inquiry());
            cmdColls.Add(capacity());
            return cmdColls;
        }

        public CdbCmd inquiry() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x12;
            cmd.cdb[4] = 0x24;
            cmd.direction = CdbCmd.DATA_IN;
            cmd.desc = "UFI: Inquiry";
            cmd.len = 0x24;
            return cmd;
        }

        public CdbCmd capacity() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x25;
            cmd.direction = CdbCmd.DATA_IN;
            cmd.desc = "UFI: Capacity";
            cmd.len = 0x08;
            return cmd;
        }

    }
}
