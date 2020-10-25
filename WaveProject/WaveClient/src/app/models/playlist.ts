import { Image } from "./image"
import { PlaylistElement } from "./playlistelement"

export class Playlist {
  id: string
  title: string

  isPublic: boolean
  isMy: boolean
  numberOf: number
  image: Image
  playlistElements: Array<PlaylistElement>
  type: string;
}