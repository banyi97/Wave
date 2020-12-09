import { Component, ViewChild } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { ContextMenuComponent } from 'ngx-contextmenu';
import { MenuActionService } from '../services/menuAction';
import { Playlist } from '../models';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { MatDialog } from '@angular/material/dialog';
import { PlaylistDialog } from '../dialogs/playlistDialog/playlistDialog';
import { PlaylistService } from '../services/playlist'
import { HttpClient } from '@angular/common/http';
import { EndpointService } from '../services/endpoints';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  constructor(
    public auth: AuthService,
    private menuActionService: MenuActionService,
    public dialog: MatDialog,
    public ps: PlaylistService,
    private http: HttpClient,
    private ep: EndpointService
    ){
    this.contextMenuActions = this.menuActionService.SideMenuActions
   }

  @ViewChild("sidebarMenu", { static: false }) public sidebarMenu: ContextMenuComponent;
  public contextMenuActions

  isExpanded = false;

  drop(event: CdkDragDrop<string[]>) {
    this.ps.replacePlaylist({id: null, from: event.previousIndex, to: event.currentIndex}, true)
  }
  createNewPlaylist() {
    if(this.auth.loggedIn){
      const dialogRef = this.dialog.open(PlaylistDialog, {data: {isCreate: true, title: ""}});
      dialogRef.afterClosed().subscribe(result => {
        if(result){
          console.log(result)
          this.http.post(this.ep.playlistCreate(), {title: result}).subscribe(resp => {
            console.log(resp)
          }, error => console.log(error))
        }
      });
    }
    else{

    }
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}