import React, { useState, useEffect } from 'react';
import { FolderKanban, GitBranch, Hammer, ExternalLink } from 'lucide-react';
import StatusBadge from '../components/StatusBadge';
import { getProjects } from '../services/api';

const FALLBACK_PROJECTS = [
  { id: 1, name: 'Nova Frontline', description: 'Next-gen multiplayer FPS', repository: 'studio/nova-frontline', status: 'Active', team: 'Stockholm Studio', pipelines: [{}, {}, {}], builds: [{}, {}] },
  { id: 2, name: 'Striker 26', description: 'Competitive soccer simulation', repository: 'studio/striker-26', status: 'Active', team: 'Vancouver Studio', pipelines: [{}, {}], builds: [{}] },
  { id: 3, name: 'Gridiron 26', description: 'Pro football simulation', repository: 'studio/gridiron-26', status: 'Active', team: 'Orlando Studio', pipelines: [{}], builds: [{}] },
  { id: 4, name: 'Forge Engine', description: 'Core game engine', repository: 'studio/forge-engine', status: 'Active', team: 'Engine Team', pipelines: [{}], builds: [{}] },
  { id: 5, name: 'Nexus Platform', description: 'Game launcher services', repository: 'studio/nexus-platform', status: 'Active', team: 'Platform Team', pipelines: [{}], builds: [] },
];

export default function Projects() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getProjects()
      .then((res) => setProjects(res.data))
      .catch(() => setProjects(FALLBACK_PROJECTS))
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
          <h1 className="text-2xl font-bold text-white">Projects</h1>
          <p className="text-ea-muted mt-1">Manage game development projects</p>
        </div>
        <button className="px-4 py-2 bg-ea-accent text-white rounded-lg text-sm font-medium hover:bg-ea-accent/80 transition-colors">
          + New Project
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
        {projects.map((project) => (
          <div key={project.id} className="bg-ea-card border border-ea-border rounded-xl p-5 hover:border-ea-accent/30 transition-all hover:shadow-lg hover:shadow-ea-accent/5">
            <div className="flex items-start justify-between mb-3">
              <div className="flex items-center gap-2">
                <FolderKanban size={18} className="text-ea-accent" />
                <h3 className="text-base font-semibold text-white">{project.name}</h3>
              </div>
              <StatusBadge status={project.status} />
            </div>

            <p className="text-sm text-ea-muted mb-4">{project.description}</p>

            <div className="flex items-center gap-2 mb-4 text-xs text-ea-muted">
              <ExternalLink size={12} />
              <span className="font-mono">{project.repository}</span>
            </div>

            <div className="flex items-center justify-between pt-3 border-t border-ea-border">
              <span className="text-xs text-ea-muted">{project.team}</span>
              <div className="flex items-center gap-4 text-xs text-ea-muted">
                <span className="flex items-center gap-1">
                  <GitBranch size={12} />
                  {project.pipelines?.length || 0} pipelines
                </span>
                <span className="flex items-center gap-1">
                  <Hammer size={12} />
                  {project.builds?.length || 0} builds
                </span>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
