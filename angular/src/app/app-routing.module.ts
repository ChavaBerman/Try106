import { Routes, RouterModule } from '@angular/router';

import {
    LoginComponent,
    ManagerComponent,
    AddWorkerComponent,
    ManageTeamComponent,
    AddProjectComponent,
    EditWorkerComponent,
    SetPermissionComponent,
    ManagerHomeComponent,
    TeamHeadComponent,
    TeamHeadHomeComponent,
    HoursChartComponent,
    ProjectsStateComponent,
    UpdateHoursComponent,
    WorkerComponent,
    WorkerHomeComponent,
    ApplyToManagerComponent,
    MyTasksComponent,
    MyHoursComponent,
    ForgotPasswordComponent,
    ReportComponent,
    AuthManger,
    AuthTeamHead,
    AuthWorker,

} from './shared/imports';
import { ChangePasswordComponent } from './components/change-password/change-password.component';

const appRoutes: Routes = [
    {

        path: 'taskManagement', children: [
            {
                path: 'login', component: LoginComponent
            },
            {
                path: 'forgot-password', component: ForgotPasswordComponent
            },
            {
                path: 'change-password/:requestId', component: ChangePasswordComponent
            },
            {
                path: 'manager', component: ManagerComponent, canActivate: [AuthManger], children: [
                    { path: 'manage-reports', component: ReportComponent, canActivate: [AuthManger] },
                    { path: '', component: ManagerHomeComponent, canActivate: [AuthManger] },
                    { path: 'manage-team', component: ManageTeamComponent, canActivate: [AuthManger] },
                    { path: 'manage-users/add-worker', component: AddWorkerComponent, canActivate: [AuthManger] },
                    { path: 'manage-users/set-permission', component: SetPermissionComponent, canActivate: [AuthManger] },
                    { path: 'manage-users/edit-worker', component: EditWorkerComponent, canActivate: [AuthManger] },
                    { path: 'add-project', component: AddProjectComponent, canActivate: [AuthManger] }
                ]
            },
            {
                path: 'teamHead', component: TeamHeadComponent, canActivate: [AuthTeamHead], children: [
                    { path: '', component: TeamHeadHomeComponent, canActivate: [AuthTeamHead] },
                    { path: 'hours-chart', component: HoursChartComponent, canActivate: [AuthTeamHead] },
                    { path: 'projects-state', component: ProjectsStateComponent, canActivate: [AuthTeamHead] },
                    { path: 'update-hours', component: UpdateHoursComponent, canActivate: [AuthTeamHead] }
                ]
            },
            {
                path: 'worker', component: WorkerComponent, canActivate: [AuthWorker], children: [
                    { path: '', component: WorkerHomeComponent, canActivate: [AuthWorker] },
                    { path: 'apply-to-manager', component: ApplyToManagerComponent, canActivate: [AuthWorker] },
                    { path: 'my-tasks', component: MyTasksComponent, canActivate: [AuthWorker] },
                    { path: 'my-hours', component: MyHoursComponent, canActivate: [AuthWorker] }
                ]
            }

        ]
    },
    { path: '', component: LoginComponent },
    // // otherwise redirect to LoginComponent
    { path: '**', component: LoginComponent }
];

export const AppRoutingModule = RouterModule.forRoot(appRoutes);