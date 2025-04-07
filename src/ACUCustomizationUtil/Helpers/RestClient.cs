using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using Request = ACUCustomizationUtils.Helpers.RestModel.Request;
using Response = ACUCustomizationUtils.Helpers.RestModel.Response;

namespace ACUCustomizationUtils.Helpers
{
    internal class RestClient : IDisposable
    {
        private readonly string? _packageName;
        private readonly string? _packageDirectory;
        private readonly HttpClient _client;
        public RestClient(IAcuConfiguration configuration)
        {
            var baseAddress = configuration.Pkg.RestUrl!;
            var username = configuration.Pkg.Login!;
            var password = configuration.Pkg.Password!;
            _packageName = configuration.Pkg.PkgName;
            _packageDirectory = configuration.Pkg.PkgDirectory;

            HttpClientHandler options = new()
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };
            Console.WriteLine(baseAddress.ToString());
            _client = new HttpClient(options)
            {
                BaseAddress = baseAddress,
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            Request.Login login = new()
            {
                Name = username,
                Password = password,
                Tenant = null, 
                Branch = null,
                Locale = null
            };
            Login(login);
        }

        public async Task GetPackage()
        {
            var res = await GetProjectAsync(_packageName!);
            var directory = _packageDirectory!;
            var file = _packageName!.EndsWith(".zip") ? _packageName : $"{_packageName}.zip";
            var filePath = Path.Combine(directory, file);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(res.ProjectContentBase64));
        }

        public async Task<Response.GetProject> GetProjectAsync(string projectName, bool isAutoResolveConflicts = true)
        {
            Request.GetProject model = new()
            {
                IsAutoResolveConflicts = isAutoResolveConflicts,
                ProjectName = projectName
            };
            return await PostAsync<Response.GetProject>(APIResource.GetProject, model);
        }

        private void Login(Request.Login model)
        {
            using HttpResponseMessage response = Post(APIResource.Login, model);
            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new InvalidCredentialException(
                    $"Error login to service: {response.StatusCode} {response.Content}");
            }
        }

        private void Logout()
        {
            Post(APIResource.Logout, null);
        }

        public void Dispose()
        {
            Logout();
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

        private HttpResponseMessage Post(string resource, object? model)
        {
            using HttpRequestMessage message = CreateHttpRequestMessage(resource, model);
            return _client.Send(message);
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string resource, object? model)
        {
            HttpRequestMessage message = new(HttpMethod.Post, resource);
            if (model != null)
            {
                message.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            }
            return message;
        }

        private async Task<T> PostAsync<T>(string resource, object? model)
        {
            using HttpResponseMessage response = await _client.PostAsJsonAsync(resource, model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>()
                ?? throw new Exception(Messages.DeserializeError);
        }
    }
}
