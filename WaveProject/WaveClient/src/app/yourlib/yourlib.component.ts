import { Component } from '@angular/core';
import { PlaylistService } from '../services/playlist';

@Component({
  selector: 'app-yourlib',
  templateUrl: './yourlib.component.html',
  styleUrls: ['./yourlib.component.css']
})
export class YourLibComponent {
  constructor(
    public ps: PlaylistService
  ){}

  asd(){
    this.ps.getPlaylists();
  }
}