using GoLogs.Api.Models;
using System;
using System.Collections.Generic;

namespace GoLogs.Api.ViewModels
{
    public class DeliveryOrderViewModel : BaseWithoutUUIDModel
    {
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
        public PersonViewModel PersonData { get; set; }
        public List<DeliveryOrderContainerModel> DOContainerData { get; set; }
    }
}
