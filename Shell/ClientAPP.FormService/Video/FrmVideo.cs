using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientAPP.FormService.Video
{
    public partial class FrmVideo : Form
    {
        public FrmVideo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置视频面板
        /// </summary>
        /// <param name="panel"></param>
        public void SetVideoPanel(Control panel)
        {
            panel.Dock = DockStyle.Fill;
            this.Controls.Add(panel);
        }

        private void FrmVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
