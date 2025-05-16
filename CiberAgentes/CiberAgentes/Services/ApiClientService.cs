using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CiberAgentes.Services
{
    /// <summary>
    /// Servicio para realizar peticiones a la API PHP
    /// </summary>
    public class ApiClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBaseUrl;

        /// <summary>
        /// Constructor con inyección de dependencias
        /// </summary>
        public ApiClientService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContextAccessor = httpContextAccessor;

            // Obtener la URL base de la API de la configuración
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];

            // Configurar cliente HTTP
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Realiza una petición GET a la API
        /// </summary>
        /// <typeparam name="T">Tipo del objeto de respuesta</typeparam>
        /// <param name="endpoint">Endpoint de la API sin la URL base</param>
        /// <returns>Objeto deserializado de la respuesta</returns>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            // Configurar el token de autenticación si el usuario está autenticado
            await SetAuthorizationHeader();

            // Hacer la solicitud
            HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/{endpoint}");

            // Procesar la respuesta
            return await HandleResponse<T>(response);
        }

        /// <summary>
        /// Realiza una petición POST a la API
        /// </summary>
        /// <typeparam name="T">Tipo del objeto de respuesta</typeparam>
        /// <param name="endpoint">Endpoint de la API sin la URL base</param>
        /// <param name="data">Objeto a enviar en el cuerpo de la petición</param>
        /// <returns>Objeto deserializado de la respuesta</returns>
        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            await SetAuthorizationHeader();

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_apiBaseUrl}/{endpoint}", content);

            return await HandleResponse<T>(response);
        }

        /// <summary>
        /// Realiza una petición PUT a la API
        /// </summary>
        /// <typeparam name="T">Tipo del objeto de respuesta</typeparam>
        /// <param name="endpoint">Endpoint de la API sin la URL base</param>
        /// <param name="data">Objeto a enviar en el cuerpo de la petición</param>
        /// <returns>Objeto deserializado de la respuesta</returns>
        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            await SetAuthorizationHeader();

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{_apiBaseUrl}/{endpoint}", content);

            return await HandleResponse<T>(response);
        }

        /// <summary>
        /// Realiza una petición DELETE a la API
        /// </summary>
        /// <typeparam name="T">Tipo del objeto de respuesta</typeparam>
        /// <param name="endpoint">Endpoint de la API sin la URL base</param>
        /// <returns>Objeto deserializado de la respuesta</returns>
        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            await SetAuthorizationHeader();

            HttpResponseMessage response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{endpoint}");

            return await HandleResponse<T>(response);
        }

        /// <summary>
        /// Autentica al usuario con la API y obtiene un token JWT
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>Resultado de la autenticación</returns>
        public async Task<AuthResult> AuthenticateAsync(string email, string password)
        {
            try
            {
                var authModel = new
                {
                    email = email,
                    password = password
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(authModel),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/auth/authenticate.php", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (authResponse != null)
                    {
                        // Almacenar el token
                        await StoreToken(authResponse.Token, authResponse.Expires);

                        return new AuthResult
                        {
                            Success = true,
                            Message = "Autenticación exitosa",
                            UserId = authResponse.User.Id,
                            UserName = authResponse.User.Name,
                            Token = authResponse.Token
                        };
                    }
                }

                // Error de autenticación
                string errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new AuthResult
                {
                    Success = false,
                    Message = errorResponse?.Message ?? "Error de autenticación desconocido"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = $"Error al conectar con la API: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Configura el encabezado de autorización con el token JWT
        /// </summary>
        private async Task SetAuthorizationHeader()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Intentar obtener el token de la sesión
                string token = _httpContextAccessor.HttpContext.Session.GetString("api_token");

                // Si no está en la sesión, intentar obtenerlo de la cookie
                if (string.IsNullOrEmpty(token))
                {
                    token = _httpContextAccessor.HttpContext.Request.Cookies["api_token"];
                }

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        /// <summary>
        /// Almacena el token JWT en una cookie y en la sesión
        /// </summary>
        private async Task StoreToken(string token, long expires)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Almacenar en cookie segura
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Requiere HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.FromUnixTimeSeconds(expires)
            };

            httpContext.Response.Cookies.Append("api_token", token, cookieOptions);

            // También guardar en la sesión para uso interno
            httpContext.Session.SetString("api_token", token);
        }

        /// <summary>
        /// Procesa la respuesta de la API
        /// </summary>
        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Manejar error
            string errorContent = await response.Content.ReadAsStringAsync();

            throw new ApiException(
                $"Error en la API: {(int)response.StatusCode} {response.ReasonPhrase}",
                (int)response.StatusCode,
                errorContent);
        }
    }

    /// <summary>
    /// Clase para excepción personalizada de API
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Código de estado HTTP
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Contenido de la respuesta de error
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Constructor de la excepción
        /// </summary>
        public ApiException(string message, int statusCode, string content) : base(message)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }

    /// <summary>
    /// Clase para deserializar la respuesta de autenticación
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Mensaje de la respuesta
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Token JWT
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Información del usuario
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// Timestamp de expiración del token
        /// </summary>
        public long Expires { get; set; }
    }

    /// <summary>
    /// Clase para información básica del usuario
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// Clase para deserializar respuestas de error
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Mensaje de error
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Clase para resultados de autenticación
    /// </summary>
    public class AuthResult
    {
        /// <summary>
        /// Indica si la autenticación fue exitosa
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ID del usuario autenticado
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Nombre del usuario autenticado
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Token JWT obtenido
        /// </summary>
        public string Token { get; set; }
    }
}