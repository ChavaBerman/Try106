using Client_WinForm.HelpModel;
using Client_WinForm.Models;
using Client_WinForm.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Client_WinForm.TeamHead
{
    public partial class HoursChart : Form
    {
        /// <summary>
        ///Allowence for teamHead- View for each worker of this teamHead in every project his reserving and given hours
        /// </summary>
        /// <param name="teamHead">current teamHead</param>
        public HoursChart(Models.Worker teamHead)
        {
            
            InitializeComponent();
            cmb_projects.DataSource = ProjectRequests.GetAllProjectsByTeamHead(teamHead.WorkerId);
            cmb_projects.DisplayMember = "ProjectName";

            
        }
        
        private void cmb_projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            //asignning data into series of charts.
           Dictionary<string, Hours> workersDictionary = new Dictionary<string, Hours>();
            workersDictionary = WorkerRequests.GetWorkersDictionary(((sender as ComboBox).SelectedItem as Project).ProjectId);
            List<decimal> givenList = workersDictionary.Values.Select(p => p.GivenHours).ToList();
            List<decimal> reservingList = workersDictionary.Values.Select(p => p.ReservingHours).ToList();
            chart1.Series[0].Points.DataBindXY(workersDictionary.Keys, givenList);
            chart1.Series[1].Points.DataBindXY(workersDictionary.Keys, reservingList);
        }
    }
}
