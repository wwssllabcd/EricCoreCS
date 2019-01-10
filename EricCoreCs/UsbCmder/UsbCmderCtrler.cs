using EricCore.Scsi;
using EricCore.Device;
using EricCore.Utilitys;
using System.Collections.Generic;

namespace EricCore.UsbCmder {
    public class UsbCmderCtrller {
        UsbCmderView m_view;
        Utility m_u = new Utility();
        public void init(UsbCmderView cmderView) {
            m_view = cmderView;
            CmdSetUfi cmdset = new CmdSetUfi();
            m_view.init(cmdset.get_cmd_colls(new List<CdbCmd>()));
        }

        public void cmd_select_changed() {
            m_view.cmd_select_changed();
        }
        public void execute() {
            CdbCmd cmd = m_view.get_cmd_from_ui(new CdbCmd());
            DevCtrl device = new ClDevice();

            byte[] buf = new byte[128 * 1024];
            device.send_cmd(cmd, ref buf);
            m_view.send_msg(m_u.makeHexTable(buf, cmd.length));
        }
    }
}
