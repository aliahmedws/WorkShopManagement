namespace WorkShopManagement.Issues;

public enum IssueDeteriorationType
{
    NotApplicable = 1,

    // Airbag
    Airbags = 2,
    AirbagWarningLamp = 3,

    // Brakes
    BrakeFluid = 4,
    BrakeHoses = 5,
    BrakePads = 6,
    BrakeRotors = 7,

    // Exhaust
    CatalyticConverterExhaust = 8,
    ExhaustMuffler = 9,
    ExhaustTailpipe = 10,

    // Glass
    Windshield = 11,
    Windows = 12,

    // Lighting
    CHMSL = 13,
    HeadLamp = 14,
    RegistrationPlateLamp = 15,
    RearRetroReflectors = 16,
    SideIndicatorLamp = 17,
    TailLamp = 18,

    // Seatbelts
    FrontLeftSeatbelt = 19,
    FrontRightSeatbelt = 20,
    RearLeftSeatbelt = 21,
    RearRightSeatbelt = 22,

    // Tyres
    TyresFrontAndRear = 23,
    TyresSpare = 24
}