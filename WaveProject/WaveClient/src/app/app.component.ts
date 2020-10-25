import { Component, OnInit } from '@angular/core';
import { SignalRService } from './services/signalR'
import { PlaylistService } from './services/playlist';
import { PlayerService } from './services/player';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  constructor(
    public signalRService: SignalRService,
    private ps: PlaylistService,
    public player: PlayerService
    ){}
  
  ngOnInit(): void {
    this.ps.getPlaylists()
    //this.signalRService.startConnection();
    //this.signalRService.createPlaylistListener();
  }

}