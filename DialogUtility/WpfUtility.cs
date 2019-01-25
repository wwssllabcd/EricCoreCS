

using System;
namespace EricCore.Utilitys {
    public class WpfUtility {
        public string get_file_path(string filter = "Any File|*.*", string defaultExt = "", string defFileName = "") {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;
            dlg.FileName = defFileName;

            Nullable<bool> result = dlg.ShowDialog();
            string res = "";
            if (result == true) {
                 res = dlg.FileName;
            }
            return res;
        }

    }
}
