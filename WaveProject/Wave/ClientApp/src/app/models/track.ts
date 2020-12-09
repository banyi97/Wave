import { Artist } from "./artist";

export class Track {
  id: string;
  title: string;

  albumId: string;
  albumLabel: string;

  artistId: string;
  artistName: string;

  plays: number;
  isExplicit: boolean;
  duration: number;
  isPlayable: boolean;
  uri: string;
  albumImageUri: string;

  discNumber: number;
  numberOf: number;

  supportArtists: Array<Artist>;
  type: string;
}