import React, { useState, useEffect } from 'react';
import { Plug, Github, MessageSquare, Ticket, GitlabIcon, RefreshCw, ExternalLink } from 'lucide-react';
import StatusBadge from '../components/StatusBadge';
import { getIntegrations, toggleIntegration, getJiraIssues } from '../services/api';

const FALLBACK_INTEGRATIONS = [
  { id: 1, provider: 'GitHub', name: 'GitHub Enterprise', status: 'Connected', webhookUrl: '/api/webhooks/github' },
  { id: 2, provider: 'Slack', name: 'Studio Slack Workspace', status: 'Connected', webhookUrl: '/api/webhooks/slack' },
  { id: 3, provider: 'JIRA', name: 'JIRA Cloud', status: 'Connected', webhookUrl: '/api/webhooks/jira' },
  { id: 4, provider: 'GitLab', name: 'Engine Team GitLab', status: 'Disconnected', webhookUrl: '/api/webhooks/gitlab' },
];

const providerMeta = {
  GitHub: { icon: Github, color: 'text-white', bg: 'bg-gray-800', desc: 'Repository hosting, PR management, and CI/CD triggers' },
  Slack: { icon: MessageSquare, color: 'text-[#E01E5A]', bg: 'bg-[#E01E5A]/10', desc: 'Team notifications, build alerts, and deployment approvals' },
  JIRA: { icon: Ticket, color: 'text-[#0052CC]', bg: 'bg-[#0052CC]/10', desc: 'Issue tracking, sprint management, and backlog sync' },
  GitLab: { icon: GitlabIcon, color: 'text-[#FC6D26]', bg: 'bg-[#FC6D26]/10', desc: 'Source control, CI pipelines, and merge request tracking' },
};

const MOCK_JIRA_ISSUES = [
  { key: 'BF-101', summary: 'Implement new matchmaking algorithm', status: 'In Progress', issueType: 'Story', priority: 'High' },
  { key: 'BF-102', summary: 'Fix memory leak in asset loader', status: 'Open', issueType: 'Bug', priority: 'Critical' },
  { key: 'BF-103', summary: 'Add telemetry for player sessions', status: 'In Review', issueType: 'Task', priority: 'Medium' },
  { key: 'BF-104', summary: 'Update CI pipeline for ARM builds', status: 'Done', issueType: 'Task', priority: 'Low' },
  { key: 'BF-105', summary: 'Optimize texture streaming for open world', status: 'In Progress', issueType: 'Story', priority: 'High' },
];

const priorityColors = {
  Critical: 'text-ea-red',
  High: 'text-ea-orange',
  Medium: 'text-ea-yellow',
  Low: 'text-ea-green',
};

export default function Integrations() {
  const [integrations, setIntegrations] = useState([]);
  const [jiraIssues, setJiraIssues] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activePanel, setActivePanel] = useState(null);

  useEffect(() => {
    Promise.all([
      getIntegrations().catch(() => ({ data: FALLBACK_INTEGRATIONS })),
      getJiraIssues('BF').catch(() => ({ data: MOCK_JIRA_ISSUES })),
    ]).then(([intRes, jiraRes]) => {
      setIntegrations(intRes.data);
      setJiraIssues(jiraRes.data);
      setLoading(false);
    });
  }, []);

  const handleToggle = async (id) => {
    try {
      const res = await toggleIntegration(id);
      setIntegrations((prev) => prev.map((i) => (i.id === id ? res.data : i)));
    } catch {
      setIntegrations((prev) =>
        prev.map((i) =>
          i.id === id ? { ...i, status: i.status === 'Connected' ? 'Disconnected' : 'Connected' } : i
        )
      );
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="w-8 h-8 border-2 border-ea-accent border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-white">Integrations</h1>
        <p className="text-ea-muted mt-1">Connect and manage external services</p>
      </div>

      {/* Integration Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
        {integrations.map((integration) => {
          const meta = providerMeta[integration.provider] || {};
          const Icon = meta.icon || Plug;

          return (
            <div key={integration.id} className="bg-ea-card border border-ea-border rounded-xl p-5 hover:border-ea-accent/20 transition-colors">
              <div className="flex items-start justify-between mb-3">
                <div className="flex items-center gap-3">
                  <div className={`p-2.5 rounded-lg ${meta.bg || 'bg-ea-dark'}`}>
                    <Icon size={20} className={meta.color || 'text-ea-muted'} />
                  </div>
                  <div>
                    <h3 className="text-sm font-semibold text-white">{integration.name}</h3>
                    <p className="text-xs text-ea-muted">{integration.provider}</p>
                  </div>
                </div>
                <StatusBadge status={integration.status} />
              </div>

              <p className="text-xs text-ea-muted mb-4">{meta.desc || 'External service integration'}</p>

              <div className="flex items-center gap-2 mb-4">
                <span className="text-xs font-mono text-ea-muted bg-ea-dark px-2 py-1 rounded">
                  {integration.webhookUrl}
                </span>
              </div>

              <div className="flex items-center gap-2 pt-3 border-t border-ea-border">
                <button
                  onClick={() => handleToggle(integration.id)}
                  className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
                    integration.status === 'Connected'
                      ? 'bg-ea-red/10 text-ea-red hover:bg-ea-red/20'
                      : 'bg-ea-green/10 text-ea-green hover:bg-ea-green/20'
                  }`}
                >
                  {integration.status === 'Connected' ? 'Disconnect' : 'Connect'}
                </button>
                <button
                  onClick={() => setActivePanel(activePanel === integration.provider ? null : integration.provider)}
                  className="px-3 py-1.5 rounded-lg text-xs font-medium text-ea-muted bg-ea-dark hover:text-ea-text transition-colors"
                >
                  Details
                </button>
              </div>
            </div>
          );
        })}
      </div>

      {/* JIRA Issues Panel */}
      <div className="bg-ea-card border border-ea-border rounded-xl p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-sm font-semibold text-white flex items-center gap-2">
            <Ticket size={16} className="text-[#0052CC]" />
            JIRA Issues — Nova Frontline (NF)
          </h3>
          <button className="text-xs text-ea-muted hover:text-ea-text flex items-center gap-1">
            <RefreshCw size={12} /> Refresh
          </button>
        </div>

        <div className="space-y-2">
          {jiraIssues.map((issue) => (
            <div key={issue.key} className="flex items-center gap-4 p-3 rounded-lg hover:bg-ea-dark/50 transition-colors">
              <span className="text-xs font-mono text-ea-accent min-w-[70px]">{issue.key}</span>
              <span className={`text-xs font-medium min-w-[55px] ${priorityColors[issue.priority] || 'text-ea-muted'}`}>
                {issue.priority}
              </span>
              <span className="text-xs text-ea-muted min-w-[50px]">{issue.issueType}</span>
              <span className="text-sm text-ea-text flex-1">{issue.summary}</span>
              <span className="text-xs px-2 py-0.5 rounded bg-ea-dark text-ea-muted">{issue.status}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
