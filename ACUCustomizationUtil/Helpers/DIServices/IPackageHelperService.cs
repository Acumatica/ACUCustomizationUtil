namespace ACUCustomizationUtils.Helpers.DIServices;

public interface IPackageHelperService
{
    string GetPackageName();
    string? GetPackageDescription();
    void MakePackage();
}