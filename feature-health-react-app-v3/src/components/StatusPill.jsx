const toneMap = {
  good: 'bg-emerald-500/15 text-emerald-300 ring-emerald-400/20',
  warning: 'bg-amber-500/15 text-amber-300 ring-amber-400/20',
  bad: 'bg-rose-500/15 text-rose-300 ring-rose-400/20',
};

export default function StatusPill({ label, tone = 'good', small = false }) {
  return (
    <span className={`rounded-full ring-1 ${toneMap[tone]} ${small ? 'px-2.5 py-1 text-[11px]' : 'px-3 py-1.5 text-xs'}`}>
      {label}
    </span>
  );
}
