import { Component, OnInit } from '@angular/core';
import { TreeNode } from 'primeng/components/common/treenode';
import { ReportData, ReportService, Project, WorkerService, ProjectService } from 'src/app/shared/imports';
import { FormControl, FormGroup } from '@angular/forms';
import { wrapListenerWithPreventDefault } from '@angular/core/src/render3/instructions';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent implements OnInit {

  reportDataTreenode: TreeNode[] = [];
  filterReportData: TreeNode[] = [];
  cols: any[];
  reportData: Array<ReportData>;
  filterArrayData: Array<ReportData>=[];
  children: TreeNode[] = [];
  workersArray: Array<Worker>;
  teamHeadsArray: Array<Worker>;
  projectsArray: Array<Project>;
  monthArray = ["January",
    "Fabruary",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December"];
  formGroup: FormGroup;

  constructor(private reportService: ReportService, private workerService: WorkerService, private projectService: ProjectService) { }

  ngOnInit() {
    let formGroupConfig = {
      worker: new FormControl(null),
      project: new FormControl(null),
      teamHead: new FormControl(null),
      month: new FormControl(null)
    };
    this.formGroup = new FormGroup(formGroupConfig);

    this.workerService.getAllWorkers().subscribe(res => {
      this.workersArray = res;
    });
    this.workerService.getAllTeamHeads().subscribe(res => {
      this.teamHeadsArray = res;
    });
    this.projectService.getAllProjects().subscribe((res) => {
      this.projectsArray = res;
    });
    this.reportService.createProjectReport().subscribe(data => {
      this.reportData = data;
      this.filterArrayData=data;
      this.reportData.forEach(element => {
        if (element.parentId == 0) {
          this.children = [];
          this.reportData.forEach(child => {
            if (child.parentId == element.id)
              this.children.push(this.childToTreeNode(child));
          });
          this.reportDataTreenode.push(this.parentToTreeNode(element, this.children));
        }
      });
      this.filterReportData = this.reportDataTreenode;
      console.log(this.reportDataTreenode);

      this.cols = [
        { field: 'name', header: 'Name' },
        { field: 'teamHeader', header: 'Team Header' },
        { field: 'reservingHours', header: 'Reserving Hours' },
        { field: 'givenHours', header: 'Given Hours' },
        { field: 'customer', header: 'Customer' },
        { field: 'dateBegin', header: 'Date Begin' },
        { field: 'dateEnd', header: 'Date End' },
        { field: 'days', header: 'Days' },
        { field: 'workedDays', header: 'Worked Days' },
      ];
    });

  }

  private childToTreeNode(child: ReportData): TreeNode {
    return {
      data: child
    }
  }
  private parentToTreeNode(parent: ReportData, children: TreeNode[]): TreeNode {
    return {
      data: parent,
      children: children
    }
  }

  filterArray() {
    this.filterReportData = [];
    let workerName = this.formGroup.controls["worker"].value != null ? this.formGroup.controls["worker"].value : "ok";
    let teamHeadName = this.formGroup.controls["teamHead"].value != null ? this.formGroup.controls["teamHead"].value : "ok";
    let month = this.formGroup.controls["month"].value != null ? this.formGroup.controls["month"].value : 0;
    let projectName = this.formGroup.controls["project"].value != null ? this.formGroup.controls["project"].value : "ok";

    this.reportDataTreenode.forEach(element => {
      let dateBegin = new Date(element.data.dateBegin);
      let dateEnd = new Date(element.data.dateEnd);
      if ((projectName != "ok" && element.data.name == projectName || projectName == "ok") &&
        (teamHeadName != "ok" && element.data.teamHead == teamHeadName || teamHeadName == "ok") &&
        (dateBegin.getMonth()+1 <= month && dateEnd.getMonth()+1 >= month || month == 0)) {
        this.children = [];
        element.children.forEach(child => {
          if (child.data.name == workerName || workerName == "ok")
            this.children.push(child);
            this.filterArrayData.push(child.data);
        });
        if (this.children.length > 0)
         {
            this.filterReportData.push(this.parentToTreeNode(element.data, this.children));
            this.filterArrayData.push(element.data);
          }
      }
    });
  }

  refresh(){
    this.formGroup.reset();
    this.filterArray();
  }
  exportToExcel(){
this.reportService.exportAsExcelFile(this.filterArrayData,"Report" );
  }
}
