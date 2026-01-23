using Microsoft.Extensions.Localization;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Content;
using WorkShopManagement.Cars.Stages;

namespace WorkShopManagement.Stages;

public class StageExcelHelper
{
    public static async Task<IRemoteStreamContent> GenerateAsync(Dictionary<Stage, List<StageDto>> itemsByStage, IStringLocalizer l)
    {
        // Humanized headers (these become the Excel column headers)
        const string HVin = "VIN";
        const string HOwner = "Owner";
        const string HModel = "Model";
        const string HColor = "Color";
        const string HEtaScd = "Eta Scd";
        const string HPort = "Port";
        const string HRecalls = "Recalls";
        const string HNotes = "Notes";
        const string HEstRelease = "Est. Release";
        const string HAvv = "AVV";
        const string HIssues = "Issues";
        const string HCre = "CRE";
        const string HBookingNumber = "Booking Number";
        const string HClearingAgent = "Clearing Agent";
        const string HModified = "Modified";

        static Dictionary<string, object?> BlankRow() => new()
        {
            [HVin] = null,
            [HOwner] = null,
            [HModel] = null,
            [HColor] = null,
            [HEtaScd] = null,
            [HPort] = null,
            [HRecalls] = null,
            [HNotes] = null,
            [HEstRelease] = null,
            [HAvv] = null,
            [HIssues] = null,
            [HCre] = null,
            [HBookingNumber] = null,
            [HClearingAgent] = null,
            [HModified] = null
        };

        // MiniExcel works very well with a list of dictionaries
        var rows = new List<Dictionary<string, object?>>();

        foreach (var stage in Enum.GetValues<Stage>().OrderBy(s => s))
        {
            if (!itemsByStage.TryGetValue(stage, out var stageItems) || stageItems.Count == 0)
                continue;

            var stageName = l["Enum:Stage." + (int)stage];// stage.Humanize(LetterCasing.Title);

            // 1) Stage "header row" (put stage name under VIN; others blank)
            rows.Add(new Dictionary<string, object?>
            {
                [HVin] = stageName,
                [HOwner] = null,
                [HModel] = null,
                [HColor] = null,
                [HEtaScd] = null,
                [HPort] = null,
                [HRecalls] = null,
                [HNotes] = null,
                [HEstRelease] = null,
                [HAvv] = null,
                [HIssues] = null,
                [HCre] = null,
                [HBookingNumber] = null,
                [HClearingAgent] = null,
                [HModified] = null
            });

            // 2) Item rows
            foreach (var x in stageItems)
            {
                rows.Add(new Dictionary<string, object?>
                {
                    [HVin] = x.Vin,
                    [HOwner] = x.OwnerName?.ToUpperInvariant(),
                    [HModel] = x.ModelName,
                    [HColor] = x.Color?.ToUpperInvariant(),
                    [HEtaScd] = x.EtaScd?.Date.ToString("dd-MMM-yyyy"),
                    [HPort] = x.Port.HasValue ? l["Enum:Port." + (int)x.Port.Value] : string.Empty,
                    [HRecalls] = x.RecallStatus.HasValue ? l["Enum:RecallStatus." + (int)x.RecallStatus.Value] : string.Empty,
                    [HNotes] = x.Notes,
                    [HEstRelease] = x.EstimatedRelease?.Date,
                    [HAvv] = x.AvvStatus.HasValue ? l["Enum:AvvStatus." + (int)x.AvvStatus.Value] : string.Empty,
                    [HIssues] = x.IssueStatus.HasValue ? l["Enum:IssueStatus." + (int)x.IssueStatus.Value] : string.Empty,
                    [HCre] = x.CreStatus.HasValue ? l["Enum:CreStatus." + (int)x.CreStatus.Value] : string.Empty,
                    [HBookingNumber] = x.BookingNumber,
                    [HClearingAgent] = x.ClearingAgent,
                    [HModified] = x.LastModificationTime?.Date
                });
            }

            // 3) Empty rows between stages (2 rows)
            rows.Add(BlankRow());
            rows.Add(BlankRow());
        }

        var stream = new MemoryStream();
        await MiniExcel.SaveAsAsync(stream, rows);
        stream.Position = 0;

        return new RemoteStreamContent(stream, fileName: $"Production_{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx", contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
}
