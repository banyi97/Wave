import { Injectable } from "@angular/core";

@Injectable({ providedIn: "root" })
export class EndpointService {
  constructor() {
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
  albumImageUpload(id: string){
    return `${this.serverUri}/api/Album/${id}/images`
  }
  albumImageRemove(id: string, sId: string){
    return `${this.serverUri}/api/Album/${id}/images/${sId}`
  }
  albumTrack(id: string, sId: string){
    return `${this.serverUri}/api/Album/${id}/track/${sId}`
  }
  albumTrackOrder(id: string, sId: string, next: number){
    return `${this.serverUri}/api/Album/${id}/tracks/${sId}?next=${next}`
  }
  albumRemoveTrack(id: string, sId: string){
    return `${this.serverUri}/api/Album/${id}/tracks/${sId}`
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
  artistImageUpload(id: string){
    return `${this.serverUri}/api/Artist/${id}/images`
  }
  artistImageRemove(id: string, sId: string){
    return `${this.serverUri}/api/Artist/${id}/images/${sId}`
  }

  // ### Home

  home() {
    return `${this.serverUri}/api/Home`; 
  }

  yourLib(id: string) {
    return `${this.serverUri}/api/Home/yourlib`; 
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
  playlistImageUpload(id: string){
    return `${this.serverUri}/api/Playlist/${id}/images`
  }
  playlistImageRemove(id: string, sId: string){
    return `${this.serverUri}/api/Playlist/${id}/images/${sId}`
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

  // ### Admin
  users(id: string = null) {
    if(id){
      return `${this.serverUri}/api/Admin/users/${id}`
    }
    return `${this.serverUri}/api/Admin/users`
  }
  roles(id: string = null){
    if(id){
      return `${this.serverUri}/api/Admin/roles/${id}`
    }
    return `${this.serverUri}/api/Admin/roles`
  }
  userRole(userId){
    return `${this.serverUri}/api/Admin/users/${userId}/roles`
  }
  userRoleRemove(userId){
    return `${this.serverUri}/api/Admin/users/${userId}/roles/removes`
  }

}