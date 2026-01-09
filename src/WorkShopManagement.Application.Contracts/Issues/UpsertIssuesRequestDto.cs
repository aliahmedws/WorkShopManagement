using System.Collections.Generic;

namespace WorkShopManagement.Issues;

public class UpsertIssuesRequestDto
{
    public List<UpsertIssueDto> Items { get; set; } = [];
}
