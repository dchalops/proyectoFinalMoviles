using proyectoFinalMoviles.Configurations;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using proyectoFinalMoviles.Models;
using Microsoft.Maui.Storage;
using System.ComponentModel.Design;
using System.Text;

namespace proyectoFinalMoviles.Views;

public partial class OrdersWorkPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public OrdersWorkPage()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(AppConfig.ApiBaseUrl)
        };

        InitializeComponent();
        _ = LoadPlannersAsync();
    }

    private async Task LoadPlannersAsync()
    {
        await GetAllPlannersAsync();
    }

    private async Task GetAllPlannersAsync()
    {
        try
        {
            var companyId = "6a2d54ec-cb7b-4630-90ea-3ab66b38b6bf";
            //_httpClient.DefaultRequestHeaders.Add("company_id", companyId.ToString());
            //var response = await _httpClient.GetAsync("/programmer/getAllPlanners/");
            var response = await _httpClient.PostAsync("/programmer/getAllPlanners/",
            new StringContent(JsonSerializer.Serialize(new { company_id = companyId }), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Contenido de la respuesta: " + responseContent);

                var responseData = JsonSerializer.Deserialize<AuthResponse>(responseContent);

                if (responseData != null && responseData.status == 200)
                {
                    await SecureStorage.SetAsync("authToken", responseData.token);
                    await SecureStorage.SetAsync("hasAuth", "true");
                    Console.WriteLine("Datos almacenados correctamente.");
                }
                else
                {
                    Console.WriteLine("No se pudo deserializar la respuesta o el estado no es 200.");
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

                Console.WriteLine($"Error: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener los datos: {ex.Message}");
        }
    }
}
