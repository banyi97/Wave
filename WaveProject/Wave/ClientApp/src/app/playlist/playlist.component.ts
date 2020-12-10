import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { MenuActionService } from '../services/menuAction';
import { PlaylistService } from '../services/playlist';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Playlist, Track } from '../models';
import { ContextMenuComponent } from 'ngx-contextmenu';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { PlayerService } from '../services/player';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css']
})
export class PlaylistComponent implements OnInit, OnDestroy {
  constructor(
    private auth: AuthService,
    private menuActionService: MenuActionService,
    private player: PlayerService,
    public ps: PlaylistService,
    private ac: ActivatedRoute) { }

  sub: Subscription

  @ViewChild("playlistMenu", { static: false }) public playlistMenu: ContextMenuComponent;
  public contextMenuActions

  ngOnInit(): void {
    this.contextMenuActions = this.menuActionService.Actions
    this.ps.selPlaylist
    this.ac.paramMap.subscribe(query => {
      let id = query.get("id")
      this.ps.selectedPlaylist(id);  
    })
  }

  public play(index: number = 0) {
    const tracks: Array<Track> = []

    this.ps.selPlaylistVal.playlistElements.forEach(q => tracks.push(q.track))
    if (!tracks[index].uri) {
      for (let i = 0; i < tracks.length - 1; i++) {
        if (tracks[i].uri) {
          index = i;
          break;
        }
      }
    }
    this.player.playTracks(tracks, index)
  }

  drop(event: CdkDragDrop<string[]>) {
    this.ps.reorderPlaylist(event.previousIndex, event.currentIndex)
  }

  ngOnDestroy(): void {
    //this.sub.unsubscribe()
  }
}