using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;

using static ACUCustomizationUtils.Common.Messages;

using Request = ACUCustomizationUtils.Helpers.RestModel.Request;
using Response = ACUCustomizationUtils.Helpers.RestModel.Response;

namespace ACUCustomizationUtils.Helpers
{
    internal class RestClient : IAcuCustomizationClient
    {
        private readonly string? _packageName;
        private readonly string? _packageDirectory;
        private readonly HttpClient _client;

        public RestClient(IAcuConfiguration configuration)
        {
            Uri baseAddress = configuration.Pkg.Url!;
            string username = configuration.Pkg.Login!;
            string password = configuration.Pkg.Password!;
            string? tenant = configuration.Pkg.Tenant;
            string? branch = configuration.Pkg.Branch;
            _packageName = configuration.Pkg.PkgName;
            _packageDirectory = configuration.Pkg.PkgDirectory;
            HttpClientHandler options = new()
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer(),
            };

            _client = new HttpClient(options)
            {
                BaseAddress = baseAddress,
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") },
                },
            };

            Request.Login login = new()
            {
                Name = username,
                Password = password,
                Tenant = tenant,
                Branch = branch,
                Locale = null,
            };

            Login(login);
        }

        public async Task GetPackage()
        {
            Response.GetProject res = await GetProjectAsync(_packageName!);
            string directory = _packageDirectory!;
            string file = _packageName!.EndsWith(".zip") ? _packageName : $"{_packageName}.zip";
            string filePath = Path.Combine(directory, file);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(
                filePath,
                Convert.FromBase64String(res.ProjectContentBase64)
            );
        }

        public async Task UnpublishAllPackages()
        {
            await UnpublishAllAsync();
        }

        public async Task UploadPackage()
        {
            string packageName = _packageName!;
            string directory = _packageDirectory!;
            string file = _packageName!.EndsWith(".zip") ? _packageName : $"{_packageName}.zip";
            string filePath = Path.Combine(directory, file);
            byte[] packageContents = await File.ReadAllBytesAsync(filePath);
            string projectContentBase64 = Convert.ToBase64String(packageContents);

            await ImportAsync(packageName, projectContentBase64);
        }

        public async Task PublishPackages()
        {
            string[] packageNames = new[] { _packageName ?? string.Empty };
            const bool mergeWithExistingPackages = true;
            bool isPublished = false;

            await PublishBeginAsync(packageNames, mergeWithExistingPackages);
            while (!isPublished)
            {
                await Task.Delay(1000);
                Response.PublishEnd res = await PublishEndAsync();
                isPublished = res.IsCompleted;

                if (res.IsFailed)
                {
                    string msg = string.Empty;
                    foreach (RestModel.Log? log in res.Log!.Where(l => l.LogType == Messages.ErrorLogType))
                    {
                        msg += $"\n{log.Message}";
                    }
                    throw new Exception(msg);
                }
            }
        }

        private async Task<Response.PublishBegin> PublishBeginAsync(
            string[] projectNames,
            bool isMergeWithExistingPackages = false,
            bool isOnlyValidation = false,
            bool isOnlyDbUpdates = false,
            bool isReplayPreviouslyExecutedScripts = false,
            string tenantMode = TenantMode.All
        )
        {
            Request.PublishBegin model = new()
            {
                ProjectNames = projectNames,
                IsMergeWithExistingPackages = isMergeWithExistingPackages,
                IsOnlyValidation = isOnlyValidation,
                IsOnlyDbUpdates = isOnlyDbUpdates,
                IsReplayPreviouslyExecutedScripts = isReplayPreviouslyExecutedScripts,
                TenantMode = tenantMode,
            };
            return await PostAsync<Response.PublishBegin>(APIResource.PublishBegin, model);
        }

        private async Task<Response.UnpublishAll> UnpublishAllAsync(
            string tenantMode = TenantMode.All,
            string[]? tenantLoginNames = null
        )
        {
            Request.UnpublishAll model = new()
            {
                TenantMode = tenantMode,
                TenantLoginNames = tenantLoginNames,
            };
            return await PostAsync<Response.UnpublishAll>(APIResource.UnpublishAll, model);
        }

        private async Task<Response.PublishEnd> PublishEndAsync()
        {
            return await PostAsync<Response.PublishEnd>(APIResource.PublishEnd, null);
        }

        private async Task<Response.Import> ImportAsync(
            string projectName,
            string projectContentBase64,
            int projectLevel = 0,
            bool isReplaceIfExists = true,
            string? projectDescription = null
        )
        {
            Request.Import model = new()
            {
                ProjectLevel = projectLevel,
                IsReplaceIfExists = isReplaceIfExists,
                ProjectName = projectName,
                ProjectDescription = projectDescription,
                ProjectContentBase64 = projectContentBase64,
            };
            return await PostAsync<Response.Import>(APIResource.Import, model);
        }

        private async Task<Response.GetProject> GetProjectAsync(
            string projectName,
            bool isAutoResolveConflicts = true
        )
        {
            Request.GetProject model = new()
            {
                IsAutoResolveConflicts = isAutoResolveConflicts,
                ProjectName = projectName,
            };
            return await PostAsync<Response.GetProject>(APIResource.GetProject, model);
        }

        private void Login(Request.Login model)
        {
            using HttpResponseMessage response = Post(APIResource.Login, model);
            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new InvalidCredentialException(
                    $"Error login to service: {response.StatusCode} {response.Content}"
                );
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
                message.Content = new StringContent(
                    JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json"
                );
            }
            return message;
        }

        private async Task<T> PostAsync<T>(string resource, object? model)
        {
            using HttpResponseMessage response = await _client.PostAsJsonAsync(resource, model);
            return await response.Content.ReadFromJsonAsync<T>()
                ?? throw new Exception(Messages.DeserializeError);
        }
    }
}
