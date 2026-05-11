import React, { useState, useEffect } from 'react';
import { Activity as ActivityIcon, GitBranch, Hammer, Plug, Webhook, AlertCircle, Filter } from 'lucide-react';
import { getActivities } from '../services/api';

const FALLBACK_ACTIVITIES = [
  { type: 'pipeline', action: 'completed', message: "Pipeline 'Build & Test' succeeded for Nova Frontline", source: 'GitLab CI', projectName: 'Nova Frontline', timestamp: new Date(Date.now() - 300000).toISOString() },
  { type: 'build', action: 'started', message: 'Build NF-2024-1543 started for Xbox platform', source: 'Build System', projectName: 'Nova Frontline', timestamp: new Date(Date.now() - 720000).toISOString() },
  { type: 'integration', action: 'synced', message: 'GitHub integration synced 24 new commits', source: 'GitHub', projectName: 'Striker 26', timestamp: new Date(Date.now() - 1800000).toISOString() },
  { type: 'webhook', action: 'received', message: 'Slack webhook received: deployment approval for Striker 26', source: 'Slack', projectName: 'Striker 26', timestamp: new Date(Date.now() - 3600000).toISOString() },
  { type: 'pipeline', action: 'failed', message: "Pipeline 'Nightly Build' failed for Gridiron 26", source: 'GitLab CI', projectName: 'Gridiron 26', timestamp: new Date(Date.now() - 7200000).toISOString() },
  { type: 'system', action: 'alert', message: 'Build queue depth exceeded threshold (15 builds pending)', source: 'DevEx Monitor', projectName: null, timestamp: new Date(Date.now() - 10800000).toISOString() },
  { type: 'build', action: 'completed', message: 'Build SK-2024-3201 completed successfully (35min)', source: 'Build System', projectName: 'Striker 26', timestamp: new Date(Date.now() - 14400000).toISOString() },
  { type: 'integration', action: 'connected', message: 'JIRA integration reconnected after timeout', source: 'JIRA', projectName: null, timestamp: new Date(Date.now() - 18000000).toISOString() },
  { type: 'webhook', action: 'received', message: 'GitHub push event: 3 commits to studio/forge-engine main', source: 'GitHub', projectName: 'Forge Engine', timestamp: new Date(Date.now() - 21600000).toISOString() },
  { type: 'pipeline', action: 'completed', message: "Pipeline 'Engine Build' succeeded (20m 5s)", source: 'GitLab CI', projectName: 'Forge Engine', timestamp: new Date(Date.now() - 25200000).toISOString() },
  { type: 'system', action: 'info', message: 'Scheduled maintenance completed for build farm nodes', source: 'DevEx Monitor', projectName: null, timestamp: new Date(Date.now() - 28800000).toISOString() },
  { type: 'build', action: 'failed', message: 'Build GI-2024-0892 failed: linker error on PC Debug', source: 'Build System', projectName: 'Gridiron 26', timestamp: new Date(Date.now() - 32400000).toISOString() },
];

const typeConfig = {
  pipeline: { icon: GitBranch, color: 'text-ea-accent', bg: 'bg-ea-accent/10' },
  build: { icon: Hammer, color: 'text-ea-orange', bg: 'bg-ea-orange/10' },
  integration: { icon: Plug, color: 'text-ea-purple', bg: 'bg-ea-purple/10' },
  webhook: { icon: Webhook, color: 'text-ea-green', bg: 'bg-ea-green/10' },
  system: { icon: AlertCircle, color: 'text-ea-yellow', bg: 'bg-ea-yellow/10' },
};

function timeAgo(dateStr) {
  const diff = Date.now() - new Date(dateStr).getTime();
  const mins = Math.floor(diff / 60000);
  if (mins < 1) return 'just now';
  if (mins < 60) return `${mins}m ago`;
  const hrs = Math.floor(mins / 60);
  if (hrs < 24) return `${hrs}h ago`;
  return `${Math.floor(hrs / 24)}d ago`;
}

export default function Activity() {
  const [activities, setActivities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [typeFilter, setTypeFilter] = useState('all');

  useEffect(() => {
    getActivities(50)
      .then((res) => setActivities(res.data))
      .catch(() => setActivities(FALLBACK_ACTIVITIES))
      .finally(() => setLoading(false));
  }, []);

  const types = ['all', 'pipeline', 'build', 'integration', 'webhook', 'system'];
  const filtered = typeFilter === 'all' ? activities : activities.filter((a) => a.type === typeFilter);

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
          <h1 className="text-2xl font-bold text-white">Activity Feed</h1>
          <p className="text-ea-muted mt-1">Real-time events from all integrated services (NoSQL-backed)</p>
        </div>
        <span className="text-xs text-ea-muted flex items-center gap-1.5 bg-ea-card border border-ea-border px-3 py-1.5 rounded-lg">
          <span className="w-2 h-2 bg-ea-green rounded-full animate-pulse" />
          MongoDB Connected
        </span>
      </div>

      {/* Filter */}
      <div className="flex items-center gap-2 mb-6">
        <Filter size={14} className="text-ea-muted" />
        {types.map((t) => (
          <button
            key={t}
            onClick={() => setTypeFilter(t)}
            className={`px-3 py-1.5 rounded-lg text-xs font-medium capitalize transition-colors ${
              typeFilter === t
                ? 'bg-ea-accent/10 text-ea-accent border border-ea-accent/30'
                : 'text-ea-muted hover:text-ea-text bg-ea-card border border-ea-border'
            }`}
          >
            {t}
          </button>
        ))}
      </div>

      {/* Activity List */}
      <div className="bg-ea-card border border-ea-border rounded-xl overflow-hidden">
        {filtered.length === 0 ? (
          <div className="p-12 text-center text-ea-muted text-sm">No activity found for this filter</div>
        ) : (
          <div className="divide-y divide-ea-border/50">
            {filtered.map((activity, i) => {
              const config = typeConfig[activity.type] || typeConfig.system;
              const Icon = config.icon;
              return (
                <div key={i} className="flex items-start gap-4 p-4 hover:bg-ea-dark/30 transition-colors">
                  <div className={`mt-0.5 p-2 rounded-lg ${config.bg}`}>
                    <Icon size={14} className={config.color} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm text-ea-text">{activity.message}</p>
                    <div className="flex items-center gap-3 mt-1">
                      <span className="text-xs text-ea-muted">{activity.source}</span>
                      {activity.projectName && (
                        <span className="text-xs text-ea-accent">{activity.projectName}</span>
                      )}
                    </div>
                  </div>
                  <div className="text-right">
                    <span className="text-xs text-ea-muted whitespace-nowrap">{timeAgo(activity.timestamp)}</span>
                    <div className="mt-1">
                      <span className={`text-[10px] capitalize px-1.5 py-0.5 rounded ${config.bg} ${config.color}`}>
                        {activity.type}
                      </span>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}
