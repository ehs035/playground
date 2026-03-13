import { useEffect, useState } from 'react'
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useMsal,
  useAccount,
} from '@azure/msal-react'
import { InteractionStatus, type AccountInfo } from '@azure/msal-browser'
import './App.css'
import { apiConfig, loginRequest } from './authConfig'

function SignInButton() {
  const { instance } = useMsal()

  const handleLogin = () => {
    instance.loginRedirect(loginRequest)
  }

  return (
    <button className="primary" onClick={handleLogin}>
      Sign in with Microsoft
    </button>
  )
}

function SignOutButton() {
  const { instance, accounts } = useMsal()

  const handleLogout = () => {
    const activeAccount = instance.getActiveAccount() ?? accounts[0]
    instance.logoutRedirect({
      account: activeAccount,
    })
  }

  return (
    <button className="secondary" onClick={handleLogout}>
      Sign out
    </button>
  )
}

function useActiveAccount(): AccountInfo | null {
  const { instance, accounts } = useMsal()
  const account = useAccount(instance.getActiveAccount() ?? accounts[0])
  return account ?? null
}

type MeResponse = {
  name: string
  roles: string[]
  groups: string[]
  customAttribute?: string
}

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

function UserInfo() {
  const account = useActiveAccount()
  const { loading, error, data } = useApiMe()

  if (!account) {
    return null
  }

  return (
    <div className="card">
      <h2>Signed in as</h2>
      <p>
        <strong>{data?.name ?? account.name}</strong>
      </p>
      <p>{account.username}</p>

      <h3>Roles</h3>
      {loading && <p>Loading roles, groups and attributes...</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && (data?.roles?.length ?? 0) === 0 && (
        <p>No roles found for this user.</p>
      )}
      {!loading && (data?.roles?.length ?? 0) > 0 && (
        <ul>
          {data?.roles.map((role) => (
            <li key={role}>{role}</li>
          ))}
        </ul>
      )}

      <h3>Groups</h3>
      {!loading && !error && (data?.groups?.length ?? 0) === 0 && (
        <p>No groups found for this user.</p>
      )}
      {!loading && (data?.groups?.length ?? 0) > 0 && (
        <ul>
          {data?.groups.map((group) => (
            <li key={group}>{group}</li>
          ))}
        </ul>
      )}

      <h3>Custom attribute</h3>
      {!loading && !error && !data?.customAttribute && (
        <p>No custom attribute found for this user.</p>
      )}
      {data?.customAttribute && <p>{data.customAttribute}</p>}
    </div>
  )
}

function App() {
  return (
    <main className="app-container">
      <header>
        <h1>Microsoft Entra ID Login (Vite + React)</h1>
      </header>

      <section>
        <AuthenticatedTemplate>
          <div className="actions">
            <SignOutButton />
          </div>
          <UserInfo />
        </AuthenticatedTemplate>

        <UnauthenticatedTemplate>
          <p>Please sign in with your Microsoft Entra ID account to view your roles and groups.</p>
          <SignInButton />
        </UnauthenticatedTemplate>
      </section>
    </main>
  )
}

export default App
