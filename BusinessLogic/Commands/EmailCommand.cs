using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Commands
{
    public class EmailCommand
    {
        public string JobNumber { get; set; }
        public List<string> emailCC { get; set; }
        public string BLCode { get; set; }
    }
	public class EmailInvKojaCommand
    {
        public string CustName { get; set; }
        public string CustEmail { get; set; }
		public List<string> emailCC { get; set; }
        public string TransNum { get; set; }
		public string InvNum { get; set; }
		public string InvAmount { get; set; }
		public string InvUrl { get; set; }
    }
	public class EmailGatePassKojaCommand
    {
        public string CustName { get; set; }
        public string CustEmail { get; set; }
		public List<string> emailCC { get; set; }
        public string TransNum { get; set; }
		public string GPUrl { get; set; }
    }
}
