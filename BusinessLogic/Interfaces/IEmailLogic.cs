using GoLogs.Api.BusinessLogic.Commands;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Interfaces
{
    public interface IEmailLogic
    {
        Task AfterDelegateAsync(EmailCommand command);
        Task AfterDORequestDelegateAsync(EmailCommand command);
        Task AfterInvoiceDelegateAsync(EmailCommand command);
        Task AfterPaymentDelegateAsync(EmailCommand command);
        Task AfterDOReleaseDelegateAsync(EmailCommand command);

        Task AfterDORequestAsync(EmailCommand command);
        Task AfterInvoiceAsync(EmailCommand command);
        Task AfterPaymentAsync(EmailCommand command);
        Task AfterDOReleaseAsync(EmailCommand command);
		Task AfterInvoiceKojaAsync(EmailInvKojaCommand command);
		Task GatePassAsync(EmailGatePassKojaCommand command);
        Task Activation(string activationCode);
        Task ResendActivation(string email);
    }
}
