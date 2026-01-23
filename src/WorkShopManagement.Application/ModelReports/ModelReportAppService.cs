using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using WorkShopManagement.Cars;

namespace WorkShopManagement.ModelReports;

//get car data , then make DTO(section + rows) , then generate PDF bytes from QuestPDF, then return download stream to client.

[RemoteService(IsEnabled = false)]
public class ModelReportAppService : ApplicationService, IModelReportAppService
{
    private readonly ICarRepository _carRepository;
    private readonly ModelReportPdfGenerator _pdf;

    public ModelReportAppService(
        ICarRepository carRepository,
        ModelReportPdfGenerator pdf)
    {
        _carRepository = carRepository;
        _pdf = pdf;
    }

    public async Task<IRemoteStreamContent> DownloadAsync(Guid carId)
    {
        if (carId == Guid.Empty)
            throw new UserFriendlyException("CarId is required.");

        // Use your existing method (includes LogisticsDetail in your repo)
        var car = await _carRepository.GetDetailsForReportAsync(carId, asNoTracking: true);

        // Build DTO (sections + rows)
        var report = BuildReport(car);

        var bytes = _pdf.Generate(report);

        var fileName = $"ModelReport_{car.Vin}_{Clock.Now:yyyyMMdd_HHmmss}.pdf";

        return new RemoteStreamContent(
            stream: bytes.ToStream(),
            fileName: fileName,
            contentType: "application/pdf"
        );
    }

