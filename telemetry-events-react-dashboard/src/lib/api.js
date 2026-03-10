const API_BASE_URL = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, '') || 'http://localhost:5000';

async function requestJson(path) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
  });

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status} ${response.statusText}`);
  }

  return response.json();
}

export function getApiBaseUrl() {
  return API_BASE_URL;
}

export async function fetchTelemetrySummary() {
  return requestJson('/api/telemetry/summary');
}

export async function fetchTelemetryEvents() {
  return requestJson('/api/telemetry/events');
}
