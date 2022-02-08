using AutoMapper;
using GoLogs.Api.Application.Externals;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Handler
{
    public class NLELogic : INLELogic
    {
        private readonly IPersonLogic _personLogic;
        private readonly IMapper _mapper;
        private readonly GoLogsContext _context;

        public NLELogic(IPersonLogic personLogic, IMapper mapper, GoLogsContext context)
        {
            _personLogic = personLogic;
            _mapper = mapper;
            _context = context;
        }

        public async Task<NLEAuthModel> GetAuthTokenAsync()
        {
            var responseStr = await NLEApi.GetAuthToken();
            var responseObj = JObject.Parse(responseStr);
            var result = JsonConvert.DeserializeObject<NLEAuthModel>(responseObj["data"].ToString());
            return result;
        }
        public async Task<NLECustDataViewModel> GetCustomerDataProfileOSSAsync(string npwp)
        {
            if (string.IsNullOrEmpty(npwp))
            {
                return null;
            }

            var responseStr = await NLEApi.GetCustomerDataProfileOSS(npwp);
            var responseObj = JObject.Parse(responseStr);

            if (responseObj.First.First.HasValues)
            {
                var firstData = responseObj.First.First[0];
                var custData = JsonConvert.DeserializeObject<NLECustDataModel>(firstData.ToString());
                var custDataView = _mapper.Map<NLECustDataModel, NLECustDataViewModel>(custData);
                custDataView.Is_exist = false;
                return custDataView;
            }
            else { 
                return null; 
            }
        }

        public async Task RegistrationAsync(RegistrationCommand command)
        {
            using (var dbx = _context.Database.BeginTransaction())
            {
                try
                {
                    var exist = await _context.Persons
                        .Where(x => x.Email == command.Person_Information.Email).AnyAsync();

                    if (!exist)
                    {
                        var company = new CompanyModel()
                        {
                            Id = Guid.NewGuid(),
                            Email = command.Company_Information.Email,
                            Name = command.Company_Information.Company_name,
                            District = command.Company_Information.District,
                            City = command.Company_Information.City,
                            PostalCode = command.Company_Information.Postal_code,
                            NIB = command.Company_Information.Nib,
                            SubDistrict = command.Company_Information.Sub_district,
                            Address = command.Company_Information.Address,
                            Province = command.Company_Information.Province,
                            Type = command.Company_Information.Company_type,
                            NPWP = command.Company_Information.NPWP,
                            Phone = command.Company_Information.Phone
                        };

                        var companyExist = await _context.Companies.Where(x => x.NPWP == command.Company_Information.NPWP).FirstOrDefaultAsync();

                        if (companyExist != null)
                        {
                            company = companyExist;
                        }
                        else
                        {
                            _context.Companies.Add(company);
                        }

                        var defaultPass = GlobalHelper.Encrypt(Constant.GoLogsDefaultPass);
                        var person = new PersonModel()
                        {
                            Id = Guid.NewGuid(),
                            Email = command.Person_Information.Email,
                            FullName = command.Person_Information.FullName,
                            Phone = command.Person_Information.Phone,
                            NPWP = company.NPWP,
                            PasswordHash = defaultPass,
                            ActivationCode = Guid.NewGuid(),
                            CompanyId = company.Id
                        };

                        _context.Persons.Add(person);
                        await _context.SaveChangesAsync();

                        // Send email with default password here
                        var activationUrl = Constant.GoLogsAppDomain + Constant.ActivationUrl + person.ActivationCode.ToString();
                        var template = await _context.EmailTemplates.Where(x => x.Type == "AccountRegistration").FirstOrDefaultAsync();
                        var signature = await _context.EmailTemplates.Where(x => x.Type == "Signature").FirstOrDefaultAsync();
                        var body = template.Template + signature.Template;

                        body = body.Replace("@FullName", person.FullName);
                        body = body.Replace("@Email", person.Email);
                        body = body.Replace("@ActivationUrl", activationUrl);

                        GlobalHelper.SendEmail(person.Email, template.Subject, body);

                        dbx.Commit();
                    }
                    else
                    {
                        throw new ArgumentException(Constant.ErrorFromServer + "This customer already exists.");
                    }
                }
                catch (Exception ex)
                {
                    dbx.Rollback();
                    throw ex;
                }
            }
        }
    }
}
