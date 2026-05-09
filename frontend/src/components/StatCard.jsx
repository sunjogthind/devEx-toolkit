import React from 'react';

export default function StatCard({ title, value, subtitle, icon: Icon, color = 'text-ea-accent' }) {
  return (
    <div className="bg-ea-card border border-ea-border rounded-xl p-5 hover:border-ea-accent/30 transition-colors">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-ea-muted font-medium">{title}</p>
          <p className={`text-3xl font-bold mt-1 ${color}`}>{value}</p>
          {subtitle && <p className="text-xs text-ea-muted mt-1">{subtitle}</p>}
        </div>
        {Icon && (
          <div className={`p-2.5 rounded-lg bg-ea-dark ${color}`}>
            <Icon size={20} />
          </div>
        )}
      </div>
    </div>
  );
}
