using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using u32 = System.UInt32;
using u8 = System.Byte;

namespace EricCore
{
    public class FLVContext
    {
        public u8 soundCodeId = 0;
        public u8 videoCodecId = 0;
        public u8[] flvHeader = new u8[9];
    }
    public class FlvMerge
    {
        const uint FLV_TAG_HEADER_SIZE = 15;
        const uint FLV_HEADER_SIZE = 9;

        FileStream m_flvFs;
        FLVContext m_flvInfo;
        u32 m_baseTime;

        public FlvMerge(string filePath) {
            if (File.Exists(filePath) == true) {
                m_flvFs = new FileStream(filePath, FileMode.Open);
            } else {
                m_flvFs = new FileStream(filePath, FileMode.CreateNew);
            }
        }

        public void closeFile()  // destructor
        {
            m_flvFs.Close();
        }

        public void open(string path) {
            m_flvFs = new FileStream(path, FileMode.Open);
            m_flvInfo = get_flv_info(m_flvFs);
            if (check_flv_header(m_flvInfo.flvHeader) == false) {
                throw new System.ArgumentException("");
            }
        }

        u32 get_data_length(u8[] tag) {
            int len = (tag[5] << 0x10) + (tag[6] << 0x08) + (tag[7]);
            return (u32)len;
        }
        u32 get_timestamp(u8[] tag) {
            int len = (tag[8] << 0x10) + (tag[9] << 0x08) + (tag[10]);
            return (u32)len;
        }

        void set_timestamp(u8[] tag, u32 timeStamp) {
            tag[11] = (byte)(timeStamp >> 24);
            tag[8] = (byte)(timeStamp >> 16);
            tag[9] = (byte)(timeStamp >> 8);
            tag[10] = (byte)(timeStamp);
        }

        bool check_flv_header(u8[] flvHeader) {
            if (flvHeader[0] != 'F' || flvHeader[1] != 'L' || flvHeader[2] != 'V' || flvHeader[3] != 0x01) {
                return false;
            } else {
                return true;
            }
        }

        public FLVContext get_flv_info(FileStream fs) {
            FLVContext flvInfo = new FLVContext();
            fs.Position = 0;
            fs.Read(flvInfo.flvHeader, 0, flvInfo.flvHeader.Length);
            fs.Position = FLV_HEADER_SIZE;
            u8[] tag = new byte[FLV_TAG_HEADER_SIZE];

            bool hasAudioParams = false;
            bool hasVideoParams = false;

            while (((hasAudioParams == true) && (hasVideoParams == true)) == false) {
                fs.Read(tag, 0, tag.Length);
                //OFFSET 4:　tag類型（1位元組）；0x8音訊；0x9影片；0x12指令碼資料
                u8 tagType = (byte)(tag[4] & 0x1f);
                switch (tagType) {
                    case 0x08:
                        flvInfo.soundCodeId = (byte)(tag[FLV_TAG_HEADER_SIZE - 1]);
                        hasAudioParams = true;
                        break;
                    case 9:
                        flvInfo.videoCodecId = (byte)(tag[FLV_TAG_HEADER_SIZE - 1]);
                        hasVideoParams = true;
                        break;
                    default:
                        break;
                }
                u32 dataSize = get_data_length(tag);
                fs.Position += dataSize;
            }
            return flvInfo;
        }

        u32 _merge_base(FileStream merge, FileStream fs, u32 baseTimestamp) {
            byte[] header = new byte[FLV_HEADER_SIZE];
            byte[] tag = new byte[FLV_TAG_HEADER_SIZE];
            byte[] dataBuf = new byte[16777220];

            fs.Position = 0;

            if (merge.Length == 0) {
                fs.Read(header, 0, header.Length);
                merge.Write(header, 0, header.Length);
            } else {
                fs.Position = FLV_HEADER_SIZE;
            }

            u32 curTimestamp = 0;
            while (true) {
                int readLen = fs.Read(tag, 0, (int)FLV_TAG_HEADER_SIZE);
                if (readLen != FLV_TAG_HEADER_SIZE) {
                    break;
                }

                //tag 內容大小（3 byte）
                u32 dataSize = get_data_length(tag);
                curTimestamp = baseTimestamp + get_timestamp(tag);
                set_timestamp(tag, curTimestamp);

                int readDataLen = fs.Read(dataBuf, 0, (int)dataSize);

                if (readDataLen != dataSize) {
                    break;
                }

                merge.Write(tag, 0, tag.Length);
                merge.Write(dataBuf, 0, (int)dataSize);
            }

            return curTimestamp;
        }

        public void merge(string path) {
            FileStream flv = new FileStream(path, FileMode.Open);
            m_baseTime = _merge_base(this.m_flvFs, flv, m_baseTime);
            flv.Close();
        }
    }
}
