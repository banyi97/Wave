import { Injectable } from "@angular/core";

@Injectable({ providedIn: "root" })
export class EndpointService {
  constructor() {
    // if (location.hostname === "localhost") {
    //   this.serverUri = "https://localhost:44363";
    // }
    // else {
    //   this.serverUri = "https://waveprojectapi.azurewebsites.net";
    // }
  }
  serverUri: string = "";
 
  // ### Album

  albumUri(id: string = null) {
    if (id) {
        return `${this.serverUri}/api/Album/${id}`
    }
    return `${this.serverUri}/api/Album`
  }
  albumTopTracks(id: string) {
    return `${this.serverUri}/api/Album/${id}/tracks`
  }
  albumSong(id: string) {
    return `${this.serverUri}/api/Album/track/${id}`
  }
  albumAddExtraAuthorToSong(id: string, sId: string) {
    return `${this.serverUri}/api/Album/track/${id}/add/${sId}`
  }
  albumRemoveExtraAuthorFromSong(id: string, sId: string) {
    return `${this.serverUri}/api/Album/track/${id}/remove/${sId}`
  }

  // ### Artist 
  artistUri(id: string = null) {
    if (id) {
        return `${this.serverUri}/api/Artist/${id}`
    }
    return `${this.serverUri}/api/Artist`
  }
  artistTop(id: string) {
    return `${this.serverUri}/api/Artist/${id}/top`
  }
  artistAlbums(id: string) {
    return `${this.serverUri}/api/Artist/${id}/albums`
  }

  // ### Player

  player(id: string) {
    return `${this.serverUri}/api/Player/${id}`; 
  }

  // ### Playlist
  playlistMe() { // GET
    return `${this.serverUri}/api/Playlist/me`;
  }
  playlistCreate() { // POST
    return `${this.serverUri}/api/Playlist`;
  }
  playlist(id: string) { // GET PUT DELETE
    return `${this.serverUri}/api/Playlist/${id}`;
  }
  playlistRename(id: string) { // PATCH
    return `${this.serverUri}/api/Playlist/${id}/rename`;
  }
  playlistReplace(id: string, from: number, to: number) { // PUT
    return `${this.serverUri}/api/Playlist/${id}?from=${from}&to=${to}`;
  }
  playlistMakePublic(id: string) { // PATCH
    return `${this.serverUri}/api/Playlist/${id}/public`;
  }
  playlistAdd(id: string, sId: string) { // POST
    return `${this.serverUri}/api/Playlist/${id}/${sId}`;
  }
  playlistReOrder(id: string, sId: string, next: number) { // PUT
    return `${this.serverUri}/api/Playlist/${id}/${sId}?next=${next}`;
  }
  playlistRemoveFromTrack(id: string) { // DELETE
    return `${this.serverUri}/api/Playlist/element/${id}`;
  }

  // ### Search
  searchTop(tag: string) {
    return `${this.serverUri}/api/Search/result/${tag}`
  }
  searchAlbum(tag: string) {
    return `${this.serverUri}/api/Search/albums/${tag}`
  }
  searchArtist(tag: string) {
    return `${this.serverUri}/api/Search/artists/${tag}`
  }
  searchSong(tag: string) {
    return `${this.serverUri}/api/Search/songs/${tag}`
  }
  searchPlaylist(tag: string) {
    return `${this.serverUri}/api/Search/playlists/${tag}`
  }

  // ### Upload
  uploadPic(id: string, type: string) {
    return `${this.serverUri}/api/Upload/pic/${type}/${id}`
  }
  uploadSong(id: string) {
    return `${this.serverUri}/api/Upload/track/${id}`
  }
}