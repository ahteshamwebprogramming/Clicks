using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;


public partial class PaySlipComponents
{

      public int ID { get; set; }

    public string? Types { get; set; }

    public float? PaidDays { get; set; }

    public bool? IsVariable { get; set; }

    public string? Component { get; set; }

    public decimal? Standard { get; set; }

    public decimal? Actual { get; set; }

    public decimal? Arrears { get; set; }

    public decimal? Total { get; set; }

    public string? DComponent { get; set; }

    public decimal? DActual { get; set; }

    public decimal? DArrears { get; set; }

    public decimal? DTotal { get; set; }



}
