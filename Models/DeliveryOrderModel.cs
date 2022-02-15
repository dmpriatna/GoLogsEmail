using System;

namespace GoLogs.Api.Models
{
    public class DeliveryOrderModel : BaseWithoutUUIDModel
    {
        public new Guid? Id { get; set; }
		public string DeliveryOrderType { get; set; }
        public string DeliveryOrderNumber { get; set; }
        public DateTime? DeliveryOrderExpiredDate { get; set; }
        public string DeliveryOrderStatus { get; set; }
        public string JobNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string BillOfLadingNumber { get; set; }
        public DateTime? BillOfLadingDate { get; set; }
        public string ShippingLineName { get; set; }
        public string ShippingLineEmail { get; set; }
        public string Vessel { get; set; }
        public string VoyageNumber { get; set; }
        public string Consignee { get; set; }
        public string PortOfLoading { get; set; }
        public string PortOfDischarge { get; set; }
        public string PortOfDelivery { get; set; }
        public string NotifyPartyName { get; set; }
        public string NotifyPartyAdress { get; set; }
		public double ProformaInvoiceAmount { get; set; }
        public Guid? CustomerID { get; set; }

        public string ContractNumber { get; set; }
        public string ServiceName { get; set; }
    }
}
