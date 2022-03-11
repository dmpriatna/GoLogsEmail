using AutoMapper;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace GoLogs.Api.BusinessLogic.Handler
{
    public class EmailLogic : IEmailLogic
    {
        private readonly GoLogsContext _context;
        private readonly IDeliveryOrderLogic _doLogic;
        private readonly IMapper _mapper;
        private readonly IEmailTemplateLogic _emailTemplateLogic;
        private readonly IPersonLogic _personLogic;
        private readonly INotifyLogic _notifyLogic;

        public EmailLogic(
            GoLogsContext context,
            IDeliveryOrderLogic doLogic, 
            IMapper mapper, 
            IEmailTemplateLogic emailTemplateLogic,
            IPersonLogic personLogic,
            INotifyLogic notifyLogic)
        {
            _context = context;
            _doLogic = doLogic;
            _mapper = mapper;
            _emailTemplateLogic = emailTemplateLogic;
            _personLogic = personLogic;
            _notifyLogic = notifyLogic;
        }

        private async Task<string> GetSignature()
        {
            var signatureM = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("Signature");
            var signatureBody = signatureM.Template.Replace("@DownloadAppsUrl", Constant.GoLogsDomain + Constant.DownloadAppsUrl)
                .Replace("@SupportUrl", Constant.GoLogsDomain + Constant.SupportUrl);
            return signatureBody;
        }

        private async Task<string> GetContainers(List<DeliveryOrderContainerModel> model)
        {
            StringBuilder sb = new StringBuilder();
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var container = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("ContainerRepeater");
                    var containerTemplate = container.Template.Replace("@ContainerNo", item.ContainerNo)
                        .Replace("@ContainerNo", item.ContainerNo)
                        .Replace("@SealNo", item.SealNo)
                        .Replace("@SizeType", item.ContainerType)
                        .Replace("@GrossWeight", item.GrossWeight.ToString())
                        //.Replace("@DepoName", item.DepoName)
                        //.Replace("@PhoneNo", item.PhoneNumber);
						.Replace("@LoadType", item.LoadType);
                    sb.Append(containerTemplate);
                }
            }
            return sb.ToString();
        }

        private async Task<string> GetStaticTemplate(PersonViewModel person, DeliveryOrderViewModel doView)
        {
            var staticM = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("StaticTemplate");
            var staticBody = staticM.Template;
            staticBody = staticBody.Replace("@FullName", person.FullName)
                .Replace("@JobNo", doView.JobNumber)
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@CreatedDate", doView.CreatedDate.ToString("dd MMM yyyy HH:mm"))
                .Replace("@Status", doView.DeliveryOrderStatus)
                .Replace("@CompanyName", person.Company.Name + "&nbsp;")
                .Replace("@ShippingLineName", doView.ShippingLineName)
                .Replace("@ShippingLineEmail", doView.ShippingLineEmail)
                .Replace("@VesselName", doView.Vessel)
                .Replace("@BLNo", doView.BillOfLadingNumber)
                .Replace("@Consignee", doView.Consignee)
                .Replace("@VesselCode", doView.Vessel)
                .Replace("@BLNo", doView.BillOfLadingNumber)
                .Replace("@VoyageNo", doView.VoyageNumber)
                .Replace("@PortOfLoading", doView.PortOfLoading)
                .Replace("@PortOfDischarge", doView.PortOfDischarge)
                .Replace("@PortOfDelivery", doView.PortOfDelivery)
                .Replace("@NotifyParty", doView.NotifyPartyName);
            return staticBody;
        }

        private async Task<string> GetStaticTemplate(DeliveryOrderViewModel doView, string FullName, string CompanyName)
        {
            var staticM = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("StaticTemplate");
            var staticBody = staticM.Template;
            staticBody = staticBody.Replace("@FullName", FullName)
                .Replace("@JobNo", doView.JobNumber)
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@CreatedDate", doView.CreatedDate.ToString("dd MMM yyyy HH:mm"))
                .Replace("@Status", doView.DeliveryOrderStatus)
                .Replace("@CompanyName", CompanyName + "&nbsp;")
                .Replace("@ShippingLineName", doView.ShippingLineName)
                .Replace("@ShippingLineEmail", doView.ShippingLineEmail)
                .Replace("@VesselName", doView.Vessel)
                .Replace("@BLNo", doView.BillOfLadingNumber)
                .Replace("@Consignee", doView.Consignee)
                .Replace("@VesselCode", doView.Vessel)
                .Replace("@BLNo", doView.BillOfLadingNumber)
                .Replace("@VoyageNo", doView.VoyageNumber)
                .Replace("@PortOfLoading", doView.PortOfLoading)
                .Replace("@PortOfDischarge", doView.PortOfDischarge)
                .Replace("@PortOfDelivery", doView.PortOfDelivery)
                .Replace("@NotifyParty", doView.NotifyPartyName);
            return staticBody;
        }

        private string ReplaceSubject(string subject, string jobNo, string companyName = null, string transNum = null)
        {
            subject = subject.Replace("@SelectedService", "Delivery Order")
                .Replace("@JobNo", jobNo)
                .Replace("@TransNum", transNum)
				.Replace("@CompanyName", companyName);
            return subject;
        }

        public async Task AfterDORequestAsync(EmailCommand command)
        {
            var selectedService = "Delivery Order";
            var signature = await GetSignature();
            var doView = await _doLogic.GetDOByJobNumberAsync(command.JobNumber);
            var person = await _personLogic.GetPersonViewByIdAsync((Guid)doView.CustomerID);
            var staticTemplate = await GetStaticTemplate(person, doView);

            var BLCodeParam = Constant.BLCodeParam.Replace("@BLCodeParam", command.BLCode);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORequestCustomer");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + signature;
            custBody = custBody.Replace("@FullName", person.FullName)
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@StatusUrl", Constant.GoLogsAppDomain + "do-request/" + doView.Id);

            var ship = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORequestShippingLine");
            var shipSubject = ReplaceSubject(ship.Subject, doView.JobNumber, person.Company.Name);
            var shipBody = ship.Template + signature;
            shipBody = shipBody.Replace("@ShippingLineName", doView.ShippingLineName)
                .Replace("@CompanyName", person.Company.Name + "&nbsp;")
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@StaticTemplate", staticTemplate)
				.Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + "order/" + doView.Id)                
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl);

            // To Customer
            // Request Form Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep1");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            await _notifyLogic.TransactionRequestAsync(selectedService, doView.JobNumber, doView.PersonData.Id);
            GlobalHelper.SendEmailWithCC(doView.PersonData.Email, command.emailCC, custSubject, custBody);

            // To ShippingLine
            // Confirm Request Status
            var shippingLineStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("ShippingLineStep1");
            shipBody = shipBody.Replace("@StepStatus", shippingLineStepStatus.Template);

            GlobalHelper.SendEmailWithCC(doView.ShippingLineEmail, command.emailCC, shipSubject, shipBody);
        }

        public async Task AfterInvoiceAsync(EmailCommand command)
        {
            var selectedService = "Delivery Order";
            var doView = await _doLogic.GetDOByJobNumberAsync(command.JobNumber);
            var person = await _personLogic.GetPersonViewByIdAsync((Guid)doView.CustomerID);
            var staticTemplate = await GetStaticTemplate(person, doView);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterInvoice");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + await GetSignature();
			DateTime today = DateTime.Now;
			DateTime timeLimit = today.AddHours(2);
			string specifier;
			CultureInfo culture;
			specifier = "C";
			culture = CultureInfo.CreateSpecificCulture("id-ID");
			var proformaAmount = doView.ProformaInvoiceAmount.ToString(specifier, culture);
			var totalAmount = doView.ProformaInvoiceAmount + 55000;
            custBody = custBody.Replace("@FullName", person.FullName)
                .Replace("@JobNo", doView.JobNumber)
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@StaticTemplate", staticTemplate)
				.Replace("@CompleteBeforeDate", timeLimit.ToString("dd MMMM yyyy HH:mm"))
                .Replace("@BillingDetail",  Constant.GoLogsAppDomain + "do-request/" + doView.Id)
				.Replace("@ConductPaymentUrl", Constant.GoLogsAppDomain + Constant.ConductPaymentUrl)
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl)
				.Replace("@Amount", proformaAmount)
				.Replace("@ServiceFee", "Rp 50.000")
				.Replace("@AddedTax", "Rp 5.000")
				.Replace("@TotalAmount", totalAmount.ToString(specifier, culture));

            // To Customer
            // Confirmation From Shipping Line Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep2");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            await _notifyLogic.TransactionInvoiceAsync(selectedService, doView.JobNumber, doView.PersonData.Id);
            GlobalHelper.SendEmailWithCC(doView.PersonData.Email, command.emailCC, custSubject, custBody);
        }

        public async Task AfterPaymentAsync(EmailCommand command)
        {
            var selectedService = "Delivery Order";
            var signature = await GetSignature();
            var doView = await _doLogic.GetDOByJobNumberAsync(command.JobNumber);
            var person = await _personLogic.GetPersonViewByIdAsync((Guid)doView.CustomerID);
            var staticTemplate = await GetStaticTemplate(person, doView);
			var BLCodeParam = Constant.BLCodeParam.Replace("@BLCodeParam", command.BLCode);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterPaymentCustomer");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + signature;
            custBody = custBody.Replace("@FullName", person.FullName)
                .Replace("@JobNo", doView.JobNumber)
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@PaymentUploadUrl", Constant.GoLogsAppDomain + Constant.PaymentUploadUrl + BLCodeParam + "tab=3")
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl);

            var ship = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterPaymentShippingLine");
            var shipSubject = ReplaceSubject(ship.Subject, doView.JobNumber, person.Company.Name);
            var shipBody = ship.Template + signature;
            shipBody = shipBody.Replace("@ShippingLineName", doView.ShippingLineName)
                .Replace("@CompanyName", person.Company.Name + "&nbsp;")
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + Constant.DocumentUploadUrl + BLCodeParam + "tab=4");

            // To Customer
            // Proforma Invoice & Payment Confirmation Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep4");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            await _notifyLogic.TransactionPaymentAsync(selectedService, doView.JobNumber, doView.PersonData.Id);
            GlobalHelper.SendEmailWithCC(doView.PersonData.Email, command.emailCC, custSubject, custBody);

            // To ShippingLine
            // Payment Confirmation Status
            var shippingLineStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("ShippingLineStep3");
            shipBody = shipBody.Replace("@StepStatus", shippingLineStepStatus.Template);

            GlobalHelper.SendEmailWithCC(doView.ShippingLineEmail, command.emailCC, shipSubject, shipBody);
        }

        public async Task AfterDOReleaseAsync(EmailCommand command)
        {
            var selectedService = "Delivery Order";
            var doView = await _doLogic.GetDOByJobNumberAsync(command.JobNumber);
            var person = await _personLogic.GetPersonViewByIdAsync((Guid)doView.CustomerID);
            var staticTemplate = await GetStaticTemplate(person, doView);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORelease");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + await GetSignature();
            custBody = custBody.Replace("@FullName", person.FullName)
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + "do-request/" + doView.Id)
				.Replace("@StatusUrl", Constant.GoLogsAppDomain + "do-request/" + doView.Id)
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl)
                .Replace("@SHOWDOURL", Constant.GoLogsAppDomain + "do-request/" + doView.Id);

            // To Customer
            // DO Release Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep5");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            await _notifyLogic.TransactionReleaseAsync(selectedService, doView.JobNumber, doView.PersonData.Id);
            GlobalHelper.SendEmailWithCC(doView.PersonData.Email, command.emailCC, custSubject, custBody);
        }
		
		public async Task AfterInvoiceKojaAsync(EmailInvKojaCommand command)
        {   
			var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterInvoiceKoja");
			var custSubject = ReplaceSubject(cust.Subject, "","",command.TransNum);
            var custBody = cust.Template + await GetSignature();
			DateTime today = DateTime.Now;
			DateTime timeLimit = today.AddHours(2);
			CultureInfo culture;
			culture = CultureInfo.CreateSpecificCulture("id-ID");
			var proformaAmount = double.Parse(command.InvAmount, System.Globalization.CultureInfo.InvariantCulture);
			var totalAmount = proformaAmount + 165000;
            custBody = custBody.Replace("@CustName", command.CustName)
                .Replace("@TransNum", command.TransNum)
				.Replace("@InvNum", command.InvNum)
				.Replace("@InvAmount", "Rp " + command.InvAmount)
				.Replace("@ServiceFee", "Rp 150.000")
				.Replace("@VAT", "Rp 15.000")
				.Replace("@TotalAmount", "Rp " + totalAmount.ToString())
				.Replace("@CompleteBeforeDate", timeLimit.ToString("dd MMMM yyyy HH:mm"))
                .Replace("@ConductPaymentUrl", Constant.GoLogsAppDomain + Constant.ConductPaymentUrl)
				.Replace("@InvUrl", command.InvUrl)
				.Replace("@SupportUrl", Constant.GoLogsDomain + Constant.SupportUrl);
			GlobalHelper.SendEmailWithCC(command.CustEmail, command.emailCC, custSubject, custBody);
        }
		
		public async Task GatePassAsync(EmailGatePassKojaCommand command)
        {
            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("GatePass");
			var custSubject = ReplaceSubject(cust.Subject, "","",command.TransNum);
            var custBody = cust.Template + await GetSignature();
            custBody = custBody.Replace("@CustName", command.CustName)
                .Replace("@TransNum", command.TransNum)
				.Replace("@GPUrl", command.GPUrl)
				.Replace("@SupportUrl", Constant.GoLogsDomain + Constant.SupportUrl);
			GlobalHelper.SendEmailWithCC(command.CustEmail, command.emailCC, custSubject, custBody);
        }

        public async Task Activation(string activationCode)
        {
            try
            {
                var person = await _personLogic.GetPersonByActivationCodeAsync(activationCode);
                
                if (person == null)
                {
                    throw new ArgumentException(Constant.ErrorFromServer + "Activation Code does not exist.");
                }
                else
                {
                    if (person.Activated)
                    {
                        throw new ArgumentException(Constant.ErrorFromServer + "This customer has been activated.");
                    }
                    else
                    {
                        person.Activated = true;
                        _context.Update(person);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ResendActivation(string email)
        {
            try
            {
                var person = await _personLogic.GetPersonByEmailAsync(email);

                if (person == null)
                {
                    throw new ArgumentException(Constant.ErrorFromServer + "This customer does not exist.");
                }
                else
                {
                    if (person.Activated)
                    {
                        throw new ArgumentException(Constant.ErrorFromServer + "This customer has been activated.");
                    }
                    else
                    {
                        var activationUrl = Constant.GoLogsAppDomain + Constant.ActivationUrl + person.ActivationCode.ToString();
                        var template = await _context.EmailTemplates.Where(x => x.Type == "AccountRegistration").FirstOrDefaultAsync();
                        var signature = await _context.EmailTemplates.Where(x => x.Type == "Signature").FirstOrDefaultAsync();
                        var body = template.Template + signature.Template;

                        body = body.Replace("@FullName", person.FullName);
                        body = body.Replace("@Email", person.Email);
                        body = body.Replace("@ActivationUrl", activationUrl);

                        GlobalHelper.SendEmail(person.Email, template.Subject, body);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region DELEGATE

        string fullname = "";
        string emailTo = "";
        string selectedService = "Delivery Order";
        PersonModel personEntity = null;
        DeliveryOrderModel doEntity = null;
        DeliveryOrderViewModel doView = null;

        private async Task DelegateInfo(EmailCommand command)
        {
            doEntity = await _context.DeliveryOrders
                .Where(w => w.JobNumber == command.JobNumber &&
                w.ServiceName != null &&
                w.RowStatus == 1)
                .SingleOrDefaultAsync();

            if (doEntity == null)
            throw new ArgumentException(Constant.ErrorFromServer + "Delivery Order does not exist.");

            var containerEntity = await _context.DeliveryOrderContainers
            .Where(w => w.DeliveryOrderId == doEntity.Id)
            .ToListAsync();

            if (string.IsNullOrWhiteSpace(doEntity.ContractNumber)) return; // Tidak ada alamat email penerima.

            var contractEntity = await _context.Contract
            .Where(w => w.ContractNumber == doEntity.ContractNumber)
            .SingleOrDefaultAsync();

            if (contractEntity == null || string.IsNullOrWhiteSpace(contractEntity.EmailPPJK))
            {
                return; // Tidak ada alamat email penerima.
            }
            else
            {
                emailTo = contractEntity.EmailPPJK;
            }

            personEntity = await _context.Persons
            .Where(w => w.Email == emailTo)
            .SingleOrDefaultAsync();

            fullname = personEntity == null ? emailTo.Split('@')[0] : personEntity.FullName;

            doView = _mapper.Map<DeliveryOrderModel, DeliveryOrderViewModel>(doEntity);
            doView.DOContainerData = containerEntity;
            doView.DeliveryOrderStatus = doEntity.PositionStatusName;
        }

        public async Task AfterDelegateAsync(EmailCommand command)
        {
            try
            {
                string emailTo = null;
                string customer = "pelanggan";
                string service = "Delivery Order";
                
                var dEntity = await _context.DeliveryOrders
                .Where(w => w.JobNumber == command.JobNumber &&
                w.ServiceName != null &&
                w.RowStatus == 1)
                .SingleOrDefaultAsync();
                
                var sEntity = await _context.SP2
                .Where(w => w.JobNumber == command.JobNumber &&
                w.ServiceName != null &&
                w.RowStatus == 1)
                .SingleOrDefaultAsync();

                if (dEntity == null && sEntity == null)
                throw new ArgumentException($"{Constant.ErrorFromServer}No delegate by jobnumber: {command.JobNumber}");

                var contractNumber = dEntity != null ? dEntity.ContractNumber : sEntity.ContractNumber;
                if (string.IsNullOrWhiteSpace(contractNumber))
                {
                    return; // Tidak ada alamat email penerima.
                }
                else
                {
                    var cEntity = await _context.Contract
                    .Where(w => w.ContractNumber == contractNumber)
                    .SingleOrDefaultAsync();
                    if (cEntity == null || string.IsNullOrWhiteSpace(cEntity.EmailPPJK))
                    {
                        return; // Tidak ada alamat email penerima.
                    }
                    else
                    {
                        emailTo = cEntity.EmailPPJK;
                    }
                }

                var personEntity = await _context.Persons
                .Where(w => w.Email == emailTo)
                .SingleOrDefaultAsync();

                customer = personEntity == null ? emailTo.Split('@')[0] : personEntity.FullName;
                service = dEntity == null ? "SP2" : service;
                var idEntity = dEntity == null ? sEntity.Id : dEntity.Id;
                var requestor = dEntity == null ? sEntity.CreatedBy : dEntity.CreatedBy;
                var createdDate = dEntity == null ? sEntity.CreatedDate : dEntity.CreatedDate;
                var status = dEntity == null ? sEntity.PositionStatusName : dEntity.PositionStatusName;

                var dEmail = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("Delegate");
                var dSubject = dEmail.Subject
                    .Replace("@FFPPJKCompanyName", customer)
                    .Replace("@JobNumber", command.JobNumber);
                var dBody = dEmail.Template
                    .Replace("@CustomerName", customer)
                    .Replace("@ServiceName", service)
                    .Replace("@JobNumer", command.JobNumber)
                    .Replace("@No", "1")
                    .Replace("@Requestor", requestor)
                    .Replace("@DelegateTo", emailTo)
                    .Replace("@DelegateService", service)
                    .Replace("@CreatedDate", createdDate.ToString())
                    .Replace("@Status", status)
                    .Replace("@DetailUrl", Constant.GoLogsAppDomain + "delegate/" + idEntity);

                GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, dSubject, dBody);
            }
            catch (System.Exception se)
            {
                throw se;
            }
        }

        public async Task AfterForwarderDelegate(EmailCommand command)
        {
            try
            {
                string emailTo = null;
                string customer = "pelanggan";
                string service = "Delivery Order";
                ContractModel cEntity;
                
                var dEntity = await _context.DeliveryOrders
                .Where(w => w.JobNumber == command.JobNumber &&
                w.ServiceName != null &&
                w.RowStatus == 1)
                .SingleOrDefaultAsync();
                
                var sEntity = await _context.SP2
                .Where(w => w.JobNumber == command.JobNumber &&
                w.ServiceName != null &&
                w.RowStatus == 1)
                .SingleOrDefaultAsync();

                if (dEntity == null && sEntity == null)
                throw new ArgumentException($"{Constant.ErrorFromServer}No delegate by jobnumber: {command.JobNumber}");

                var contractNumber = dEntity != null ? dEntity.ContractNumber : sEntity.ContractNumber;
                if (string.IsNullOrWhiteSpace(contractNumber))
                {
                    return; // Tidak ada alamat email penerima.
                }
                else
                {
                    cEntity = await _context.Contract
                    .Where(w => w.ContractNumber == contractNumber)
                    .SingleOrDefaultAsync();
                    if (cEntity == null || string.IsNullOrWhiteSpace(cEntity.EmailPPJK))
                    {
                        return; // Tidak ada alamat email penerima.
                    }
                    else
                    {
                        emailTo = cEntity.EmailPPJK;
                    }
                }

                var personEntity = await _context.Persons
                .Where(w => w.Email == emailTo)
                .SingleOrDefaultAsync();

                var companyEntity = await _context.Companies
                .Where(w => w.Id == cEntity.CargoOwnerId)
                .SingleOrDefaultAsync();

                customer = personEntity == null ? emailTo.Split('@')[0] : personEntity.FullName;
                service = dEntity == null ? "SP2" : service;
                var idEntity = dEntity == null ? sEntity.Id : dEntity.Id;
                var requestor = dEntity == null ? sEntity.CreatedBy : dEntity.CreatedBy;
                var delegator = dEntity == null ? sEntity.FrieghtForwarderName : dEntity.FrieghtForwarderName;
                var delegateTo = companyEntity == null ? delegator : companyEntity.Email;
                var fCompanyName = cEntity.FirstParty;
                var cCompanyName = cEntity.SecondParty;
                var createdDate = dEntity == null ? sEntity.CreatedDate : dEntity.CreatedDate;
                var status = dEntity == null ? sEntity.PositionStatusName : dEntity.PositionStatusName;

                var dEmail = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("DelegateForwarder");
                var dSubject = dEmail.Subject
                    .Replace("@RequestorCompanyName", fCompanyName)
                    .Replace("@ServiceName", service)
                    .Replace("@CustomerCompanyName", cCompanyName);
                var dBody = dEmail.Template
                    .Replace("@CustomerName", customer)
                    .Replace("@ServiceName", service)
                    .Replace("@JobNumer", command.JobNumber)
                    .Replace("@No", "1")
                    .Replace("@Requestor", requestor)
                    .Replace("@DelegateTo", delegateTo)
                    .Replace("@DelegateService", service)
                    .Replace("@CreatedDate", createdDate.ToString())
                    .Replace("@Status", status)
                    .Replace("@DetailUrl", $"{Constant.GoLogsAppDomain}forwarder/{idEntity}/continue");

                GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, dSubject, dBody);
            }
            catch (System.Exception se)
            {
                throw se;
            }
        }

        public async Task AfterDORequestDelegateAsync(EmailCommand command)
        {
            await DelegateInfo(command);
            var signature = await GetSignature();
            var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);
            var BLCodeParam = Constant.BLCodeParam.Replace("@BLCodeParam", command.BLCode);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORequestCustomer");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + signature;
            custBody = custBody.Replace("@FullName", fullname)
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@StatusUrl", Constant.GoLogsAppDomain + "do-request/" + doView.Id);

            var ship = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORequestShippingLine");
            var shipSubject = ReplaceSubject(ship.Subject, doView.JobNumber, doEntity.FrieghtForwarderName);
            var shipBody = ship.Template + signature;
            shipBody = shipBody.Replace("@ShippingLineName", doView.ShippingLineName)
                .Replace("@CompanyName", doEntity.FrieghtForwarderName + "&nbsp;")
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", staticTemplate)
				.Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + "order/" + doView.Id)                
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl);

            // To Customer
            // Request Form Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep1");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            if (personEntity != null)
            await _notifyLogic.TransactionRequestAsync(selectedService, doView.JobNumber, personEntity.Id);
            GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);

            // To ShippingLine
            // Confirm Request Status
            var shippingLineStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("ShippingLineStep1");
            shipBody = shipBody.Replace("@StepStatus", shippingLineStepStatus.Template);

            GlobalHelper.SendEmailWithCC(doView.ShippingLineEmail, command.emailCC, shipSubject, shipBody);
        }

        public async Task AfterInvoiceDelegateAsync(EmailCommand command)
        {
            await DelegateInfo(command);
            var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterInvoice");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + await GetSignature();
			DateTime today = DateTime.Now;
			DateTime timeLimit = today.AddHours(2);
			string specifier;
			CultureInfo culture;
			specifier = "C";
			culture = CultureInfo.CreateSpecificCulture("id-ID");
			var proformaAmount = doView.ProformaInvoiceAmount.ToString(specifier, culture);
			var totalAmount = doView.ProformaInvoiceAmount + 55000;
            custBody = custBody.Replace("@FullName", fullname)
                .Replace("@JobNo", doView.JobNumber)
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", staticTemplate)
				.Replace("@CompleteBeforeDate", timeLimit.ToString("dd MMMM yyyy HH:mm"))
                .Replace("@BillingDetail",  Constant.GoLogsAppDomain + "do-request/" + doView.Id)
				.Replace("@ConductPaymentUrl", Constant.GoLogsAppDomain + Constant.ConductPaymentUrl)
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl)
				.Replace("@Amount", proformaAmount)
				.Replace("@ServiceFee", "Rp 50.000")
				.Replace("@AddedTax", "Rp 5.000")
				.Replace("@TotalAmount", totalAmount.ToString(specifier, culture));

            // To Customer
            // Confirmation From Shipping Line Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep2");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            if (personEntity != null)
            await _notifyLogic.TransactionInvoiceAsync(selectedService, doView.JobNumber, personEntity.Id);
            GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);
        }

        public async Task AfterPaymentDelegateAsync(EmailCommand command)
        {
            await DelegateInfo(command);
            var signature = await GetSignature();
            var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);
			var BLCodeParam = Constant.BLCodeParam.Replace("@BLCodeParam", command.BLCode);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterPaymentCustomer");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + signature;
            custBody = custBody.Replace("@FullName", fullname)
                .Replace("@JobNo", doView.JobNumber)
                .Replace("@SelectedService", "Delivery Order")
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@PaymentUploadUrl", Constant.GoLogsAppDomain + Constant.PaymentUploadUrl + BLCodeParam + "tab=3")
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl);

            var ship = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterPaymentShippingLine");
            var shipSubject = ReplaceSubject(ship.Subject, doView.JobNumber, doEntity.FrieghtForwarderName);
            var shipBody = ship.Template + signature;
            shipBody = shipBody.Replace("@ShippingLineName", doView.ShippingLineName)
                .Replace("@CompanyName", doEntity.FrieghtForwarderName + "&nbsp;")
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + Constant.DocumentUploadUrl + BLCodeParam + "tab=4");

            // To Customer
            // Proforma Invoice & Payment Confirmation Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep4");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            if (personEntity != null)
            await _notifyLogic.TransactionPaymentAsync(selectedService, doView.JobNumber, personEntity.Id);
            GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);

            // To ShippingLine
            // Payment Confirmation Status
            var shippingLineStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("ShippingLineStep3");
            shipBody = shipBody.Replace("@StepStatus", shippingLineStepStatus.Template);

            GlobalHelper.SendEmailWithCC(doView.ShippingLineEmail, command.emailCC, shipSubject, shipBody);
        }

        public async Task AfterDOReleaseDelegateAsync(EmailCommand command)
        {
            await DelegateInfo(command);
            var selectedService = "Delivery Order";
            var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);

            if (doView.DOContainerData.Count > 0)
            {
                var containerTemplate = await GetContainers(doView.DOContainerData);
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            }
            else
            {
                staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORelease");
            var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            var custBody = cust.Template + await GetSignature();
            custBody = custBody.Replace("@FullName", fullname)
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", staticTemplate)
                .Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + "do-request/" + doView.Id)
				.Replace("@StatusUrl", Constant.GoLogsAppDomain + "do-request/" + doView.Id)
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl)
                .Replace("@SHOWDOURL", Constant.GoLogsAppDomain + "do-request/" + doView.Id);

            // To Customer
            // DO Release Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep5");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            if (personEntity != null)
            await _notifyLogic.TransactionReleaseAsync(selectedService, doView.JobNumber, personEntity.Id);
            GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);
        }

        #endregion

        private CustomClearanceModel Custom { get; set; }

        private async Task CustomInfo(EmailCommand command)
        {
            selectedService = "Custom Clearance";
            Custom = await _context.CustomClearance
            .Where(w => w.JobNumber == command.JobNumber)
            .SingleOrDefaultAsync();

            var companyEntity = await _context.Companies
            .Where(w => w.Name == Custom.CargoOwnerName)
            .FirstOrDefaultAsync();

            if (companyEntity != null)
            {
                emailTo = companyEntity.Email;
                personEntity = await _context.Persons
                .Where(w => w.Email == emailTo)
                .SingleOrDefaultAsync();
                fullname = personEntity.FullName;
            }
        }
        
        public async Task AfterCustomRequestAsync(EmailCommand command)
        {
            await CustomInfo(command);
            var signature = await GetSignature();
            // var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);
            // var BLCodeParam = Constant.BLCodeParam.Replace("@BLCodeParam", command.BLCode);

            // if (doView.DOContainerData.Count > 0)
            // {
            //     var containerTemplate = await GetContainers(doView.DOContainerData);
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            // }
            // else
            // {
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            // }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORequestCustomer");
            var custSubject = ReplaceSubject(cust.Subject, Custom.JobNumber);
            var custBody = cust.Template + signature;
            custBody = custBody.Replace("@FullName", fullname)
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", "")
                .Replace("@StatusUrl", Constant.GoLogsAppDomain + "custom-clearance/" + Custom.Id);

            // var ship = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORequestShippingLine");
            // var shipSubject = ReplaceSubject(ship.Subject, doView.JobNumber, doEntity.FrieghtForwarderName);
            // var shipBody = ship.Template + signature;
            // shipBody = shipBody.Replace("@ShippingLineName", doView.ShippingLineName)
            //     .Replace("@CompanyName", doEntity.FrieghtForwarderName + "&nbsp;")
            //     .Replace("@SelectedService", selectedService)
            //     .Replace("@StaticTemplate", staticTemplate)
			// 	.Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + "order/" + doView.Id)                
            //     .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl);

            // To Customer
            // Request Form Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep1");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            if (personEntity != null)
            await _notifyLogic.TransactionRequestAsync(selectedService, Custom.JobNumber, personEntity.Id);
            GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);

            // To ShippingLine
            // Confirm Request Status
            // var shippingLineStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("ShippingLineStep1");
            // shipBody = shipBody.Replace("@StepStatus", shippingLineStepStatus.Template);

            // GlobalHelper.SendEmailWithCC(doView.ShippingLineEmail, command.emailCC, shipSubject, shipBody);
        }

        public async Task AfterInvoiceCustomAsync(EmailCommand command)
        {
            await CustomInfo(command);
            // var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);

            // if (doView.DOContainerData.Count > 0)
            // {
            //     var containerTemplate = await GetContainers(doView.DOContainerData);
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            // }
            // else
            // {
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            // }

            // var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterInvoice");
            // var custSubject = ReplaceSubject(cust.Subject, doView.JobNumber);
            // var custBody = cust.Template + await GetSignature();
			// DateTime today = DateTime.Now;
			// DateTime timeLimit = today.AddHours(2);
			// string specifier;
			// CultureInfo culture;
			// specifier = "C";
			// culture = CultureInfo.CreateSpecificCulture("id-ID");
			// var proformaAmount = doView.ProformaInvoiceAmount.ToString(specifier, culture);
			// var totalAmount = doView.ProformaInvoiceAmount + 55000;
            // custBody = custBody.Replace("@FullName", fullname)
            //     .Replace("@JobNo", doView.JobNumber)
            //     .Replace("@SelectedService", selectedService)
            //     .Replace("@StaticTemplate", staticTemplate)
			// 	.Replace("@CompleteBeforeDate", timeLimit.ToString("dd MMMM yyyy HH:mm"))
            //     .Replace("@BillingDetail",  Constant.GoLogsAppDomain + "do-request/" + doView.Id)
			// 	.Replace("@ConductPaymentUrl", Constant.GoLogsAppDomain + Constant.ConductPaymentUrl)
            //     .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl)
			// 	.Replace("@Amount", proformaAmount)
			// 	.Replace("@ServiceFee", "Rp 50.000")
			// 	.Replace("@AddedTax", "Rp 5.000")
			// 	.Replace("@TotalAmount", totalAmount.ToString(specifier, culture));

            // To Customer
            // Confirmation From Shipping Line Status
            // var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep2");
            // custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            // if (personEntity != null)
            // await _notifyLogic.TransactionInvoiceAsync(selectedService, doView.JobNumber, personEntity.Id);
            // GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);
        }

        public async Task AfterPaymentCustomAsync(EmailCommand command)
        {
            await CustomInfo(command);
            var signature = await GetSignature();
            // var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);
			// var BLCodeParam = Constant.BLCodeParam.Replace("@BLCodeParam", command.BLCode);

            // if (doView.DOContainerData.Count > 0)
            // {
            //     var containerTemplate = await GetContainers(doView.DOContainerData);
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            // }
            // else
            // {
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            // }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterPaymentCustomer");
            var custSubject = ReplaceSubject(cust.Subject, Custom.JobNumber);
            var custBody = cust.Template + signature;
            custBody = custBody.Replace("@FullName", fullname)
                .Replace("@JobNo", Custom.JobNumber)
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", "")
                .Replace("@PaymentUploadUrl", Constant.GoLogsAppDomain + Constant.PaymentUploadUrl + command.BLCode + "tab=3")
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl);

            // var ship = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterPaymentShippingLine");
            // var shipSubject = ReplaceSubject(ship.Subject, doView.JobNumber, doEntity.FrieghtForwarderName);
            // var shipBody = ship.Template + signature;
            // shipBody = shipBody.Replace("@ShippingLineName", doView.ShippingLineName)
            //     .Replace("@CompanyName", doEntity.FrieghtForwarderName + "&nbsp;")
            //     .Replace("@SelectedService", selectedService)
            //     .Replace("@StaticTemplate", staticTemplate)
            //     .Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + Constant.DocumentUploadUrl + BLCodeParam + "tab=4");

            // To Customer
            // Proforma Invoice & Payment Confirmation Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep4");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            if (personEntity != null)
            await _notifyLogic.TransactionPaymentAsync(selectedService, Custom.JobNumber, personEntity.Id);
            GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);

            // To ShippingLine
            // Payment Confirmation Status
            // var shippingLineStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("ShippingLineStep3");
            // shipBody = shipBody.Replace("@StepStatus", shippingLineStepStatus.Template);

            // GlobalHelper.SendEmailWithCC(doView.ShippingLineEmail, command.emailCC, shipSubject, shipBody);
        }

        public async Task AfterReleaseCustomAsync(EmailCommand command)
        {
            await CustomInfo(command);
            // var staticTemplate = await GetStaticTemplate(doView, fullname, doEntity.FrieghtForwarderName);

            // if (doView.DOContainerData.Count > 0)
            // {
            //     var containerTemplate = await GetContainers(doView.DOContainerData);
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", containerTemplate);
            // }
            // else
            // {
            //     staticTemplate = staticTemplate.Replace("@ContainerRepeater", "");
            // }

            var cust = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("AfterDORelease");
            var custSubject = ReplaceSubject(cust.Subject, Custom.JobNumber);
            var custBody = cust.Template + await GetSignature();
            custBody = custBody.Replace("@FullName", fullname)
                .Replace("@SelectedService", selectedService)
                .Replace("@StaticTemplate", "")
                .Replace("@DocumentUploadUrl", Constant.GoLogsAppDomain + "custom-clearance/" + Custom.Id)
				.Replace("@StatusUrl", Constant.GoLogsAppDomain + "custom-clearance/" + Custom.Id)
                .Replace("@SupportUrl", Constant.GoLogsAppDomain + Constant.SupportUrl)
                .Replace("@SHOWDOURL", Constant.GoLogsAppDomain + "custom-clearance/" + Custom.Id);

            // To Customer
            // DO Release Status
            var custStepStatus = await _emailTemplateLogic.GetEmailTemplateByTypeAsync("CustomerStep5");
            custBody = custBody.Replace("@StepStatus", custStepStatus.Template);

            if (personEntity != null)
            await _notifyLogic.TransactionReleaseAsync(selectedService, Custom.JobNumber, personEntity.Id);
            GlobalHelper.SendEmailWithCC(emailTo, command.emailCC, custSubject, custBody);
        }
    }
}
