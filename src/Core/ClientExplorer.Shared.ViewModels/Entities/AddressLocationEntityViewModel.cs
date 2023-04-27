namespace ClientExplorer.Shared.ViewModels;

public sealed class AddressLocationEntityViewModel
{
    #region Public Properties

    public string CityName { get; init; } = string.Empty;
    public string StreetName { get; init; } = string.Empty;
    public string HouseNumber { get; init; } = string.Empty;

    #endregion
}