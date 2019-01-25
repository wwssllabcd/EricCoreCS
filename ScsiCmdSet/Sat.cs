
using u8 = System.Byte;
using static EricCore.Scsi.SatConst;
using static EricCore.Constant;

namespace EricCore.Scsi {
    public static class SatConst {
        internal const u8 ATA_PASSTHROUGH_12 =0xA1;
        internal const u8 ATA_PASS_THROUGH_16 = 0x85;

        internal const u8 CDB_LENGTH_6 = 6;
        internal const u8 CDB_LENGTH_10 = 10;
        internal const u8 CDB_LENGTH_12 = 12;
        internal const u8 CDB_LENGTH_16 = 16;

        // ATA Pass Through Parameters define
        internal const u8 PROTOCOL_HARD_RESET = 0;
        internal const u8 PROTOCOL_SRST = 1;
        internal const u8 PROTOCOL_NON_DATA = 3;
        internal const u8 PROTOCOL_PIO_DATA_IN = 4;
        internal const u8 PROTOCOL_PIO_DATA_OUT = 5;
        internal const u8 PROTOCOL_DMA = 6;
        internal const u8 PROTOCOL_DMA_QUEUED = 7;
        internal const u8 PROTOCOL_DEVICE_DIAGNOSTIC = 8;
        internal const u8 PROTOCOL_DEVICE_RESET = 9;
        internal const u8 PROTOCOL_UDMA_DATA_IN = 10;
        internal const u8 PROTOCOL_UDMA_DATA_OUT = 11;
        internal const u8 PROTOCOL_FPDMA = 12;
        internal const u8 PROTOCOL_RETURN_RESPONSE_INFO = 15;

        internal const u8 BYT_BLOK_BYTES = 0;
        internal const u8 BYT_BLOK_BLOCKS = 1;

        internal const u8 CK_COND_NO_RETURN_STATUS = 0;
        internal const u8 CK_COND_RETURN_STATUS = 1;

        internal const u8 T_DIR_DATA_OUT = 0;
        internal const u8 T_DIR_DATA_IN = 1;
        internal const u8 T_LENGTH_NONE_DATA = 0;
        internal const u8 T_LENGTH_IN_FEATURE_FIELD = 1;
        internal const u8 T_LENGTH_IN_SECTOR_COUNT = 2;
        internal const u8 T_LENGTH_IN_STPSIU = 3;
    }


    public class Sat {
        
        public static CdbCmd to_atapt_16(CdbCmd cmd) {

            CdbCmd newCmd = new CdbCmd();

            newCmd.cdb[0] = ATA_PASS_THROUGH_16;
            newCmd.cdb[1] = ((PROTOCOL_DMA) << 1) | BIT_00;

            newCmd.cdb[2] = ((T_DIR_DATA_OUT & 0x01) << 3) | ((BYT_BLOK_BYTES & 0x01) << 2) | (T_LENGTH_IN_SECTOR_COUNT & 0x03);
            newCmd.cdb[3] = 0;
            newCmd.cdb[4] = 0;






            //IntPtr bufPtr = Marshal.UnsafeAddrOfPinnedArrayElement(cmd.GetInstance<SuperBlockType>().Buf, 0);
            //_sptdwb.sptd.DataTransferLength = lenSec * 512;
            //_sptdwb.sptd.DataIn = GlobalConst.SCSI_IOCTL_DATA_IN;
            //_sptdwb.sptd.DataBuffer = bufPtr;
            //_sptdwb.sptd.CdbLength = GlobalConst.CDB_LENGTH_16;

            //_sptdwb.sptd.Cdb[0] = GlobalConst.SCSI_CMD_ATA_PASS_THROUGH_16;
            //_sptdwb.sptd.Cdb[1] = GetATAPTByte1(0, GlobalConst.PROTOCOL_DMA, 1);
            //_sptdwb.sptd.Cdb[2] = GetATAPTByte2(0, 0, GlobalConst.T_DIR_DATA_OUT, GlobalConst.BYT_BLOK_BYTES, GlobalConst.T_LENGTH_IN_SECTOR_COUNT);
            //_sptdwb.sptd.Cdb[3] = 0x00;                                      // Features_exp     , offset 11
            //_sptdwb.sptd.Cdb[4] = 0x00;                                      // Features         , offset 3
            //_sptdwb.sptd.Cdb[5] = (byte)(lenSec >> 8);                       // SectorCount_exp  , offset 13
            //_sptdwb.sptd.Cdb[6] = (byte)(lenSec & 0xFF);                     // SectorCount      , offset 12
            //_sptdwb.sptd.Cdb[7] = (byte)(superPage >> 8);                    // secNum_exp       , offset 8
            //_sptdwb.sptd.Cdb[8] = (byte)(superBlk & 0xFF);                   // SectorNumber     , offset 4
            //_sptdwb.sptd.Cdb[9] = 0;                                         // CylLow_exp       , offset 9
            //_sptdwb.sptd.Cdb[10] = (byte)(superBlk >> 8);                    // CylLow           , offset 5
            //_sptdwb.sptd.Cdb[11] = (byte)(slcEn | (retryEn << 1));           // CylHigh_exp      , offset 10, firmware changed function to slcEn & retryEn
            //_sptdwb.sptd.Cdb[12] = (byte)superPage;                          // CylHigh          , offset 6
            //_sptdwb.sptd.Cdb[13] = GlobalConst.ALCOR_VDR_EX_SUPER_PAGE_READ; // Dev_Head         , offset 7
            //_sptdwb.sptd.Cdb[14] = GlobalConst.ALCOR_VENDOR_COMMAND_EX;      // Command          , offset 2
            //_sptdwb.sptd.Cdb[15] = 0x00;


            return newCmd;
        }
    }
}
