using System.Windows.Forms;
using Client_WinForm.TeamHead;

namespace Client_WinForm
{
    public partial class Welcome : Form
    {
        /// <summary>
        /// Try to login by ip address
        /// </summary>
        public Welcome()
        {
            IsMdiContainer = true;
            Models.Worker worker = Requests.WorkerRequests.LoginByComputerWorker();
            if (worker != null)
            {
                switch (worker.statusObj.StatusName)
                {
                    case "Manager":
                        Manager.ManagerMainScreen managerMainScreen = new Manager.ManagerMainScreen(worker);

                        managerMainScreen.Show();
                        break;
                    case "TeamHead":
                        TeamHeadScreen TeamHeadScreen = new TeamHeadScreen(worker);

                        TeamHeadScreen.Show();
                        break;

                    default:
                        WorkerScreen workerScreen = new WorkerScreen(worker);

                        workerScreen.Show();
                        break;

                }
            }
            else
            {
                ManagementTaskLogin managementTaskLogin = new ManagementTaskLogin();
                managementTaskLogin.MdiParent = this;
                managementTaskLogin.Show();

            }


            InitializeComponent();
        }

    }
}
