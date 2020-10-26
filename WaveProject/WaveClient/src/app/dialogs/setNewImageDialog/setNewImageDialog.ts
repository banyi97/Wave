import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EndpointService } from '../../services/endpoints'
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-setNewImageDialog',
  templateUrl: './setNewImageDialog.html',
  styleUrls: ['./setNewImageDialog.css']
})
export class SetNewImageDialog implements OnInit {
  constructor(
    private http: HttpClient, 
    private ep: EndpointService,
    public dialogRef: MatDialogRef<SetNewImageDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder
    ) {}
    ngOnInit(): void {
      this.imageFrom = this.fb.group({
        file: [null, Validators.required]
      })
    }
  
    public imageFrom: FormGroup;
    get f() { return this.imageFrom.controls; }

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
      if(!this.imageFrom.valid){
        return
      }
      let form = new FormData();
      form.append("file", this.f.file.value)
      this.http.post(this.ep.uploadPic(this.data.id, this.data.type), form).subscribe(resp => {
        console.log(resp)
        this.dialogRef.close(resp)
      }, error => {
        console.log(error)
        this.dialogRef.close()
      })

    }
}
