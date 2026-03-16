import { LogLevel, type Configuration } from '@azure/msal-browser'

// TODO: Replace the placeholder values below with your own Entra ID app registration values.
// - clientId: The Application (client) ID of your app registration
// - authority: Your tenant authority, e.g. https://login.microsoftonline.com/{tenantId}
// - redirectUri: The SPA redirect URI you configured in the app registration

export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AAD_CLIENT_ID ?? '3dcd5b92-d5ff-4b7f-913e-a23125ba5b2c',
    authority:
      import.meta.env.VITE_AAD_AUTHORITY ??
      'https://login.microsoftonline.com/fedf3975-ea80-43b4-a875-9c3d9cad1b8e',
    redirectUri: "http://localhost:5173/" ?? window.location.origin,
    postLogoutRedirectUri:
      import.meta.env.VITE_AAD_POST_LOGOUT_REDIRECT_URI ?? window.location.origin,
  },
  cache: {
    cacheLocation: 'sessionStorage',
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) {
          return
        }
        switch (level) {
          case LogLevel.Error:
            console.error(message)
            return
          case LogLevel.Warning:
            console.warn(message)
            return
          case LogLevel.Info:
            console.info(message)
            return
          case LogLevel.Verbose:
            console.debug(message)
            return
        }
      },
    },
  },
}

export const loginRequest = {
  scopes: [import.meta.env.VITE_API_SCOPE ?? 'https://deviehporg.onmicrosoft.com/3dcd5b92-d5ff-4b7f-913e-a23125ba5b2c/User.Read'],
}

export const apiConfig = {
  baseUrl: import.meta.env.VITE_API_BASE ?? 'https://localhost:5001',
  meEndpoint: '/api/me',
}

