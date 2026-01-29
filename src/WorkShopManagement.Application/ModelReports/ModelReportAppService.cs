using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Timing;
using WorkShopManagement.Cars;
using WorkShopManagement.External.CarsXe;

namespace WorkShopManagement.ModelReports;

//get car data , then make DTO(section + rows) , then generate PDF bytes from QuestPDF, then return download stream to client.

[RemoteService(IsEnabled = false)]
public class ModelReportAppService : ApplicationService, IModelReportAppService
{
    private readonly ICarRepository _carRepository;
    private readonly ModelReportPdfGenerator _pdf;
    private readonly ICarXeService _carXeService;

    public ModelReportAppService(
        ICarRepository carRepository,
        ModelReportPdfGenerator pdf,
        ICarXeService carXeService)
    {
        _carRepository = carRepository;
        _pdf = pdf;
        _carXeService = carXeService;
    }

    public async Task<IRemoteStreamContent> DownloadAsync(Guid carId)
    {
        if (carId == Guid.Empty)
            throw new UserFriendlyException("CarId is required.");

        var car = await _carRepository.GetDetailsForReportAsync(carId, asNoTracking: true);

        SpecsResponseDto? specs = null;
        if (!string.IsNullOrWhiteSpace(car?.Vin))
        {

            specs = await _carXeService.GetSpecsAsync(car.Vin, CancellationToken.None);
        }
        var now = Clock.Now;

        var report = BuildReport(car!, specs, now);

        var bytes = _pdf.Generate(report);
        var fileName = $"ModelReport_{car!.Vin}_{Clock.Now:yyyyMMdd_HHmmss}.pdf";

        return new RemoteStreamContent(
            stream: bytes.ToStream(),
            fileName: fileName,
            contentType: "application/pdf"
        );
    }

    private static string NA(string? v) => string.IsNullOrWhiteSpace(v) ? "N/A" : v.Trim();
    private static string NADate(DateTime? v) => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : "N/A";
    private static string NAList(IEnumerable<string>? v) => v == null ? "N/A" : NA(string.Join(", ", v.Where(x => !string.IsNullOrWhiteSpace(x))));

    private static void AddSection(ModelReportDto report, string title, params (string Parameter, string? Value)[] rows)
    {
        var section = new ModelReportSectionDto { Title = title };
        foreach (var (parameter, value) in rows)
        {
            section.Rows.Add(new ModelReportRowDto { Parameter = parameter, Value = NA(value) });
        }
        report.Sections.Add(section);
    }

    private static string AppendShortCodeToModel(string shortCode, string modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            return "N/A";
        return $"{shortCode}_{modelName}".ToUpper();
    }

    //private static double ConvertKgToPounds(double kg)
    //{
    //    return kg * 2.205;
    //}

    //private static double ConvertInchesToMm(double inches)
    //{
    //    return inches * 25.4;
    //}

    //private static double ParseToDouble(string? value)
    //{
    //    if (string.IsNullOrWhiteSpace(value))
    //        return 0;

    //    var cleanValue = new string(value.Where(c => char.IsDigit(c) || c == '.')).ToArray();

    //    return double.TryParse(value, out double result) ? result : 0;
    //}

