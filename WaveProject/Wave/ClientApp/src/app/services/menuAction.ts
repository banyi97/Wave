import { NgbModal } from "@ng-bootstrap/ng-bootstrap"
import { Injectable } from "@angular/core"
import { MatDialog } from "@angular/material/dialog"
import { PlaylistDialog } from "../dialogs/playlistDialog/playlistDialog";
import { PlaylistService } from "./playlist";
import { HttpClient } from "@angular/common/http";
import { EndpointService } from "./endpoints";
import { AddToPlaylistDialog } from "../dialogs/addToPlaylistDialog/addToPlaylistDialog";

@Injectable({ providedIn: "root" })
export class MenuActionService{
  constructor(
    private modalService: NgbModal,
    private dialog: MatDialog,
    private ps: PlaylistService,
    private http: HttpClient,
    private ep: EndpointService
  ) { }

  public get SideMenuActions() {
    return [
      {
        html: (item) => `Rename playlist`,
        click: (item) => {
            const dialogRef = this.dialog.open(PlaylistDialog, {data: {isCreate: false, title: item.title}});
            dialogRef.afterClosed().subscribe(result => {
              if(result){
                this.http.patch(this.ep.playlistRename(item.id), {title: result}).subscribe(resp => {
                  console.log(resp)
                }, error => console.log(error))
              }
            });
        },
        enabled: (item) => true,
        visible: (item) => item,
      },
      {
        html: (item) => `Delete playlist`,
        click: (item) => {
          this.http.delete(this.ep.playlist(item.id)).subscribe(data => {console.log("removed"), error => console.log(error)})
        },
        enabled: (item) => true,
        visible: (item) => item,
      },
      {
        html: (item) => item.isPublic ? `Make private` : `Make public`,
        click: (item) => {
          console.log(!item.isPublic)
          this.http.patch(this.ep.playlistMakePublic(item.id), { isPublic: !item.isPublic }).subscribe(data => console.log(data), error => console.log(error))
        },
        enabled: (item) => true,
        visible: (item) => item,
      },
    ]
  }

  public get Actions(){
    return [
      // Song params
      {
        html: (item) => `Add to playlist`,
        click: (item) => {
          const dialogRef = this.dialog.open(AddToPlaylistDialog, {data: item});
        },
        enabled: (item) => true,
        visible: (item) => item.type == "Track",
      },
      // Album params
      {
        html: (item) => `Album`,
        click: (item) => alert('Or not...'),
        enabled: (item) => true,
        visible: (item) => item.type == "Album",
      },
      // Artist params
      {
        html: (item) => `Artist`,
        click: (item) => alert(item),
        enabled: (item) => true,
        visible: (item) => item.type == "Artist",
      },
      // Playlist params
      {
        html: (item) => `Something else`,
        click: (item) => alert('Or not...'),
        enabled: (item) => true,
        visible: (item) => item.type == "Playlist",
      },
      // PlaylistElement params
      {
        html: (item) => `Add to playlist`,
        click: (item) => {
          //const modalRef = this.modalService.open(AddToPlaylistComponent);
          //modalRef.componentInstance.track = item.track
        },
        enabled: (item) => true,
        visible: (item) => item.type == "PlaylistElement",
      },
      {
        html: (item) => `Remove from this playlist`,
        click: (item) => {
          this.http.delete(this.ep.playlistRemoveFromTrack(item.id)).subscribe(data => {console.log(data)}, error => console.log(error))
          //this.ps.removeFromPlaylist(item.id).subscribe(data => console.log(data), error => console.log(error))
        },
        enabled: (item) => true,
        visible: (item) => item.type == "PlaylistElement",
      },
    ]
  }
}