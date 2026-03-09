export default function Tabs({ tabs, active, onChange }) {
  return (
    <div className="inline-flex rounded-2xl border border-white/10 bg-white/[0.04] p-1">
      {tabs.map((tab) => (
        <button
          key={tab.value}
          onClick={() => onChange(tab.value)}
          className={`rounded-xl px-4 py-2 text-sm transition ${
            active === tab.value ? 'bg-white text-slate-900' : 'text-slate-300 hover:bg-white/5 hover:text-white'
          }`}
        >
          {tab.label}
        </button>
      ))}
    </div>
  );
}
