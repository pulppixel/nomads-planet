using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Services.Authentication;
using UnityEngine;

namespace NomadsPlanet
{
    public static class AuthenticationWrapper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxTries = 5)
        {
            if (AuthState == AuthState.Authenticated)
            {
                return AuthState;
            }

            AuthState = AuthState.Authenticating;
            
            int tries = 0;
            while (AuthState == AuthState.Authenticating && tries < maxTries)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }

                tries++;
                await Task.Delay(1000);
            }

            return AuthState;
        }
    }
}