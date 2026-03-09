import StatusPill from './StatusPill';

export default function EventList({ items = [] }) {
  return (
    <div className="space-y-3">
      {items.map((item) => (
        <div key={`${item.time}-${item.feature}`} className="rounded-2xl border border-white/10 bg-black/20 p-4">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
            <div>
              <div className="flex items-center gap-3">
                <span className="text-sm font-medium text-white">{item.feature}</span>
                <StatusPill label={item.outcome} tone={item.outcome === 'Success' ? 'good' : item.outcome === 'Warning' ? 'warning' : 'bad'} small />
              </div>
              <p className="mt-2 text-sm text-slate-400">{item.module} · {item.note}</p>
            </div>
            <div className="text-sm text-slate-500">{item.time}</div>
          </div>
        </div>
      ))}
    </div>
  );
}
