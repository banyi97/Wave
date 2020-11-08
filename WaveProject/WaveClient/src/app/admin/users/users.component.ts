import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EndpointService } from '../../services/endpoints'
import { MatDialog } from '@angular/material/dialog';
import { CreateArtistDialog } from '../../dialogs/createArtistDialog/createArtistDialog';
import { Router } from '@angular/router';
import { UserDialog } from 'src/app/dialogs/userDialog/userDialog';

export interface UserElement {
  fullName: string;
  position: number;
  email: string;
  isBlocked: boolean;
  userId: string;
}

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class AdminUsersComponent {
  constructor(
    private http: HttpClient, 
    private ep: EndpointService, 
    private dialog: MatDialog,
    private router: Router) {
    this.http.get<any>(this.ep.users()).subscribe(resp =>{
      console.log(resp)
      this.users = resp
    })
  }
  public users = []


  displayedColumns: string[] = ['position', 'email', 'fullName', 'isBlocked', 'action'];

  visitUser(user: UserElement){
    const dialogRef = this.dialog.open(UserDialog, {
      data: user
    });
    dialogRef.afterClosed().subscribe(id => {
      if(id){
        this.router.navigate([`/admin/artist/${id}`])
      }
    })
  }

}