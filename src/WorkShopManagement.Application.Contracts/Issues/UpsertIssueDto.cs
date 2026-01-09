using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.Issues;

public class UpsertIssueDto
{
    public Guid? Id { get; set; }

    [Range(0, 1000)]
    public int SrNo { get; set; }

    [Range(0, 100)]
    public decimal XPercent { get; set; }

    [Range(0, 100)]
    public decimal YPercent { get; set; }
    public IssueType Type { get; set; }
    public IssueStatus Status { get; set; }
    public IssueOriginStage OriginStage { get; set; }
    public IssueDeteriorationType DeteriorationType { get; set; }

    [Required]
    [StringLength(IssueConsts.MaxDescriptionLength)]
    public string Description { get; set; } = default!;

    [StringLength(IssueConsts.MaxRectificationActionLength)]
    public string? RectificationAction { get; set; }
    
    [StringLength(IssueConsts.MaxRectificationNotesLength)]
    public string? RectificationNotes { get; set; }

    [StringLength(IssueConsts.MaxQualityControlActionLength)]
    public string? QualityControlAction { get; set; }

    [StringLength(IssueConsts.MaxQualityControlNotesLength)]
    public string? QualityControlNotes { get; set; }

    [StringLength(IssueConsts.MaxRepairerActionLength)]
    public string? RepairerAction { get; set; }

    [StringLength(IssueConsts.MaxRepairerNotesLength)]
    public string? RepairerNotes { get; set; }

    public List<FileAttachmentDto> TempFiles { get; set; } = [];
    public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
}