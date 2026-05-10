import React, { useState, useEffect } from 'react';
import {
  FolderKanban,
  GitBranch,
  Hammer,
  Plug,
  TrendingUp,
  Clock,
  Activity,
} from 'lucide-react';
import { PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';
import StatCard from '../components/StatCard';
import { getDashboard } from '../services/api';

const PIE_COLORS = ['#3fb950', '#f85149', '#58a6ff', '#d29922', '#8b949e'];

function timeAgo(dateStr) {
  const diff = Date.now() - new Date(dateStr).getTime();
  const mins = Math.floor(diff / 60000);
  if (mins < 1) return 'just now';
  if (mins < 60) return `${mins}m ago`;
  const hrs = Math.floor(mins / 60);
  if (hrs < 24) return `${hrs}h ago`;
  return `${Math.floor(hrs / 24)}d ago`;
}

const typeIcons = {
  pipeline: GitBranch,
  build: Hammer,
  integration: Plug,
  webhook: Activity,
  system: Activity,
};

const typeColors = {
  pipeline: 'text-ea-accent',
  build: 'text-ea-orange',
  integration: 'text-ea-purple',
  webhook: 'text-ea-green',
  system: 'text-ea-yellow',
};

export default function Dashboard() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getDashboard()
      .then((res) => setData(res.data))
      .catch(() => {
        setData({
          totalProjects: 5,
          activePipelines: 1,
          totalBuilds: 5,
          connectedIntegrations: 3,
          pipelineSuccessRate: 75.0,
          avgBuildDurationMinutes: 28.3,
          pipelinesByStatus: [
            { status: 'Success', count: 3 },
            { status: 'Running', count: 1 },
            { status: 'Failed', count: 1 },
            { status: 'Cancelled', count: 1 },
          ],
          buildsByStatus: [
            { status: 'Success', count: 3 },
            { status: 'Building', count: 1 },
            { status: 'Failed', count: 1 },
          ],
          recentActivity: [
            { type: 'pipeline', message: "Pipeline 'Build & Test' succeeded for Nova Frontline", source: 'GitLab CI', timestamp: new Date(Date.now() - 300000).toISOString() },
            { type: 'build', message: 'Build NF-2024-1543 started for Xbox platform', source: 'Build System', timestamp: new Date(Date.now() - 720000).toISOString() },
            { type: 'integration', message: 'GitHub integration synced 24 new commits', source: 'GitHub', timestamp: new Date(Date.now() - 1800000).toISOString() },
            { type: 'webhook', message: 'Slack webhook received: deployment approval for Striker 26', source: 'Slack', timestamp: new Date(Date.now() - 3600000).toISOString() },
            { type: 'pipeline', message: "Pipeline 'Nightly Build' failed for Gridiron 26", source: 'GitLab CI', timestamp: new Date(Date.now() - 7200000).toISOString() },
            { type: 'system', message: 'Build queue depth exceeded threshold (15 builds pending)', source: 'DevEx Monitor', timestamp: new Date(Date.now() - 10800000).toISOString() },
          ],
        });
      })
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="w-8 h-8 border-2 border-ea-accent border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!data) return null;

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-white">Dashboard</h1>
        <p className="text-ea-muted mt-1">Overview of your game development environment</p>
      </div>

      {/* Stat Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <StatCard title="Projects" value={data.totalProjects} subtitle="Active game projects" icon={FolderKanban} color="text-ea-accent" />
        <StatCard title="Active Pipelines" value={data.activePipelines} subtitle="Currently running" icon={GitBranch} color="text-ea-green" />
        <StatCard title="Total Builds" value={data.totalBuilds} subtitle="Across all platforms" icon={Hammer} color="text-ea-orange" />
        <StatCard title="Integrations" value={data.connectedIntegrations} subtitle="Connected services" icon={Plug} color="text-ea-purple" />
      </div>

      {/* Metrics Row */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
        <StatCard title="Pipeline Success Rate" value={`${data.pipelineSuccessRate}%`} subtitle="Across all pipelines" icon={TrendingUp} color="text-ea-green" />
        <StatCard title="Avg Build Duration" value={`${data.avgBuildDurationMinutes}m`} subtitle="Mean build time" icon={Clock} color="text-ea-yellow" />
      </div>

      {/* Charts Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Pipeline Status Chart */}
        <div className="bg-ea-card border border-ea-border rounded-xl p-6">
          <h3 className="text-sm font-semibold text-white mb-4">Pipeline Status Distribution</h3>
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={data.pipelinesByStatus}
                  cx="50%"
                  cy="50%"
                  innerRadius={60}
                  outerRadius={90}
                  dataKey="count"
                  nameKey="status"
                  strokeWidth={0}
                >
                  {data.pipelinesByStatus.map((_, i) => (
                    <Cell key={i} fill={PIE_COLORS[i % PIE_COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip
                  contentStyle={{ backgroundColor: '#161b22', border: '1px solid #30363d', borderRadius: 8, color: '#c9d1d9' }}
                  itemStyle={{ color: '#c9d1d9' }}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
          <div className="flex flex-wrap gap-3 mt-2 justify-center">
            {data.pipelinesByStatus.map((item, i) => (
              <div key={item.status} className="flex items-center gap-1.5 text-xs text-ea-muted">
                <span className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: PIE_COLORS[i % PIE_COLORS.length] }} />
                {item.status} ({item.count})
              </div>
            ))}
          </div>
        </div>

        {/* Build Status Chart */}
        <div className="bg-ea-card border border-ea-border rounded-xl p-6">
          <h3 className="text-sm font-semibold text-white mb-4">Build Status Overview</h3>
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={data.buildsByStatus}>
                <XAxis dataKey="status" tick={{ fill: '#8b949e', fontSize: 12 }} axisLine={false} tickLine={false} />
                <YAxis tick={{ fill: '#8b949e', fontSize: 12 }} axisLine={false} tickLine={false} allowDecimals={false} />
                <Tooltip
                  contentStyle={{ backgroundColor: '#161b22', border: '1px solid #30363d', borderRadius: 8, color: '#c9d1d9' }}
                  itemStyle={{ color: '#c9d1d9' }}
                />
                <Bar dataKey="count" radius={[6, 6, 0, 0]}>
                  {data.buildsByStatus.map((entry, i) => (
                    <Cell
                      key={i}
                      fill={
                        entry.status === 'Success' ? '#3fb950' :
                        entry.status === 'Failed' ? '#f85149' :
                        entry.status === 'Building' ? '#58a6ff' : '#d29922'
                      }
                    />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Activity Feed */}
      <div className="bg-ea-card border border-ea-border rounded-xl p-6">
        <h3 className="text-sm font-semibold text-white mb-4">Recent Activity</h3>
        <div className="space-y-3">
          {data.recentActivity.map((item, i) => {
            const Icon = typeIcons[item.type] || Activity;
            const color = typeColors[item.type] || 'text-ea-muted';
            return (
              <div key={i} className="flex items-start gap-3 p-3 rounded-lg hover:bg-ea-dark/50 transition-colors">
                <div className={`mt-0.5 ${color}`}>
                  <Icon size={16} />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm text-ea-text">{item.message}</p>
                  <p className="text-xs text-ea-muted mt-0.5">{item.source}</p>
                </div>
                <span className="text-xs text-ea-muted whitespace-nowrap">{timeAgo(item.timestamp)}</span>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
