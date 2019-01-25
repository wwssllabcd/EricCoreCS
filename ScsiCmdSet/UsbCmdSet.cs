using ULONG = System.UInt32;
using BYTE = System.Byte;

namespace EricWang
{
    using System.Xml.Serialization;
    [System.Serializable]
    public class UsbCmdSet
    {
        [XmlElement("values", DataType = "hexBinary")]
        public BYTE[] cdb = new BYTE[12];
        public ULONG length;
        public BYTE direction;
        public string cmdName;
        public string desc;
    }
}
