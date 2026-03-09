import StatusPill from './StatusPill';

export default function DetailRow({ label, value, tone }) {
  return (
    <div className="flex items-center justify-between rounded-2xl border border-white/10 bg-black/20 px-4 py-4">
      <div>
        <p className="text-sm font-medium text-white">{label}</p>
        <p className="mt-1 text-sm text-slate-400">Application-specific operational check</p>
      </div>
      <div className="flex items-center gap-3">
        <span className="text-sm font-semibold text-white">{value}</span>
        <StatusPill label={tone === 'good' ? 'Good' : tone === 'warning' ? 'Degraded' : 'Critical'} tone={tone} small />
      </div>
    </div>
  );
}
