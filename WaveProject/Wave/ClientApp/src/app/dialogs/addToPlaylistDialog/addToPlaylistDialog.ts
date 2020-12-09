import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { PlaylistDialogModel, Track } from '../../models'
import { FormBuilder, FormGroup } from '@angular/forms';
import { PlaylistService } from '../../services/playlist'
import { HttpClient } from '@angular/common/http';
import { EndpointService } from 'src/app/services/endpoints';

@Component({
    selector: 'addToPlaylist-dialog',
    templateUrl: 'addToPlaylistDialog.html',
    styleUrls: ['./addToPlaylistDialog.css']
})
export class AddToPlaylistDialog implements OnInit {
    constructor(
        public dialogRef: MatDialogRef<AddToPlaylistDialog>,
        @Inject(MAT_DIALOG_DATA) public data: Track,
        public ps: PlaylistService,
        private http: HttpClient,
        private ep: EndpointService
        ) {}

    ngOnInit(): void {
        console.log(this.data)
    }
    addToPlaylist(id: string) {
        this.http.post(this.ep.playlistAdd(id, this.data.id), {}).subscribe(data => {
            console.log(data)
        }, error => console.log(error))
        this.dialogRef.close()
    }
}