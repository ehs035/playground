import { Link, useParams } from 'react-router-dom';
import TopBar from '../components/TopBar';
import StatusPill from '../components/StatusPill';
import DetailRow from '../components/DetailRow';
import EventList from '../components/EventList';
import PageLoader from '../components/PageLoader';
import { useApp } from '../hooks/useApp';
import { statusMeta } from '../data/apps';

export default function ApplicationDetailPage() {
  const { appId } = useParams();
  const { data: app, isLoading } = useApp(appId);

  if (isLoading) {
    return <><TopBar title="Application Detail" subtitle="Loading application health context..." /><PageLoader /></>;
  }

  if (!app) {
    return <div className="px-8 py-12"><div className="panel p-8"><h1 className="text-2xl font-semibold text-white">Application not found</h1><Link to="/applications" className="mt-4 inline-block text-cyan-300">Back to applications →</Link></div></div>;
  }

  const meta = statusMeta[app.status];

  return (
    <>
      <TopBar title={app.name} subtitle="Application-specific tab for operational checks, recent records, and health context." right={<div className="flex items-center gap-3"><Link to="/applications" className="rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-slate-300 transition hover:bg-white/10">Back to applications</Link><StatusPill label={meta.label} tone={app.status} /></div>} />
      <div className="space-y-6 px-6 py-8 sm:px-8">
        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <div className="panel p-5"><p className="text-sm text-slate-400">Health score</p><p className="mt-3 text-4xl font-semibold text-white">{app.healthScore}</p></div>
          <div className="panel p-5"><p className="text-sm text-slate-400">Owner</p><p className="mt-3 text-2xl font-semibold text-white">{app.owner}</p></div>
          <div className="panel p-5"><p className="text-sm text-slate-400">Live users</p><p className="mt-3 text-4xl font-semibold text-white">{app.liveUsers}</p></div>
          <div className="panel p-5"><p className="text-sm text-slate-400">Transactions today</p><p className="mt-3 text-4xl font-semibold text-white">{app.transactions}</p></div>
        </section>
        <section className="grid gap-6 xl:grid-cols-[1.05fr_0.95fr]">
          <div className="panel p-6"><div className="flex items-center justify-between"><div><p className="text-sm text-slate-400">Operational checks</p><h2 className="mt-1 text-2xl font-semibold text-white">Application health signals</h2></div><div className={`rounded-full px-3 py-1 text-xs ring-1 ${meta.badge}`}>{meta.label}</div></div><p className="mt-4 max-w-2xl text-sm leading-6 text-slate-300">{app.summary}</p><div className="mt-6 space-y-3">{app.checks.map((check) => <DetailRow key={check.label} label={check.label} value={check.value} tone={check.state} />)}</div></div>
          <div className="panel p-6"><div className="flex items-center justify-between"><div><p className="text-sm text-slate-400">Recent records</p><h2 className="mt-1 text-2xl font-semibold text-white">Latest application events</h2></div><div className="rounded-2xl border border-white/10 bg-white/5 px-4 py-2 text-sm text-slate-400">App detail tab</div></div><div className="mt-6"><EventList items={app.details} /></div></div>
        </section>
      </div>
    </>
  );
}
