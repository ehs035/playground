export default function Header({ apiBaseUrl }) {
  return (
    <header className="border-b border-white/10 bg-slate-950/70 px-6 py-5 backdrop-blur-xl sm:px-8">
      <div className="flex flex-col gap-4 xl:flex-row xl:items-end xl:justify-between">
        <div>
          <p className="text-sm font-medium text-cyan-300/80">Telemetry Events Dashboard</p>
          <h1 className="mt-1 text-3xl font-semibold tracking-tight text-white">
            Raw events and aggregate telemetry summary
          </h1>
          <p className="mt-2 max-w-3xl text-sm text-slate-400">
            This Vite + React + Tailwind example reads telemetry summary and raw event data from your
            existing ASP.NET Core telemetry API.
          </p>
        </div>

        <div className="panel px-4 py-3 text-sm text-slate-300">
          API base URL: <span className="font-semibold text-white">{apiBaseUrl}</span>
        </div>
      </div>
    </header>
  );
}
