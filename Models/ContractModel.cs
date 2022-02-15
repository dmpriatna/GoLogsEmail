using System;
using System.ComponentModel.DataAnnotations;

namespace GoLogs.Api.Models
{
    public class ContractModel
    {
        [Key] public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
        public string ContractNumber { get; set; }
        public string EmailPPJK { get; set; }
        public string FirstParty { get; set; }
        public string SecondParty { get; set; }
        public string Services { get; set; }
        public string BillingPeriod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? PriceRate { get; set; }

        #region system need
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        #endregion

        public bool RowStatus { get; set; }
    }
}