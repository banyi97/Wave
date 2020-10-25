import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Track } from '../../models/track';

@Component({
  selector: 'app-track-view',
  templateUrl: './trackview.component.html',
  styleUrls: ['./trackview.component.css']
})
export class TrackViewComponent {
  constructor() {}
  @Input() isArtistTop: boolean = false
  @Input() track: Track
  @Input() index: number
  @Output() playThis = new EventEmitter<number>();

  play() {
    this.playThis.emit(this.index);
  }
}