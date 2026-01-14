using System;
using System.Collections.Generic;
using WorkShopManagement.CheckLists;

namespace WorkShopManagement.CarBays;

public class CarBayDetails
{
    public CarBay? CarBay {  get; set; }
    public Dictionary<Guid, CheckListProgressStatus> Progress { get; set; } = [];
}
