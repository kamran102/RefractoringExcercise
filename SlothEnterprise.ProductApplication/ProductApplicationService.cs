using System;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication
{
    public class ProductApplicationService : IProductApplicationService
    {
        private readonly ISelectInvoiceService _selectInvoiceService;
        private readonly IConfidentialInvoiceService _confidentialInvoiceWebService;
        private readonly IBusinessLoansService _businessLoansService;

        public ProductApplicationService(ISelectInvoiceService selectInvoiceService, IConfidentialInvoiceService confidentialInvoiceWebService, IBusinessLoansService businessLoansService)
        {
            _selectInvoiceService = selectInvoiceService;
            _confidentialInvoiceWebService = confidentialInvoiceWebService;
            _businessLoansService = businessLoansService;
        }

        /// <inheritdoc />
        [Obsolete("Use individual application methods to invoke services.")]
        public int SubmitApplicationFor(ISellerApplication application)
        {
            if (application.Product is SelectiveInvoiceDiscount sid)
            {
                return SubmitApplicationForSelectiveInvoiceDiscount(application.CompanyData, sid);
            }

            if (application.Product is ConfidentialInvoiceDiscount cid)
            {
                return SubmitForConfidentialInvoiceDiscount(application.CompanyData, cid);
            }

            if (application.Product is BusinessLoans loans)
            {
                return SubmitApplicationForBusinessLoans(application.CompanyData, loans);
            }

            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public int SubmitApplicationForSelectiveInvoiceDiscount(ISellerCompanyData companyData, SelectiveInvoiceDiscount sid)
        {
            return _selectInvoiceService.SubmitApplicationFor(companyData.Number.ToString(), sid.InvoiceAmount, sid.AdvancePercentage);
        }

        /// <inheritdoc />
        public int SubmitApplicationForBusinessLoans(ISellerCompanyData companyData, BusinessLoans loans)
        {
            var result = _businessLoansService.SubmitApplicationFor(new CompanyDataRequest
            {
                CompanyFounded = companyData.Founded,
                CompanyNumber = companyData.Number,
                CompanyName = companyData.Name,
                DirectorName = companyData.DirectorName
            }, new LoansRequest
            {
                InterestRatePerAnnum = loans.InterestRatePerAnnum,
                LoanAmount = loans.LoanAmount
            });
            return (result.Success) ? result.ApplicationId ?? -1 : -1;
        }

        /// <inheritdoc />
        public int SubmitForConfidentialInvoiceDiscount(ISellerCompanyData companyData, ConfidentialInvoiceDiscount cid)
        {
            var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                                new CompanyDataRequest
                                {
                                    CompanyFounded = companyData.Founded,
                                    CompanyNumber = companyData.Number,
                                    CompanyName = companyData.Name,
                                    DirectorName = companyData.DirectorName
                                }, cid.TotalLedgerNetworth, cid.AdvancePercentage, cid.VatRate);

            return (result.Success) ? result.ApplicationId ?? -1 : -1;
        }
    }
}
