import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-defaultDialog',
  templateUrl: './defaultDialog.html',
  styleUrls: ['./defaultDialog.css']
})
export class DefaultDialog implements OnInit {
  constructor(
    public dialogRef: MatDialogRef<DefaultDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    ) {}
    ngOnInit(): void {
    }
  
}