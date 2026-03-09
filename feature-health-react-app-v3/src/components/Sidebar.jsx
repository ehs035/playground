import { NavLink } from 'react-router-dom';

const navItems = [
  { to: '/', label: 'Overview' },
  { to: '/applications', label: 'Applications' },
  { to: '/alerts', label: 'Alerts' },
  { to: '/deployments', label: 'Deployments' },
];

export default function Sidebar() {
  return (
    <aside className="hidden w-72 shrink-0 border-r border-white/10 bg-black/20 p-6 backdrop-blur-2xl lg:flex lg:flex-col">
      <div className="rounded-3xl border border-white/10 bg-white/[0.04] p-5">
        <div className="flex items-center gap-4">
          <div className="grid h-14 w-14 place-items-center rounded-2xl bg-gradient-to-br from-cyan-400 via-sky-500 to-violet-600 text-lg font-semibold text-white shadow-glow">FH</div>
          <div>
            <p className="text-xs uppercase tracking-[0.24em] text-cyan-300/70">Production Ops</p>
            <h2 className="mt-1 text-lg font-semibold text-white">Feature Health</h2>
          </div>
        </div>
      </div>
      <nav className="mt-8 space-y-2">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              `flex w-full items-center gap-3 rounded-2xl px-4 py-3 text-left text-sm transition ${
                isActive
                  ? 'bg-gradient-to-r from-cyan-500/20 to-violet-500/20 text-white ring-1 ring-cyan-400/20'
                  : 'text-slate-300 hover:bg-white/5 hover:text-white'
              }`
            }
          >
            <span className="h-2.5 w-2.5 rounded-full bg-current opacity-80" />
            {item.label}
          </NavLink>
        ))}
      </nav>
      <div className="mt-auto rounded-3xl border border-white/10 bg-white/[0.04] p-5">
        <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Shell</p>
        <p className="mt-3 text-lg font-semibold text-white">Layout route ready</p>
        <p className="mt-2 text-sm leading-6 text-slate-400">Mock services and loading states are included so the app behaves more like a real product.</p>
      </div>
    </aside>
  );
}
