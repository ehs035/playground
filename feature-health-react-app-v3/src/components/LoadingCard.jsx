export default function LoadingCard() {
  return (
    <div className="panel p-5">
      <div className="skeleton h-4 w-24" />
      <div className="mt-5 skeleton h-12 w-28" />
      <div className="mt-6 grid grid-cols-2 gap-3">
        <div className="skeleton h-20 w-full" />
        <div className="skeleton h-20 w-full" />
      </div>
      <div className="mt-5 skeleton h-4 w-full" />
      <div className="mt-2 skeleton h-4 w-4/5" />
    </div>
  );
}
