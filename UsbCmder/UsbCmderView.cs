

using System.Collections.Generic;
using System.Windows.Controls;

using EricCore.Utilitys;
using EricCore.Scsi;



namespace EricCore.UsbCmder {
    public class UsbCmderView {
        public List<TextBox> m_txtCdb = new List<TextBox>(16);
        public TextBox m_mainMsg;
        public TextBox m_msg2nd;

        TextBox m_txtLength;
        RadioButton m_DataIn;
        RadioButton m_DataOut;
        ComboBox m_cmdSelBox;

        Utility m_u = new Utility();
        public void init(List<TextBox> txtCdb, ComboBox cmdSelBox, TextBox txtDataLength, RadioButton dataIn, RadioButton dataOut, TextBox msg1st, TextBox msg2nd) {
            m_txtCdb = txtCdb;
            m_txtLength = txtDataLength;
            m_DataIn = dataIn;
            m_DataOut = dataOut;
            m_mainMsg = msg1st;
            m_msg2nd = msg2nd;
            m_cmdSelBox = cmdSelBox;
        }

        public void cmd_select_changebind_cmd_sel() {
            int idx = m_cmdSelBox.SelectedIndex;
            CdbCmd cmd = (CdbCmd)m_cmdSelBox.Items[idx];
            set_ui(cmd);
        }

        public void bind_cmd_sel(List<CdbCmd> cmdColls) {
            foreach (CdbCmd cmd in cmdColls) {
                m_cmdSelBox.Items.Add(cmd);
                set_ui(cmd);
            }

            if (m_cmdSelBox.Items.Count != 0) {
                m_cmdSelBox.SelectedIndex = 0;
            }
        }

        public void set_ui(CdbCmd cmd) {
            for (int i = 0; i < 16; i++) {
                m_txtCdb[i].Text = m_u.toHexString(cmd.cdb[i]);
            }

            if (cmd.direction == ScsiConst.DATA_IN) {
                m_DataIn.IsChecked = true;
                m_DataOut.IsChecked = false;
            } else {
                m_DataIn.IsChecked = false;
                m_DataOut.IsChecked = true;
            }
            m_txtLength.Text = cmd.length.ToString("X");
        }

        public void cmd_select_changed() {
            set_ui(m_cmdSelBox.SelectedItem as CdbCmd);
        }

        public CdbCmd get_cmd_from_ui(CdbCmd cmd) {
            for (int i = 0; i < 16; i++) {
                cmd.cdb[i] = m_u.to_byte(m_txtCdb[i].Text);
            }

            cmd.length = m_u.to_u32(m_txtLength.Text);

            if (m_DataIn.IsChecked == true) {
                cmd.direction = ScsiConst.DATA_IN;
            } else {
                cmd.direction = ScsiConst.DATA_OUT;
            }
            return cmd;
        }

        public void send_msg(string msg, bool isAppend = false) {
            if (isAppend == false) {
                m_mainMsg.Clear();
            }
            m_mainMsg.AppendText(msg);
        }

        public void send_2nd_msg(string msg, bool isAppend = false) {
            if (isAppend == false) {
                m_msg2nd.Clear();
            }
            m_msg2nd.AppendText(msg);
        }
    }
}
