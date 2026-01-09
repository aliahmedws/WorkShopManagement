namespace WorkShopManagement;

public static class WorkShopManagementDomainErrorCodes
{
    public const string EmptyFile = "EmptyFileError:00004";
    public const string InvalidFileFormat = "InvalidFileFormatError:00005";
    public const string NullField = "NullField:00006";
    public const string CarModelAlreadyExists = "CarModelAlreadyExists";
    public const string CarModelNotFound = "CarModelNotFound";
    public const string InvalidVinLength = "Error:InvalidVinLength";
    public const string DuplicateRecord = "DuplicateRecord:00001";
    public const string DuplicateRecordWithPropertyName = "DuplicateRecord:00002";
    public const string DuplicateRecordWithValue = "DuplicateRecord:00003";
    public const string MissingConfigurations = "MissingConfiguration:00001";
    public const string MissingConfigurationsWithPropertyNames = "MissingConfiguration:00002";

    // CheckInReport
    public const string CheckInReportInvalidBuildMonth = "CheckInReport:InvalidBuildMonth";
    public const string CheckInReportInvalidBuildYear = "CheckInReport:InvalidBuildYear";
    public const string CheckInReportInvalidEntryKms = "CheckInReport:InvalidEntryKms";
    public const string CheckInReportInvalidFrontGwar = "CheckInReport:InvalidFrontGwar";
    public const string CheckInReportInvalidRearGwar = "CheckInReport:InvalidRearGwar";
    public const string CheckInReportInvalidMaxTowingCapacity = "CheckInReport:InvalidMaxTowingCapacity";
}
