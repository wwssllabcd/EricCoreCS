

using System.Collections.Generic;
using System.Windows.Controls;

using EricCore.Utilitys;
using EricCore.Scsi;
using System.Windows;

namespace EricCore.UsbCmder {
    public class UsbCmderView {
        public List<TextBox> m_txtCdb = new List<TextBox>(16);
        public RichTextBox m_mainMsg;
        public TextBox m_msg2nd;

        TextBox m_txtLength;
        RadioButton m_DataIn;
        RadioButton m_DataOut;
        ComboBox m_cmdBox;

        Utility m_u = new Utility();
        public void bind_ui_component(List<TextBox> txtCdb, TextBox txtLength, RadioButton dataIn, RadioButton dataOut, RichTextBox mainMsg, TextBox msg2nd, ComboBox cmdBox) {
            m_txtCdb = txtCdb;
            m_txtLength = txtLength;
            m_DataIn = dataIn;
            m_DataOut = dataOut;
            m_mainMsg = mainMsg;
            m_msg2nd = msg2nd;
            m_cmdBox = cmdBox;
        }

        public void init(List<CdbCmd> cmdColls) {
            foreach (CdbCmd cmd in cmdColls) {
                m_cmdBox.Items.Add(cmd);
                set_ui(cmd);
            }

            if (m_cmdBox.Items.Count != 0) {
                m_cmdBox.SelectedIndex = 0;
            }
        }

        public void set_ui(CdbCmd cmd) {
            for (int i = 0; i < 16; i++) {
                m_txtCdb[i].Text = m_u.toHexString(cmd.cdb[i]);
            }

            if (cmd.direction == CdbCmd.DATA_IN) {
                m_DataIn.IsChecked = true;
                m_DataOut.IsChecked = false;
            } else {
                m_DataIn.IsChecked = false;
                m_DataOut.IsChecked = true;
            }

            m_txtLength.Text = m_u.toHexString(cmd.length);

        }

        public void cmd_select_changed() {
            set_ui(m_cmdBox.SelectedItem as CdbCmd);
        }

        public CdbCmd get_cmd_from_ui(CdbCmd cmd) {
            for (int i = 0; i < 16; i++) {
                cmd.cdb[i] = m_u.to_byte(m_txtCdb[i].Text);
            }

            cmd.length = m_u.to_u32(m_txtLength.Text);

            if (m_DataIn.IsChecked == true) {
                cmd.direction = CdbCmd.DATA_IN;
            } else {
                cmd.direction = CdbCmd.DATA_OUT;
            }
            return cmd;
        }

        public void send_msg(string msg, bool isAppend = false) {
            if (isAppend == false) {
                m_mainMsg.Document.Blocks.Clear();
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
