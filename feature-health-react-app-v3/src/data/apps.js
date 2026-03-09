export const appData = [
  {
    id: 'find-a-doctor',
    name: 'Find a Doctor',
    shortName: 'FAD',
    owner: 'Digital Experience',
    healthScore: 94,
    status: 'good',
    liveUsers: '12.4k',
    transactions: '8,942',
    issues: 2,
    summary: 'Search, provider lookup, and detail pages are operating normally.',
    checks: [
      { label: 'Search success rate', value: '99.2%', state: 'good' },
      { label: 'Provider detail latency', value: '243ms', state: 'good' },
      { label: 'Timeout signals', value: '2 alerts', state: 'warning' }
    ],
    details: [
      { time: '09:42', feature: 'ProviderSearch', module: 'Search', outcome: 'Success', note: 'Query completed normally' },
      { time: '09:31', feature: 'ProviderDetail', module: 'Doctor Profile', outcome: 'Success', note: 'Detail page rendered normally' },
      { time: '09:18', feature: 'MapLookup', module: 'Locations', outcome: 'Warning', note: 'Latency slightly elevated' }
    ]
  },
  {
    id: 'pqa',
    name: 'PQA',
    shortName: 'PQA',
    owner: 'Quality Programs',
    healthScore: 88,
    status: 'warning',
    liveUsers: '3.1k',
    transactions: '2,184',
    issues: 5,
    summary: 'Core workflows are healthy, with elevated validation failures in one module.',
    checks: [
      { label: 'Measure submission rate', value: '96.8%', state: 'good' },
      { label: 'Validation failures', value: '18 today', state: 'warning' },
      { label: 'Timeout signals', value: '0', state: 'good' }
    ],
    details: [
      { time: '09:37', feature: 'MeasureSubmit', module: 'Submission', outcome: 'Success', note: 'Measure posted successfully' },
      { time: '09:25', feature: 'EligibilityCheck', module: 'Validation', outcome: 'Failure', note: 'Invalid payload detected' },
      { time: '09:07', feature: 'MemberLookup', module: 'Search', outcome: 'Success', note: 'Lookup completed' }
    ]
  },
  {
    id: 'phi-form',
    name: 'PHI Form',
    shortName: 'PHI',
    owner: 'Member Services',
    healthScore: 67,
    status: 'bad',
    liveUsers: '618',
    transactions: '712',
    issues: 14,
    summary: 'Submission flow is degraded due to intermittent timeout and dependency errors.',
    checks: [
      { label: 'Submit success rate', value: '82.4%', state: 'bad' },
      { label: 'Dependency availability', value: '91.0%', state: 'warning' },
      { label: 'Timeout signals', value: '11 alerts', state: 'bad' }
    ],
    details: [
      { time: '09:44', feature: 'SubmitPHIForm', module: 'Submission', outcome: 'Timeout', note: 'Downstream endpoint timed out' },
      { time: '09:33', feature: 'AttachmentUpload', module: 'Files', outcome: 'Failure', note: 'Blob storage error' },
      { time: '09:14', feature: 'ConsentValidation', module: 'Validation', outcome: 'Success', note: 'Validation completed' }
    ]
  },
  {
    id: 'pdr',
    name: 'PDR',
    shortName: 'PDR',
    owner: 'Provider Operations',
    healthScore: 97,
    status: 'good',
    liveUsers: '5.7k',
    transactions: '4,301',
    issues: 1,
    summary: 'System is stable with low operational risk and strong feature completion.',
    checks: [
      { label: 'Document retrieval', value: '99.7%', state: 'good' },
      { label: 'Search latency', value: '181ms', state: 'good' },
      { label: 'Timeout signals', value: '1 alert', state: 'good' }
    ],
    details: [
      { time: '09:41', feature: 'DocumentSearch', module: 'Search', outcome: 'Success', note: 'Returned results successfully' },
      { time: '09:29', feature: 'DownloadDocument', module: 'Documents', outcome: 'Success', note: 'Download succeeded' },
      { time: '09:08', feature: 'ViewerLaunch', module: 'Viewer', outcome: 'Success', note: 'Viewer loaded normally' }
    ]
  }
];

export const statusMeta = {
  good: {
    label: 'Healthy',
    badge: 'bg-emerald-500/15 text-emerald-300 ring-emerald-400/20',
    accent: 'from-emerald-400 via-cyan-400 to-sky-500'
  },
  warning: {
    label: 'Degraded',
    badge: 'bg-amber-500/15 text-amber-300 ring-amber-400/20',
    accent: 'from-amber-300 via-orange-400 to-amber-500'
  },
  bad: {
    label: 'Critical',
    badge: 'bg-rose-500/15 text-rose-300 ring-rose-400/20',
    accent: 'from-rose-400 via-fuchsia-500 to-pink-500'
  }
};
