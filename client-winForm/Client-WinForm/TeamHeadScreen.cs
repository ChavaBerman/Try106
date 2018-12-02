using System;
using System.Windows.Forms;

namespace Client_WinForm.TeamHead
{
    /// <summary>
    /// Allowence for TeamHead only-TeamHead main screen, navigating between all his windows
    /// </summary>
    public partial class TeamHeadScreen : Form
    {
        Models.Worker Teamhead;
        public TeamHeadScreen(Models.Worker Teamhead)
        {
            this.Teamhead = Teamhead;
            InitializeComponent();
            IsMdiContainer = true;
        }

        private void hoursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HoursChart hoursChart = new HoursChart(Teamhead);
            hoursChart.MdiParent = this;
            hoursChart.Show();
        }

        private void viewMyProjectsStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectState projectState = new ProjectState(Teamhead);
            projectState.MdiParent = this;
            projectState.Show();
        }

        private void updateHoursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHours updateHours = new UpdateHours(Teamhead);
            updateHours.MdiParent = this;
            updateHours.Show();
        }

        private void logout_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            
                ManagementTaskLogin managementTaskLogin = new ManagementTaskLogin();
                managementTaskLogin.Show();
                Close();

            
        }
    }
}
