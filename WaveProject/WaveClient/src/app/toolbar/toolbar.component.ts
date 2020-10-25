import { Component } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css']
})
export class ToolbarComponent {
  constructor(
    public auth: AuthService,
    public location: Location
    ) {}
}