
using EricCore.Scsi;

namespace EricCore.Device {
    public interface DevCtrl {
        byte[] send_cmd(CdbCmd cmd, ref byte[] buffer);
    }
}
