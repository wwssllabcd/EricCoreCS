using System.Collections.Generic;

using u32 = System.UInt32;
using u8 = System.Byte;

namespace EricCore.Scsi {
    using System.Xml.Serialization;
    [System.Serializable]
    public class CdbCmd {
        [XmlElement("values", DataType = "hexBinary")]
        public u8[] cdb = new u8[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public u32 length;
        public string desc;
        public u8 direction;
        public override string ToString() {
            return desc;
        }
    }

    public static class ScsiConst {
        public const u8 DATA_OUT = 0;
        public const u8 DATA_IN = 1;
        public const u8 DATA_NON = 2;
    }

    public class CmdSetUfi {
        public List<CdbCmd> get_cmd_colls(List<CdbCmd> cmdColls) {
            cmdColls.Add(inquiry());
            cmdColls.Add(requestSense());
            cmdColls.Add(readCapacity());
            cmdColls.Add(readFormatCapacity());
            cmdColls.Add(testUnitReady());
            cmdColls.Add(read10());
            cmdColls.Add(write10());
            return cmdColls;
        }

        public CdbCmd inquiry() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x12;
            cmd.cdb[4] = 0x24;
            cmd.length = 0x24;
            
            cmd.direction = ScsiConst.DATA_IN;
            cmd.desc = "UFI: Inquiry";
            return cmd;
        }

        public CdbCmd requestSense() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x3;
            cmd.cdb[4] = 0x12;
            cmd.length = 0x12;
            cmd.direction = ScsiConst.DATA_IN;
            cmd.desc = "UFI: Request Sense";
            return cmd;
        }

        public CdbCmd readCapacity() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x25;
            cmd.length = 8;
            cmd.direction = ScsiConst.DATA_IN;
            cmd.desc = "UFI: Read Capacity";
            return cmd;
        }

        public CdbCmd readFormatCapacity() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x23;
            cmd.length = 12;
            cmd.direction = ScsiConst.DATA_IN;
            cmd.desc = "UFI: Read Format Capacity";
            return cmd;
        }

        public CdbCmd testUnitReady() {
            CdbCmd cmd = new CdbCmd();
            cmd.direction = ScsiConst.DATA_OUT;
            cmd.desc = "UFI: Test Unit Ready";
            return cmd;
        }

        public CdbCmd read10() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x28;
            cmd.cdb[8] = 0x01;
            cmd.length = 512;
            cmd.direction = ScsiConst.DATA_IN;
            cmd.desc = "UFI: Read(10)";
            return cmd;
        }

        public CdbCmd write10() {
            CdbCmd cmd = new CdbCmd();
            cmd.cdb[0] = 0x2A;
            cmd.cdb[8] = 0x01;
            cmd.length = 512;
            cmd.direction = ScsiConst.DATA_OUT;
            cmd.desc = "UFI: Write(10)";
            return cmd;
        }

    }
}
