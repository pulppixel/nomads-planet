using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace NomadsPlanet
{
    public static class AuthenticationWrapper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxTries = 10)
        {
            if (AuthState == AuthState.Authenticated)
            {
                return AuthState;
            }

            if (AuthState == AuthState.Authenticating)
            {
                CustomFunc.ConsoleLog("이미 인증되었습니다!!");
                await Authenticating();
                return AuthState;
            }

            await SignInAnonymouslyAsync(maxTries);

            return AuthState;
        }

        private static async Task Authenticating()
        {
            while (AuthState is AuthState.Authenticating or AuthState.NotAuthenticated)
            {
                await Task.Delay(250);
            }
        }

        private static async Task SignInAnonymouslyAsync(int maxRetries)
        {
            AuthState = AuthState.Authenticating;

            int retries = 0;
            while (AuthState == AuthState.Authenticating && retries < maxRetries)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        AuthState = AuthState.Authenticated;
                        break;
                    }
                }
                catch (AuthenticationException authException)
                {
                    CustomFunc.ConsoleLog(authException, true);
                    AuthState = AuthState.Error;
                }
                catch (RequestFailedException requestException)
                {
                    CustomFunc.ConsoleLog(requestException, true);
                    AuthState = AuthState.Error;
                }

                retries++;
                await Task.Delay(1500);
            }

            if (AuthState != AuthState.Authenticated)
            {
                CustomFunc.ConsoleLog($"플레이어가 제대로 로그인하지 못했습니다. {retries} 회 시도");
                AuthState = AuthState.TimeOut;
            }
        }
    }
}