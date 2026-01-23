using System.Collections.Generic;

namespace WorkShopManagement.ModelReports;

public class ModelReportSectionDto
{
    public string Title { get; set; } = default!;
    public List<ModelReportRowDto> Rows { get; set; } = new();
}
