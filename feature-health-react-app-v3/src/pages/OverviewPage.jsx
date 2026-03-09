import TopBar from '../components/TopBar';
import StatusPill from '../components/StatusPill';
import AppOverviewTile from '../components/AppOverviewTile';
import PageLoader from '../components/PageLoader';
import { useApps } from '../hooks/useApps';
import { statusMeta } from '../data/apps';

export default function OverviewPage() {
  const { data, isLoading } = useApps();

  if (isLoading) {
    return <><TopBar title="Application Health Overview" subtitle="Loading mock application health data..." /><PageLoader /></>;
  }

  const healthyApps = data.filter((app) => app.status === 'good').length;
  const degradedApps = data.filter((app) => app.status === 'warning').length;
  const criticalApps = data.filter((app) => app.status === 'bad').length;
  const avgHealthScore = Math.round(data.reduce((total, app) => total + app.healthScore, 0) / data.length);

  return (
    <>
      <TopBar
        title="Application Health Overview"
        subtitle="Modern production overview focused on application health scores. Detailed telemetry and drill-down records live inside the application-specific view."
        right={<div className="glass flex items-center gap-3 rounded-2xl px-4 py-3 shadow-glow"><div className="rounded-xl bg-white/10 px-3 py-2 text-sm text-slate-300">Environment: <span className="font-semibold text-white">Production</span></div><div className="rounded-xl bg-white/10 px-3 py-2 text-sm text-slate-300">Last refresh: <span className="font-semibold text-white">09:45 AM</span></div></div>}
      />
      <div className="space-y-8 px-6 py-8 sm:px-8">
        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <div className="panel p-5"><p className="text-sm text-slate-400">Average health score</p><div className="mt-3 flex items-end gap-3"><span className="text-4xl font-semibold text-white">{avgHealthScore}</span><span className="mb-1 text-sm text-emerald-300">overall stable</span></div></div>
          <div className="panel p-5"><p className="text-sm text-slate-400">Healthy apps</p><div className="mt-3 flex items-end gap-3"><span className="text-4xl font-semibold text-white">{healthyApps}</span><span className="mb-1 text-sm text-slate-400">green status</span></div></div>
          <div className="panel p-5"><p className="text-sm text-slate-400">Degraded apps</p><div className="mt-3 flex items-end gap-3"><span className="text-4xl font-semibold text-white">{degradedApps}</span><span className="mb-1 text-sm text-amber-300">needs review</span></div></div>
          <div className="panel p-5"><p className="text-sm text-slate-400">Critical apps</p><div className="mt-3 flex items-end gap-3"><span className="text-4xl font-semibold text-white">{criticalApps}</span><span className="mb-1 text-sm text-rose-300">attention required</span></div></div>
        </section>
        <section className="panel overflow-hidden bg-mesh p-6 shadow-glow">
          <div className="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
            <div>
              <p className="text-sm uppercase tracking-[0.28em] text-cyan-300/75">Overview experience</p>
              <h2 className="mt-3 text-2xl font-semibold text-white">Modern application tiles with color-coded health score</h2>
              <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-300">The overview page stays intentionally high level. Every application appears as a visual health tile with a score, status, usage snapshot, and issue count.</p>
            </div>
            <div className="flex gap-2"><StatusPill label="Green = good" tone="good" /><StatusPill label="Amber = degraded" tone="warning" /><StatusPill label="Red = critical" tone="bad" /></div>
          </div>
        </section>
        <section>
          <div className="mb-4 flex items-center justify-between"><div><p className="text-sm text-slate-400">Overview tab</p><h2 className="text-2xl font-semibold text-white">Application health tiles</h2></div><div className="rounded-2xl border border-white/10 bg-white/5 px-4 py-2 text-sm text-slate-400">Details removed from overview</div></div>
          <div className="grid gap-5 lg:grid-cols-2 2xl:grid-cols-4">{data.map((app) => <AppOverviewTile key={app.id} app={app} meta={statusMeta[app.status]} />)}</div>
        </section>
      </div>
    </>
  );
}
