export default function TopBar({ title, subtitle, right }) {
  return (
    <header className="sticky top-0 z-20 border-b border-white/10 bg-slate-950/70 px-6 py-5 backdrop-blur-xl sm:px-8">
      <div className="flex flex-col gap-4 xl:flex-row xl:items-center xl:justify-between">
        <div>
          <p className="text-sm font-medium text-cyan-300/80">Production Feature Health Platform</p>
          <h1 className="mt-1 text-3xl font-semibold tracking-tight text-white">{title}</h1>
          <p className="mt-2 max-w-3xl text-sm text-slate-400">{subtitle}</p>
        </div>
        {right}
      </div>
    </header>
  );
}
