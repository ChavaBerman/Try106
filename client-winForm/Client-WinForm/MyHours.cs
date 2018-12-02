using Client_WinForm.HelpModel;
using Client_WinForm.Requests;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Client_WinForm.Worker
{
    public partial class MyHours : Form
    {
        /// <summary>
        /// Viewing hours chart for each project which current worker included in
        /// </summary>
        /// <param name="worker"> current worker</param>
        public MyHours(Models.Worker worker)
        {

            InitializeComponent();
            Dictionary<string, Hours> workersDictionary = new Dictionary<string, Hours>();
            workersDictionary = TaskRequests.GetWorkerTasksDictionary(worker.WorkerId);
            List<decimal> givenList = workersDictionary.Values.Select(p => p.GivenHours).ToList();
            List<decimal> reservingList = workersDictionary.Values.Select(p => p.ReservingHours).ToList();
            chart1.Series[0].Points.DataBindXY(workersDictionary.Keys, givenList);
            chart1.Series[1].Points.DataBindXY(workersDictionary.Keys, reservingList);
        }
    }
}
