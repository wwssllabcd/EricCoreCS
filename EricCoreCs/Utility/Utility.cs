using System;
using System.Text;
using System.IO;


using ULONG = System.UInt32;
using u8 = System.Byte;


namespace EricCore
{
    public class Utility
    {
        public const u8 BIT0 = 0x01;
        public const u8 BIT1 = 0x02;
        public const u8 BIT2 = 0x04;
        public const u8 BIT3 = 0x08;
        public const u8 BIT4 = 0x10;
        public const u8 BIT5 = 0x20;
        public const u8 BIT6 = 0x40;
        public const u8 BIT7 = 0x80;

        public string makeHeader(string meg) {
            StringReader sr = new StringReader(meg);
            StringBuilder strB = new StringBuilder();

            strB.Append("0000| 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F").Append(crlf());
            strB.Append("====|================================================");

            String line = sr.ReadLine();
            int cnt = 0;

            while (!String.IsNullOrEmpty(line)) {

                if ((cnt % 0x200) == 0) {
                    strB.Append(crlf());
                }

                strB.Append(cnt.ToString("X04") + "| ");
                strB.Append(line);
                line = sr.ReadLine();

                cnt += 0x10;
            }
            return strB.ToString();
        }

        public string makeHexTable_NoHeader(byte[] array) {
            return makeHexTable_NoHeader(array, (uint)array.Length);
        }

        public string makeHexTable(byte[] array) {
            return makeHexTable(array, (uint)array.Length);
        }

        public string makeHexTable(byte[] array, uint length) {
            //return makeHexTable_NoHeader(array, length);
            return makeHeader(makeHexTable_NoHeader(array, length));
        }

        public string makeHexTable_NoHeader(byte[] array, uint length) {
            StringBuilder strB = new StringBuilder();
            for (uint i = 0; i < length; i++) {
                strB.Append(array[i].ToString("X2"));
                strB.Append(" ");
                if (((i + 1) % 0x10) == 0) {
                    //prevent last crlf
                    if ((i + 1) < length) {
                        strB.Append(crlf());
                    }
                }
            }
            return strB.ToString();
        }

        public string makeAsciiTable(byte[] array) {
            return makeAsciiTable(array, (uint)array.Length);
        }

        public string makeAsciiTable(byte[] array, uint length) {
            StringBuilder strB = new StringBuilder();
            for (uint i = 0; i < length; i++) {
                //skip "0"
                if (array[i] == 0) {
                    strB.Append(".");
                } else {
                    strB.Append(Convert.ToChar(array[i]));
                }

                if (((i + 1) % 0x10) == 0) {
                    if ((i + 1) < length) {
                        strB.Append(crlf());
                    }
                }
            }
            return strB.ToString();
        }

        public byte[] getFileData(string path) {
            FileStream fp = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fp);
            int size = (int)br.BaseStream.Length;

            byte[] ioBuf = new byte[size];
            ioBuf = br.ReadBytes(size);

            fp.Dispose();

            return ioBuf;
        }

        void must_have_file(string fineName) {
            if (!File.Exists(fineName)) {
                using (FileStream fs = File.Create(fineName)) {
                }
            }
        }

        public void to_file(string fineName, u8[] buf) {
            must_have_file(fineName);
            //開啟建立檔案
            using (FileStream file = File.Open(fineName, FileMode.Open, FileAccess.ReadWrite)) {
                using (BinaryWriter writeFile = new BinaryWriter(file)) {
                    writeFile.Write(buf);
                }
            }
        }

        public void to_file(string fineName, string txt) {
            must_have_file(fineName);
            using (FileStream file = File.Open(fineName, FileMode.Open, FileAccess.ReadWrite)) {
                using (StreamWriter sw = new StreamWriter(file)) {
                    sw.WriteLine(txt);
                }
            }
        }

        public string getDate() {
            DateTime t = DateTime.Now;
            string res = t.Year.ToString() + "-" + t.Month.ToString("D2") + t.Day.ToString("D2") + "-" + t.Minute.ToString("D2") + t.Second.ToString("D2");
            return res;
        }


        public string crlf() {
            return "\r\n";
        }

        public void ulongToArray(ULONG source, byte[] ary, int offset) {
            ary[offset + 0] = (u8)(source >> 0x18);
            ary[offset + 1] = (u8)(source >> 0x10);
            ary[offset + 2] = (u8)(source >> 0x08);
            ary[offset + 3] = (u8)(source >> 0x00);
        }

        public ULONG arrayToUlong(byte[] ary, int offset) {
            ULONG res = 0;
            res += (ULONG)ary[offset + 0] << 0x18;
            res += (ULONG)ary[offset + 1] << 0x10;
            res += (ULONG)ary[offset + 2] << 0x08;
            res += (ULONG)ary[offset + 3] << 0x00;
            return res;
        }

        public void genPattern(u8[] array, ULONG value, int startOffset, int count) {
            int i = startOffset;
            while (i < count) {
                if ((count - i) >= 4) {
                    ulongToArray(value, array, i);
                    i += 4;
                } else {
                    int tmp = i % 4;
                    array[i] = (u8)(value >> (0x18 - (tmp * 8)));
                    i += 1;
                }
            }
        }

        public void genPattern(u8[] array, ULONG value, int offset) {
            genPattern(array, value, offset, array.Length);
        }

        public void genPattern_increase(u8[] array) {
            for (int i = 0; i < array.Length; i++) {
                array[i] = (u8)(i & 0xFF);
            }
        }


        public bool cmpWriteReadBuf_9k(u8[] writeBuf, u8[] readBuf) {
            for (int i = 0; i < 8192; i++) {
                if (writeBuf[1024 + i] != readBuf[i]) {
                    return false;
                }
            }
            return true;
        }

        public bool memcmp(u8[] buf1, u8[] buf2, ULONG offset1, ULONG offset2, ULONG length) {
            for (int i = 0; i < length; i++) {
                if (buf1[offset1 + i] != buf2[offset2 + i]) {
                    return false;
                }
            }
            return true;
        }

        public bool memcmp(u8[] buf1, u8[] buf2, ULONG length) {
            return memcmp(buf1, buf2, 0, 0, length);
        }

        public void memcpy(u8[] source, u8[] target) {
            memcpy(source, target, 0, 0, source.Length);
        }


        public void memcpy(u8[] source, u8[] target, ULONG source_offset, ULONG target_offset, int length) {
            for (int i = 0; i < length; i++) {
                target[target_offset + i] = source[source_offset + i];
            }
        }

        public void memset(u8[] target, u8 offset, u8 value, int length) {
            for (int i = 0; i < length; i++) {
                target[offset + i] = value;
            }
        }

        public ULONG hexStringToUlong(string str) {
            return Convert.ToUInt32(str, 16);
        }

    }
}
