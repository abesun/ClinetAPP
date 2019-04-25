using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
namespace ClientAPP.FormService
{
    public partial class FrmMain : Form
    {

        private ServiceManager m_Manager;

        private ServiceLog LogModule;

        private FrmLog FrmLog;

        public FrmMain()
        {
            InitializeComponent();
            this.init();


        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //this.Hide();
        }

        private void init()
        {
            LogModule = new ServiceLog();
            FrmLog = new FrmLog();
            FrmLog.Show();
            FrmLog.Hide();


            this.m_Manager = new ServiceManager() { LogModule = LogModule };
            this.m_Manager.Init(FrmLog);
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        private void 显示日志窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            menu.Checked = !menu.Checked;
            FrmLog.Visible = menu.Checked;
           
        }
    }
}
