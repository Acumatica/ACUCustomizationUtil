using ACUCustomizationUtils.Configuration.ACU;
using AcuSOAP;
using System.Security.Authentication;
using System.ServiceModel;

namespace ACUCustomizationUtils.Helpers;

internal class SoapClient : IAcuCustomizationClient
{
    private readonly string? _packageName;
    private readonly string? _packageDirectory;
    private readonly ServiceGateSoapClient _client;

    public SoapClient(IAcuConfiguration configuration)
    {
        configuration.Pkg.SetDefaultValues(configuration);
        Uri serviceUrl = configuration.Pkg.Url!;
        string username = configuration.Pkg.Login!;
        string password = configuration.Pkg.Password!;
        string? tenant = configuration.Pkg.Tenant;

        if (!string.IsNullOrEmpty(tenant) && !string.IsNullOrEmpty(username))
        {
            username = $"{username}@{tenant}";
        }
        _packageName = configuration.Pkg.PkgName;
        _packageDirectory = configuration.Pkg.PkgDirectory;

        EndpointAddress endpointAddress = new EndpointAddress(serviceUrl);
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding(
            endpointAddress.Uri.Scheme.ToLower() == "http"
                ? BasicHttpSecurityMode.None
                : BasicHttpSecurityMode.Transport
        )
        {
            OpenTimeout = TimeSpan.MaxValue,
            CloseTimeout = TimeSpan.MaxValue,
            ReceiveTimeout = TimeSpan.MaxValue,
            SendTimeout = TimeSpan.MaxValue,
            AllowCookies = true,
            MaxReceivedMessageSize = 6553600,
        };
        _client = new ServiceGateSoapClient(basicHttpBinding, endpointAddress);
        Task<LoginResult> login = _client.LoginAsync(username, password);

        if (login.Result.Code != ErrorCode.OK)
            throw new InvalidCredentialException(
                $"Error login to service: {login.Result.Code} {login.Result.Message}"
            );
    }

    public async Task GetPackage()
    {
        GetPackageResponse res = await _client.GetPackageAsync(_packageName);
        byte[] pkg = res.GetPackageResult;
        if (pkg == null)
            throw new Exception($"Package {_packageName} not found");
        string directory = _packageDirectory!;
        string file = _packageName!.EndsWith(".zip") ? _packageName : $"{_packageName}.zip";
        string filePath = Path.Combine(directory, file);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllBytesAsync(filePath, pkg);
    }

    public async Task PublishPackages()
    {
        string[] packageNames = new[] { _packageName ?? string.Empty };
        const bool mergeWithExistingPackages = true;

        await _client.PublishPackagesAsync(packageNames, mergeWithExistingPackages);
    }

    public async Task UnpublishAllPackages()
    {
        await _client.UnpublishAllPackagesAsync();
    }

    public async Task UploadPackage()
    {
        const bool replaceIfPackageExists = true;
        string packageName = _packageName!;
        string directory = _packageDirectory!;
        string file = _packageName!.EndsWith(".zip") ? _packageName : $"{_packageName}.zip";
        string filePath = Path.Combine(directory, file);
        byte[] packageContents = await File.ReadAllBytesAsync(filePath);

        await _client.UploadPackageAsync(packageName, packageContents, replaceIfPackageExists);
    }

    public void Dispose()
    {
        _client.LogoutAsync();
        _client.Abort();
        ((IDisposable)_client).Dispose();
    }
}
