namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public interface ISubFunction : IFunction
    {
        ISettings? GetSettingsFromParent(ISettings parentSettings);
    }
}
