﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AddReceiptsViewModel
    {
        public int? TimesheetReimbursementId { get; set; } = 0;
        public int TimesheetDetailId { get; set; } = 0;
        [Required]
        public string Items {  get; set; }
        [Required]
        public int Amount { get; set; } = 0;
        public IFormFile? BillAttachment {  get; set; }
        public string? BillAttachmentFileName {  get; set; }
        public DateOnly? DateOfAddReceipts { get; set; }
    }
}