    private static ModelReportDto BuildReport(Car car, SpecsResponseDto? specs, DateTime now)
    {
        var report = new ModelReportDto
        {
            Title = "Model Report",
            GeneratedAt = now
        };

        // Common properties that will be used multiple times
        var make = specs?.Attributes?.Make;
        var model = specs?.Attributes?.Model;
        var trim = specs?.Attributes?.Trim;
        var vehicleCategory = specs?.Attributes?.Category;
        var year = specs?.Attributes?.Year;
        var style = specs?.Attributes?.Style;
        var fuelType = specs?.Attributes?.FuelType;
        var engine = specs?.Attributes?.Engine;
        var engineCylinders = specs?.Attributes?.EngineCylinders;
        var drivetrain = specs?.Attributes?.Drivetrain;
        var widthExcludingMirrors = specs?.Attributes?.OverallWidth;
        var overAllHeight = specs?.Attributes?.OverallHeight;
        var wheelbase = specs?.Attributes?.WheelbaseLength;
        var groundClearance = specs?.Attributes?.GroundClearance;
        //var otherBatteryInfo = specs?.Attributes?.
        //var driveType = specs?.Attributes?.
        string grossVehicleMassStr = specs?.Attributes?.GrossVehicleWeightRating ?? "N/A";
        //double grossVehicleMassKg = ParseToDouble(grossVehicleMassStr);
        //double grossVehicleMassPounds = ConvertKgToPounds(grossVehicleMassKg);

        string lengthStr = specs?.Attributes?.OverallLength ?? "N/A";
        //double lengthInches = ParseToDouble(lengthStr);
        //double lengthMm = ConvertInchesToMm(lengthInches);


        AddSection(report, "Model Report Vehicle Details",
            ("Make", make),
            ("Model", model),
            ("Trim", trim),
            ("Vehicle Category", vehicleCategory),
            ("VIN", car.Vin),
            ("Build Date", specs?.Attributes?.Year), //need to confirm
            ("Model Report Version", "N/A"), //need to confirm
            ("SEVS Entry Number", "N/A"), //need to confirm
            ("Model Report Variant", "N/A")
        );

        AddSection(report, "Model Report Author Information",
            ("Name", car.Owner?.Name),
            ("Address", "N/A"),
            ("Email", car.Owner?.Email),
            ("Contact", car.Owner?.ContactId)
        );

        AddDocumentationSection(report, specs);

        AddSection(report, "Model Report Pre-Modification Scope",
       ("Date Performed", "N/A"),
       ("Make", make),  //from external api
       ("Model", model),
       ("Trim", trim), // from externla api

       ("Vehicle Category", vehicleCategory),
       ("Body Shape", "N/A"),
       ("Build Date Range", "N/A"),
       ("Gross Vehicle Mass", grossVehicleMassStr),
       ("Unladen Mass", "N/A"),
       ("Steering Location", "N/A"),
       ("Work Instruction Unique Document", $"WI_{model?.ToUpper()}"),
       ("Report Type", "SEVS"),
       ("SEVS Entry Number", "N/A"),
       ("SEVS Steering Location", "N/A"),
       ("# Front Seats", "N/A"),
       ("# Rear Seats", "N/A"),
       ("# Side Doors", "N/A"),
       ("# Rear Doors", "N/A"),
       ("Length", lengthStr),
       ("Width (excluding mirrors)", widthExcludingMirrors),
       ("Width (including mirrors)", widthExcludingMirrors),
       ("Height", overAllHeight),
       ("Wheelbase", wheelbase),
       ("Rear Overhang", "N/A"),
       ("Running Clearance", groundClearance),
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


        AddSection(report, "Model Report Recall & Rectification Check",
          ("Identifying Outstanding Recalls", "N/A"),
          ("Sourcing Replacement Parts", "N/A"),
          ("Recall Rectification", "N/A")
      );

        AddSection(report, "Model Report Damage & Corrosion Check",
          ("Levels Preventing Verification", "N/A"),
          ("Acceptable Levels of Damage", "N/A"),
          ("Body Alignment Check", "N/A")
      );
        AddSection(report, "Model Report Deterioration Check & Rectification",
          ("Headlamp", "N/A"),
          ("Taillamp", "N/A"),
          ("CHMSL", "N/A"),
          ("Side Indicator Lamp", "N/A"),
          ("Registration Plate Lamp", "N/A"),
          ("Windshield", "N/A"),
          ("Windows", "N/A"),
          ("Front Left Seatbelt", "N/A"),
          ("Front Right Seatbelt", "N/A"),
          ("Front Right Seatbelt", "N/A"),
          ("Rear Left Seatbelt", "N/A"),
          ("Rear Center Seatbelt", "N/A"),
          ("Rear Right Seatbelt", "N/A"),
          ("Tyres (Front and Rear)", "N/A"),
          ("Tyres (Front and Rear)", "N/A"),
          ("Tyres (Spare)", "N/A"),
          ("Airbags", "N/A"),
          ("Airbag Warning Lamp", "N/A"),
          ("Brake Hoses", "N/A"),
          ("Brake Fluid", "N/A"),
          ("Brake Pads", "N/A"),
          ("Brake Rotors", "N/A")
      );

        AddSection(report, "Model Report Odometer Check",
        ("Signs of Odometer Tampering", "N/A"),
        ("Odometer Reading", "N/A")
        );

        AddSection(report, "Model Report Documentation",
           ("Model Report Vehicle Scope", "N/A"),
           ("Model Report Work Instructions", "N/A"),
           ("Model Report Verification Checklist", "N/A"),
           ("Applicable To", "N/A")
       );

        AddSection(report, "Model Report Component Check",
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


        AddSection(report, "Model Report Manufacturing Steps",
           ("ADR 01/00", "N/A"),
           ("ADR 04/06", "N/A"),
           ("ADR 06/00", "N/A"),
           ("ADR 13/00", "N/A"),
           ("ADR 14/02", "N/A"),
           ("ADR 35/06", "N/A"),
           ("ADR 42/05", "N/A"),
           ("ADR 90/00", "N/A")
       );

        AddSection(report, "Model Report Manufacturing Checks",
            ("ADR 01/00", "N/A"),
            ("ADR 04/06", "N/A"),
            ("ADR 06/00", "N/A"),
            ("ADR 13/00", "N/A"),
            ("ADR 14/02", "N/A"),
            ("ADR 35/06", "N/A"),
            ("ADR 42/05", "N/A"),
            ("ADR 90/00", "N/A")
        );


        AddSpecsSections(report, specs, NA, NAList);

        return report;
    }

    private static void AddSpecsSections(
        ModelReportDto report,
        SpecsResponseDto? specs,
        Func<string?, string> NA,
        Func<IEnumerable<string>?, string> NAList)
    {
        if (specs == null)
            return;

        void AddSection(string title, params (string P, string? V)[] rows)
        {
            var section = new ModelReportSectionDto { Title = title };
            foreach (var (p, v) in rows)
                section.Rows.Add(new ModelReportRowDto { Parameter = p, Value = NA(v) });
            report.Sections.Add(section);
        }

        var a = specs.Attributes;
        if (a != null)
        {
            string grossVehicleMassStr = a.GrossVehicleWeightRating!;
            string vehicleCategory = a.Category;
            //double grossVehicleMassKg = ParseToDouble(grossVehicleMassStr); // Parse and safely convert the value
            //double grossVehicleMassPounds = ConvertKgToPounds(grossVehicleMassKg);

            AddSection("Model Report Post-Modification Scope",
                ("Date Performed", "N/A"),
                ("Make", a.Make),
                ("Model", a.Model),
                ("Trim", a.Trim),
                ("Vehicle Category", vehicleCategory),
                ("Body Shape", "N/A"),
                ("Build Date Range", "N/A"),
                ("Gross Vehicle Mass", grossVehicleMassStr),
                ("Unladen Mass", "N/A"),
                ("Steering Location", "N/A"),
                ("Work Instruction Unique Document", "N/A"),
                ("SEVS Entry Number", "N/A"),
                ("SEVS Steering Location", "N/A"),
                ("Report Type", "N/A"),
                ("Track Front", a.TrackFront),
                ("Track Rear", a.TrackRear),
                ("Doors", a.Doors),
                ("Rear Doors", "N/A"),
                ("Overall Length", a.OverallLength),
                ("Overall Width", a.OverallWidth),
                ("Width At Wall", a.WidthAtWall),
                ("Overall Height", a.OverallHeight),
                ("Wheelbase Length", a.WheelbaseLength),
                ("Rear Overhang", "N/A"),
                ("Ground Clearance", a.GroundClearance),
                ("Motive Power", "N/A"),
                ("Motor Configuration", "N/A"),
                ("Battery Range", "N/A"),
                ("Net Motor Power", "N/A"),
                ("Tyre Make", "N/A"),
                ("Tyre Made", "N/A"),
                ("Tyre Profile", "N/A"),
                ("Tyre Description", "N/A"),
                ("Tyre Load Index", "N/A"),
                ("Tyre Speed Rating", "N/A"),
                ("Wheel Description", "N/A"),
                ("Wheel Profile", "N/A"),
                ("Wheel Offset", "N/A"),
                ("Wheel PCD", "N/A"),
                ("Wheel Lug Nut", "N/A"),
                ("Wheel Centre Bore", "N/A"),


                ("Year", a.Year),
                ("Style", a.Style),
                ("Type", a.Type),
                ("Made In", a.MadeIn),
                ("Made In City", a.MadeInCity),

                ("Fuel Type", a.FuelType),
                ("Fuel Capacity", a.FuelCapacity),
                ("City Mileage", a.CityMileage),
                ("Highway Mileage", a.HighwayMileage),

                ("Engine", a.Engine),
                ("Engine Cylinders", a.EngineCylinders),
                ("Engine Size", a.EngineSize),

                ("Transmission", a.Transmission),
                ("Transmission Short", a.TransmissionShort),
                ("Transmission Type", a.TransmissionType),
                ("Transmission Speeds", a.TransmissionSpeeds),

                ("Drivetrain", a.Drivetrain),
                ("Anti Brake System", a.AntiBrakeSystem),
                ("Steering Type", a.SteeringType),

                ("Curb Weight", a.CurbWeight),
                ("Curb Weight Manual", a.CurbWeightManual),
                ("Gross Vehicle Weight Rating", a.GrossVehicleWeightRating),




                ("Standard Seating", a.StandardSeating),
                ("Optional Seating", a.OptionalSeating),

                ("Passenger Volume", a.PassengerVolume),
                ("Cargo Volume", a.CargoVolume),
                ("Cargo Volume Seats In Place", a.CargoVolumeSeatsInPlace),
                ("Maximum Cargo Volume", a.MaximumCargoVolume),

                ("Cargo Length", a.CargoLength),
                ("Width At Wheelwell", a.WidthAtWheelwell),
                ("Depth", a.Depth),

                ("Standard Towing", a.StandardTowing),
                ("Maximum Towing", a.MaximumTowing),
                ("Standard Payload", a.StandardPayload),
                ("Maximum Payload", a.MaximumPayload),
                ("Maximum GVWR", a.MaximumGvwr),

                ("Vehicle Class", a.VehicleClass),
                ("Vehicle Rating", a.VehicleRating),

                ("Invoice Price", a.InvoicePrice),
                ("Delivery Charges", a.DeliveryCharges),
                ("MSRP", a.ManufacturerSuggestedRetailPrice),

                ("Front Brake Type", a.FrontBrakeType),
                ("Rear Brake Type", a.RearBrakeType),
                ("Turning Diameter", a.TurningDiameter),

                ("Front Suspension", a.FrontSuspension),
                ("Rear Suspension", a.RearSuspension),
                ("Front Spring Type", a.FrontSpringType),
                ("Rear Spring Type", a.RearSpringType),

                ("Tires", a.Tires),

                ("Front Headroom", a.FrontHeadroom),
                ("Rear Headroom", a.RearHeadroom),
                ("Front Legroom", a.FrontLegroom),
                ("Rear Legroom", a.RearLegroom),
                ("Front Shoulder Room", a.FrontShoulderRoom),
                ("Rear Shoulder Room", a.RearShoulderRoom),
                ("Front Hip Room", a.FrontHipRoom),
                ("Rear Hip Room", a.RearHipRoom),

                ("Production Seq Number", a.ProductionSeqNumber),
                ("Interior Trim", NAList(a.InteriorTrim)),
                ("Exterior Color", NAList(a.ExteriorColor)),
                ("Size", a.Size),
                ("Category", a.Category)
            );
        }
    }

    private static void AddDocumentationSection(ModelReportDto report, SpecsResponseDto? specs)
    {
        // Safely get the model name from specs
        string model = specs?.Attributes?.Model ?? "N/A"; // Default to "N/A" if model is null

        AddSection(report, "Model Report Documentation",
            ("Model Report Vehicle Scope", AppendShortCodeToModel("VS", model)),
            ("Model Report Work Instructions", AppendShortCodeToModel("WI", model)),
            ("Model Report Verification Checklist", AppendShortCodeToModel("VC", model)),
            ("Applicable To", "N/A")
        );
    }
}
