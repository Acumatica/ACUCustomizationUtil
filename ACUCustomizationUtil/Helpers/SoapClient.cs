using System.Security.Authentication;
using System.ServiceModel;
using ACUCustomizationUtils.Configuration;
using AcuSOAP;

namespace ACUCustomizationUtils.Helpers;

internal class SoapClient : IDisposable
{
    private readonly string? _packageName;
    private readonly string? _packageDirectory;
    private readonly ServiceGateSoapClient _client;

    public SoapClient(IAcuConfiguration configuration)
    {
        var serviceUrl = configuration.Package.Url!;
        var username = configuration.Package.Login!;
        var password = configuration.Package.Password!;
        _packageName = configuration.Package.PackageName;
        _packageDirectory = configuration.Package.PackageDirectory;

        var endpointAddress = new EndpointAddress(serviceUrl);
        var basicHttpBinding = new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http"
            ? BasicHttpSecurityMode.None
            : BasicHttpSecurityMode.Transport)
        {
            OpenTimeout = TimeSpan.MaxValue,
            CloseTimeout = TimeSpan.MaxValue,
            ReceiveTimeout = TimeSpan.MaxValue,
            SendTimeout = TimeSpan.MaxValue,
            AllowCookies = true,
            MaxReceivedMessageSize = 6553600
        };
        _client = new ServiceGateSoapClient(basicHttpBinding, endpointAddress);
        var login = _client.LoginAsync(username, password);

        if (login.Result.Code != ErrorCode.OK)
            throw new InvalidCredentialException(
                $"Error login to service: {login.Result.Code} {login.Result.Message}");
    }

    public async Task GetPackage()
    {
        var res = await _client.GetPackageAsync(_packageName);
        var pkg = res.GetPackageResult;
        if (pkg == null)
            throw new Exception($"Package {_packageName} not found");
        var directory = _packageDirectory!;
        var file = _packageName!.EndsWith(".zip") ? _packageName : $"{_packageName}.zip";
        var filePath = Path.Combine(directory, file);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllBytesAsync(filePath, pkg);
    }

    public async Task PublishPackages()
    {
        var packageNames = new[] { _packageName ?? string.Empty };
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
        var packageName = _packageName!;
        var directory = _packageDirectory!;
        var file = _packageName!.EndsWith(".zip") ? _packageName : $"{_packageName}.zip";
        var filePath = Path.Combine(directory, file);
        var packageContents = await File.ReadAllBytesAsync(filePath);

        await _client.UploadPackageAsync(packageName, packageContents, replaceIfPackageExists);
    }

    public void Dispose()
    {
        _client.LogoutAsync();
        _client.Abort();
        ((IDisposable)_client).Dispose();
    }
}