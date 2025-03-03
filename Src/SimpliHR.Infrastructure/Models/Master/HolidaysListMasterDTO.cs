using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Infrastructure.Models.Masters;

public partial class HolidaysListMasterDTO
{
    public int HolidayId { get; set; }
    public int UnitHolidayId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Date, ErrorMessage = "Date is not in a correct format")]
    [DisplayFormat(DataFormatString = "{d-M-Y}")]
    [Required(ErrorMessage = "Please enter date")]
    public DateTime? HolidayDate { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter holiday month")]
    [Range(1, 12, ErrorMessage = "Please enter valid Month Number")]
    public int? HolidayMonth { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter day of holiday")]
    [Range(1, 12, ErrorMessage = "Please enter valid Day Number")]
    public int? HolidayDay { get; set; }

    [ValidateNever]
    public string? HolidayDayName { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter holiday year")]
    [RegularExpression(@"^(\d{4})$", ErrorMessage = "Enter a valid 4 digit Year")]
    public int? HolidayYear { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter holiday name"), MaxLength(100, ErrorMessage = "Holiday name cannot exceed 100 characters")]
    public string? HolidayName { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter holiday type"), MaxLength(50, ErrorMessage = "Holiday type cannot exceed 50 characters")]
    public string? HolidayType { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public List<RoleMasterDTO>? RoleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<HolidaysListMasterDTO>? HolidaysListMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }
}
