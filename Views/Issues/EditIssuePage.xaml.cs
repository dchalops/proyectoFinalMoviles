namespace proyectoFinalMoviles.Views.Issues;

using proyectoFinalMoviles.Configurations;
using proyectoFinalMoviles.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
public partial class EditIssuePage : ContentPage
{
    private readonly HttpClient _httpClient;
    private string _photoPath;
    private string _issueId;

    public EditIssuePage(string issueId)
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(AppConfig.ApiBaseUrl) };
        _issueId = issueId;
        LoadIssueForEditing();
    }

    private async void LoadIssueForEditing()
    {
        try
        {
            var authToken = await SecureStorage.GetAsync("authToken");
            if (string.IsNullOrEmpty(authToken))
            {
                await DisplayAlert("Error", "No hay token de autenticación disponible.", "OK");
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.GetAsync($"/api/editIssue/{_issueId}/");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<Issue>>(responseContent, options);

                if (apiResponse != null && apiResponse.response != null)
                {
                    var issue = apiResponse.response;

                    MessageEntry.Text = issue.message;
                    TypeEntry.Text = issue.type;

                    await LoadBatchPickerData(issue.batch_id);

                    if (!string.IsNullOrEmpty(issue.evidence))
                    {
                        var baseUrl = AppConfig.ApiBaseUrl.TrimEnd('/');
                        EvidenceImage.Source = $"{baseUrl}{issue.evidence}";
                    }
                    else
                    {
                        EvidenceImage.Source = null;
                    }

                }
                else
                {
                    await DisplayAlert("Error", "No se pudieron cargar los datos del registro.", "OK");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Error al cargar los datos: {errorContent}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al cargar los datos: {ex.Message}", "OK");
        }
    }

    private async Task LoadBatchPickerData(string selectedBatchId = null)
    {
        try
        {
            var authToken = await SecureStorage.GetAsync("authToken");
            if (string.IsNullOrEmpty(authToken))
            {
                await DisplayAlert("Error", "No hay token de autenticación disponible.", "OK");
                return;
            }

            var payloadData = new
            {
                catalog_id = "4c14d530-3783-484f-b14c-b83f9575b4ef",
                company_id = "6a2d54ec-cb7b-4630-90ea-3ab66b38b6bf"
            };

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var payload = JsonSerializer.Serialize(payloadData);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/getFeatureByCatalog/", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Feature>>>(responseContent, options);

                if (apiResponse != null && apiResponse.response != null)
                {
                    var features = apiResponse.response;

                    BatchPicker.ItemsSource = features;


                    if (!string.IsNullOrEmpty(selectedBatchId))
                    {
                        var selectedFeature = features.FirstOrDefault(f => f.pk == selectedBatchId);
                        if (selectedFeature != null)
                        {
                            BatchPicker.SelectedItem = selectedFeature;
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la lista de lotes.", "OK");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Falló la carga de datos: {errorContent}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al cargar los datos: {ex.Message}", "OK");
        }
    }
    private async void CapturePhotoButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                _photoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(_photoPath))
                    await stream.CopyToAsync(newStream);

                EvidenceImage.Source = ImageSource.FromFile(_photoPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo capturar la foto: {ex.Message}", "OK");
        }
    }

    private async void SubmitIssueButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (BatchPicker.SelectedItem == null || string.IsNullOrWhiteSpace(MessageEntry.Text) || string.IsNullOrWhiteSpace(TypeEntry.Text))
            {
                await DisplayAlert("Error", "Por favor llena todos los campos.", "OK");
                return;
            }

            var authToken = await SecureStorage.GetAsync("authToken");
            if (string.IsNullOrEmpty(authToken))
            {
                await DisplayAlert("Error", "No hay token de autenticación disponible.", "OK");
                return;
            }

            var selectedBatch = BatchPicker.SelectedItem as Feature;
            if (selectedBatch == null)
            {
                await DisplayAlert("Error", "No se seleccionó un lote válido.", "OK");
                return;
            }

            var batchId = selectedBatch.pk;

            var content = new MultipartFormDataContent
        {
            { new StringContent(MessageEntry.Text), "message" },
            { new StringContent(TypeEntry.Text), "type" },
            { new StringContent(batchId), "batch_id" }
        };

            if (!string.IsNullOrEmpty(_photoPath) && File.Exists(_photoPath))
            {
                var fileStream = File.OpenRead(_photoPath);
                content.Add(new StreamContent(fileStream), "evidence", Path.GetFileName(_photoPath));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.PostAsync($"/api/updateIssue/{_issueId}/", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Éxito", "La novedad fue actualizada correctamente.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Falló la actualización de la novedad: {response.StatusCode} - {errorContent}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al actualizar la novedad: {ex.Message}", "OK");
        }
    }
}