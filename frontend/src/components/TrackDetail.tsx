
import { useEffect, useState } from "react";
import { getTrackById } from "../services/trackApi";
import type { TrackDetail as TrackDetailData } from "../types/track";
import "./TrackDetail.css";

interface TrackDetailProps {
  trackId: number;
  onBack: () => void;
}

function TrackDetail({ trackId, onBack }: TrackDetailProps) {
  const [track, setTrack] = useState<TrackDetailData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    async function loadTrack() {
      try {
        const data = await getTrackById(trackId);

        if (active) {
          setTrack(data);
        }
      } catch {
        if (active) {
          setError("Could not load track details.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    void loadTrack();

    return () => {
      active = false;
    };
  }, [trackId]);

  return (
    <div>
      <button
        type="button"
        className="back-button"
        onClick={onBack}
      >
        ← Back to Tracks
      </button>

      {loading && <p className="message">Loading track details...</p>}

      {error && <p className="message error-message">{error}</p>}

      {!loading && !error && track && (
        <>
          <div className="detail-card">
            <div className="detail-header">
              <div>
                <p className="eyebrow">TRACK DETAILS</p>
                <h1>{track.title}</h1>
                <p className="page-description">
                  {track.artistName} · {track.genre}
                </p>
              </div>

              <span
                className={`status-badge status-${track.status}`}
              >
                {track.status}
              </span>
            </div>

            <div className="detail-grid">
              <div>
                <span className="detail-label">Artist</span>
                <strong>{track.artistName}</strong>
              </div>

              <div>
                <span className="detail-label">Genre</span>
                <strong>{track.genre}</strong>
              </div>

              <div>
                <span className="detail-label">ISRC</span>
                <strong>{track.isrc}</strong>
              </div>

              <div>
                <span className="detail-label">Release Date</span>
                <strong>{track.releaseDate}</strong>
              </div>
            </div>
          </div>

          <div className="detail-card">
            <h2>DSP Distributions</h2>
            <p className="page-description">
              Distribution status across music platforms
            </p>

            {track.distributions.length === 0 ? (
              <p className="message">
                This track has not been submitted to any DSP yet.
              </p>
            ) : (
              <div className="distribution-list">
                {track.distributions.map((distribution) => (
                  <div
                    key={distribution.id}
                    className="distribution-row"
                  >
                    <div>
                      <strong>{distribution.dspName}</strong>
                      <p className="distribution-date">
                        Submitted:{" "}
                        {new Date(
                          distribution.submittedAt
                        ).toLocaleDateString()}
                      </p>
                    </div>

                    <span
                      className={`status-badge status-${distribution.status}`}
                    >
                      {distribution.status}
                    </span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}

export default TrackDetail;
