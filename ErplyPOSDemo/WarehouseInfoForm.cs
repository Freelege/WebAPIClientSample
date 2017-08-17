using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ErplyPOSDemo
{
    public partial class WarehouseInfoForm : Form
    {
        private int desiredStartLocationX;
        private int desiredStartLocationY;
        private string addr;
        public WarehouseInfoForm(int x, int y, string addr)
        {
            InitializeComponent();
            this.desiredStartLocationX = x;
            this.desiredStartLocationY = y;
            this.addr = addr;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WarehouseInfoForm_Load(object sender, EventArgs e)
        {
            this.SetDesktopLocation(desiredStartLocationX, desiredStartLocationY);
            label_address.Text = "Address: " + addr;
            
            this.Width = label_address.Size.Width + 24;
            buttonOK.Anchor = AnchorStyles.Bottom;
            buttonOK.Dock = DockStyle.None;
           /* Point buttonLocation = new Point();
            buttonLocation.X = this.Location.X. + this.Width / 2 + buttonOK.Width / 2;
            buttonLocation.Y = this.Location.Y + this.Height - buttonOK.Height;
            buttonLocation = this.Location.//  + new Point(this.Width / 2 + buttonOK.Width / 2, this.Height - buttonOK.Height);
            buttonOK.Location = buttonLocation;*/
        }
    }
}
