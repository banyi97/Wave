import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';


@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {

  constructor(
    public auth: AuthService
    ) { 
    this.auth.getTokenSilently$().subscribe(token => {
      this.jwttoken = token;
    })
  }
  jwttoken: string = ""
  ngOnInit() {
  }
}