
import { useEffect, useState } from "react";
import { getTracks } from "./services/trackApi";
import type { Track } from "./types/track";
import "./App.css";

function App() {
  const [tracks, setTracks] = useState<Track[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [statusFilter, setStatusFilter] =
    useState<Track["status"] | "all">("all");

  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    let active = true;

    async function loadTracks() {
      try {
        const data = await getTracks();

        if (active) {
          setTracks(data);
        }
      } catch {
        if (active) {
          setError("Could not load tracks. Please check your API.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    void loadTracks();

    return () => {
      active = false;
    };
  }, []);

  // Filter tracks by status and search text
  const filteredTracks = tracks.filter((track) => {
    const matchesStatus =
      statusFilter === "all" || track.status === statusFilter;

    const searchText = searchTerm.toLowerCase().trim();

    const matchesSearch =
      track.title.toLowerCase().includes(searchText) ||
      track.artistName.toLowerCase().includes(searchText) ||
      track.genre.toLowerCase().includes(searchText);

    return matchesStatus && matchesSearch;
  });

  const countByStatus = (status: Track["status"]) =>
    tracks.filter((track) => track.status === status).length;

  return (
    <div className="app-shell">
      <header className="topbar">
        <div className="brand-icon">♫</div>
        <span className="brand-name">TrackFlow</span>
        <span className="brand-label">Music Distribution</span>
      </header>

      <main className="page-content">
        <section className="page-heading">
          <div>
            <p className="eyebrow">CATALOG OVERVIEW</p>
            <h1>Track Management</h1>
            <p className="page-description">
              View your music catalog and track distribution progress.
            </p>
          </div>

          <span className="total-label">
            {tracks.length} Total Tracks
          </span>
        </section>

        <section className="stats-grid">
          <div className="stat-card">
            <span className="stat-title">Total Tracks</span>
            <strong className="stat-value">{tracks.length}</strong>
            <span className="stat-description">
              Tracks in your catalog
            </span>
          </div>

          <div className="stat-card">
            <span className="stat-title">Draft</span>
            <strong className="stat-value">
              {countByStatus("draft")}
            </strong>
            <span className="stat-description">
              Not yet submitted
            </span>
          </div>

          <div className="stat-card">
            <span className="stat-title">Submitted</span>
            <strong className="stat-value">
              {countByStatus("submitted")}
            </strong>
            <span className="stat-description">
              In distribution workflow
            </span>
          </div>

          <div className="stat-card">
            <span className="stat-title">Distributed</span>
            <strong className="stat-value">
              {countByStatus("distributed")}
            </strong>
            <span className="stat-description">
              Marked as distributed
            </span>
          </div>
        </section>

        <section className="catalog-card">
          <div className="catalog-header">
            <h2>Music Catalog</h2>
            <p>Browse and filter your tracks</p>
          </div>

          <div className="filters-row">
            <div className="filter-field search-field">
              <label htmlFor="search">Search tracks</label>
              <input
                id="search"
                type="text"
                placeholder="Search by title, artist, or genre..."
                value={searchTerm}
                onChange={(event) =>
                  setSearchTerm(event.target.value)
                }
              />
            </div>

            <div className="filter-field">
              <label htmlFor="status-filter">
                Filter by status
              </label>

              <select
                id="status-filter"
                value={statusFilter}
                onChange={(event) =>
                  setStatusFilter(
                    event.target.value as
                      | Track["status"]
                      | "all"
                  )
                }
              >
                <option value="all">Status</option>
                <option value="draft">Draft</option>
                <option value="submitted">Submitted</option>
                <option value="distributed">
                  Distributed
                </option>
              </select>
            </div>
          </div>

          {loading && (
            <p className="message">Loading tracks...</p>
          )}

          {error && (
            <p className="message error-message">{error}</p>
          )}

          {!loading && !error && (
            <>
              <p className="results-count">
                Showing {filteredTracks.length} of {tracks.length} tracks
              </p>

              <div className="table-container">
                <table className="tracks-table">
                  <thead>
                    <tr>
                      <th>Track</th>
                      <th>Artist</th>
                      <th>Genre</th>
                      <th>Status</th>
                      <th>Release Date</th>
                    </tr>
                  </thead>

                  <tbody>
                    {filteredTracks.map((track) => (
                      <tr key={track.id}>
                        <td>
                          <div className="track-title">
                            {track.title}
                          </div>
                          <div className="track-isrc">
                            ISRC: {track.isrc}
                          </div>
                        </td>

                        <td>{track.artistName}</td>
                        <td>{track.genre}</td>

                        <td>
                          <span
                            className={`status-badge status-${track.status}`}
                          >
                            {track.status}
                          </span>
                        </td>

                        <td>{track.releaseDate}</td>
                      </tr>
                    ))}

                    {filteredTracks.length === 0 && (
                      <tr>
                        <td colSpan={5} className="empty-cell">
                          No tracks match your filters.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </>
          )}
        </section>
      </main>
    </div>
  );
}

export default App;
