using proyectoFinalMoviles.Configurations;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using proyectoFinalMoviles.Models;
using Microsoft.Maui.Storage;
using System.Text;

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
                var jsonCredentials = JsonSerializer.Serialize(credentials);

                var content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");

                Console.WriteLine("Datos enviados (JSON): " + jsonCredentials);

                var response = await _httpClient.PostAsync("/api/login/", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Contenido de la respuesta (RAW): " + responseContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Respuesta exitosa. Intentando deserializar...");
                    var responseData = JsonSerializer.Deserialize<AuthResponse>(responseContent);

                    if (responseData != null && responseData.status == 200)
                    {
                        await SecureStorage.SetAsync("authToken", responseData.token);
                        await SecureStorage.SetAsync("hasAuth", "true");

                        //await SecureStorage.SetAsync("companies", responseData.Companies);

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
                    Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                    Console.WriteLine("Contenido del error (RAW): " + responseContent);

                    string errorMessage = "Unknown error occurred.";
                    try
                    {
                        var errorData = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
                        if (errorData?.message != null && errorData.message.Count > 0)
                        {
                            errorMessage = string.Join(", ", errorData.message);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error al deserializar la respuesta de error: {ex.Message}");
                    }

                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al autenticarse: {ex.Message}");
                return (false, $"Exception: {ex.Message}");
            }
        }
    }

}
