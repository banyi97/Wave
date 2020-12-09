import { Component, OnInit, ViewChild } from '@angular/core';
import { MenuActionService } from '../services/menuAction';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Location } from '@angular/common'
import { EndpointService } from '../services/endpoints';
import { ContextMenuComponent } from 'ngx-contextmenu';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit{
  constructor(
    private menuActionService: MenuActionService,
    private ac: ActivatedRoute,
    private http: HttpClient,
    private ep: EndpointService,
    private location: Location
    ) { }

  @ViewChild("basicMenu", { static: false }) public basicMenu: ContextMenuComponent;
  public contextMenuActions

  ngOnInit(): void {
    this.contextMenuActions = this.menuActionService.Actions
    if (this.ac.snapshot.paramMap.get("id")) {
      this.box = this.ac.snapshot.paramMap.get("id");
    }
    if (this.ac.snapshot.paramMap.get("types")) {
      switch (this.ac.snapshot.paramMap.get("types")) {
        case "result": this.selected = 0; break;
        case "artists": this.selected = 1; break;
        case "albums": this.selected = 2; break;
        case "songs": this.selected = 3; break;
        case "playlists": this.selected = 4; break;
        default: this.selected = 0; break;
      }
    }
    if (this.box == "") {
    }
    else if (this.box.trim() == "") {
      this.isEmpty = true
    }
    else {
      this.isResult = true
      this.search(this.box)
    }
  }

  selected: number = 0
  searchTypes: Array<string> = ["result", "artists", "albums", "songs", "playlists"]
  box: string = ""
  isEmpty: boolean = false
  isResult: boolean = false
  searchRes: any = null
  isLoaded: boolean = false
  changeSelectedTab(n: number) {
    if(n >= this.searchTypes.length)
      return
    this.selected = n
    this.location.replaceState(`/search/${this.searchTypes[this.selected]}/${this.box}`)
    this.search(this.box)
  }

  onSearch(s: string) {
    if (s == "") {
      this.box = s
      this.isEmpty = false
      this.isResult = false
      this.location.replaceState("/search/resent");
    }
    else if (s.trim() == "") {
      this.box = s
      this.isEmpty = true
      this.isResult = false
      this.location.replaceState(`/search/${this.searchTypes[this.selected]}/${s}`);
    }
    else {
      this.box = s
      this.isEmpty = false
      this.isResult = true
      this.search(s)
      this.location.replaceState(`/search/${this.searchTypes[this.selected]}/${s}`);
    }
  }

  search(tag: string) {
    switch (this.selected) {
      case 0:
        this.sendSearch<any>(this.ep.searchTop(tag));
        break;
      case 1:
        this.sendSearch<any>(this.ep.searchArtist(tag));
        break;
      case 2:
        this.sendSearch<any>(this.ep.searchAlbum(tag));
        break;
      case 3:
        this.sendSearch<any>(this.ep.searchSong(tag));
        break;
      case 4:
        this.sendSearch<any>(this.ep.searchPlaylist(tag));
        break;
      default:
        this.searchRes = null;
        break;
    }
  }

  sendSearch<T>(url: string) {
    this.isLoaded = false
    this.http.get<T>(url).subscribe(
      data => {
        console.log(data)
        this.searchRes = data
        this.isLoaded = true
      },
      error => {
        console.log(error)
      })
  }

}