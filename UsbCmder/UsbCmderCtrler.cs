using EricCore.Scsi;
using EricCore.Device;
using EricCore.Utilitys;
using System.Collections.Generic;

using System.Windows.Input;// for key up/down
using u32 = System.UInt32;
using u8 = System.Byte;

namespace EricCore.UsbCmder {
    public class UsbCmderCtrller {
        public UsbCmderView m_view;
        Utility m_u = new Utility();
        WpfUtility m_du = new WpfUtility();
        public delegate void ExtraPageCtrol(CdbCmd cmd, bool isPageDown);
        public ExtraPageCtrol extPageCtrl = null;
        public u8[] m_buffer;

        public delegate string Msg2nd(CdbCmd cmd, u8[] buffer);
        public Msg2nd msg2nd;

        public List<CdbCmd> get_extra_cmd_set(List<CdbCmd> cmdColls) {
            string extraCmdFilePath = "ExtraCmd.xml";
            if (m_u.is_file_exist(extraCmdFilePath)) {
                List<CdbCmd> extCmdSet = m_u.deserialize<List<CdbCmd>>(extraCmdFilePath);
                cmdColls.AddRange(extCmdSet);
            }
            return cmdColls;
        }

        public void init(UsbCmderView cmderView) {
            m_view = cmderView;
            CmdSetUfi cmdset = new CmdSetUfi();
            List<CdbCmd> cmdColls = cmdset.get_cmd_colls(get_extra_cmd_set(new List<CdbCmd>()));
            m_view.bind_cmd_sel(cmdColls);
            msg2nd = default_2nd_msg;
        }

        public void cmd_select_changed() {
            m_view.cmd_select_changed();
        }

        string default_2nd_msg(CdbCmd cmd, u8[] buffer) {
            return m_u.make_ascii_table(buffer, cmd.length);
        }

        public void refresh() {

        }

        public void execute() {
            CdbCmd cmd = m_view.get_cmd_from_ui(new CdbCmd());
            DevCtrl device = DeviceFactory.get_device();

            u8[] buf = new byte[128 * 1024];
            device.send_cmd(cmd, ref buf);
            m_view.send_msg(m_u.make_hex_table(buf, cmd.length));
            m_view.send_2nd_msg(msg2nd(cmd, buf));
        }

        public bool load_file() {
            string filePath = m_du.get_file_path("Bin|*.bin");
            if (filePath.Length == 0) {
                m_view.send_msg("No file have been selected");
                return false;
            }
            m_buffer = m_u.getFileData(filePath);
            return true;
        }

        public bool save_msg_2nd() {
            string filePath = m_du.get_file_path("txt|*.txt", "txt", "Msg2nd.txt");
            if (filePath.Length == 0) {
                m_view.send_msg("No file have been selected");
                return false;
            }
            m_u.to_file(filePath, m_view.m_msg2nd.ToString());
            return true;
        }

        CdbCmd page_up_down_ctrl(CdbCmd cmd, bool isPageDown) {
            if ((cmd.cdb[0] == 0x28) || (cmd.cdb[0] == 0x2A)) {
                u32 lba = m_u.to_u32(cmd.cdb, 2);
                if (isPageDown) {
                    lba++;
                } else {
                    lba--;
                }
                m_u.set_value_to_array_be(lba, cmd.cdb, 2);
            }

            extPageCtrl?.Invoke(cmd, isPageDown);

            return cmd;
        }

        void _page_control(bool isPageDown) {
            m_view.set_ui(page_up_down_ctrl(m_view.get_cmd_from_ui(new CdbCmd()), isPageDown));
            execute();
        }

        public void page_control(System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.PageDown) {
                _page_control(true);
            }
            if (e.Key == Key.PageUp) {
                _page_control(false);
            }
        }
    }
}