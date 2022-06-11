﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Headway.RemediatR.Core.Model
{
    public class RefundCalculation
    {
        public int RefundCalculationId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BasicRefundAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CompensatoryAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CompensatoryInterestAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalCompensatoryAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalRefundAmount { get; set; }

        public DateTime? CalculatedDate { get; set; }

        [MaxLength(50)]
        public string? CalculatedBy { get; set; }
    }
}