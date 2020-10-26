import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../services/auth/auth.service';
import { PlaylistService } from './playlist';
import { EndpointService } from './endpoints';

@Injectable({providedIn: 'root'})
export class SignalRService {
    constructor(
        private auth: AuthService,
        private ps: PlaylistService,
        private ep: EndpointService
        ){
            console.log("signalr ctor")
            this.auth.isAuthenticated$.subscribe(isAuth => {
                console.log(isAuth)
            })
            this.auth.getTokenSilently$().subscribe(token =>{
                if(token){
                    this.startConnection(token)
                    console.log("signalr")
                }
                else{
                    this.hubConnection.onclose(res => {
                        console.log(res)
                    });
                }
            })
        }
    private hubConnection: signalR.HubConnection

    public startConnection = (token = "") => {
        this.hubConnection = 
            new signalR.HubConnectionBuilder()
                .withUrl(this.ep.serverUri + '/notification', { accessTokenFactory: () => token })
                .withAutomaticReconnect()
                .build();
        this.hubConnection
            .start()
            .then(() => console.log('Connection started'))
            .catch(err => console.log('Error while starting connection: ' + err))

            this.createPlaylistListener();
            this.reorderPlaylistListener();
            this.renamePlaylistListener();
            this.removedPlaylistListener();
            this.makePublicPlaylistListener();
            this.addToPlaylistListener();
            this.removeFromPlaylistListener();
    }

    private createPlaylistListener = () => {
        this.hubConnection.on('createdPlaylist', (data) =>{
            console.log("createPlaylistListener")
            console.log(data)
            this.ps.addCreatedPlaylist(data)
        })
    }

    private renamePlaylistListener = () => {
        this.hubConnection.on('renamedPlaylist', (data) =>{
            console.log("renamePlaylistListener")
            console.log(data)
            this.ps.renamePlaylist(data)
        })
    }

    private reorderPlaylistListener = () => {
        this.hubConnection.on('reorderPlaylist', (data) => {
            console.log("reorderPlaylistListener")
            this.ps.replacePlaylist(data)
        })
    }

    private removedPlaylistListener = () => {
        this.hubConnection.on('removedPlaylist', (data) => {
            console.log("removedPlaylistListener")
            console.log(data)
            this.ps.removePlaylist(data)
        })
    }

    private makePublicPlaylistListener = () => {
        this.hubConnection.on('makePublic', (data) => {
            console.log("makePublicPlaylistListener")
            this.ps.makePublic(data)
        })
    }

    private addToPlaylistListener = () => {
        this.hubConnection.on('addedToPlaylist', (data) => {
            console.log("addToPlaylistListener")
            console.log(data)
            this.ps.addToPlaylist(data)
        })
    }

    private removeFromPlaylistListener = () => {
        this.hubConnection.on('removedFromPlaylist', (data) => {
            console.log("removeFromPlaylistListener")
            console.log(data)
            this.ps.removeFromPlaylist(data)
        })
    }
}
