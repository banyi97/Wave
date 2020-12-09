import { Injectable } from "@angular/core"
import { HttpClient } from "@angular/common/http"
import { BehaviorSubject, Observable } from "rxjs"
import { Playlist } from "../models/playlist"
import { map } from "rxjs/operators"
import { PlaylistElement } from "../models/playlistelement"
import { moveItemInArray } from "@angular/cdk/drag-drop"
import { AuthService } from '../services/auth/auth.service' 
import { EndpointService } from './endpoints'
import { ReplacePlaylist } from "../models"

@Injectable({ providedIn: "root" })
export class PlaylistService {
  constructor(
    private auth: AuthService,
    private http: HttpClient,
    private ep: EndpointService
  ) {}

  private playlistSubject$: BehaviorSubject<Playlist[]> = new BehaviorSubject<Playlist[]>(new Array<Playlist>())
  public playlists$: Observable<Playlist[]> = this.playlistSubject$.asObservable()

  private selectedPlaylistSubject$: BehaviorSubject<Playlist|null> = new BehaviorSubject<Playlist|null>(null)
  public selPlaylist: Observable<Playlist|null> = this.selectedPlaylistSubject$.asObservable()
  public get selPlaylistVal () {return this.selectedPlaylistSubject$.value}
  public selectedPlaylist(id: string) {
    this.http.get<Playlist>(this.ep.playlist(id)).subscribe(data => {
      this.selectedPlaylistSubject$.next(data);
    }, error => {
      this.selectedPlaylistSubject$.next(null)
      console.log(error)
    })
  }

  public getPlaylists() {
    this.http.get<Playlist[]>(this.ep.playlistMe()).subscribe(data => {
      console.log(data)
      this.playlistSubject$.next(data);
    }, error => console.log(error))
  }

  public replacePlaylist(data: ReplacePlaylist, sendNot: boolean = false){
    if(data.from === data.to)
      return
    const list = this.playlistSubject$.value 
    if(list.length > data.to){
      if(sendNot){
        this.http.put(this.ep.playlistReplace(list[data.from].id, data.from, data.to), null).subscribe(data => console.log(data), error => console.log(error))
      }
      if(!(data.id && list[data.to].id === data.id)){
        moveItemInArray(list, data.from, data.to)
        this.playlistSubject$.next(list)
      }
    }
  }

  public addCreatedPlaylist(data: Playlist){
    const list = this.playlistSubject$.value
    list.push(data)
    this.playlistSubject$.next(list)
  }

  public renamePlaylist(data: Playlist){
    const list = this.playlistSubject$.value
    if(list.length > data.numberOf && list[data.numberOf].id == data.id){
      list[data.numberOf].title = data.title
      this.playlistSubject$.next(list)
      return
    }
    this.getPlaylists()
  }

  public removePlaylist(id: string){
    const list = this.playlistSubject$.value
    var index = 0
    const list2: Array<Playlist> = []
    list.forEach(element => {
      if(element.id != id){
        element.numberOf = index++
        list2.push(element)
      }
    })
    this.playlistSubject$.next(list2)
  }

  public makePublic(data: Playlist) {
    const list = this.playlistSubject$.value
    if(data.numberOf < list.length && list[data.numberOf].id == data.id){
      list[data.numberOf].isPublic = data.isPublic
      this.playlistSubject$.next(list)
    }
  }

  public addToPlaylist(psElement: PlaylistElement){
    const selfPs = this.selectedPlaylistSubject$.value
    if(selfPs && selfPs.id == psElement.playlistId){
      selfPs.playlistElements.push(psElement)
      this.selectedPlaylistSubject$.next(selfPs)
    }
  }

  public removeFromPlaylist({id, sId}){
    const selfPs = this.selectedPlaylistSubject$.value
    if(selfPs.id == id){
      const list: Array<PlaylistElement> = []
      selfPs.playlistElements.forEach(element => {
        if(element.id != sId){
          list.push(element)
        } 
      })
      selfPs.playlistElements = list
      this.selectedPlaylistSubject$.next(selfPs)
    }
  }

}
