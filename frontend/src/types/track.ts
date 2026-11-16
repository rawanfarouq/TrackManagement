
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


export type DistributionStatus = "pending" | "live" | "rejected";

export interface TrackDistribution {
  id: number;
  dspId: number;
  dspName: string;
  submittedAt: string;
  status: DistributionStatus;
}

export interface TrackDetail extends Track {
  distributions: TrackDistribution[];
}
