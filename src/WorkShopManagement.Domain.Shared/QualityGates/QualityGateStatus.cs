namespace WorkShopManagement.QualityGates;

public enum QualityGateStatus
{
    PASSED = 1,
    CONDITIONALPASSEDMAJOR = 2, //(RED)
    CONDITIONALPASSEDMINOR = 3, //(YELLOW)
    OPEN = 4, //(BLUE)
    RESET = 5, //(GREY)
}
