using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockTests.Entities;
using MockTests.Repositories;
using MockTests.Services;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace SalesTests
{
    public class SalesServiceLooseTests
    {
        private const string SuccessNotificationMessage = "Successful Sale";
        private readonly SalesService _salesService;
        private readonly Mock<ISaleValidationService> _saleValidationService;
        private readonly Mock<ISalesRepository> _salesRepository;
        private readonly Mock<ISaleNotificationService> _saleNotificationService;
        private readonly Mock<INewSaleEventDispatcher> _newSaleEventDispatcher;
        private readonly Mock<ILogger<SalesService>> _logger;
        private readonly Fixture _builder = new Fixture();

        public SalesServiceLooseTests()
        {
            //Loose
            _saleValidationService = new Mock<ISaleValidationService>();
            _salesRepository = new Mock<ISalesRepository>();
            _saleNotificationService = new Mock<ISaleNotificationService>();
            _newSaleEventDispatcher = new Mock<INewSaleEventDispatcher>();
            _logger = new Mock<ILogger<SalesService>>();
            _builder.Customize<Sale>(x => x.With(s => s.Pending, false));

            _salesService = new SalesService(
                _saleValidationService.Object,
                _salesRepository.Object,
                _saleNotificationService.Object,
                //_newSaleEventDispatcher.Object,
                _logger.Object);
        }

        [Fact]
        public void MakeSaleWithoutTryCatch_WithVerify_ShouldExecuteCorrectly()
        {
            //Arrange
            var sale = _builder.Create<Sale>();
            _saleValidationService
                .Setup(x => x.Validate(sale));
                //.Verifiable();
            _salesRepository
                .Setup(x => x.Add(sale));
                //.Verifiable();
            _saleNotificationService
                .Setup(x => x.SendNotification(sale.ClientName, "wrong_message"));
                //.Verifiable();

            //without dispatchEvent setuup

            //Act
            _salesService.MakeSaleWithoutTryCatch(sale);

            //Assert
            //_saleValidationService.Verify(x => x.Validate(sale), Times.Never);
            //_saleValidationService.Verify();
            //_salesRepository.Verify();
            //_saleNotificationService.Verify();
        }

        [Fact]
        public void MakeSaleWithoutTryCatch_WithVerifyAll_ShouldExecuteCorrectly()
        {
            var sale = _builder.Create<Sale>();
            _saleValidationService
                .Setup(x => x.Validate(sale));
            _salesRepository
                .Setup(x => x.Add(sale));
            _saleNotificationService
                .Setup(x => x.SendNotification(sale.ClientName, "wrong_message"));
            //without dispatchEvent setuup

            _salesService.MakeSaleWithoutTryCatch(sale);

            //_saleValidationService.VerifyAll();
            //_salesRepository.VerifyAll();
            //_saleNotificationService.VerifyAll();
        }

        [Fact]
        public void MakeSaleWithTryCatch_WhenNotificationThrowException_ShouldWriteLog()
        {
            const string errorMessage = "error_message";
            var exception = new Exception(errorMessage);
            var sale = _builder.Create<Sale>();
            _saleValidationService
                .Setup(x => x.Validate(sale));
            _salesRepository
                .Setup(x => x.Add(sale));
            _saleNotificationService
                .Setup(x => x.SendNotification(sale.ClientName, SuccessNotificationMessage))
                .Throws(exception);
            //without logger setup

            _salesService.MakeSaleWithTryCatch(sale);

            _saleValidationService.VerifyAll();
            _salesRepository.VerifyAll();
            _saleNotificationService.VerifyAll();
            _logger.VerifyAll();
        }

        [Fact]
        public void MakeSales_ShouldExecuteCorrectly()
        {
            var sales = _builder.CreateMany<Sale>();
            _salesRepository
                .Setup(x => x.Add(sales.First()));

            _salesService.MakeSales(sales);

            _salesRepository.VerifyAll();
        }

        [Fact]
        public void MakeSales_WhenSalesIsEmpty_ShouldNotCallRepository()
        {
            var sales = Enumerable.Empty<Sale>();
            //var sales = _builder.CreateMany<Sale>();

            _salesService.MakeSales(sales);

            //_salesRepository.Verify(x => x.Add(It.IsAny<Sale>()), Times.Never);
            //_saleNotificationService.Verify(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _salesRepository.VerifyAll();
        }
    }
}
