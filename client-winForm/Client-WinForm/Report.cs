using Client_WinForm.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace Client_WinForm.Manager
{/// <summary>
/// Report for manager- view all the project and workers information and enable filtering according to chosen params
/// </summary>
    public partial class Report : Form
    {
        bool isStarted = false;
        List<ReportData> reportDataList = new List<ReportData>();
        //Intalize report for manager with reqired data from server and fill it.
        public Report()
        {
            InitializeComponent();
            reportDataList = Requests.ReportsRequests.CreateReport();
            grid_data_report.Relations.AddSelfReference(grid_data_report.MasterTemplate, "Id", "ParentId");
            grid_data_report.DataSource = reportDataList;
            grid_data_report.Columns["Id"].IsVisible = false;
            grid_data_report.Columns["ParentId"].IsVisible = false;
            cmb_projects.DataSource = Requests.ProjectRequests.GetAllProjects();
            cmb_projects.DisplayMember = "ProjectName";
            cmb_projects.SelectedIndex = -1;
            cmb_teamHeads.DataSource = Requests.WorkerRequests.GetAllTeamHeads();
            cmb_teamHeads.DisplayMember = "WorkerName";
            cmb_teamHeads.SelectedIndex = -1;
            cmb_workers.DataSource = Requests.WorkerRequests.GetWorkers();
            cmb_workers.DisplayMember = "WorkerName";
            cmb_workers.SelectedIndex = -1;
            isStarted = true;
        }

        private void cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isStarted)
            {  grid_data_report.DataSource = FilterData();
                grid_data_report.Columns["Id"].IsVisible = false;
                grid_data_report.Columns["ParentId"].IsVisible = false;
            }
        }

        //refresh the report by raise the event of selectedIndexChange of parameters to filter which bring the data back again.
        private void btn_refresh_Click(object sender, EventArgs e)
        {
            cmb_projects.SelectedIndex = -1;
            cmb_teamHeads.SelectedIndex = -1;
            cmb_workers.SelectedIndex = -1;
            cmb_month.SelectedIndex = -1;
        }

        private void RunExportToExcelML(string folderPath, bool openExportFile)
        {

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                folderPath = folderBrowserDialog1.SelectedPath;
            //Creating DataTable
            System.Data.DataTable dt = new System.Data.DataTable();
  dt.Columns.Add("Project/Worker", typeof(string));
            //Adding the Columns
            foreach (var column in grid_data_report.Columns)
            {
              
                if (column.HeaderText != "ParentId" && column.HeaderText != "Id")
                    dt.Columns.Add(column.HeaderText, typeof(string));
            }

            //Adding the Rows
            foreach (var row in grid_data_report.Rows)
            {
                dt.Rows.Add();
                dt.Rows[dt.Rows.Count - 1][0] = row.Cells[1].Value.ToString() =="0"?  "Project": "Worker";
                for (int i = 2; i < row.Cells.Count; i++)
                {
                    dt.Rows[dt.Rows.Count - 1][i-1] = row.Cells[i].Value != null ? row.Cells[i].Value.ToString() : "";
                }
            }

            //Exporting to Excel
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "data");
                wb.SaveAs(folderPath + @"\ReportData.xlsx");
            }
        }

        private void btn_exportToExcel_Click(object sender, EventArgs e)
        {
            RunExportToExcelML("C:\\Excel\\", true);
        }

        /// <summary>
        /// filtering the report according to one or more parameters were chosen by the user
        /// return list of filtered data
        /// </summary>
        /// <returns></returns>
        private List<ReportData> FilterData()
        {
            int requiredMonth = cmb_month.SelectedIndex + 1;
            string projectName = cmb_projects.SelectedIndex > -1 ? (cmb_projects.SelectedItem as Project).ProjectName : "ok";
            string workerName = cmb_workers.SelectedIndex > -1 ? (cmb_workers.SelectedItem as Models.Worker).WorkerName : "ok";
            string teamHeadName = cmb_teamHeads.SelectedIndex > -1 ? (cmb_teamHeads.SelectedItem as Models.Worker).WorkerName : "ok";

            
                int id = 1;
                List<ReportData> listToFilter = new List<ReportData>();
                List<ReportData> projects = new List<ReportData>();
                projects = reportDataList.Where(p => p.ParentId == 0).ToList();
                foreach (ReportData project in projects)
                {
                    int parentId = id++;
                string teamHead = project.TeamHeader;
                    if (projectName == "ok" || project.Name == projectName)
                        if (requiredMonth == 0 || ((DateTime)project.DateBegin).Month <= requiredMonth && ((DateTime)project.DateEnd).Month >= requiredMonth)
                            if (teamHeadName == "ok" || teamHeadName == teamHead)
                            {
                                decimal givenHoursOfProject = 0;
                                List<ReportData> workersOfProject = new List<ReportData>();
                            workersOfProject = reportDataList.Where(p => p.ParentId == project.Id).ToList();
                                foreach (ReportData worker in workersOfProject)
                                {
                                   
                                    givenHoursOfProject += project.GivenHours;
                                    if (workerName == "ok" || worker.Name == workerName)
                                    {
                                        ReportData reportData = new ReportData
                                        {
                                            Id = id++,
                                            ParentId = parentId,
                                            Name = worker.Name,
                                            TeamHeader = worker.TeamHeader,
                                            ReservingHours = worker.ReservingHours,
                                            GivenHours = worker.GivenHours,
                                            DateBegin = null,
                                            DateEnd = null
                                        };
                                    listToFilter.Add(reportData);
                                    }
                                }
                            listToFilter.Add(new ReportData
                                {
                                    Id = parentId,
                                    ParentId = 0,
                                    Name = project.Name,
                                    TeamHeader = project.TeamHeader,
                                    ReservingHours = project.ReservingHours,
                                    GivenHours = givenHoursOfProject,
                                    Customer = project.Customer,
                                    DateBegin = project.DateBegin,
                                    DateEnd = project.DateEnd,
                                    Days =((TimeSpan)(project.DateEnd - project.DateBegin)).Days,
                                    WorkedDays =((DateTime.Now > project.DateEnd)) ?((TimeSpan) (project.DateEnd - project.DateBegin)).Days : ((TimeSpan)(DateTime.Now - project.DateBegin)).Days
                                });
                            }
                }
                return listToFilter;
            
        }
    }

}