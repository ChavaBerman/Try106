
import { Injectable } from '@angular/core';
import { Router, CanActivate} from '@angular/router';
import { WorkerService,Worker } from '../imports'
import { Global } from '../global';

@Injectable({ providedIn: 'root' })
export class AuthManger implements CanActivate {

    constructor(private router: Router,private workerService:WorkerService) { }

    canActivate() {

        let currentWorker:Worker= this.workerService.getCurrentWorker();
        if (currentWorker!=null && currentWorker.statusObj.statusName=="Manager") {
            return true;
        }
        this.router.navigate(['taskManagement/login']);
        Global.isLogedOut=true;
        return false;
    }
}