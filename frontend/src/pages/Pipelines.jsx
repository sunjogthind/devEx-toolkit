import React, { useState, useEffect } from 'react';
import { GitBranch, Clock, User, GitCommit } from 'lucide-react';
import StatusBadge from '../components/StatusBadge';
import { getPipelines } from '../services/api';

const FALLBACK_PIPELINES = [
  { id: 1, name: 'Build & Test', status: 'Success', branch: 'main', commitSha: 'a1b2c3d', commitMessage: 'Fix player physics collision', triggeredBy: 'john.dev', durationSeconds: 342, project: { name: 'Nova Frontline' } },
  { id: 2, name: 'Deploy Staging', status: 'Running', branch: 'feature/new-ui', commitSha: 'e4f5g6h', commitMessage: 'Add new stadium rendering', triggeredBy: 'sarah.eng', durationSeconds: 0, project: { name: 'Striker 26' } },
  { id: 3, name: 'Nightly Build', status: 'Failed', branch: 'develop', commitSha: 'i7j8k9l', commitMessage: 'Update roster data pipeline', triggeredBy: 'ci-bot', durationSeconds: 128, project: { name: 'Gridiron 26' } },
  { id: 4, name: 'Engine Build', status: 'Success', branch: 'main', commitSha: 'm0n1o2p', commitMessage: 'Optimize shader compilation', triggeredBy: 'alex.gpu', durationSeconds: 1205, project: { name: 'Forge Engine' } },
  { id: 5, name: 'Integration Tests', status: 'Success', branch: 'main', commitSha: 'q3r4s5t', commitMessage: 'Add OAuth2 flow tests', triggeredBy: 'mike.auth', durationSeconds: 89, project: { name: 'Nexus Platform' } },
  { id: 6, name: 'Build & Test', status: 'Cancelled', branch: 'hotfix/crash', commitSha: 'u6v7w8x', commitMessage: 'Emergency crash fix', triggeredBy: 'john.dev', durationSeconds: 45, project: { name: 'Nova Frontline' } },
  { id: 7, name: 'Deploy Production', status: 'Success', branch: 'release/1.2', commitSha: 'y9z0a1b', commitMessage: 'Release 1.2.0', triggeredBy: 'release-bot', durationSeconds: 567, project: { name: 'Striker 26' } },
];

function formatDuration(seconds) {
  if (!seconds) return '--';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return m > 0 ? `${m}m ${s}s` : `${s}s`;
}

export default function Pipelines() {
  const [pipelines, setPipelines] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('All');

  useEffect(() => {
    getPipelines()
      .then((res) => setPipelines(res.data))
      .catch(() => setPipelines(FALLBACK_PIPELINES))
      .finally(() => setLoading(false));
  }, []);

  const filtered = filter === 'All' ? pipelines : pipelines.filter((p) => p.status === filter);
  const statuses = ['All', 'Success', 'Running', 'Failed', 'Pending', 'Cancelled'];

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
          <h1 className="text-2xl font-bold text-white">Pipelines</h1>
          <p className="text-ea-muted mt-1">CI/CD pipeline runs across all projects</p>
        </div>
      </div>

      {/* Filter Tabs */}
      <div className="flex gap-2 mb-6">
        {statuses.map((s) => (
          <button
            key={s}
            onClick={() => setFilter(s)}
            className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
              filter === s
                ? 'bg-ea-accent/10 text-ea-accent border border-ea-accent/30'
                : 'text-ea-muted hover:text-ea-text bg-ea-card border border-ea-border'
            }`}
          >
            {s}
          </button>
        ))}
      </div>

      {/* Pipeline List */}
      <div className="space-y-3">
        {filtered.map((pipeline) => (
          <div key={pipeline.id} className="bg-ea-card border border-ea-border rounded-xl p-5 hover:border-ea-accent/20 transition-colors">
            <div className="flex items-center justify-between mb-3">
              <div className="flex items-center gap-3">
                <GitBranch size={18} className="text-ea-accent" />
                <div>
                  <h3 className="text-sm font-semibold text-white">{pipeline.name}</h3>
                  <p className="text-xs text-ea-muted">{pipeline.project?.name}</p>
                </div>
              </div>
              <StatusBadge status={pipeline.status} />
            </div>

            <div className="flex items-center gap-6 text-xs text-ea-muted">
              <span className="flex items-center gap-1.5 font-mono">
                <GitBranch size={12} />
                {pipeline.branch}
              </span>
              <span className="flex items-center gap-1.5 font-mono">
                <GitCommit size={12} />
                {pipeline.commitSha}
              </span>
              <span className="flex items-center gap-1.5">
                <User size={12} />
                {pipeline.triggeredBy}
              </span>
              <span className="flex items-center gap-1.5">
                <Clock size={12} />
                {formatDuration(pipeline.durationSeconds)}
              </span>
            </div>

            <p className="text-xs text-ea-muted mt-2 italic">"{pipeline.commitMessage}"</p>
          </div>
        ))}
      </div>
    </div>
  );
}
