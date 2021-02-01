using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using System;

namespace SlothEnterprise.ProductApplication
{
    /// <summary>
    /// Interface to allow invocation of application services.
    /// </summary>
    public interface IProductApplicationService
    {
        /// <summary>
        /// Submits an application for a number of application types.
        /// </summary>
        /// <remarks>This method has been marked as obselete in favour of individual method calls</remarks>
        /// <param name="application">Details of the application.</param>
        /// <returns>Result of the appication request.</returns>
        int SubmitApplicationFor(ISellerApplication application);

        /// <summary>
        /// Submits an application for selective invoice discount. 
        /// </summary>
        /// <param name="companyData">Company to process data for.</param>
        /// <param name="invoiceDiscount">Application invoice discount</param>
        /// <returns>Result of the operation</returns>
        int SubmitApplicationForSelectiveInvoiceDiscount(ISellerCompanyData companyData, SelectiveInvoiceDiscount invoiceDiscount);

        /// <summary>
        /// Submit application for business loans.
        /// </summary>
        /// <param name="companyData">Company to process data for.</param>
        /// <param name="loans">The details of the loan.</param>
        /// <returns></returns>
        int SubmitApplicationForBusinessLoans(ISellerCompanyData companyData, BusinessLoans loans);

        /// <summary>
        /// Submits an application for confidential invoice discount
        /// </summary>
        /// <param name="companyData">Company to process data for.</param>
        /// <param name="invoiceDiscount">Application invoice discount</param>
        /// <returns>Result of the operation</returns>
        int SubmitForConfidentialInvoiceDiscount(ISellerCompanyData application, ConfidentialInvoiceDiscount invoiceDiscount);
    }
}