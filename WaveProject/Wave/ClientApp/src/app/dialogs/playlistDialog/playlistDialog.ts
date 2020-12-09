import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { PlaylistDialogModel } from '../../models'
import { FormBuilder, FormGroup } from '@angular/forms';
  
@Component({
    selector: 'playlist-dialog',
    templateUrl: 'playlistDialog.html',
    styleUrls: ['./playlistDialog.css']
})
export class PlaylistDialog implements OnInit {
    constructor(
        public dialogRef: MatDialogRef<PlaylistDialog>,
        @Inject(MAT_DIALOG_DATA) public data: PlaylistDialogModel,
        private fb: FormBuilder
        ) {}
    ngOnInit(): void {
        this.titleForm = this.fb.group({
            title: [this.data.title ? this.data.title : ""]
          })
    }

    titleForm: FormGroup;
    get f() { return this.titleForm.controls; }

    onSubmit(){
        const val = this.f.title.value
        if(this.data.isCreate){
            if(val.trim() != "")
                this.dialogRef.close(val)
            else
                this.dialogRef.close("New playlist")
        }
        else{
            if(val.trim() != "")
                this.dialogRef.close(val)
            else
                this.dialogRef.close(null)
        }
    }
}
