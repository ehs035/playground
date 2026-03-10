export default function IntegrationGuide() {
  return (
    <section className="panel p-6">
      <p className="text-sm text-slate-400">How to wire this into your current React app</p>
      <h2 className="mt-1 text-xl font-semibold text-white">Integration notes</h2>

      <div className="mt-4 space-y-4 text-sm leading-6 text-slate-300">
        <div>
          <p className="font-medium text-white">1. Copy the API helper</p>
          <p>
            Move <code>src/lib/api.js</code> into your existing app and point
            <code>VITE_API_BASE_URL</code> to your ASP.NET Core telemetry API.
          </p>
        </div>

        <div>
          <p className="font-medium text-white">2. Copy the polling hook</p>
          <p>
            Move <code>src/hooks/usePollingQuery.js</code> and use it on your
            overview page, alert page, or app detail page.
          </p>
        </div>

        <div>
          <p className="font-medium text-white">3. Reuse the summary and events components</p>
          <p>
            Import <code>SummaryCards</code>, <code>SummaryTable</code>, and
            <code>EventsTable</code> directly into an existing route or dashboard tab.
          </p>
        </div>

        <div>
          <p className="font-medium text-white">4. Use your existing shell</p>
          <p>
            In a real product, you usually keep your existing sidebar and app shell, then mount these
            components inside a route like <code>/telemetry</code>.
          </p>
        </div>

        <div>
          <p className="font-medium text-white">5. Start with polling</p>
          <p>
            Polling every 5–15 seconds is enough for most dashboards. Later, replace polling with SignalR
            notifications and keep the same summary/event APIs.
          </p>
        </div>
      </div>
    </section>
  );
}
