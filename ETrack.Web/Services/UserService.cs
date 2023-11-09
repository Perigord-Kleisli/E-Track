using System.Net;
using ETrack.Models.Dtos;
using ETrack.Web.Services.Contracts;

namespace ETrack.Web.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient httpClient;

        public UserService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task UserConfirm(string email)
        {
            try
            {
                var response = await httpClient.PostAsync(
                    $"/api/Auth/confirmation-token?emailToVerify={email}"
                    , new FormUrlEncodedContent (new[] {
                        new KeyValuePair<string,string>("","")
                    })
                    );

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UserLogin(UserLoginDto userLoginDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Auth/login", userLoginDto);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new BadHttpRequestException(message);
                        default:
                            throw new Exception($"Http Status Code: {response.StatusCode}, message: {message}");
                    }
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UserRegister(UserRegisterDto userRegisterDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Auth/register", userRegisterDto);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new BadHttpRequestException(message);
                        default:
                            throw new Exception($"Http Status Code: {response.StatusCode}, message: {message}");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}