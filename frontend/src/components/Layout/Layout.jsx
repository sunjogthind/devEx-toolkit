import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  FolderKanban,
  GitBranch,
  Hammer,
  Plug,
  Activity,
  Gamepad2,
} from 'lucide-react';

const navItems = [
  { to: '/', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/projects', icon: FolderKanban, label: 'Projects' },
  { to: '/pipelines', icon: GitBranch, label: 'Pipelines' },
  { to: '/builds', icon: Hammer, label: 'Builds' },
  { to: '/integrations', icon: Plug, label: 'Integrations' },
  { to: '/activity', icon: Activity, label: 'Activity' },
];

export default function Layout({ children }) {
  return (
    <div className="flex h-screen overflow-hidden">
      {/* Sidebar */}
      <aside className="w-64 bg-ea-card border-r border-ea-border flex flex-col">
        <div className="p-5 border-b border-ea-border">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 bg-ea-accent rounded-lg flex items-center justify-center">
              <Gamepad2 size={20} className="text-white" />
            </div>
            <div>
              <h1 className="text-base font-bold text-white">DevEx Toolkit</h1>
              <p className="text-xs text-ea-muted">Game Dev Experience</p>
            </div>
          </div>
        </div>

        <nav className="flex-1 p-3 space-y-1">
          {navItems.map(({ to, icon: Icon, label }) => (
            <NavLink
              key={to}
              to={to}
              end={to === '/'}
              className={({ isActive }) =>
                `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                  isActive
                    ? 'bg-ea-accent/10 text-ea-accent'
                    : 'text-ea-muted hover:text-ea-text hover:bg-ea-dark'
                }`
              }
            >
              <Icon size={18} />
              {label}
            </NavLink>
          ))}
        </nav>

        <div className="p-4 border-t border-ea-border">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-ea-purple/20 flex items-center justify-center text-ea-purple text-sm font-bold">
              DX
            </div>
            <div>
              <p className="text-sm text-white font-medium">DevEx Platform</p>
              <p className="text-xs text-ea-muted">v1.0.0</p>
            </div>
          </div>
        </div>
      </aside>

      {/* Main content */}
      <main className="flex-1 overflow-auto bg-ea-dark">
        <div className="p-8">{children}</div>
      </main>
    </div>
  );
}
