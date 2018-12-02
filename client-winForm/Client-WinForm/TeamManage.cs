using Client_WinForm.Requests;
using System;
using System.Windows.Forms;

namespace Client_WinForm.Manager
{
    public partial class TeamManage : Form
    {
      /// <summary>
     /// Change team header to workers
     /// </summary>
        public TeamManage()
        {
            InitializeComponent();

            cmb_managerName.DataSource = WorkerRequests.GetAllTeamHeads();
            cmb_managerName.DisplayMember = "WorkerName";

            cmb_workers.DataSource = WorkerRequests.GetWorkers();
            cmb_workers.DisplayMember = "WorkerName";
        }

        private void cmb_workers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Models.Worker currentWorker = ((sender as ComboBox).SelectedItem as Models.Worker);
            foreach (var cbi in cmb_managerName.Items)
            {
                if ((cbi as Models.Worker).WorkerId == currentWorker.ManagerId)
                {
                    cmb_managerName.SelectedItem = cbi;
                    break;
                }
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            Models.Worker worker = cmb_workers.SelectedItem as Models.Worker;
            worker.ManagerId = (cmb_managerName.SelectedItem as Models.Worker).WorkerId;
            if (WorkerRequests.UpdateWorker(worker))
            {
                MessageBox.Show("Succeeded!");
                Close();
            }
            else MessageBox.Show("Did not succeed...");
        }
    }
}
