// App.tsx
import { useEffect, useState } from 'react'
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useMsal,
  useAccount,
} from '@azure/msal-react'
import { InteractionStatus, type AccountInfo } from '@azure/msal-browser'
import { apiConfig, loginRequest } from './authConfig'
import './index.css' // Tailwind + component classes

/** -----------------------------
 *  Sign In / Sign Out Buttons
 *  ----------------------------- */
function SignInButton() {
  const { instance } = useMsal()

  const handleLogin = () => {
    instance.loginRedirect(loginRequest)
  }

  return (
    <button className="btn-primary" onClick={handleLogin}>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        className="h-4 w-4"
        viewBox="0 0 24 24"
        fill="currentColor"
      >
        <path d="M12 0C5.371 0 0 5.371 0 12s5.371 12 12 12 12-5.371 12-12S18.629 0 12 0zm0 21.6C6.396 21.6 2.4 17.604 2.4 12S6.396 2.4 12 2.4 21.6 6.396 21.6 12 17.604 21.6 12 21.6z" />
        <path d="M11.4 6h1.2v5.4H18V12h-5.4v6h-1.2v-6H6v-0.6h5.4V6z" />
      </svg>
      Sign in with Microsoft
    </button>
  )
}

function SignOutButton() {
  const { instance, accounts } = useMsal()

  const handleLogout = () => {
    const activeAccount = instance.getActiveAccount() ?? accounts[0]
    instance.logoutRedirect({ account: activeAccount })
  }

  return (
    <button className="btn-secondary" onClick={handleLogout}>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        className="h-4 w-4"
        viewBox="0 0 24 24"
        fill="currentColor"
      >
        <path d="M10 17l5-5-5-5v10z" />
        <path d="M4 4h8v2H6v12h6v2H4z" />
      </svg>
      Sign out
    </button>
  )
}

/** -----------------------------
 *  Active Account Helper
 *  ----------------------------- */
function useActiveAccount(): AccountInfo | null {
  const { instance, accounts } = useMsal()
  const account = useAccount(instance.getActiveAccount() ?? accounts[0])
  return account ?? null
}

/** -----------------------------
 *  Types for API response
 *  ----------------------------- */
type Claim = {
  type: string
  value: string
}

type UserContext = {
  brokerId: string
  agencyId: string
  fullName: string
  isAdmin?: boolean // optional since your sample didn't include it
  role: string
}

type MeResponse = {
  name: string
  roles: string[]
  groups: string[]
  role: string
  // ✅ Support both object and array shapes
  userContext?: UserContext | UserContext[]
  customAttribute?: string
  claims?: Claim[] // optional
}

/** -----------------------------
 *  Hook to call your /me API
 *  ----------------------------- */
function useApiMe() {
  const { instance, inProgress } = useMsal()
  const account = useActiveAccount()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [data, setData] = useState<MeResponse | null>(null)

  useEffect(() => {
    if (!account) {
      setData(null)
      return
    }
    if (inProgress !== InteractionStatus.None) {
      return
    }

    const fetchData = async () => {
      try {
        setLoading(true)
        setError(null)

        const response = await instance.acquireTokenSilent({
          ...loginRequest,
          account,
        })

        const res = await fetch(`${apiConfig.baseUrl}${apiConfig.meEndpoint}`, {
          headers: {
            Authorization: `Bearer ${response.accessToken}`,
          },
        })

        if (!res.ok) {
          const text = await res.text()
          throw new Error(`Graph error ${res.status}: ${text}`)
        }

        const json: MeResponse = await res.json()
        console.log('ME response:', json) // 👈 helps verify shape
        setData(json)
      } catch (e) {
        const message = e instanceof Error ? e.message : String(e)
        setError(message)
      } finally {
        setLoading(false)
      }
    }

    void fetchData()
  }, [account, inProgress, instance])

  return { loading, error, data }
}

/** -----------------------------
 *  Helpers
 *  ----------------------------- */
// Normalize userContext to an array ([], [UserContext], or existing array)
function normalizeUserContext(ctx?: UserContext | UserContext[]): UserContext[] {
  if (!ctx) return []
  return Array.isArray(ctx) ? ctx : [ctx]
}

/** -----------------------------
 *  User Info Card
 *  ----------------------------- */
