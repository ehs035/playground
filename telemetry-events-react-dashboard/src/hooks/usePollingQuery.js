import { useEffect, useState } from 'react';

export function usePollingQuery(queryFn, intervalMs = 5000) {
  const [data, setData] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let mounted = true;
    let timerId;

    const load = async () => {
      try {
        const result = await queryFn();
        if (!mounted) return;
        setData(result);
        setError('');
      } catch (err) {
        if (!mounted) return;
        setError(err instanceof Error ? err.message : 'Unknown request error');
      } finally {
        if (mounted) {
          setIsLoading(false);
        }
      }
    };

    load();
    timerId = setInterval(load, intervalMs);

    return () => {
      mounted = false;
      clearInterval(timerId);
    };
  }, [queryFn, intervalMs]);

  return { data, isLoading, error };
}
