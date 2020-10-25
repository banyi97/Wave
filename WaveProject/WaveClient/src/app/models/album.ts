import { Image } from "./image"
import { Track } from "./track";

export class Album {
  id: string;
  label: string;

  artistId: string;
  artistName: string;
  
  releaseDate: Date;
  releaseDatePrecision: string;
  albumType: string;

  image: Image;
  tracks: Array<Track>

  type: string;
}