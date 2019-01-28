
using EricCore.Scsi;
using u8 = System.Byte;

namespace EricCore.Device {
    public interface DevCtrl {
        u8[] send_cmd(CdbCmd cmd, ref u8[] buffer);
    }

    class NullDevice : DevCtrl {
        u8[] DevCtrl.send_cmd(CdbCmd cmd, ref u8[] buffer) {
            for (int i = 0; i < cmd.length; i++) {
                buffer[i] = (u8)i;
            }
            return buffer;
        }
    }

    public class DeviceFactory {
        public static DevCtrl get_device() {
            return new NullDevice();
        }
    }
}
