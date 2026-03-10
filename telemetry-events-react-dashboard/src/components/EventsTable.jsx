function outcomeBadgeClass(outcome) {
  switch ((outcome || '').toLowerCase()) {
    case 'success':
      return 'bg-emerald-500/15 text-emerald-300 ring-emerald-400/20';
    case 'timeout':
      return 'bg-amber-500/15 text-amber-300 ring-amber-400/20';
    case 'failure':
      return 'bg-rose-500/15 text-rose-300 ring-rose-400/20';
    default:
      return 'bg-cyan-500/15 text-cyan-300 ring-cyan-400/20';
  }
}

export default function EventsTable({ events = [] }) {
  return (
    <section className="panel overflow-hidden">
      <div className="border-b border-white/10 px-6 py-5">
        <p className="text-sm text-slate-400">Raw event stream</p>
        <h2 className="mt-1 text-xl font-semibold text-white">Latest telemetry events</h2>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full text-left text-sm">
          <thead className="bg-white/[0.03] text-slate-400">
            <tr>
              <th className="px-6 py-4 font-medium">Occurred</th>
              <th className="px-6 py-4 font-medium">App</th>
              <th className="px-6 py-4 font-medium">Module</th>
              <th className="px-6 py-4 font-medium">Feature</th>
              <th className="px-6 py-4 font-medium">Outcome</th>
              <th className="px-6 py-4 font-medium">Duration (ms)</th>
              <th className="px-6 py-4 font-medium">Correlation</th>
              <th className="px-6 py-4 font-medium">Trace</th>
            </tr>
          </thead>
          <tbody>
            {events.map((item, index) => (
              <tr
                key={`${item.occurredAtUtc}-${item.featureName}-${index}`}
                className={index < events.length - 1 ? 'border-b border-white/10' : ''}
              >
                <td className="px-6 py-4 text-slate-300">{item.occurredAtUtc}</td>
                <td className="px-6 py-4 text-white">{item.appName}</td>
                <td className="px-6 py-4 text-slate-300">{item.moduleName}</td>
                <td className="px-6 py-4 font-medium text-cyan-300">{item.featureName}</td>
                <td className="px-6 py-4">
                  <span className={`badge ${outcomeBadgeClass(item.outcome)}`}>{item.outcome}</span>
                </td>
                <td className="px-6 py-4 text-slate-300">{item.durationMs ?? '-'}</td>
                <td className="px-6 py-4 font-mono text-xs text-slate-400">{item.correlationId ?? '-'}</td>
                <td className="px-6 py-4 font-mono text-xs text-slate-400">{item.traceId ?? '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
