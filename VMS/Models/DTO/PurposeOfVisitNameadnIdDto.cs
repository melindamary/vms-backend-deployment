﻿using System.ComponentModel.DataAnnotations;

namespace VMS.Models.DTO
{
    public class PurposeOfVisitNameadnIdDTO
    {
        [Required]
        public int PurposeId { get; set; }
        [Required]
        public string PurposeName { get; set; }
    }
}
