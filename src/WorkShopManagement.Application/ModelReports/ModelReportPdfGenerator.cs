using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Volo.Abp.DependencyInjection;

namespace WorkShopManagement.ModelReports;

//Here we will generate ModelReportDto into bytes using (QuestPDF) 
public class ModelReportPdfGenerator : ITransientDependency
{
    public byte[] Generate(ModelReportDto report)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var doc = new ModelReportPdfDocument(report);
        return doc.GeneratePdf();
    }
}
