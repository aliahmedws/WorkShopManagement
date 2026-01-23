using System;
using System.Collections.Generic;

namespace WorkShopManagement.ModelReports;

public class ModelReportDto
{
    public string Title { get; set; } = "Model Report";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public List<ModelReportSectionDto> Sections { get; set; } = new();
}
