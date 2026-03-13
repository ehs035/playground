import { useEffect, useState } from 'react'
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useMsal,
  useAccount,
} from '@azure/msal-react'
import { InteractionStatus, type AccountInfo } from '@azure/msal-browser'
import './App.css'
import { graphConfig, loginRequest } from './authConfig'

type GraphDirectoryObject = {
  id?: string
  displayName?: string
  '@odata.type'?: string
}

type GraphMemberOfResponse = {
  value: GraphDirectoryObject[]
}

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

function useGraphRolesAndGroups() {
  const { instance, inProgress } = useMsal()
  const account = useActiveAccount()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [groups, setGroups] = useState<GraphDirectoryObject[]>([])
  const [roles, setRoles] = useState<GraphDirectoryObject[]>([])

  useEffect(() => {
    if (!account) {
      setGroups([])
      setRoles([])
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

        const res = await fetch(graphConfig.graphMemberOfEndpoint, {
          headers: {
            Authorization: `Bearer ${response.accessToken}`,
          },
        })

        if (!res.ok) {
          const text = await res.text()
          throw new Error(`Graph error ${res.status}: ${text}`)
        }

        const data: GraphMemberOfResponse = await res.json()

        const groupsData = data.value.filter(
          (item) => item['@odata.type']?.includes('group') ?? false,
        )
        const rolesData = data.value.filter(
          (item) => item['@odata.type']?.includes('appRoleAssignment') ?? false,
        )

        setGroups(groupsData)
        setRoles(rolesData)
      } catch (e) {
        const message = e instanceof Error ? e.message : String(e)
        setError(message)
      } finally {
        setLoading(false)
      }
    }

    void fetchData()
  }, [account, inProgress, instance])

  return { loading, error, groups, roles }
}

function UserInfo() {
  const account = useActiveAccount()
  const { loading, error, groups, roles } = useGraphRolesAndGroups()

  if (!account) {
    return null
  }

  return (
    <div className="card">
      <h2>Signed in as</h2>
      <p>
        <strong>{account.name}</strong>
      </p>
      <p>{account.username}</p>

      <h3>Roles</h3>
      {loading && <p>Loading roles and groups...</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && roles.length === 0 && <p>No roles found for this user.</p>}
      {!loading && roles.length > 0 && (
        <ul>
          {roles.map((role) => (
            <li key={role.id ?? role.displayName}>{role.displayName ?? role.id}</li>
          ))}
        </ul>
      )}

      <h3>Groups</h3>
      {!loading && !error && groups.length === 0 && <p>No groups found for this user.</p>}
      {!loading && groups.length > 0 && (
        <ul>
          {groups.map((group) => (
            <li key={group.id ?? group.displayName}>{group.displayName ?? group.id}</li>
          ))}
        </ul>
      )}
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
