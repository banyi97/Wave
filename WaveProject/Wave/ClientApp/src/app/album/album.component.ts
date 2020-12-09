import { Component, OnInit, ViewChild } from '@angular/core';
import { Album } from '../models';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { EndpointService } from '../services/endpoints';
import { MenuActionService } from '../services/menuAction'
import { ContextMenuComponent } from 'ngx-contextmenu';
import { PlayerService } from '../services/player';

@Component({
  selector: 'app-album',
  templateUrl: './album.component.html',
  styleUrls: ['./album.component.css']
})
export class AlbumComponent implements OnInit {
  constructor(
    private menuActionService: MenuActionService,
    private http: HttpClient,
    private route: ActivatedRoute,
    private ep: EndpointService,
    private player: PlayerService
    ) { }
    
  ngOnInit(): void {
    this.contextMenuActions = this.menuActionService.Actions

    this.route.paramMap.subscribe(query => {
      let id = query.get("id")
      this.http.get<Album>(this.ep.albumUri(id)).subscribe(
        data => {
          this.album = data
        },
        error => {
          console.log(error)
        });
    })
  }

  play(index: number = 0){
    this.player.playTracks(this.album.tracks, index)
  }

  @ViewChild("albumMenu", { static: false }) public albumMenu: ContextMenuComponent;
  public contextMenuActions

  album: Album

}