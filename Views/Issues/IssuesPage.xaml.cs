namespace proyectoFinalMoviles.Views.Issues;

using proyectoFinalMoviles.Configurations;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using proyectoFinalMoviles.Models;
using Microsoft.Maui.Storage;
using System.Text;

public partial class IssuesPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public IssuesPage()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(AppConfig.ApiBaseUrl)
        };

        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadIssuesList();
    }

    public async Task<(bool IsSuccess, string ErrorMessage, List<Issue> Issues)> GetIssuesData(string authToken, string companyId)
    {
        var payloadData = new
        {
            company_id = companyId
        };

        try
        {
            Console.WriteLine("Datos enviados: " + JsonSerializer.Serialize(payloadData));

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var payload = JsonSerializer.Serialize(payloadData);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/getIssuesData/", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Contenido de la respuesta del servidor: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Issue>>>(responseContent, options);

                if (apiResponse != null && apiResponse.response != null)
                {
                    return (true, null, apiResponse.response);
                }
                else
                {
                    return (false, "Invalid response format.", null);
                }
            }
            else
            {
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

                return (false, errorMessage, null);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener las novedades: {ex.Message}");
            return (false, $"Exception: {ex.Message}", null);
        }
    }


    private async void AddIssueButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.Issues.AddIssuePage());
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var issue = button?.BindingContext as Issue;

        if (issue != null)
        {
            await Navigation.PushAsync(new EditIssuePage(issue.id));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var issue = button?.BindingContext as Issue;

        if (issue != null)
        {
            var confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas eliminar esta novedad?", "Sí", "No");
            if (confirm)
            {
                await DeleteIssue(issue.id);
            }
        }
    }

    private async Task DeleteIssue(string issueId)
    {
        try
        {
            var authToken = await SecureStorage.GetAsync("authToken");
            if (string.IsNullOrEmpty(authToken))
            {
                await DisplayAlert("Error", "No hay token de autenticación disponible.", "OK");
                return;
            }

            using (var httpClient = new HttpClient { BaseAddress = new Uri(AppConfig.ApiBaseUrl) })
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                var response = await httpClient.DeleteAsync($"/api/deleteIssue/{issueId}/");

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "La novedad fue eliminada correctamente.", "OK");
                    await LoadIssuesList();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"Falló la eliminación de la novedad: {response.StatusCode} - {errorContent}", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al eliminar la novedad: {ex.Message}", "OK");
        }
    }

    private async Task LoadIssuesList()
    {
        try
        {
            var authToken = await SecureStorage.GetAsync("authToken");
            var companyId = "6a2d54ec-cb7b-4630-90ea-3ab66b38b6bf";

            if (string.IsNullOrEmpty(authToken))
            {
                await DisplayAlert("Error", "No authentication token found.", "OK");
                return;
            }

            var (isSuccess, errorMessage, issues) = await GetIssuesData(authToken, companyId);

            if (isSuccess && issues != null)
            {
                IssuesCollectionView.ItemsSource = issues;
            }
            else
            {
                await DisplayAlert("Error", errorMessage ?? "An error occurred.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al cargar la lista: {ex.Message}", "OK");
        }
    }

}
