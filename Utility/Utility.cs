using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using u32 = System.UInt32;
using u16 = System.UInt16;
using u8 = System.Byte;


namespace EricCore.Utilitys {
    public class Utility {
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
                strB.Append(line).Append(crlf());
                line = sr.ReadLine();

                cnt += 0x10;
            }
            return strB.ToString();
        }

        public string makeHexTable_NoHeader(u8[] array, uint length) {
            StringBuilder strB = new StringBuilder();
            for (uint i = 0; i < length; i++) {
                strB.Append(array[i].ToString("X2"));
                strB.Append(" ");
                if (((i + 1) % 0x10) == 0) {
                    strB.Append(crlf());
                }
            }
            return strB.ToString();
        }

        public string makeHexTable_NoHeader(u8[] array) {
            return makeHexTable_NoHeader(array, (uint)array.Length);
        }

        public string make_hex_table(u8[] array) {
            return make_hex_table(array, (uint)array.Length);
        }

        public string make_hex_table(u8[] array, uint length) {
            return makeHeader(makeHexTable_NoHeader(array, length));
        }

        public string make_ascii_table(u8[] array, uint length) {
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

        public string make_ascii_table(u8[] array) {
            return make_ascii_table(array, (uint)array.Length);
        }

        public u8[] getFileData(string path) {
            FileStream fp = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fp);
            int size = (int)br.BaseStream.Length;

            u8[] ioBuf = new byte[size];
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
            return Environment.NewLine;
        }

        public void set_value_to_array_be(u32 source, u8[] ary, int offset) {
            ary[offset + 0] = (u8)(source >> 0x18);
            ary[offset + 1] = (u8)(source >> 0x10);
            ary[offset + 2] = (u8)(source >> 0x08);
            ary[offset + 3] = (u8)(source >> 0x00);
        }


        u16 to_u16_le(u8[] ary, int offset) {
            u16 res = 0;
            res += (u16)(ary[offset + 1] << 0x08);
            res += (u16)(ary[offset + 0] << 0x00);
            return res;
        }

        u32 to_u32_le(u8[] ary, int offset) {
            u32 res = 0;
            res += (u32)ary[offset + 3] << 0x18;
            res += (u32)ary[offset + 2] << 0x10;
            res += (u32)ary[offset + 1] << 0x08;
            res += (u32)ary[offset + 0] << 0x00;
            return res;
        }

        public u16 to_u16(u8[] ary, int offset) {
            return to_u16_le(ary, offset);
        }

        public u32 to_u32(u8[] ary, int offset) {
            return to_u32_le(ary, offset);
        }


        public void genPattern(u8[] array, u32 value, int startOffset, int count) {
            int i = startOffset;
            while (i < count) {
                if ((count - i) >= 4) {
                    set_value_to_array_be(value, array, i);
                    i += 4;
                } else {
                    int tmp = i % 4;
                    array[i] = (u8)(value >> (0x18 - (tmp * 8)));
                    i += 1;
                }
            }
        }

        public void genPattern(u8[] array, u32 value, int offset) {
            genPattern(array, value, offset, array.Length);
        }

        public void genPattern_increase(u8[] array) {
            for (int i = 0; i < array.Length; i++) {
                array[i] = (u8)(i & 0xFF);
            }
        }

        public bool memcmp(u8[] buf1, u8[] buf2, u32 offset1, u32 offset2, u32 length) {
            for (int i = 0; i < length; i++) {
                if (buf1[offset1 + i] != buf2[offset2 + i]) {
                    return false;
                }
            }
            return true;
        }

        public bool memcmp(u8[] buf1, u8[] buf2) {
            int len1 = buf1.Length;
            int len2 = buf2.Length;
            if (len1 != len2) {
                throw new System.ArgumentException("buf len is not equal");
            }
            return memcmp(buf1, buf2, 0, 0, (u32)len2);
        }

        public void memcpy(u8[] source, u8[] target) {
            memcpy(source, target, 0, 0, source.Length);
        }

        public void memcpy(u8[] source, u8[] target, u32 source_offset, u32 target_offset, int length) {
            for (int i = 0; i < length; i++) {
                target[target_offset + i] = source[source_offset + i];
            }
        }

        public void memset(u8[] target, u8 offset, u8 value, int length) {
            for (int i = 0; i < length; i++) {
                target[offset + i] = value;
            }
        }

        public u32 to_u32(string hexString) {
            return Convert.ToUInt32(hexString, 16);
        }
        public u8 to_byte(string hexString) {
            return Convert.ToByte(hexString, 16);
        }

        public string toHexString(u8 val) {
            return val.ToString("X2");
        }

        public string toHexString(u16 val) {
            return val.ToString("X4");
        }

        public string toHexString(u32 val) {
            return val.ToString("X8");
        }

        public void serialize<T>(T item, string fileName) {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(item.GetType());
            Stream s = File.Open(fileName, FileMode.Create);
            ser.Serialize(s, item);
            s.Close();
        }

        public T deserialize<T>(string fileName) {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(fileName);
            object obj = ser.Deserialize(reader);
            reader.Close();
            return (T)obj;

        }

        public bool is_file_exist(string filePath) {
            return System.IO.File.Exists(filePath);
        }

        public string show_buf_diff(u8[] buf1, u8[] buf2) {
            int len = buf1.Length;
            string res = "";
            for (int i = 0; i < len; i++) {
                byte a = buf1[i];
                byte b = buf2[i];
                if (a != b) {
                    res += $"diff[{i}], {a}, {b}";
                }
            }
            return res;
        }

        public T[] init_obj_array<T>(int length) where T : new() {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i) {
                array[i] = new T();
            }
            return array;
        }


    }
}
