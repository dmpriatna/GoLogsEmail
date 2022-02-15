using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoLogs.Api.Models
{
    public class SP2Model
    {
        [Key] public Guid Id { get; set; }
        public string TerminalId { get; set; }
        [Column("TerminalName")] public string TerminalOperator { get; set; }
        [Column("TransactionType")] public string TypeTransaction { get; set; }
        public string TransactionName { get; set; }
        [Column("DocumentCode")] public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string BLNumber { get; set; }
        
        public string JobNumber { get; set; }
        public DateTime? BLDate { get; set; }
        public string SPPBNumber { get; set; }
        public DateTime? SPPBDate { get; set; }
        public string PIBNumber { get; set; }
        public DateTime? PIBDate { get; set; }
        public string DONumber { get; set; }
        public DateTime? DODate { get; set; }

        public int PositionStatus { get; set; }
        public string PaymentMethod { get; set; }

        public string CargoOwnerTaxId { get; set; }
        public string CargoOwnerName { get; set; }
        public string ForwarderTaxId { get; set; }
        public string ForwarderName { get; set; }
        public DateTime DueDate { get; set; }
        public string ProformaInvoiceNo { get; set; }
        public string ProformaInvoiceUrl { get; set; }
        public double SubTotalByThirdParty { get; set; }
        public double PlatformFee { get; set; }
        public double Vat { get; set; }
        public double GrandTotal { get; set; }
        public string CancelReason { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte RowStatus { get; set; }

        [Column("IsDraft")] public bool SaveAsDraft { get; set; }
        public string ServiceName { get; set; }
        public string ContractNumber { get; set; }
        public string FrieghtForwarderName { get; set; }
        [Column("BillOfLadingFile")] public string BLDocument { get; set; }
        [Column("LetterOfIndemnityFile")] public string LetterOfIndemnity { get; set; }
        [Column("AttorneyLetterFile")] public string AttorneyLetter { get; set; }
        public string PositionStatusName { get; set; }
        [Column("NoticeEmail")] public string NotifyEmails { get; set; }
    }
}