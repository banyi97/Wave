import { Component, Input, ViewChild } from '@angular/core';
import { Artist } from '../../models/artist';
import { ContextMenuComponent } from 'ngx-contextmenu';

@Component({
  selector: 'app-artist-view',
  templateUrl: './artistview.component.html',
  styleUrls: ['./artistview.component.css']
})
export class ArtistViewComponent {
  constructor() {console.log("artistview")}

  @ViewChild(ContextMenuComponent, { static: false }) public basicMenu: ContextMenuComponent;
  @Input() artist: Artist = null;
}