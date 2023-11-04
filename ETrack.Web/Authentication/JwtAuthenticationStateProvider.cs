using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ETrack.Web.Authentication
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                string token = await _localStorage.GetItemAsStringAsync("token");

                var identity = new ClaimsIdentity();
                _http.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrEmpty(token))
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("bearer", token.Replace("\"", ""));
                }

                var user = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(user);

                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return state;
            }
            catch (InvalidOperationException)
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)!;

            // Lack of Sum Types makes JSON Desrialization of polymorphic structures a pain
            // There is probably a better way to do this
            var roles = JsonSerializer.Deserialize<IEnumerable<string>>(keyValuePairs[ClaimTypes.Role].ToString()!)!;
            keyValuePairs.Remove(ClaimTypes.Role);

            return keyValuePairs
                .Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!))
                .Concat(roles.Select(role => new Claim (ClaimTypes.Role, role)));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}