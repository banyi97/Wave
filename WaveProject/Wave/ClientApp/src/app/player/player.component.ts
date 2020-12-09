import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { PlaylistService } from '../services/playlist';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../services/player'; 
import { fromEvent, Subscription } from 'rxjs';


@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class PlayerComponent implements OnInit, OnDestroy {
  constructor(
    private auth: AuthService,
    public player: PlayerService) { }
  
  private sub: Subscription
  ngOnDestroy(): void {
    if(this.sub){
      this.sub.unsubscribe()
    }
  }

  ngOnInit(): void {
    this.sub = this.clicksInDocument.subscribe(q => {
      if(this.isLocal){
        this.player.seek = this.locNum
        this.isLocal = false
      }
    })
  }
  private clicksInDocument = fromEvent(document, 'mouseup')
  public isLocal: boolean = false;
  public locNum: number = 0;
  inputListener(event){
    console.log("event")
    this.locNum = event.value
  }
  onDown(){
    this.isLocal = true;
    console.log("down")
  }

}