import axios from 'axios';

const API_BASE = '/api';

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
});

// Dashboard
export const getDashboard = () => api.get('/dashboard');

// Projects
export const getProjects = () => api.get('/projects');
export const getProject = (id) => api.get(`/projects/${id}`);
export const createProject = (data) => api.post('/projects', data);
export const updateProject = (id, data) => api.put(`/projects/${id}`, data);
export const deleteProject = (id) => api.delete(`/projects/${id}`);

// Pipelines
export const getPipelines = () => api.get('/pipelines');
export const getPipeline = (id) => api.get(`/pipelines/${id}`);
export const getPipelinesByProject = (projectId) => api.get(`/pipelines/project/${projectId}`);
export const createPipeline = (data) => api.post('/pipelines', data);
export const updatePipelineStatus = (id, status) => api.put(`/pipelines/${id}/status`, { status });

// Builds
export const getBuilds = () => api.get('/builds');
export const getBuild = (id) => api.get(`/builds/${id}`);
export const getBuildsByProject = (projectId) => api.get(`/builds/project/${projectId}`);
export const createBuild = (data) => api.post('/builds', data);
export const updateBuildStatus = (id, status) => api.put(`/builds/${id}/status`, { status });

// Integrations
export const getIntegrations = () => api.get('/integrations');
export const toggleIntegration = (id) => api.put(`/integrations/${id}/toggle`);
export const getGitHubRepo = (owner, repo) => api.get(`/integrations/github/${owner}/${repo}`);
export const getGitHubCommits = (owner, repo) => api.get(`/integrations/github/${owner}/${repo}/commits`);
export const getJiraIssues = (projectKey) => api.get(`/integrations/jira/${projectKey}/issues`);

// Activity
export const getActivities = (count = 50) => api.get(`/activity?count=${count}`);
export const getActivitiesByType = (type, count = 20) => api.get(`/activity/type/${type}?count=${count}`);

export default api;
