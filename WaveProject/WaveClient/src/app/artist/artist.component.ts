import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

import { EndpointService } from '../services/endpoints'
import { Artist, Album, Track, AlbumType } from '../models'
import { ContextMenuComponent } from 'ngx-contextmenu';
import { MenuActionService } from '../services/menuAction';
import { PlayerService } from '../services/player';
@Component({
  selector: 'app-artist',
  templateUrl: './artist.component.html',
  styleUrls: ['./artist.component.css']
})
export class ArtistComponent implements OnInit {
  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private ep: EndpointService,
    private menuActionService: MenuActionService,
    private player: PlayerService
    ) { }

  public artist: Artist = null
  public topTracks: Array<Track> = [];
  public albums: Array<Album> = [];
  public singles: Array<Album> = [];

  @ViewChild("topTrackMenu", { static: false }) public topTrackMenu: ContextMenuComponent;
  public contextMenuActions

  ngOnInit(): void {
    this.contextMenuActions = this.menuActionService.Actions
    
    this.route.paramMap.subscribe(query => {
      let id = query.get("id")
      if(!id)
        return;
      this.http.get<Artist>(this.ep.artistUri(id)).subscribe(
        data => {
          this.artist = data
          console.log(data)
        },
        error => {
          console.log(error)
        });
      this.http.get<Array<Track>>(this.ep.artistTop(id)).subscribe(
        data => {
          this.topTracks = data
          console.log(data)
        },
        error => {
          console.log(error)
        })
      this.http.get<Array<Album>>(this.ep.artistAlbums(id)).subscribe(
        data => {
          console.log("albums")
          console.log(data)
          this.albums = data.filter(q => q.albumType == AlbumType.Album);
          this.singles = data.filter(q => q.albumType == AlbumType.Single);
        },
        error => {
          console.log(error)
        })
    })
  }

  public playFromTops(index: number){
      const tracks: Array<Track> = [];
      this.topTracks.forEach(q => tracks.push(q))
      this.albums.forEach(q => q.tracks.forEach(w => {
        tracks.push(w)
      }))
      this.singles.forEach(q => q.tracks.forEach(w => {
        tracks.push(w)
      }))
      this.player.playTracks(tracks, index)
  }

  public playFromAlbum(index: number){
      const tracks: Array<Track> = [];
      let realIndex = this.topTracks.length
      this.topTracks.forEach(q => tracks.push(q))
      let isOk: boolean = false
      this.albums.forEach(q => {
        if(q.id == this.albums[index].id){
          isOk = true
        }
        if(!isOk){
          realIndex += q.tracks.length
        }
        q.tracks.forEach(w => {
          tracks.push(w)
        })
      })
      this.singles.forEach(q => q.tracks.forEach(w => {
        tracks.push(w)    
      }))
      this.albums.forEach(q => q.tracks.forEach(w => {
       
      }))
      this.player.playTracks(tracks, realIndex)
  }

  public playFromSingles(index: number){
      const tracks: Array<Track> = [];
      let realIndex = this.topTracks.length
      let isOk: boolean = false
      this.topTracks.forEach(q => tracks.push(q))
      this.albums.forEach(q => {
        realIndex += q.tracks.length
        q.tracks.forEach(w => {
        tracks.push(w)
      })})
      this.singles.forEach(q => {
        if(q.id == this.singles[index].id){
          isOk = true
        }
        if(!isOk){
          realIndex += q.tracks.length
        }
        q.tracks.forEach(w => {
          tracks.push(w)
      })})
      this.player.playTracks(tracks, realIndex)
  }

}