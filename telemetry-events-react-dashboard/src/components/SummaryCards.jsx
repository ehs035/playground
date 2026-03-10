export default function SummaryCards({ summary = [] }) {
  const totals = summary.reduce(
    (acc, item) => {
      acc.success += item.successCount ?? 0;
      acc.failure += item.failureCount ?? 0;
      acc.timeout += item.timeoutCount ?? 0;
      return acc;
    },
    { success: 0, failure: 0, timeout: 0 }
  );

  const avgResponse =
    summary.length > 0
      ? (
          summary.reduce((acc, item) => acc + (item.avgSuccessDurationMs ?? 0), 0) / summary.length
        ).toFixed(2)
      : '0.00';

  const cards = [
    ['Tracked features', summary.length],
    ['Success count', totals.success],
    ['Failure count', totals.failure],
    ['Timeout count', totals.timeout],
    ['Avg success ms', avgResponse],
  ];

  return (
    <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
      {cards.map(([label, value]) => (
        <div key={label} className="panel p-5">
          <p className="text-sm text-slate-400">{label}</p>
          <p className="mt-3 text-3xl font-semibold text-white">{value}</p>
        </div>
      ))}
    </section>
  );
}
