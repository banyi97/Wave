import { Component, OnInit } from '@angular/core';
import { PlaylistService } from '../services/playlist';

@Component({
  selector: 'app-yourlib',
  templateUrl: './yourlib.component.html',
  styleUrls: ['./yourlib.component.css']
})
export class YourLibComponent implements OnInit {
  constructor(
    public ps: PlaylistService
  ){}
  ngOnInit(): void {
    this.ps.getPlaylists();
    this.ps.playlists$
  }

  public playlists = [];
}