import { fetchTelemetryEvents, fetchTelemetrySummary, getApiBaseUrl } from './lib/api';
import { usePollingQuery } from './hooks/usePollingQuery';
import Header from './components/Header';
import SummaryCards from './components/SummaryCards';
import SummaryTable from './components/SummaryTable';
import EventsTable from './components/EventsTable';
import IntegrationGuide from './components/IntegrationGuide';

const POLL_INTERVAL_MS = Number(import.meta.env.VITE_POLL_INTERVAL_MS || 5000);

export default function App() {
  const {
    data: summary,
    isLoading: isLoadingSummary,
    error: summaryError,
  } = usePollingQuery(fetchTelemetrySummary, POLL_INTERVAL_MS);

  const {
    data: events,
    isLoading: isLoadingEvents,
    error: eventsError,
  } = usePollingQuery(fetchTelemetryEvents, POLL_INTERVAL_MS);

  const isLoading = isLoadingSummary || isLoadingEvents;
  const error = summaryError || eventsError;

  return (
    <div className="min-h-screen">
      <Header apiBaseUrl={getApiBaseUrl()} />

      <main className="space-y-8 px-6 py-8 sm:px-8">
        {error && (
          <div className="panel border-rose-400/20 bg-rose-500/10 p-5 text-sm text-rose-200">
            Failed to load telemetry data: {error}
          </div>
        )}

        <SummaryCards summary={summary} />

        {isLoading ? (
          <div className="panel p-6 text-slate-400">Loading telemetry data...</div>
        ) : (
          <>
            <SummaryTable summary={summary} />
            <EventsTable events={events} />
          </>
        )}

        <IntegrationGuide />
      </main>
    </div>
  );
}
