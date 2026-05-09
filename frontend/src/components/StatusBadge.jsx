import React from 'react';

const statusColors = {
  Success: 'bg-ea-green/10 text-ea-green border-ea-green/20',
  Running: 'bg-ea-accent/10 text-ea-accent border-ea-accent/20',
  Failed: 'bg-ea-red/10 text-ea-red border-ea-red/20',
  Pending: 'bg-ea-yellow/10 text-ea-yellow border-ea-yellow/20',
  Cancelled: 'bg-ea-muted/10 text-ea-muted border-ea-muted/20',
  Queued: 'bg-ea-yellow/10 text-ea-yellow border-ea-yellow/20',
  Building: 'bg-ea-accent/10 text-ea-accent border-ea-accent/20',
  Active: 'bg-ea-green/10 text-ea-green border-ea-green/20',
  Connected: 'bg-ea-green/10 text-ea-green border-ea-green/20',
  Disconnected: 'bg-ea-muted/10 text-ea-muted border-ea-muted/20',
  Error: 'bg-ea-red/10 text-ea-red border-ea-red/20',
};

const statusDots = {
  Success: 'bg-ea-green',
  Running: 'bg-ea-accent animate-pulse',
  Failed: 'bg-ea-red',
  Pending: 'bg-ea-yellow',
  Cancelled: 'bg-ea-muted',
  Queued: 'bg-ea-yellow animate-pulse',
  Building: 'bg-ea-accent animate-pulse',
  Active: 'bg-ea-green',
  Connected: 'bg-ea-green',
  Disconnected: 'bg-ea-muted',
  Error: 'bg-ea-red animate-pulse',
};

export default function StatusBadge({ status }) {
  const color = statusColors[status] || 'bg-ea-muted/10 text-ea-muted border-ea-muted/20';
  const dot = statusDots[status] || 'bg-ea-muted';

  return (
    <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium border ${color}`}>
      <span className={`w-1.5 h-1.5 rounded-full ${dot}`} />
      {status}
    </span>
  );
}
