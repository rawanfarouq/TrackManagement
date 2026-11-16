
export type TrackStatus = "draft" | "submitted" | "distributed";

export interface Track {
  id: number;
  title: string;
  artistId: number;
  artistName: string;
  isrc: string;
  releaseDate: string;
  genre: string;
  status: TrackStatus;
}
