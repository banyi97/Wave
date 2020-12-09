import { Injectable } from "@angular/core";
import { fromEvent, Observable } from 'rxjs';

@Injectable({ providedIn: "root" })
export class FlexService {
    constructor() {
        this.sideBarButtonIsVisible = window.innerWidth < 600 ? true : false

        fromEvent(window, 'resize').subscribe(
            () => {
                this.sideBarButtonIsVisible = window.innerWidth < 600 ? true : false
                
                this.mode = this.sideBarButtonIsVisible ? "over" : "side"

                if(!this.sideBarIsOpen && !this.sideBarButtonIsVisible){
                    this.sideBarIsOpen = true
                }
            }
        )
    }

    public sideBarIsOpen: boolean = true;
    public sideBarButtonIsVisible: boolean = true;

    public toggle(){
        this.sideBarIsOpen = true
    }

    public mode = "side";
}