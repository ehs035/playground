import { Link } from 'react-router-dom';

function scoreTone(score) {
  if (score >= 90) return 'text-emerald-300';
  if (score >= 75) return 'text-amber-300';
  return 'text-rose-300';
}

export default function AppOverviewTile({ app, meta }) {
  return (
    <Link to={`/applications/${app.id}`} className="group relative overflow-hidden rounded-[28px] border border-white/10 bg-white/[0.04] p-5 text-left transition duration-300 hover:border-white/15 hover:bg-white/[0.06]">
      <div className={`absolute inset-x-0 top-0 h-1.5 bg-gradient-to-r ${meta.accent}`} />
      <div className="flex items-start justify-between gap-4">
        <div className="flex items-center gap-3">
          <div className={`grid h-12 w-12 place-items-center rounded-2xl bg-gradient-to-br ${meta.accent} text-sm font-semibold text-white`}>{app.shortName}</div>
          <div>
            <p className="text-sm text-slate-400">Application</p>
            <h3 className="text-lg font-semibold text-white">{app.name}</h3>
          </div>
        </div>
        <span className={`rounded-full px-3 py-1 text-xs ring-1 ${meta.badge}`}>{meta.label}</span>
      </div>
      <div className="mt-6 flex items-end justify-between">
        <div>
          <p className="text-sm text-slate-400">Health score</p>
          <div className={`mt-2 text-5xl font-semibold ${scoreTone(app.healthScore)}`}>{app.healthScore}</div>
        </div>
        <div className="rounded-2xl border border-white/10 bg-black/20 px-4 py-3 text-right">
          <p className="text-xs uppercase tracking-[0.22em] text-slate-500">Issues</p>
          <p className="mt-1 text-2xl font-semibold text-white">{app.issues}</p>
        </div>
      </div>
      <div className="mt-6 grid grid-cols-2 gap-3">
        <div className="rounded-2xl border border-white/10 bg-black/20 p-4"><p className="text-xs uppercase tracking-[0.18em] text-slate-500">Users</p><p className="mt-2 text-lg font-semibold text-white">{app.liveUsers}</p></div>
        <div className="rounded-2xl border border-white/10 bg-black/20 p-4"><p className="text-xs uppercase tracking-[0.18em] text-slate-500">Transactions</p><p className="mt-2 text-lg font-semibold text-white">{app.transactions}</p></div>
      </div>
      <p className="mt-5 text-sm leading-6 text-slate-400">{app.summary}</p>
      <div className="mt-5 flex items-center justify-between text-sm"><span className="text-slate-500">{app.owner}</span><span className="font-medium text-cyan-300 transition group-hover:text-cyan-200">View details →</span></div>
    </Link>
  );
}
