using ULONG = System.UInt32;
using BYTE = System.Byte;
using u8 = System.Byte;

using System.Windows.Forms;

namespace EricCore
{
    public class WfUtility
    {
        public void up_down_item(ListBox lb, int select, bool isUpItem)
        {
            int selSwitch;
            int insertPtr;
            if (isUpItem)
            {
                selSwitch = select - 1;
                insertPtr = select + 1;
            }
            else
            {
                selSwitch = select + 1;
                insertPtr = select + 2;
            }

            var selString = lb.Items[select];
            var switchString = lb.Items[selSwitch];

            if (isUpItem)
            {
                lb.Items.Insert(insertPtr, selString);
                lb.Items.Insert(insertPtr + 1, switchString);
            }
            else
            {
                lb.Items.Insert(insertPtr, switchString);
                lb.Items.Insert(insertPtr + 1, selString);
            }

            lb.Items.RemoveAt(select);
            lb.Items.RemoveAt(selSwitch);
            lb.SelectedIndex = selSwitch;
        }
    }
}
