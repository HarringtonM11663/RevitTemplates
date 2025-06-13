using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using System.Net.Http.Json;

namespace SmartTraySchedule.Commands;

/// <summary>
///     Generates a cable tray schedule and optionally sends it to an AI service
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class GenerateScheduleCommand : ExternalCommand
{
    public override void Execute()
    {
        var trays = new FilteredElementCollector(Document)
            .OfClass(typeof(CableTray))
            .Cast<CableTray>()
            .ToList();

        if (trays.Count == 0)
        {
            TaskDialog.Show("Smart Tray Schedule", "No cable trays found in the document.");
            return;
        }

        var schedule = string.Join("\n", trays.Select(t => $"{t.Id.IntegerValue}: {t.get_Parameter(BuiltInParameter.RBS_CABLETRAY_LENGTH_PARAM)?.AsValueString()}"));

        var analysis = CallAiServiceAsync(schedule).GetAwaiter().GetResult();

        TaskDialog.Show("Smart Tray Schedule", analysis ?? schedule);
    }

    private static async Task<string?> CallAiServiceAsync(string schedule)
    {
        var apiKey = Environment.GetEnvironmentVariable("SMART_TRAY_AI_KEY");
        if (string.IsNullOrWhiteSpace(apiKey)) return null;

        try
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await http.PostAsJsonAsync("https://api.example.com/analyze", new { text = schedule });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AiResponse>();
            return result?.Message;
        }
        catch
        {
            return null;
        }
    }

    private record AiResponse(string Message);
}
