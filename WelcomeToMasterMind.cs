using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterMind56955_JRN
{
    public partial class WelcomeToMasterMind : Form
    {
        public WelcomeToMasterMind()
        {
            InitializeComponent();
        }

        private void WelcomeToMasterMind_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void buttonNewClient_Click(object sender, EventArgs e)
        {
            MasterMindClient newClient = new MasterMindClient();
            newClient.Show();
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MasterMindHelp newHelp = new MasterMindHelp();
            newHelp.Show();
        }
    }
}
