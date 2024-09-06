﻿using System.ComponentModel.DataAnnotations;

namespace VMS.Models.DTO
{
    public class VisitorCreationDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
       /* [Required]
        public DateTime VisitDate { get; set; }*/
        [Required]
        public int PurposeOfVisitId { get; set; }
        [Required]
        public string PersonInContact { get; set; }
        
        [Required]
        public int OfficeLocationId { get; set; }
        public string formSubmissionMode { get; set; }

        public List<VisitorDeviceDTO>? SelectedDevice { get; set; }
        public string ImageData { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
