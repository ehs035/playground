import { useEffect, useState } from 'react';
import { getApps } from '../services/appService';

export function useApps() {
  const [data, setData] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    getApps().then((result) => {
      if (mounted) {
        setData(result);
        setIsLoading(false);
      }
    });
    return () => {
      mounted = false;
    };
  }, []);

  return { data, isLoading };
}
