export default function PageLoader() {
  return (
    <div className="space-y-6 px-6 py-8 sm:px-8">
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {[1,2,3,4].map((i) => <div key={i} className="panel p-5"><div className="skeleton h-4 w-28" /><div className="mt-4 skeleton h-12 w-20" /></div>)}
      </div>
      <div className="panel p-6"><div className="skeleton h-6 w-48" /><div className="mt-4 skeleton h-4 w-full" /><div className="mt-2 skeleton h-4 w-3/4" /></div>
    </div>
  );
}
