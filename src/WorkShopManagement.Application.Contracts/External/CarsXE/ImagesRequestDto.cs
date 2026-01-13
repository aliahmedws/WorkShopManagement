using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WorkShopManagement.External.CarsXE
{
    public class ImagesRequestDto
    {

        /// <summary>
        /// Required for Database Caching (VinInfo table key).
        /// </summary>
        [Required]
        public string Vin { get; set; } = default!;


        // --- API Query Parameters ---

        [Required]
        public string Make { get; set; } = default!;
        [Required]
        public string Model { get; set; } = default!;
        public string? Year { get; set; } 
        public string? Trim { get; set; }
        public string? Color { get; set; }
        public string Angle { get; set; } = "front";                // front, side, back

    }
}
