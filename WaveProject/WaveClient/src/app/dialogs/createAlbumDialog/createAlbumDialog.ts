import { Component, OnInit, Inject, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EndpointService } from '../../services/endpoints'
import { FormBuilder, FormGroup, Validators, FormArray, FormControl } from '@angular/forms';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Observable, BehaviorSubject, from, async } from 'rxjs';
import { map, mapTo } from 'rxjs/operators';
import { Howl, Howler } from 'howler';
import { StepperSelectionEvent } from '@angular/cdk/stepper';

export interface TrackElement {
  title: string;
  isExplicit: number;
  duration: number;
}

@Component({
  selector: 'app-createAlbumDialog',
  templateUrl: './createAlbumDialog.html',
  styleUrls: ['./createAlbumDialog.css']
})
export class CreateAlbumDialog implements OnInit {
  constructor(
    private http: HttpClient, 
    private ep: EndpointService,
    public dialogRef: MatDialogRef<CreateAlbumDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder
    ) {}
    
    ngOnInit(): void {
      this.albumForm = this.fb.group({
        label: ["", [Validators.required]],
        albumType: [null, Validators.required],
        releaseDate: [null, [Validators.required]],
        releaseDatePrecision: ["0", [Validators.required]],
        availableFrom: [null, [Validators.required]],
        file: [null, Validators.required]
      });
      this.trackForm = this.fb.group({
        tracks: this.fb.array([
          this.fb.group({
            title: ['', [Validators.required]],
            isExplicit: [false],
            file: [null, Validators.required]
          })
        ])
      })
    }

    private trackSubject$: BehaviorSubject<TrackElement[]|null> = new BehaviorSubject<TrackElement[]|null>(null)
    public track$: Observable<TrackElement[]|null> = this.trackSubject$.asObservable()
    public artistId = "id"
    albumTypes = [
      {value: '0', viewValue: 'Album'},
      {value: '1', viewValue: 'Single'},
      {value: '2', viewValue: 'EP'}
    ];
  
    public albumForm: FormGroup;
    public trackForm: FormGroup;
    get f() { return this.albumForm.controls; }
    get tracks() {
      return this.trackForm.get('tracks') as FormArray;
    }

    public img = null
    handleFileInput(files: FileList){
      if(files.length > 0){
        const img = files.item(0)
        this.f.file.setValue(img)
        let reader = new FileReader()
        reader.readAsDataURL(img); 
        reader.onload = (_event) => { 
          this.img = reader.result; 
        }
      }
    }

    trackIsNull(){
      return this.trackSubject$.value === null
    }

    async selectionChange(event){
      if(event.selectedIndex == 2) // Done
      {
        const tracks : Array<TrackElement> = await Promise.all(this.tracks.value.map(async (track) => {
          const len = await this.trackLenght(track.file)
          return {
            title: track.title,
            isExplicit: track.isExplicit,
            duration: len,
          }
        }))
        this.trackSubject$.next(tracks) 
      }
    }

    trackLenght(file: File) : Promise<number>{
      return new Promise((resolve, reject) => {
        const audioContext = new (window.AudioContext)();
        let fr = new FileReader();  
        fr.onload = (e) => {
          audioContext.decodeAudioData(e.target.result as ArrayBuffer, function(buffer) {
            const duration = buffer.duration;
            resolve(duration)
          });       
        };
        fr.onerror = () => {
          reject('onerror');
        }
        fr.readAsArrayBuffer(file);
      });
    }

    handleTrackFile(files: FileList, index: number){
      let file : any = null
      if(files.length > 0){
        file = files.item(0)
      }
      const group = this.tracks.controls[index] as FormGroup
      group.controls.file.setValue(file)
    }
    
    addTrack(){
      const group = new FormGroup({
        title: new FormControl('', [Validators.required]),
        isExplicit: new FormControl(false),
        file: new FormControl(null, Validators.required)
      });
      this.tracks.push(group)  
    }

    removeTrack(index: number){
      this.tracks.removeAt(index)
    }

    drop(event: CdkDragDrop<FormGroup[]>) {
      const arr = this.tracks.value
      moveItemInArray(arr, event.previousIndex, event.currentIndex)
      this.trackForm.controls.tracks.setValue(arr)
    }

    onSubmit(){
      if(!this.albumForm.valid || !this.trackForm.valid){
        return
      }
      console.log("submitted")
      
      const tracks = this.tracks.value.map(track => {
        return {
          title: track.title,
          isExplicit: track.isExplicit
        }
      })
      this.http.post<any>(this.ep.albumUri(), {
        label: this.f.label.value,
        artistId: this.data.id,
        albumType: new Number(this.f.albumType.value),
        releaseDate: this.f.releaseDate.value,
        releaseDatePrecision: new Number(this.f.releaseDatePrecision.value),
        availableFromUtfTime: this.f.availableFrom.value,
        tracks: tracks
      }).subscribe(resp => {
        console.log(resp)
        let form = new FormData();
        form.append("file", this.f.file.value)
        this.http.post<any>(this.ep.albumImageUpload(resp.id), form).subscribe(resp =>{
          console.log(resp)
        }, error => console.log(error))
        for(let i = 0; i < this.tracks.length; i++){
          let form = new FormData();
          const group = this.tracks.controls[i] as FormGroup
          form.append("file", group.controls.file.value)
          this.http.post(this.ep.albumTrack(this.artistId, resp.tracks[i].id), form).subscribe(resp => {
            console.log("uploaded")
            console.log(resp)
          }, error => console.log(error))
        }
      })
    }
}