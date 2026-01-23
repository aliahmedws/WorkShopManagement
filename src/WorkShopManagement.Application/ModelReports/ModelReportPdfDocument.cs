using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WorkShopManagement.ModelReports;

//QuestPDF (PDF layout/structure design)
public class ModelReportPdfDocument : IDocument
{
    private readonly ModelReportDto _report;

    public ModelReportPdfDocument(ModelReportDto report)
    {
        _report = report;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(25);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Column(col =>
            {
                foreach (var section in _report.Sections)
                    col.Item().PaddingTop(10).Element(c => ComposeSection(c, section));
            });

            page.Footer().AlignBottom().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(_report.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        });
    }
    private void ComposeSection(IContainer container, ModelReportSectionDto section)
    {
        container.Column(col =>
        {
            // Section Title (center)
            col.Item().AlignCenter().Text(section.Title).FontSize(14).SemiBold();

            col.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2).CornerRadius(4)
                .Element(tableContainer => ComposeTable(tableContainer, section));
        });
    }

    private void ComposeTable(IContainer container, ModelReportSectionDto section)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.RelativeColumn(1);
                cols.RelativeColumn(2);
            });

            // Header row (dark)
            table.Header(header =>
            {
                header.Cell().Background("#2f2f2f").Padding(8)
                    .Text("Parameter").FontColor(Colors.White).SemiBold();

                header.Cell().Background("#2f2f2f").Padding(8)
                    .Text("Value").FontColor(Colors.White).SemiBold();
            });

            for (int i = 0; i < section.Rows.Count; i++)
            {
                var r = section.Rows[i];
                var bg = (i % 2 == 0) ? Colors.Grey.Lighten3 : Colors.White;

                table.Cell().Background(bg).Padding(8).Text(r.Parameter).SemiBold();
                table.Cell().Background(bg).Padding(8).Text(string.IsNullOrWhiteSpace(r.Value) ? "N/A" : r.Value);
            }
        });
    }
}
