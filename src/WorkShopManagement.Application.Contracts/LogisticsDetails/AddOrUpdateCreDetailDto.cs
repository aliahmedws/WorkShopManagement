using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WorkShopManagement.Cars;

namespace WorkShopManagement.LogisticsDetails
{
    public class AddOrUpdateCreDetailDto
    {
        public CreStatus CreStatus { get; set; }
        public DateTime? CreSubmissionDate { get; set; }
        public string? RvsaNumber { get; set; }
    }
}
