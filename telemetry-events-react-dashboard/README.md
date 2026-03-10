# Telemetry Events React Dashboard

A small Vite + React + Tailwind app that displays:
- telemetry summary from `/api/telemetry/summary`
- raw telemetry events from `/api/telemetry/events`

It is designed to wire into your existing ASP.NET Core sample API.

## Run locally

```bash
npm install
cp .env.example .env
npm run dev
```

## Environment variables

```bash
VITE_API_BASE_URL=http://localhost:5000
VITE_POLL_INTERVAL_MS=5000
```

Change `VITE_API_BASE_URL` to your current telemetry API URL.

## Current backend endpoints used

- `GET /api/telemetry/summary`
- `GET /api/telemetry/events`

## How to wire this into your current React app

### Option 1: Copy the pieces you need
Copy these files into your existing app:

- `src/lib/api.js`
- `src/hooks/usePollingQuery.js`
- `src/components/SummaryCards.jsx`
- `src/components/SummaryTable.jsx`
- `src/components/EventsTable.jsx`

Then create a route/page in your current dashboard and call the same fetchers.

### Option 2: Mount the page inside your existing router
Take the contents of `src/App.jsx` and move them into a page like:

```jsx
src/pages/TelemetryPage.jsx
```

Then add a route such as:

```jsx
<Route path="/telemetry" element={<TelemetryPage />} />
```

### Option 3: Replace polling later with SignalR
Keep the same REST endpoints, but use SignalR only to notify React when data changed.
Then invalidate/refetch the same summary and events queries.

## Notes
This example is intentionally simple:
- no UI framework besides Tailwind
- no React Query dependency
- no auth
- no SignalR yet
