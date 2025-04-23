using LSTY.Sdtd.ServerAdmin.Services.Abstractions;
using LSTY.Sdtd.ServerAdmin.Services.Settings;

namespace LSTY.Sdtd.ServerAdmin.Services.Functions.GeneralFunction
{
    public class GeneralFunction : FunctionBase<GeneralSettings>
    {
        public GeneralFunction()
        {
        }

        protected override void OnInit()
        {
            AddSubFunction<SubAFunction>();
        }
    }

    public class SubAFunction : SubFunctionBase
    {
        public SubAFunction()
        {
        }

        protected override BasicSettings? GetSettingsFromParent(ISettings parentSettings)
        {
            return ((GeneralSettings)parentSettings).SubASettings;
        }

        protected override void OnInit()
        {
            
        }

        protected override void OnEnabled()
        {
            CommandRegistry.RegisterCommand(new Core.CommandInfo()
            {
                Name = "AA",
                Aliases = new[] { "BB" },
                Description = "SubA Command",
                Execute = (args, sender) =>
                {
                    // Command logic here
                    return Task.CompletedTask;
                }
            });
        }
    }
}
