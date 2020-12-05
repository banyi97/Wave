import { Image } from "./image"
import { Track } from "./track";

export enum AlbumType { Album, Single, EP}

export class Album {
  id: string;
  label: string;

  artistId: string;
  artistName: string;
  
  releaseDate: Date;
  releaseDatePrecision: string;
  albumType: AlbumType;

  image: Image;
  tracks: Array<Track>

  type: string;
}