import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import Dashboard from './pages/Dashboard';
import Projects from './pages/Projects';
import Pipelines from './pages/Pipelines';
import Builds from './pages/Builds';
import Integrations from './pages/Integrations';
import Activity from './pages/Activity';

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/projects" element={<Projects />} />
        <Route path="/pipelines" element={<Pipelines />} />
        <Route path="/builds" element={<Builds />} />
        <Route path="/integrations" element={<Integrations />} />
        <Route path="/activity" element={<Activity />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  );
}
