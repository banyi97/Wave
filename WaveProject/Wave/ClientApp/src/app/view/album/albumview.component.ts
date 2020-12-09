import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Album } from '../../models/album';

@Component({
  selector: 'app-album-view',
  templateUrl: './albumview.component.html',
  styleUrls: ['./albumview.component.css']
})
export class AlbumViewComponent {
  constructor() {
  }
  @Input() index: number;
  @Input() album: Album;
  @Output() playThis = new EventEmitter<number>();

  public isHover: boolean = false;

  mouseenter() {
    this.isHover = true;
  }

  mouseleave() {
    this.isHover = false;
  }

  play() {
    this.playThis.emit(this.index);
  }
}