    private ModelReportDto BuildReport(Car car)
    {
        static string NA(string? v) => string.IsNullOrWhiteSpace(v) ? "N/A" : v.Trim();
        static string NADate(DateTime? v) => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : "N/A";
        static string NAI(int? v) => v.HasValue ? v.Value.ToString() : "N/A";

        var report = new ModelReportDto
        {
            Title = "Model Report",
            GeneratedAt = Clock.Now
        };

        void AddSection(string title, params (string P, string? V)[] rows)
        {
            var section = new ModelReportSectionDto { Title = title };

            foreach (var (p, v) in rows)
                section.Rows.Add(new ModelReportRowDto { Parameter = p, Value = NA(v) });

            report.Sections.Add(section);
        }

        // 1) Vehicle Details
        AddSection("Model Report Vehicle Details",
            ("Make", "N/A"),                           // adjust if available
            ("Model", car.Model?.Name),
            ("Trim", "N/A"),
            ("Vehicle Category", car.Model?.Name),
            ("VIN", car.Vin),
            ("Year", "N/A"),
            ("Build Date", NADate(car.StartDate)),
            ("Color", car.Color),
            ("Model Report Version", "N/A"),
            ("SEVS Entry Number", "N/A"),
            ("Model Report Variant", car.Model?.ModelCategory?.Name)
        );

        // 2) Author Information
        AddSection("Model Report Author Information",
            ("Name", car.Owner?.Name),
            ("Address", "N/A"),
            ("Email", car.Owner?.Email),
            ("Contact", car.Owner?.ContactId)
        );

        // 3) Documentation
        AddSection("Model Report Documentation",
            ("Model Report Vehicle Scope", "N/A"),
            ("Model Report Work Instructions", "N/A"),
            ("Model Report Verification Checklist", "N/A"),
            ("Applicable To", car.Model?.Name)
        );

        // 4) Pre-Modification Scope (from screenshots)
        AddSection("Model Report Pre-Modification Scope",
            ("Date Performed", "N/A"),
            ("Make", "N/A"),
            ("Model", car.Model?.Name),
            ("Trim", "N/A"),

            ("Vehicle Category", car.Model?.ModelCategory?.Name),
            ("Body Shape", "N/A"),
            ("Build Date Range", "N/A"),
            ("Gross Vehicle Mass", "N/A"),
            ("Unladen Mass", "N/A"),
            ("Steering Location", "N/A"),
            ("Work Instruction Unique Document", "N/A"),
            ("Report Type", "N/A"),
            ("SEVS Entry Number", "N/A"),
            ("SEVS Steering Location", "N/A"),
            ("# Front Seats", "N/A"),
            ("# Rear Seats", "N/A"),
            ("# Side Doors", "N/A"),
            ("# Rear Doors", "N/A"),
            ("Length", "N/A"),
            ("Width (excluding mirrors)", "N/A"),
            ("Width (including mirrors)", "N/A"),
            ("Height", "N/A"),
            ("Wheelbase", "N/A"),
            ("Rear Overhang", "N/A"),
            ("Running Clearance", "N/A"),
            ("Motive Power", "N/A"),
            ("Motor Configuration", "N/A"),
            ("Battery Range", "N/A"),
            ("Net Motor Power", "N/A"),
            ("Tyre Make", "N/A"),
            ("Tyre Model", "N/A"),
            ("Tyre Profile", "N/A"),
            ("Tyre Description", "N/A"),
            ("Tyre Load Index", "N/A"),
            ("Tyre Speed Rating", "N/A"),
            ("Wheel Description", "N/A"),
            ("Wheel Profile", "N/A"),
            ("Wheel Offset", "N/A"),
            ("Wheel PCD", "N/A"),
            ("Wheel Lug Nut", "N/A"),
            ("Wheel Centre Bore", "N/A")
        );

        // 5) Recall & Rectification Check
        AddSection("Model Report Recall & Rectification Check",
            ("Identifying Outstanding Recalls", "N/A"),
            ("Sourcing Replacement Parts", "N/A"),
            ("Recall Rectification", "N/A")
        );

        // 6) Damage & Corrosion Check
        AddSection("Model Report Damage & Corrosion Check",
            ("Levels Preventing Verification", "N/A"),
            ("Acceptable Levels of Damage", "N/A"),
            ("Body Alignment Check", "N/A")
        );

        // 7) Deterioration Check & Rectification
        AddSection("Model Report Deterioration Check & Rectification",
            ("Headlamp", "N/A"),
            ("Taillamp", "N/A"),
            ("CHMSL", "N/A"),
            ("Side Indicator Lamp", "N/A"),
            ("Registration Plate Lamp", "N/A"),
            ("Windshield", "N/A"),
            ("Windows", "N/A"),
            ("Front Left Seatbelt", "N/A"),
            ("Front Right Seatbelt", "N/A"),
            ("Rear Left Seatbelt", "N/A"),
            ("Rear Center Seatbelt", "N/A"),
            ("Rear Right Seatbelt", "N/A"),
            ("Tyres (Front and Rear)", "N/A"),
            ("Tyres (Spare)", "N/A"),
            ("Airbags", "N/A"),
            ("Airbag Warning Lamp", "N/A"),
            ("Brake Hoses", "N/A"),
            ("Brake Fluid", "N/A"),
            ("Brake Pads", "N/A"),
            ("Brake Rotors", "N/A")
        );

        // 8) Odometer Check
        AddSection("Model Report Odometer Check",
            ("Signs of Odometer Tampering", "N/A"),
            ("Odometer Reading", "N/A")
        );

        // 9) Component Check
        AddSection("Model Report Component Check",
            ("Headlamp (Left)", "N/A"),
            ("Headlamp (Right)", "N/A"),
            ("Taillamp (Left)", "N/A"),
            ("Taillamp (Right)", "N/A"),
            ("CHMSL", "N/A"),
            ("Side Indicator Lamp", "N/A"),
            ("Reverse Lamp", "N/A"),
            ("Registration Plate Lamp (Left)", "N/A"),
            ("Registration Plate Lamp (Right)", "N/A"),
            ("Retro Reflectors (Rear)", "N/A"),
            ("External Mirror (Left)", "N/A"),
            ("External Mirror (Right)", "N/A"),
            ("Internal Mirror", "N/A"),
            ("Horn", "N/A"),
            ("Windshield", "N/A"),
            ("Front Left Window", "N/A"),
            ("Front Right Window", "N/A"),
            ("Rear Left Window", "N/A"),
            ("Rear Right Window", "N/A"),
            ("Rear Window", "N/A"),
            ("Front Left Seat", "N/A"),
            ("Front Right Seat", "N/A"),
            ("Rear Left Seat", "N/A"),
            ("Rear Centre Seat", "N/A"),
            ("Rear Right Seat", "N/A"),
            ("Front Left Seatbelt", "N/A"),
            ("Front Right Seatbelt", "N/A"),
            ("Rear Left Seatbelt", "N/A"),
            ("Rear Centre Seatbelt", "N/A"),
            ("Rear Right Seatbelt", "N/A"),
            ("Front Left Seatbelt Buckle", "N/A"),
            ("Front Right Seatbelt Buckle", "N/A"),
            ("Rear Left Seatbelt Buckle", "N/A"),
            ("Rear Centre Seatbelt Buckle", "N/A"),
            ("Rear Right Seatbelt Buckle", "N/A"),
            ("Tyres (Front & Rear)", "N/A"),
            ("Tyres (Spare)", "N/A")
        );

        // 10) Manufacturing Steps
        AddSection("Model Report Manufacturing Steps",
            ("ADR 01/00", "N/A"),
            ("ADR 04/06", "N/A"),
            ("ADR 06/00", "N/A"),
            ("ADR 13/00", "N/A"),
            ("ADR 14/02", "N/A"),
            ("ADR 35/06", "N/A"),
            ("ADR 42/05", "N/A"),
            ("ADR 90/00", "N/A")
        );

        // 11) Manufacturing Checks
        AddSection("Model Report Manufacturing Checks",
            ("ADR 01/00", "N/A"),
            ("ADR 04/06", "N/A"),
            ("ADR 06/00", "N/A"),
            ("ADR 13/00", "N/A"),
            ("ADR 14/02", "N/A"),
            ("ADR 35/06", "N/A"),
            ("ADR 42/05", "N/A"),
            ("ADR 90/00", "N/A")
        );

        // 12) Post-Modification Scope
        AddSection("Model Report Post-Modification Scope",
            ("Date Performed", "N/A"),
            ("Make", "N/A"),
            ("Model", car.Model?.Name),
            ("Trim", "N/A"),
            ("Vehicle Category", car.Model?.ModelCategory?.Name),
            ("Body Shape", "N/A"),
            ("Build Date Range", "N/A"),
            ("Gross Vehicle Mass", "N/A"),
            ("Unladen Mass", "N/A"),
            ("Steering Location", "N/A"),
            ("Work Instruction Unique Document", "N/A"),
            ("Report Type", "N/A"),
            ("SEVS Entry Number", "N/A"),
            ("SEVS Steering Location", "N/A"),
            ("# Front Seats", "N/A"),
            ("# Rear Seats", "N/A"),
            ("# Side Doors", "N/A"),
            ("# Rear Doors", "N/A"),
            ("Length", "N/A"),
            ("Width (excluding mirrors)", "N/A"),
            ("Width (including mirrors)", "N/A"),
            ("Height", "N/A"),
            ("Wheelbase", "N/A"),
            ("Rear Overhang", "N/A"),
            ("Running Clearance", "N/A"),
            ("Motive Power", "N/A"),
            ("Motor Configuration", "N/A"),
            ("Battery Range", "N/A"),
            ("Net Motor Power", "N/A"),
            ("Tyre Make", "N/A"),
            ("Tyre Model", "N/A"),
            ("Tyre Profile", "N/A"),
            ("Tyre Description", "N/A"),
            ("Tyre Load Index", "N/A"),
            ("Tyre Speed Rating", "N/A"),
            ("Wheel Description", "N/A"),
            ("Wheel Profile", "N/A"),
            ("Wheel Offset", "N/A"),
            ("Wheel PCD", "N/A"),
            ("Wheel Lug Nut", "N/A"),
            ("Wheel Centre Bore", "N/A")
        );

        return report;
    }
}
