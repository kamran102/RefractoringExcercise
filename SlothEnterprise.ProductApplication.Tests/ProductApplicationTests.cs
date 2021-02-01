using FluentAssertions;
using Moq;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using System;
using System.Collections.Generic;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests
{
    public class ProductApplicationTests
    {
        private readonly Mock<ISelectInvoiceService> _mockSelectInvoiceService;
        private readonly Mock<IConfidentialInvoiceService> _mockConfidentialInvoiceService;
        private readonly Mock<IBusinessLoansService> _mockBusinessLoansService;
        private readonly IProductApplicationService sut;

        public ProductApplicationTests()
        {
            _mockSelectInvoiceService = new Mock<ISelectInvoiceService>(MockBehavior.Strict);
            _mockConfidentialInvoiceService = new Mock<IConfidentialInvoiceService>(MockBehavior.Strict);
            _mockBusinessLoansService = new Mock<IBusinessLoansService>(MockBehavior.Strict);

            sut = new ProductApplicationService(
                    _mockSelectInvoiceService.Object,
                    _mockConfidentialInvoiceService.Object,
                    _mockBusinessLoansService.Object
                );
        }

        #region Tests for group operations



        #region Selective Invoice Discount

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithSelectiveInvoiceDiscount_ShouldReturnExpectedResult()
        {
            // Arrange
            ISellerApplication application = CreateSelectiveInvoiceDiscount();

            int expected = 999;

            SetupMockSelectInvoiceService(application, expected);

            // Act
            int result = sut.SubmitApplicationFor(application);

            // Assert
            result.Should().Be(expected);
            _mockSelectInvoiceService.VerifyAll();
        }

        #endregion

        #region Confidential Invoice Discount

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithConfidentialInvoiceDiscount_ShouldReturnOne()
        {
            // Arrange
            int applicationId = 999;
            bool success = true;
            IList<string> errors = null;
            var result = SetupMockActionResult(applicationId, success, errors);
            ISellerApplication application = CreateConfidentialInvoiceDiscount();

            SetupMockConfidentialInvoiceDiscount(application, result.Object);

            // Act
            int actual = sut.SubmitApplicationFor(application);

            // Assert
            actual.Should().Be(applicationId);
            _mockConfidentialInvoiceService.Verify();
        }

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithConfidentialInvoiceDiscount_ResultsInFailure_ShouldReturnMinusOne()
        {
            // Arrange
            int applicationId = 999;
            bool success = false;
            IList<string> errors = null;
            var result = SetupMockActionResult(applicationId, success, errors);
            ISellerApplication application = CreateConfidentialInvoiceDiscount();

            SetupMockConfidentialInvoiceDiscount(application, result.Object);

            // Act
            int actual = sut.SubmitApplicationFor(application);

            // Assert
            actual.Should().Be(-1);
            _mockConfidentialInvoiceService.Verify();
        }

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithConfidentialInvoiceDiscount_ResultsInNullApplicationId_ShouldReturnMinusOne()
        {
            // Arrange
            int? applicationId = null;
            bool success = true;
            IList<string> errors = null;
            var result = SetupMockActionResult(applicationId, success, errors);
            ISellerApplication application = CreateConfidentialInvoiceDiscount();

            SetupMockConfidentialInvoiceDiscount(application, result.Object);

            // Act
            int actual = sut.SubmitApplicationFor(application);

            // Assert
            actual.Should().Be(-1);
            _mockConfidentialInvoiceService.Verify();
        }

        #endregion


        #region Business Loan Tests

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithBusinessLoans_ShouldReturnExpectedValue()
        {
            // Arrange
            int? applicationId = 999;
            bool success = true;
            IList<string> errors = null;
            var result = SetupMockActionResult(applicationId, success, errors);
            ISellerApplication application = CreateBusinessLoansApplication();

            SetupMockBusinessLoans(application, result.Object);

            // Act
            int actual = sut.SubmitApplicationFor(application);

            // Assert
            actual.Should().Be(applicationId);
            _mockBusinessLoansService.Verify();
        }

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithBusinessLoans_NullApplicationId_ShouldReturnMinusOne()
        {
            // Arrange
            int? applicationId = null;
            bool success = true;
            IList<string> errors = null;
            var result = SetupMockActionResult(applicationId, success, errors);
            ISellerApplication application = CreateBusinessLoansApplication();

            SetupMockBusinessLoans(application, result.Object);

            // Act
            int actual = sut.SubmitApplicationFor(application);

            // Assert
            actual.Should().Be(-1);
            _mockBusinessLoansService.Verify();
        }

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithBusinessLoans_FailureAction_ShouldReturnMinusOne()
        {
            // Arrange
            int? applicationId = 999;
            bool success = false;
            IList<string> errors = null;
            var result = SetupMockActionResult(applicationId, success, errors);
            ISellerApplication application = CreateBusinessLoansApplication();

            SetupMockBusinessLoans(application, result.Object);

            // Act
            int actual = sut.SubmitApplicationFor(application);

            // Assert
            actual.Should().Be(-1);
            _mockBusinessLoansService.Verify();
        }

        #endregion


        #region Invalid request type tests

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithUnexpectedType_ShouldThrowException()
        {
            // Arrange
            Mock<IProduct> mockProduct = new Mock<IProduct>();
            Mock<ISellerApplication> mockApplication = new Mock<ISellerApplication>();
            mockApplication.Setup(p => p.Product).Returns(mockProduct.Object);

            // Act
            Func<int> act = new Func<int>(() => { return sut.SubmitApplicationFor(mockApplication.Object); });

            // Assert
            act.Should().Throw<InvalidOperationException>();
            _mockBusinessLoansService.Verify();
        }

        #endregion

        #endregion


        #region Service Setups

        private Mock<IApplicationResult> SetupMockActionResult(int? applicationId, bool success, IList<string> errors)
        {
            var mock = new Mock<IApplicationResult>(MockBehavior.Strict);
            mock.SetupAllProperties();
            mock.Object.ApplicationId = applicationId;
            mock.Object.Success = success;
            mock.Object.Errors = errors;

            return mock;
        }


        #region Selective Invoice Discount

        private void SetupMockSelectInvoiceService(ISellerApplication application, int rtn)
        {
            SelectiveInvoiceDiscount product = (SelectiveInvoiceDiscount)application.Product;
            _mockSelectInvoiceService
                .Setup(p => p.SubmitApplicationFor(application.CompanyData.Number.ToString(), product.InvoiceAmount, product.AdvancePercentage))
                .Returns(rtn)
                .Verifiable();
        }

        private static ISellerApplication CreateSelectiveInvoiceDiscount()
        {
            return new SellerApplication()
            {
                CompanyData = new SellerCompanyData()
                {
                    DirectorName = "Bob",
                    Founded = new System.DateTime(1900, 1, 1),
                    Name = "Bob Inc.",
                    Number = 123
                },
                Product = new SelectiveInvoiceDiscount()
                {
                    AdvancePercentage = 2.5m,
                    Id = 123,
                    InvoiceAmount = 456m
                }
            };
        }

        #endregion


        #region Confidential Invoice Setups

        private void SetupMockConfidentialInvoiceDiscount(ISellerApplication application, IApplicationResult result)
        {
            ConfidentialInvoiceDiscount product = (ConfidentialInvoiceDiscount)application.Product;

            _mockConfidentialInvoiceService
                .Setup(p => p.SubmitApplicationFor(It.IsAny<CompanyDataRequest>(), product.TotalLedgerNetworth, product.AdvancePercentage, product.VatRate))
                .Returns(result)
                .Verifiable();
        }

        private static ISellerApplication CreateConfidentialInvoiceDiscount()
        {
            return new SellerApplication()
            {
                CompanyData = new SellerCompanyData()
                {
                    DirectorName = "Bob",
                    Founded = new System.DateTime(1900, 1, 1),
                    Name = "Bob Inc.",
                    Number = 123
                },
                Product = new ConfidentialInvoiceDiscount()
                {
                    AdvancePercentage = 2.5m,
                    Id = 123,
                    TotalLedgerNetworth = 456m,
                    VatRate = 2.5m
                }
            };
        }

        #endregion


        #region Business Loans Setups

        private void SetupMockBusinessLoans(ISellerApplication application, IApplicationResult result)
        {
            _mockBusinessLoansService
                .Setup(p => p.SubmitApplicationFor(It.IsAny<CompanyDataRequest>(), It.IsAny<LoansRequest>()))
                .Returns(result)
                .Verifiable();
        }

        private static ISellerApplication CreateBusinessLoansApplication()
        {
            return new SellerApplication()
            {
                CompanyData = new SellerCompanyData()
                {
                    DirectorName = "Bob",
                    Founded = new System.DateTime(1900, 1, 1),
                    Name = "Bob Inc.",
                    Number = 123
                },
                Product = new BusinessLoans()
                {
                    InterestRatePerAnnum = 2.5m,
                    LoanAmount = 234m
                }
            };
        }

        #endregion

        #endregion
    }
}