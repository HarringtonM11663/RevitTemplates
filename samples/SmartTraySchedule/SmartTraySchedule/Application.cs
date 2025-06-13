using Nice3point.Revit.Toolkit.External;
using SmartTraySchedule.Commands;

namespace SmartTraySchedule;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Tools", "Smart Tray Schedule");

        panel.AddPushButton<GenerateScheduleCommand>("Generate Schedule");
    }
}
