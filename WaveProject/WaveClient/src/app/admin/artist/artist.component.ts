import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EndpointService } from '../../services/endpoints'
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { CreateAlbumDialog } from '../../dialogs/createAlbumDialog/createAlbumDialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { DefaultDialog } from '../../dialogs/defaultDialog/defaultDialog';
import { SetNewImageDialog } from '../../dialogs/setNewImageDialog/setNewImageDialog';

@Component({
  selector: 'app-artist',
  templateUrl: './artist.component.html',
  styleUrls: ['./artist.component.css']
})
export class AdminArtistComponent implements OnInit{
  constructor(
    private http: HttpClient, 
    private ep: EndpointService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private router: Router, 
    private fb: FormBuilder) {}

  ngOnInit(): void {
    this.artistForm = this.fb.group({
      name: ["", [Validators.required]],
      description: ["", [Validators.required]]
    })
    this.route.paramMap.subscribe(query => {
      let id = query.get("id")
      if(!id){
        this.artist = null
        this.albums = []
        return
      }
      this.id = id
      this.http.get<any>(this.ep.artistUri(id)).subscribe(resp =>{
        console.log(resp)
        this.artist = resp
      }, error =>{
        console.log(error)
        this.artist = null
      })
      this.http.get<[]>(this.ep.artistAlbums(id)).subscribe(resp =>{
        console.log(resp)
        this.albums = resp
      }, error => {
        console.log(error)
        this.albums = []
      })
    })
  }
  private id
  public artist = null
  public albums = []

  public isModify: boolean = false
  
  artistForm: FormGroup;
  get f() { return this.artistForm.controls; }
  modify(){
    this.isModify = true
    this.f.name.setValue(this.artist.name)
    this.f.description.setValue(this.artist.description)
  }

  save(){
    if(!this.artistForm.valid)
      return
    this.http.put<any>(this.ep.artistUri(this.id), {name: this.f.name.value, description: this.f.description.value}).subscribe(resp =>{ 
      this.artist.name = resp.name
      this.artist.description = resp.description
      this.isModify = false
    }, error => {
      console.log(error)
    })
    
  }

  remove(){
    const dialogRef = this.dialog.open(DefaultDialog, {
      data: {
        title: `Remove artist: ${this.artist.name}`,
        content: 'Do you want to remove this artist?',
        ok: 'Remove',
        close: 'Cancel'
      }
    })
    dialogRef.afterClosed().subscribe(res => {
      if(res){
        this.http.delete(this.ep.artistUri(this.id)).subscribe(resp =>{
          this.router.navigate(['/admin/artists'])
        }, error =>{
          console.log(error)
        })
      }
    })
  }

  setNewImg(){
    const dialogRef = this.dialog.open(SetNewImageDialog, {data:{
      id: this.id,
      type: "artist",
      title: "Set new artist image"
    }})
    dialogRef.afterClosed().subscribe(resp =>{
      if(resp){
        this.artist.image.uri = resp
      }
    })
  }

  createAlbum(){
    const dialogRef = this.dialog.open(CreateAlbumDialog, {data: {id: this.id}});
    dialogRef.afterClosed().subscribe(id => {
      if(id){
        this.router.navigate([`/admin/artist/${id}`])
      }
    })
  }
}