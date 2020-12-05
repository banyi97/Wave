import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { EndpointService } from '../services/endpoints';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor(
    private http: HttpClient,
    private ep: EndpointService
    ){}
  ngOnInit(): void {
    this.http.get<any>(this.ep.home()).subscribe(resp => {
      this.albums = resp
    
    })
  }
  public albums = []

}