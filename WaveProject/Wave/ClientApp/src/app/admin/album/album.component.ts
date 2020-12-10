import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EndpointService } from '../../services/endpoints'
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { CreateAlbumDialog } from '../../dialogs/createAlbumDialog/createAlbumDialog';
import { DefaultDialog } from '../../dialogs/defaultDialog/defaultDialog';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { SetNewImageDialog } from '../../dialogs/setNewImageDialog/setNewImageDialog';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-album',
  templateUrl: './album.component.html',
  styleUrls: ['./album.component.css']
})
export class AdminAlbumComponent implements OnInit{
  constructor(
    private http: HttpClient, 
    private ep: EndpointService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private router: Router) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(query => {
      let id = query.get("id")
      if(!id){
        return
      }
      this.id = id
      this.http.get<any>(this.ep.albumUri(id)).subscribe(resp =>{
        console.log(resp)
        this.album = resp
      })
    })
  }

  public albumForm: FormGroup;
  public trackForm: FormGroup;
  get f(){ return this.albumForm.controls }
  get tracks() {
    return this.trackForm.get('tracks') as FormArray;
  }
  private id: string
  public album = null

  public albumType = null

  public isModify: boolean = false
  public isTracksModify: boolean = false

  albumTypes = [
    {value: '0', viewValue: 'Album'},
    {value: '1', viewValue: 'Single'},
    {value: '2', viewValue: 'EP'}
  ];

  modifyAlbum(){
    this.albumForm = this.fb.group({
      label: [this.album.label, [Validators.required]],
      albumType: [`${this.album.albumType}`, Validators.required],
      releaseDate: [this.album.releaseDate, [Validators.required]],
      releaseDatePrecision: [`${this.album.releaseDatePrecision}`, [Validators.required]],
      availableFrom: ['this.album.availableFrom', [Validators.required]] 
    });
    this.isModify = true
  }

  modifyTracks(){
    const tracks = this.album.tracks.map(track => {
      return this.fb.group({
        title: [track.title, [Validators.required]],
        isExplicit: [track.isExplicit],
        file: [null, Validators.required],
        ismodify: false
      })
    })
    this.trackForm = this.fb.group({
      tracks: this.fb.array(tracks)
    })
    
    this.isTracksModify = true
  }

  modifyTrack(){

  }

  modifyBack(){
    this.isTracksModify = false;
    this.http.get<any>(this.ep.albumUri(this.id)).subscribe(resp =>{
      this.album = resp
    })
  }

  removeTrack(index: number){
    const id = this.album.tracks[index].id
    this.http.delete(this.ep.albumRemoveTrack(this.album.id, id)).subscribe(q => {
      this.tracks.removeAt(index)
      this.album.tracks = this.album.tracks.filter(q => q.id != id)
    })
  }

  drop(event: CdkDragDrop<FormGroup[]>) {
    var arr = this.tracks.value
    var track = this.album.tracks[event.previousIndex]
    console.log(track)
    moveItemInArray(arr, event.previousIndex, event.currentIndex)
    this.trackForm.controls.tracks.setValue(arr)
    this.http.put(this.ep.albumTrackOrder(this.album.id, track.id, event.currentIndex), null).subscribe(q => {
      moveItemInArray(this.album.tracks, event.previousIndex, event.currentIndex)
    })
  }

  addTrack(){
    const group = new FormGroup({
      title: new FormControl('', [Validators.required]),
      isExplicit: new FormControl(false),
      file: new FormControl(null, Validators.required)
    });
    this.tracks.push(group)
  }

  setNewImage(){
    const dialogRef = this.dialog.open(SetNewImageDialog, { data: {
      id: this.id,
      type: 'album',
      title: 'Set new album image'
    }})
    dialogRef.afterClosed().subscribe(res =>{
      if(res){
        this.album.image.uri = res
      }
    })
  }

  saveAlbum(){
    if(!this.albumForm.valid)
      return
    console.log('valid')
    this.http.put<any>(this.ep.albumUri(this.id), {
      label: this.f.label.value
    }).subscribe(resp => { 
      this.album.label = resp.label 

      this.isModify = false
    }, error => console.log(error))
  }

  removeAlbum(){
    const dialogRef = this.dialog.open(DefaultDialog, {
      data: {
        title: `Remove album: ${this.album.label}`,
        content: 'Do you want to remove this album?',
        ok: 'Remove',
        close: 'Cancel'
      }
    })
    dialogRef.afterClosed().subscribe(res => {
      if(res){
        this.http.delete(this.ep.artistUri(this.id)).subscribe(resp =>{
          this.router.navigate([`/admin/artist/${this.album.artistId}`])
        }, error => {
          console.log(error)
        })
      }
    })
  }

  handleTrackFile(files: FileList, index: number){
    let file : any = null
    if(files.length > 0){
      file = files.item(0)
    }
    const group = this.tracks.controls[index] as FormGroup
    group.controls.file.setValue(file)
  }

  createAlbum(){
    const dialogRef = this.dialog.open(CreateAlbumDialog);
    dialogRef.afterClosed().subscribe(id => {
      if(id){
        this.router.navigate([`/admin/artist/${id}`])
      }
    })
  }
}
