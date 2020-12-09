import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { PlaylistDialogModel, Track } from '../../models'
import { FormBuilder, FormGroup } from '@angular/forms';
import { PlaylistService } from '../../services/playlist'
import { HttpClient } from '@angular/common/http';
import { EndpointService } from 'src/app/services/endpoints';
import { UserElement } from 'src/app/admin/users/users.component';

@Component({
    selector: 'app-userDialog',
    templateUrl: 'userDialog.html',
    styleUrls: ['./userDialog.css']
})
export class UserDialog implements OnInit {
    constructor(
        public dialogRef: MatDialogRef<UserDialog>,
        @Inject(MAT_DIALOG_DATA) public data: UserElement,
        public ps: PlaylistService,
        private http: HttpClient,
        private ep: EndpointService
    ) {}

        public roles = []
        public userRoles = []
        public roleChanges = []
        
    ngOnInit(): void {
        console.log(this.data)
        this.http.get<any>(this.ep.roles()).subscribe(roles => {
            this.roles = roles
            console.log(roles)
        })
        this.http.get<any>(this.ep.userRole(this.data.userId)).subscribe(roles => {
            this.userRoles = roles
            console.log(roles)
        })
    }

    isElement(roleId: string) {
        let res = this.userRoles.find(role => role.id == roleId);
        return res == null ? false : true
    }

    roleChange(roleId){
        if(this.roleChanges.includes(roleId)){
            this.roleChanges = this.roleChanges.filter(id => id != roleId)
        }
        else{
            this.roleChanges.push(roleId)
        }
    }

    saveSettings(){
        let add = []
        let remove = []
        this.roleChanges.forEach(roleId => {         
            if(this.isElement(roleId)){
                remove.push(roleId)
            }
            else{
                add.push(roleId)
            }
            
        })
        this.http.post(this.ep.userRoleRemove(this.data.userId), {ids: remove}).subscribe(resp =>{

        })
        this.http.post(this.ep.userRole(this.data.userId), {ids : add}).subscribe(resp => {

        })
    }
}