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

    private static ModelReportDto BuildReport(Car car, SpecsResponseDto? specs, DateTime now)
    {
        static string NA(string? v) => string.IsNullOrWhiteSpace(v) ? "N/A" : v.Trim();
        static string NADate(DateTime? v) => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : "N/A";
        static string NAList(IEnumerable<string>? v)
            => v == null ? "N/A" : NA(string.Join(", ", v.Where(x => !string.IsNullOrWhiteSpace(x))));

        var report = new ModelReportDto
        {
            Title = "Model Report",
            GeneratedAt = now
        };

        void AddSection(string title, params (string P, string? V)[] rows)
        {
            var section = new ModelReportSectionDto { Title = title };

            foreach (var (p, v) in rows)
                section.Rows.Add(new ModelReportRowDto { Parameter = p, Value = NA(v) });

            report.Sections.Add(section);
        }


        AddSection("Model Report Vehicle Details",
            ("Make", "N/A"), // from externla api
            ("Model", car.Model?.Name),
            ("Trim", "N/A"), // from externla api
            ("Vehicle Category", "N/A"),
            ("VIN", car.Vin),
            ("Build Date", NADate(car.StartDate)), //need to confirm
            ("Model Report Version","N/A"), //need to confirm
            ("SEVS Entry Number","N/A"), //need to confirm
            //("Color", car.Color),
            ("Model Report Variant", car.Model?.ModelCategory?.Name)
        );

        AddSection("Model Report Author Information",
            ("Name", car.Owner?.Name),
            ("Addres", "N/A"),
            ("Email", car.Owner?.Email),
            ("Contact", car.Owner?.ContactId)
        );

        AddSection("Model Report Documentation",
           ("Model Report Vehicle Scope", "N/A"),
           ("Model Report Work Instructions", "N/A"),
           ("Model Report Verification Checklist", "N/A"),
           ("Applicable To", "N/A")
       );

        AddSection("Model Report Pre-Modification Scope",
       ("Date Performed", "N/A"),
       ("Make", "N/A"),  //from external api
       ("Model", car.Model?.Name),
       ("Trim", "N/A"), // from externla api

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


        AddSection("Model Report Recall & Rectification Check",
          ("Identifying Outstanding Recalls", "N/A"),
          ("Sourcing Replacement Parts", "N/A"),
          ("Recall Rectification", "N/A")
      );

        AddSection("Model Report Damage & Corrosion Check",
          ("Levels Preventing Verification", "N/A"),
          ("Acceptable Levels of Damage", "N/A"),
          ("Body Alignment Check", "N/A")
      );
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

        AddSection("Model Report Odometer Check",
        ("Signs of Odometer Tampering", "N/A"),
        ("Odometer Reading", "N/A")
        );

        AddSection("Model Report Documentation",
           ("Model Report Vehicle Scope", "N/A"),
           ("Model Report Work Instructions", "N/A"),
           ("Model Report Verification Checklist", "N/A"),
           ("Applicable To", "N/A")
       );

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

        //AddSection("External Specs (CarsXE) - Meta",
        //    ("Success", specs.Success.ToString()),
        //    ("Message", specs.Message),
        //    ("Timestamp", specs.Timestamp)
        //);

        //if (specs.Input != null)
        //{
        //    AddSection("External Specs (CarsXE) - Input",
        //        ("Input (JSON)", JsonSerializer.Serialize(specs.Input))
        //    );
        //}

        var a = specs.Attributes;
        if (a != null)
        {
            AddSection("Model Report Pre-Modification Scope",
                ("Date Performed", "N/A"),
                ("Make", a.Make),
                ("Model", a.Model),
                ("Trim", a.Trim),
                ("Vehicle Category", "N/A"),
                ("Body Shape", "N/A"),
                ("Build Date Range", "N/A"),
                ("Gross Vehicle Mass", "N/A"),
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
}
