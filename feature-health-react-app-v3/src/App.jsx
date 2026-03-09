import { Navigate, Route, Routes } from 'react-router-dom';
import AppShell from './layout/AppShell';
import OverviewPage from './pages/OverviewPage';
import ApplicationsPage from './pages/ApplicationsPage';
import ApplicationDetailPage from './pages/ApplicationDetailPage';
import PlaceholderPage from './pages/PlaceholderPage';

export default function App() {
  return (
    <Routes>
      <Route element={<AppShell />}>
        <Route path="/" element={<OverviewPage />} />
        <Route path="/applications" element={<ApplicationsPage />} />
        <Route path="/applications/:appId" element={<ApplicationDetailPage />} />
        <Route path="/alerts" element={<PlaceholderPage title="Alerts" />} />
        <Route path="/deployments" element={<PlaceholderPage title="Deployments" />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
