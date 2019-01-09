using EricCore.Scsi;

namespace EricCore.UsbCmder {
    public class UsbCmderCtrller {
        UsbCmderView m_view;
        public void init(UsbCmderView cmderView) {
            m_view = cmderView;
            CmdSetUfi cmdset = new CmdSetUfi();
            m_view.init(cmdset.get_cmd_colls());
        }

        public void cmd_select_changed() {
            m_view.cmd_select_changed();
        }
        public void execute() {
            CdbCmd cmd = new CdbCmd();
            m_view.get_cmd_from_ui(cmd);
        }
    }
}
