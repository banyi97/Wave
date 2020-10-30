import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialogRef } from '@angular/material/dialog';
import { EndpointService } from '../../services/endpoints'
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-createArtistDialog',
  templateUrl: './createArtistDialog.html',
  styleUrls: ['./createArtistDialog.css']
})
export class CreateArtistDialog implements OnInit {
  constructor(
    private http: HttpClient, 
    private ep: EndpointService,
    public dialogRef: MatDialogRef<CreateArtistDialog>,
    private fb: FormBuilder
    ) {}
    ngOnInit(): void {
      this.artistForm = this.fb.group({
        name: ["", [Validators.required]],
        description: [""],
        file: [null, Validators.required]
      })
    }
  
    public artistForm: FormGroup;
    get f() { return this.artistForm.controls; }

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
    
    onSubmit(){
      if(!this.artistForm.valid){
        return
      }

      this.http.post<any>(this.ep.artistUri(), {name: this.f.name.value, description: this.f.description.value}).subscribe(resp =>{
        if(resp.id){
          let form = new FormData();
          form.append("file", this.f.file.value)
          this.http.post(this.ep.artistImageUpload(resp.id), form).subscribe(resp => {

          }, error => console.log(error))
          this.dialogRef.close(resp.id)
        }
      }, error => {
        console.log(error)
      })
    }
}