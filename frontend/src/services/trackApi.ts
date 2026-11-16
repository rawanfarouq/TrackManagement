
import type { Track } from "../types/track";

const API_BASE_URL = "http://localhost:5150";

export async function getTracks(): Promise<Track[]> {
  const response = await fetch(`${API_BASE_URL}/api/tracks`);

  if (!response.ok) {
    throw new Error("Failed to fetch tracks");
  }
 
  //tells TypeScript that the function asynchronously returns an array of tracks
  return response.json() as Promise<Track[]>;
}
