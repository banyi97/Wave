import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

import { AuthGuard } from './services/auth/auth.guard';
import { MaterialModule } from './services/material';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

import { InterceptorService } from './services/auth/interceptor.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ContextMenuModule } from 'ngx-contextmenu';

import { DurationConvertPipe, DurationSecConvertPipe } from './services/durationConvertPipe';

import { HomeComponent } from './home/home.component';
import { SearchComponent } from './search/search.component';
import { ArtistComponent } from './artist/artist.component';
import { AlbumComponent } from './album/album.component';
import { PlaylistComponent } from './playlist/playlist.component';
import { YourLibComponent } from './yourlib/yourlib.component';
import { ProfileComponent } from './profile/profile.component';
import { NotfoundComponent } from './notfound/notfound.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { ArtistViewComponent } from './view/artist/artistview.component'
import { AlbumViewComponent } from './view/album/albumview.component'
import { TrackViewComponent } from './view/track/trackview.component'
import { PlaylistViewComponent } from './view/playlist/playlistview.component'
import { PlayerComponent } from './player/player.component'

import { PlaylistDialog } from './dialogs/playlistDialog/playlistDialog'
import { AddToPlaylistDialog } from './dialogs/addToPlaylistDialog/addToPlaylistDialog'

@NgModule({
  declarations: [
    DurationConvertPipe,
    DurationSecConvertPipe,
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ProfileComponent,
    SearchComponent,
    ArtistComponent,
    AlbumComponent,
    PlaylistComponent,
    YourLibComponent,
    ToolbarComponent,
    ArtistViewComponent,
    AlbumViewComponent,
    TrackViewComponent,
    PlaylistViewComponent,
    PlaylistDialog,
    AddToPlaylistDialog,
    PlayerComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ContextMenuModule.forRoot({ useBootstrap4: true }),
    NgbModule,
    FormsModule,
    MaterialModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
      { path: 'search/resent', component: SearchComponent },
      { path: 'search/:types/:id', component: SearchComponent },
      { path: 'yourlib', component: YourLibComponent, canActivate: [AuthGuard] },
      { path: 'artist/:id', component: ArtistComponent },
      { path: 'album/:id', component: AlbumComponent },
      { path: 'playlist/:id', component: PlaylistComponent },
      { path: '**', component: NotfoundComponent }
    ]),
    BrowserAnimationsModule
  ],
  exports: [
    DurationConvertPipe,
    DurationSecConvertPipe
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: InterceptorService,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }