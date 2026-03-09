import { useMemo, useState } from 'react';
import { statusMeta } from '../data/apps';
import { useApps } from '../hooks/useApps';
import TopBar from '../components/TopBar';
import SearchBar from '../components/SearchBar';
import Tabs from '../components/Tabs';
import AppOverviewTile from '../components/AppOverviewTile';
import LoadingCard from '../components/LoadingCard';

const tabs = [
  { value: 'all', label: 'All apps' },
  { value: 'good', label: 'Healthy' },
  { value: 'warning', label: 'Degraded' },
  { value: 'bad', label: 'Critical' },
];

export default function ApplicationsPage() {
  const { data, isLoading } = useApps();
  const [query, setQuery] = useState('');
  const [filter, setFilter] = useState('all');

  const filteredApps = useMemo(() => {
    return data.filter((app) => {
      const matchesStatus = filter === 'all' ? true : app.status === filter;
      const q = query.toLowerCase();
      const matchesQuery = app.name.toLowerCase().includes(q) || app.owner.toLowerCase().includes(q) || app.shortName.toLowerCase().includes(q);
      return matchesStatus && matchesQuery;
    });
  }, [data, query, filter]);

  return (
    <>
      <TopBar title="Applications" subtitle="Search, filter, and open the application-specific health pages. This is where the detail workflow begins." right={<div className="rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-slate-300">{isLoading ? 'Loading...' : `${filteredApps.length} applications`}</div>} />
      <div className="space-y-6 px-6 py-8 sm:px-8">
        <section className="grid gap-4 xl:grid-cols-[1fr_auto] xl:items-center"><SearchBar value={query} onChange={setQuery} placeholder="Search Find a Doctor, PQA, PHI Form, PDR..." /><Tabs tabs={tabs} active={filter} onChange={setFilter} /></section>
        <section className="grid gap-5 lg:grid-cols-2 2xl:grid-cols-3">{isLoading ? Array.from({ length: 6 }).map((_, i) => <LoadingCard key={i} />) : filteredApps.map((app) => <AppOverviewTile key={app.id} app={app} meta={statusMeta[app.status]} />)}</section>
      </div>
    </>
  );
}
