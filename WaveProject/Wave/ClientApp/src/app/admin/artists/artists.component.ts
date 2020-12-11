import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EndpointService } from '../../services/endpoints'
import { MatDialog } from '@angular/material/dialog';
import { CreateArtistDialog } from '../../dialogs/createArtistDialog/createArtistDialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-artists',
  templateUrl: './artists.component.html',
  styleUrls: ['./artists.component.css']
})
export class AdminArtistsComponent {
  constructor(
    private http: HttpClient, 
    private ep: EndpointService, 
    private dialog: MatDialog,
    private router: Router) {
    this.http.get<any>(this.ep.artistUri()).subscribe(resp =>{
      console.log(resp)
      this.artists = resp
    }, err =>{
      alert("403")
      this.router.navigate(['/'])
    })
  }
  public artists: []

  createArtist(){
    const dialogRef = this.dialog.open(CreateArtistDialog);
    dialogRef.afterClosed().subscribe(id => {
      if(id){
        this.router.navigate([`/admin/artist/${id}`])
      }
    })
  }
}