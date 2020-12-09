import { Component, Input } from '@angular/core';
import { Playlist } from '../../models/playlist';

@Component({
  selector: 'app-playlist-view',
  templateUrl: './playlistview.component.html',
  styleUrls: ['./playlistview.component.css']
})
export class PlaylistViewComponent {
  constructor() {}
  @Input() playlist: Playlist;
  @Input() isSearch: boolean = true
}