function UserInfo() {
  const account = useActiveAccount()
  const { loading, error, data } = useApiMe()

  if (!account) {
    return null
  }

  // ✅ normalized contexts (supports object or array)
  const contexts = normalizeUserContext(data?.userContext)
  const primaryCtx = contexts[0]

  return (
    <div className="card">
      <div className="flex items-start justify-between gap-4">
        <div className="flex-1">
          <h2 className="text-lg font-semibold">Signed in as</h2>
          <p className="mt-1 text-sm text-gray-600 dark:text-gray-400">
            <strong className="text-gray-900 dark:text-gray-100">
              {data?.name ?? account.name}
            </strong>
          </p>
          <p className="mono mt-1">{account.username}</p>

          {account.idTokenClaims?.oid && (
            <p className="mono mt-1">
              <span className="pill mr-2">OID</span>
              {account.idTokenClaims.oid}
            </p>
          )}

          {/* Inline primary userContext summary */}
          {!loading && !error && primaryCtx && (
            <div className="mt-3 grid grid-cols-1 gap-2 sm:grid-cols-2 lg:grid-cols-4">
              <div className="badge">
                <span className="mono mr-2">Broker ID:</span>
                {primaryCtx.brokerId}
              </div>
              <div className="badge">
                <span className="mono mr-2">Agency ID:</span>
                {primaryCtx.agencyId}
              </div>
              <div className="badge">
                <span className="mono mr-2">Full Name:</span>
                {primaryCtx.fullName}
              </div>
              <div className="badge">
                <span className="mono mr-2">Role:</span>
                {primaryCtx.role}
              </div>
            </div>
          )}
        </div>

        <div className="hidden sm:block">
          <span className="pill">Microsoft Entra ID</span>
        </div>
      </div>

      {/* Loading / Error */}
      <div className="mt-4">
        {loading && (
          <div
            className="rounded-lg border border-gray-200 p-3 animate-pulse
                        dark:border-neutral-800"
          >
            <p className="text-sm text-gray-600 dark:text-gray-400">
              Loading user information…
            </p>
          </div>
        )}
        {error && <p className="error-text mt-2">{error}</p>}
      </div>

      {/* Full User Context List */}
      <h3 className="section-title">User Context from API call: /api/auth/me</h3>
      {!loading && !error && contexts.length === 0 && (
        <p className="text-sm text-gray-600 dark:text-gray-400">
          No user context returned for this user.
        </p>
      )}
      {!loading && contexts.length > 0 && (
        <ul className="mt-2 space-y-2">
          {contexts.map((ctx, idx) => (
            <li
              key={`${ctx.brokerId}-${ctx.agencyId}-${idx}`}
              className="rounded-lg border border-gray-200 p-3 dark:border-neutral-800"
            >
              <div className="grid grid-cols-1 gap-2 sm:grid-cols-2 lg:grid-cols-4">
                <div className="badge">
                  <span className="mono mr-2">Broker ID:</span>
                  {ctx.brokerId}
                </div>
                <div className="badge">
                  <span className="mono mr-2">Agency ID:</span>
                  {ctx.agencyId}
                </div>
                <div className="badge">
                  <span className="mono mr-2">Full Name:</span>
                  {ctx.fullName}
                </div>
                <div className="badge">
                  <span className="mono mr-2">Role:</span>
                  {ctx.role}
                </div>
              </div>
              {typeof ctx.isAdmin !== 'undefined' && (
                <div className="mt-2 text-xs text-gray-600 dark:text-gray-400">
                  Admin: <span className="mono">{String(ctx.isAdmin)}</span>
                </div>
              )}
            </li>
          ))}
        </ul>
      )}


    </div>
  )
}

/** -----------------------------
 *  App Shell
 *  ----------------------------- */
function App() {
  return (
    <main className="app-container">
      {/* Header */}
      <header className="header">
        <div className="header-inner">
          <div>
            <h1 className="title">Microsoft Entra ID Login</h1>
            <p className="subtitle">Vite + React + MSAL</p>
          </div>

          <AuthenticatedTemplate>
            <div className="actions">
              <SignOutButton />
            </div>
          </AuthenticatedTemplate>
        </div>
      </header>

      {/* Content */}
      <section className="section">
        <AuthenticatedTemplate>
          <UserInfo />
        </AuthenticatedTemplate>

        <UnauthenticatedTemplate>
          <div className="card">
            <p className="text-sm text-gray-700 dark:text-gray-300">
              Please sign in with your Microsoft Entra ID account to view your
              roles, groups, and user context.
            </p>
            <div className="mt-4">
              <SignInButton />
            </div>
          </div>
        </UnauthenticatedTemplate>
      </section>
    </main>
  )
}

export default App