import { Injectable } from "@angular/core";
import { Howl, Howler } from 'howler';
import { Subject, BehaviorSubject, Observable, from } from "rxjs";
import { Track } from "../models/track";
import { EndpointService } from "./endpoints";
import { HttpClient } from "@angular/common/http";

export interface SoundProgressInterface {
    played: number;    
    remaining: number;
    position: number;
}

export interface SoundInterface {
    sourceTrack: Track;
    howl: Howl;
}

@Injectable({ providedIn: "root" })
export class PlayerService {
  constructor(
    private ep: EndpointService,
    private http: HttpClient)
    {
        let vol = localStorage.getItem('volume');
        if (!vol) {
        Howler.volume(0.5);
        localStorage.setItem('volume', '0.5')
        }
        else {
        Howler.volume(new Number(vol));
        }
        Howler.usingWebAudio = true;
        Howler.autoUnlock = false;
        this.progressSubject$.next({ played: 0, remaining: 0, position: 0 } as SoundProgressInterface)
    }

    private actualSubject$: BehaviorSubject<SoundInterface|null> = new BehaviorSubject<SoundInterface|null>(null)
    public actual$: Observable<SoundInterface|null> = this.actualSubject$.asObservable()

    private progressSubject$: BehaviorSubject<SoundProgressInterface|null> = new BehaviorSubject<SoundProgressInterface|null>(null)
    public progress$: Observable<SoundProgressInterface|null> = this.progressSubject$.asObservable()

    private tracksSubject$: BehaviorSubject<Array<SoundInterface>|null> = new BehaviorSubject<Array<SoundInterface>|null>(null)
    public tracks$: Observable<Array<SoundInterface>|null> = this.tracksSubject$.asObservable()

    private volumeLevel: number|null = 0.5

    public mute() {
        if (this.volume == 0) {
            this.volume = this.volumeLevel;
            this.volumeLevel = null;
          }
          else {
            this.volumeLevel = this.volume;
            this.volume = 0;
          }
    }

    public get volume() {
        return Howler.volume();
    }
    public set volume(num: number) {
        Howler.volume(num);
        let str: string = num.toString();
        localStorage.setItem('volume', str);
    }

    public get isMuted() {
        return this.volume > 0 ? true : false;
      }
    
    public get volumeIsHigher50Proc() {
        return this.volume > 0.5 ? true : false;
    }

    public get isPlayed() {
        if (this.actualSubject$.value.howl) {
          return this.actualSubject$.value.howl.playing()
        }
    }

    public play() {
        const sound = this.actualSubject$.value.howl;
        if (sound && !sound.playing()) {
            console.log("play")
            sound.fade(0, 1, 500);
            sound.play();
        }
    }
    
    public pause() {
        const sound = this.actualSubject$.value.howl;
        if (sound) {
          sound.fade(1, 0, 500);
          sound.pause();
        }
    }

    private stop(){
        if(this.index != null){
            const sound = this.actualSubject$.value;
            if (sound && sound.howl) {
                console.log("stopped")
                sound.howl.fade(1, 0, 500);
                sound.howl.stop();
            }
        }
    }

    public set seek(num) {
        const sound = this.actualSubject$.value.howl;
        if (sound) 
            sound.seek(num)    
    }
    
    public get seek() {
        const sound = this.actualSubject$.value.howl;
        if (sound)
            return sound.seek()
    }

    private index: number = 0
    public playTracks(tracks: Array<Track>, index: number|null = null){
        if(tracks.length == 0)
            return
        if(index == null || tracks.length <= index || index < 0 || !tracks[index].uri){
            index = null
            for(let i = 0; i < tracks.length; i++){
                if(tracks[i].uri){
                    index = i
                    break
                }
            }
            if(index == null)
                return
        }
        const trackObj: Array<SoundInterface> = tracks.map(track =>{
            return { howl: null, sourceTrack: track } as SoundInterface
        })
        this.tracksSubject$.next(trackObj)
        this.playSelected(index)
    }

    private playSelected(index: number){
        this.index = index
        const item = this.tracksSubject$.value[index]
        if(!item.howl){
            item.howl = this.createHowler(item.sourceTrack.uri)
        }
        this.stop()
        this.actualSubject$.next(item)
        this.play()
    }

    public back(){
        const maxRestartSec = 3
        if(this.seek < maxRestartSec){
            const prev = this.index
            const list = this.tracksSubject$.value
            let index = prev
            for (let i = prev - 1; i >= 0; i--) {
                if (list[i].sourceTrack.uri) {
                index = i;
                break;
                }
            }
        this.playSelected(index)
        }
        else{
            this.seek = 0
        }
    }

    public next(){
        const prev = this.index
        const list = this.tracksSubject$.value
        let index = prev
        do{
            index = index + 1
            if(index >= list.length){
                index = 0
            }
        }while(!list[index].sourceTrack.uri || index == prev)
        this.playSelected(index)
    }

    private createHowler(uri: string, autoplay = false) : Howler{
        return new Howl({
            src: uri,
            autoplay: autoplay,
            preload: true,
            html5: true,
            onplay: () => {
                requestAnimationFrame(this.seekStep);
            },
            onseek: () => {
                requestAnimationFrame(this.seekStep);
            },
            onend: () => {
                this.next()
            },
            xhr: {
                method: 'GET',
                headers: {
                  Authorization: 'Bearer:' //+ token,
                },
                withCredentials: true,
              }
        });
    }

    private seekStep = () => {
        const sound = this.actualSubject$.value.howl;
        if (sound.playing()) {
          let sSeek = sound.seek(), sDuration = sound.duration();
          let progress: SoundProgressInterface = {
            played: sSeek,
            remaining: sDuration - sSeek,
            position: Math.round((sSeek * 100) / sDuration)
          }
          this.progressSubject$.next(progress)
    
          setTimeout(() => requestAnimationFrame(this.seekStep), 1000);
        }
      }
}