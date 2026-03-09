import { useEffect, useState } from 'react';
import { getAppById } from '../services/appService';

export function useApp(appId) {
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    setIsLoading(true);
    getAppById(appId).then((result) => {
      if (mounted) {
        setData(result);
        setIsLoading(false);
      }
    });
    return () => {
      mounted = false;
    };
  }, [appId]);

  return { data, isLoading };
}
