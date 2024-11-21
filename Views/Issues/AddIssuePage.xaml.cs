namespace proyectoFinalMoviles.Views.Issues;

using proyectoFinalMoviles.Configurations;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using proyectoFinalMoviles.Models;
using Microsoft.Maui.Storage;
using System.Text;
using System.ComponentModel.Design;

public partial class AddIssuePage : ContentPage
{

    private readonly HttpClient _httpClient;
    private string _photoPath;
    public AddIssuePage()
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
        await LoadBatchPickerData();
    }

    private async Task LoadBatchPickerData()
    {
        try
        {
            var payloadData = new
            {
                catalog_id = "4c14d530-3783-484f-b14c-b83f9575b4ef",
                company_id = "6a2d54ec-cb7b-4630-90ea-3ab66b38b6bf"
            };

            var authToken = await SecureStorage.GetAsync("authToken");
            if (string.IsNullOrEmpty(authToken))
            {
                await DisplayAlert("Error", "No hay token de autenticación disponible.", "OK");
                return;
            }

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
                    BatchPicker.ItemsSource = apiResponse.response;

                    BatchPicker.ItemDisplayBinding = new Binding("fields.layer_name");
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

            var selectedFeature = BatchPicker.SelectedItem as Feature;
            var batchId = string.Empty;

            if (selectedFeature != null)
            {
                batchId = selectedFeature.pk;
            }
            else
            {
                await DisplayAlert("Error", "No se seleccionó un lote válido.", "OK");
                return;
            }

            var content = new MultipartFormDataContent
        {
            { new StringContent(batchId), "batch_id" },
            { new StringContent(MessageEntry.Text), "message" },
            { new StringContent(TypeEntry.Text), "type" }
        };

            if (!string.IsNullOrEmpty(_photoPath) && File.Exists(_photoPath))
            {
                var fileStream = File.OpenRead(_photoPath);
                content.Add(new StreamContent(fileStream), "evidence", Path.GetFileName(_photoPath));
            }

            using (var httpClient = new HttpClient { BaseAddress = new Uri(AppConfig.ApiBaseUrl) })
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                var response = await httpClient.PostAsync("/api/createIssue/", content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "La novedad fue creada correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"Falló la creación de la novedad: {response.StatusCode} - {errorContent}", "OK");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("Error de conexión", $"Error al conectar con el servidor: {ex.Message}", "OK");
        }
        catch (IOException ex)
        {
            await DisplayAlert("Error de archivo", $"Error al manejar el archivo de evidencia: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error inesperado: {ex.Message}", "OK");
        }
    }

}