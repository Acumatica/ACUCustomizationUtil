namespace ACUCustomizationUtils.Helpers
{
    interface IAcuCustomizationClient : IDisposable
    {
        Task GetPackage();
        Task UploadPackage();
        Task UnpublishAllPackages();
        Task PublishPackages();
    }
}
