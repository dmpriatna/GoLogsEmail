using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Models
{
    public class DeliveryOrderContainerModel : BaseWithoutUUIDModel
    {
        public Guid DeliveryOrderId { get; set; }
        public string ContainerType { get; set; }
        public string ContainerNo { get; set; }
        public string SealNo { get; set; }
        public string ContainerSize { get; set; }
        public int GrossWeight { get; set; }
		public string LoadType { get; set; }
    }
}
