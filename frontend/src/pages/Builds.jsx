import React, { useState, useEffect } from 'react';
import { Hammer, Clock, HardDrive, Monitor } from 'lucide-react';
import StatusBadge from '../components/StatusBadge';
import { getBuilds } from '../services/api';

const FALLBACK_BUILDS = [
  { id: 1, buildNumber: 'NF-2024-1542', status: 'Success', platform: 'PC', configuration: 'Release', artifactSizeBytes: 52428800000, durationSeconds: 1800, project: { name: 'Nova Frontline' } },
  { id: 2, buildNumber: 'NF-2024-1543', status: 'Building', platform: 'Xbox', configuration: 'Release', artifactSizeBytes: 0, durationSeconds: 0, project: { name: 'Nova Frontline' } },
  { id: 3, buildNumber: 'SK-2024-3201', status: 'Success', platform: 'PlayStation', configuration: 'Release', artifactSizeBytes: 48318382080, durationSeconds: 2100, project: { name: 'Striker 26' } },
  { id: 4, buildNumber: 'GI-2024-0892', status: 'Failed', platform: 'PC', configuration: 'Debug', artifactSizeBytes: 0, durationSeconds: 450, project: { name: 'Gridiron 26' } },
  { id: 5, buildNumber: 'FE-2024-7710', status: 'Success', platform: 'PC', configuration: 'Profile', artifactSizeBytes: 1073741824, durationSeconds: 3600, project: { name: 'Forge Engine' } },
];

function formatDuration(seconds) {
  if (!seconds) return '--';
  const m = Math.floor(seconds / 60);
  return `${m}m`;
}

function formatSize(bytes) {
  if (!bytes) return '--';
  const gb = bytes / (1024 * 1024 * 1024);
  if (gb >= 1) return `${gb.toFixed(1)} GB`;
  const mb = bytes / (1024 * 1024);
  return `${mb.toFixed(0)} MB`;
}

const platformIcons = {
  PC: '🖥️',
  Xbox: '🎮',
  PlayStation: '🎮',
  Switch: '🕹️',
};

export default function Builds() {
  const [builds, setBuilds] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getBuilds()
      .then((res) => setBuilds(res.data))
      .catch(() => setBuilds(FALLBACK_BUILDS))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="w-8 h-8 border-2 border-ea-accent border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-white">Builds</h1>
          <p className="text-ea-muted mt-1">Game build artifacts across all platforms</p>
        </div>
        <button className="px-4 py-2 bg-ea-accent text-white rounded-lg text-sm font-medium hover:bg-ea-accent/80 transition-colors">
          + Trigger Build
        </button>
      </div>

      {/* Build Table */}
      <div className="bg-ea-card border border-ea-border rounded-xl overflow-hidden">
        <table className="w-full">
          <thead>
            <tr className="border-b border-ea-border">
              <th className="text-left text-xs font-semibold text-ea-muted px-5 py-3">Build</th>
              <th className="text-left text-xs font-semibold text-ea-muted px-5 py-3">Project</th>
              <th className="text-left text-xs font-semibold text-ea-muted px-5 py-3">Platform</th>
              <th className="text-left text-xs font-semibold text-ea-muted px-5 py-3">Config</th>
              <th className="text-left text-xs font-semibold text-ea-muted px-5 py-3">Status</th>
              <th className="text-left text-xs font-semibold text-ea-muted px-5 py-3">Duration</th>
              <th className="text-left text-xs font-semibold text-ea-muted px-5 py-3">Artifact Size</th>
            </tr>
          </thead>
          <tbody>
            {builds.map((build) => (
              <tr key={build.id} className="border-b border-ea-border/50 hover:bg-ea-dark/50 transition-colors">
                <td className="px-5 py-4">
                  <div className="flex items-center gap-2">
                    <Hammer size={14} className="text-ea-orange" />
                    <span className="text-sm font-mono text-white">{build.buildNumber}</span>
                  </div>
                </td>
                <td className="px-5 py-4 text-sm text-ea-text">{build.project?.name}</td>
                <td className="px-5 py-4">
                  <div className="flex items-center gap-1.5 text-sm text-ea-text">
                    <span>{platformIcons[build.platform] || '💻'}</span>
                    {build.platform}
                  </div>
                </td>
                <td className="px-5 py-4">
                  <span className="text-xs font-mono px-2 py-1 bg-ea-dark rounded text-ea-muted">
                    {build.configuration}
                  </span>
                </td>
                <td className="px-5 py-4">
                  <StatusBadge status={build.status} />
                </td>
                <td className="px-5 py-4">
                  <div className="flex items-center gap-1.5 text-sm text-ea-muted">
                    <Clock size={12} />
                    {formatDuration(build.durationSeconds)}
                  </div>
                </td>
                <td className="px-5 py-4">
                  <div className="flex items-center gap-1.5 text-sm text-ea-muted">
                    <HardDrive size={12} />
                    {formatSize(build.artifactSizeBytes)}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
