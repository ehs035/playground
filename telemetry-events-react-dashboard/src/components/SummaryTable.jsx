function toneClass(avgMs) {
  if (avgMs >= 180) return 'text-rose-300';
  if (avgMs >= 120) return 'text-amber-300';
  return 'text-emerald-300';
}

export default function SummaryTable({ summary = [] }) {
  return (
    <section className="panel overflow-hidden">
      <div className="border-b border-white/10 px-6 py-5">
        <p className="text-sm text-slate-400">Aggregate view</p>
        <h2 className="mt-1 text-xl font-semibold text-white">Feature summary</h2>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full text-left text-sm">
          <thead className="bg-white/[0.03] text-slate-400">
            <tr>
              <th className="px-6 py-4 font-medium">App</th>
              <th className="px-6 py-4 font-medium">Module</th>
              <th className="px-6 py-4 font-medium">Feature</th>
              <th className="px-6 py-4 font-medium">Success</th>
              <th className="px-6 py-4 font-medium">Failure</th>
              <th className="px-6 py-4 font-medium">Timeout</th>
              <th className="px-6 py-4 font-medium">Avg Success (ms)</th>
              <th className="px-6 py-4 font-medium">Min</th>
              <th className="px-6 py-4 font-medium">Max</th>
            </tr>
          </thead>
          <tbody>
            {summary.map((item, index) => (
              <tr
                key={`${item.appName}-${item.moduleName}-${item.featureName}`}
                className={index < summary.length - 1 ? 'border-b border-white/10' : ''}
              >
                <td className="px-6 py-4 text-white">{item.appName}</td>
                <td className="px-6 py-4 text-slate-300">{item.moduleName}</td>
                <td className="px-6 py-4 font-medium text-cyan-300">{item.featureName}</td>
                <td className="px-6 py-4 text-emerald-300">{item.successCount}</td>
                <td className="px-6 py-4 text-rose-300">{item.failureCount}</td>
                <td className="px-6 py-4 text-amber-300">{item.timeoutCount}</td>
                <td className={`px-6 py-4 font-semibold ${toneClass(item.avgSuccessDurationMs ?? 0)}`}>
                  {(item.avgSuccessDurationMs ?? 0).toFixed(2)}
                </td>
                <td className="px-6 py-4 text-slate-300">{item.minSuccessDurationMs ?? '-'}</td>
                <td className="px-6 py-4 text-slate-300">{item.maxSuccessDurationMs ?? '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
