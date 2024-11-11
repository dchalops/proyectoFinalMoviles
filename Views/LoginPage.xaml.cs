using proyectoFinalMoviles.Configurations;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using proyectoFinalMoviles.Models;
using Microsoft.Maui.Storage;

namespace proyectoFinalMoviles.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(AppConfig.ApiBaseUrl)
            };
        }

        public async Task<(bool IsAuthenticated, string ErrorMessage)> AuthenticateAsync(string username, string password)
        {
            var credentials = new
            {
                email = username,
                password = password
            };

            try
            {
                Console.WriteLine("Datos enviados: " + JsonSerializer.Serialize(credentials));

                await Task.Delay(1);

                var response = await _httpClient.PostAsJsonAsync("/api/login/", credentials);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Contenido de la respuesta: " + responseContent);

                    var responseData = JsonSerializer.Deserialize<AuthResponse>(responseContent);

                    if (responseData != null && responseData.status == 200)
                    {
                        await SecureStorage.SetAsync("authToken", responseData.token);
                        await SecureStorage.SetAsync("hasAuth", "true");

                        return (true, null);
                    }
                    else
                    {
                        Console.WriteLine("No se pudo deserializar la respuesta o el estado no es 200.");
                        return (false, "Invalid response or status code.");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error de respuesta: " + response.StatusCode);
                    Console.WriteLine("Contenido del error: " + errorContent);

                    string errorMessage;
                    try
                    {
                        var errorData = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
                        errorMessage = errorData?.message?[0] ?? "Unknown error occurred.";
                    }
                    catch
                    {
                        errorMessage = "Unknown error occurred.";
                    }

                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al autenticarse: {ex.Message}");
                return (false, $"Exception: {ex.Message}");
            }
        }
    }
}
