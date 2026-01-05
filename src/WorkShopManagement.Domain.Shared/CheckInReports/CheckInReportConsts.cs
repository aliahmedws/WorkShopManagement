using System;
using System.Collections.Generic;
using System.Text;

namespace WorkShopManagement.CheckInReports;

public static class CheckInReportConsts
{
    public const int VINMaxLength = 64; 
    public const int MaxLength = 128;

    public static string GetNormalizedSorting(string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return "CreationTime DESC";
        }

        // Normalize sorting string to match property names
        return sorting.Trim() switch
        {
            "vinNo" or "VinNo" => "VinNo",
            "status" or "Status" => "Status",
            "model" or "Model" => "Model",
            "buildDate" or "BuildDate" => "BuildDate",
            "complianceDate" or "ComplianceDate" => "ComplianceDate",
            "entryKms" or "EntryKms" => "EntryKms",
            "storageLocation" or "StorageLocation" => "StorageLocation",
            "creationTime" or "CreationTime" => "CreationTime DESC",
            _ => "CreationTime DESC"
        };
    }
}
