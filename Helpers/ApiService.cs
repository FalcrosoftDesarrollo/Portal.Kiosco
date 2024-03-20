using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Kiosco.Helpers
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            // Inicializar HttpClient con la URL base del servicio
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7254/api/");
        }

        public async Task<string> ObtenerDatosCineFans(string documento, string passwrd)
        {
            try
            { 
                var url = $"CineFans/CineFans?Documento={documento}&Passwrd={passwrd}";
                 
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                 
                if (response.IsSuccessStatusCode)
                { 
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                     throw new Exception($"Error al obtener los datos del servicio: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            { 
                throw new Exception($"Error al comunicarse con el servicio: {ex.Message}");
            }
        }
    }
}
