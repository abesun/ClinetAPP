using ClientAPP.FormService.Log;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientAPP.FormService
{
    public partial class FrmLog : Form
    {
        public FrmLog()
        {
            InitializeComponent();
        }

        private void FrmLog_Load(object sender, EventArgs e)
        {

            log4net.Repository.Hierarchy.Hierarchy hierarchy = LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
            if (hierarchy != null)
                hierarchy.Root.AddAppender(new RichEditAppender(this.richTextBox_Log, 1000, 50, 5));
        }

        private void FrmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
