import TopBar from '../components/TopBar';

export default function PlaceholderPage({ title }) {
  return (
    <>
      <TopBar title={title} subtitle="Placeholder view included so the app feels like a real routed product shell." right={<div className="rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-slate-300">Coming next</div>} />
      <div className="px-6 py-8 sm:px-8"><div className="panel p-8"><p className="text-sm uppercase tracking-[0.22em] text-slate-500">Placeholder</p><h2 className="mt-3 text-2xl font-semibold text-white">{title}</h2><p className="mt-3 max-w-2xl text-sm leading-6 text-slate-400">This page is intentionally lightweight for now. The route structure is in place so you can keep expanding the product with dedicated alert, deployment, and dependency workflows.</p></div></div>
    </>
  );
}
