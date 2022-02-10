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
    public class SalesServiceStrictTests
    {
        private const string SuccessNotificationMessage = "Successful Sale";
        private const string PendingNotificationMessage = "Pending Sale";
        private readonly SalesService _salesService;
        private readonly Mock<ISaleValidationService> _saleValidationService;
        private readonly Mock<ISalesRepository> _salesRepository;
        private readonly Mock<ISaleNotificationService> _saleNotificationService;
        private readonly Mock<INewSaleEventDispatcher> _newSaleEventDispatcher;
        private readonly Mock<ILogger<SalesService>> _logger;
        private readonly Fixture _builder = new Fixture();
       
        public SalesServiceStrictTests()
        {
            _saleValidationService = new Mock<ISaleValidationService>(MockBehavior.Strict);
            _salesRepository = new Mock<ISalesRepository>(MockBehavior.Strict);
            _saleNotificationService = new Mock<ISaleNotificationService>(MockBehavior.Strict);
            _newSaleEventDispatcher = new Mock<INewSaleEventDispatcher>(MockBehavior.Strict);
            _logger = new Mock<ILogger<SalesService>>(MockBehavior.Strict);
            _builder.Customize<Sale>(x => x.With(s => s.Pending, false));

            _salesService = new SalesService(
                _saleValidationService.Object,
                _salesRepository.Object,
                _saleNotificationService.Object,
                //_newSaleEventDispatcher.Object,
                _logger.Object);
        }

        [Fact]
        public void MakeSale_ShouldExecuteCorrectly()
        {
            var sale = _builder.Create<Sale>();
            _saleValidationService
                .Setup(x => x.Validate(sale));
            _salesRepository
                .Setup(x => x.Add(sale));
            _saleNotificationService
                .Setup(x => x.SendNotification(sale.ClientName, "wrong-message"));
            //_newSaleEventDispatcher
            //    .Setup(x => x.DispatchEvent(sale));

            _salesService.MakeSaleWithoutTryCatch(sale);

            _saleValidationService.VerifyAll();
            _salesRepository.VerifyAll();
            _saleNotificationService.VerifyAll();
            _newSaleEventDispatcher.VerifyAll();
            _logger.VerifyAll();

            //Mock.VerifyAll(
            //    _saleValidationService,
            //    _salesRepository,
            //    _saleNotificationService,
            //    _newSaleEventDispatcher,
            //    _logger);
        }

        [Fact]
        public void MakeSaleWithoutTryCatch_WhenNotificationThrowException_ShouldThrowException()
        {
            const string errorMessage = "error_message";
            var sale = _builder.Create<Sale>();
            _saleValidationService
                .Setup(x => x.Validate(sale));
            _salesRepository
                .Setup(x => x.Add(sale));
            _saleNotificationService
                .Setup(x => x.SendNotification(sale.ClientName, SuccessNotificationMessage))
                .Throws(new Exception(errorMessage));
            //dispatch setup is not necessary because sendNotification throws exception

            Action act = () => _salesService.MakeSaleWithoutTryCatch(sale);

            act.Should().Throw<Exception>().WithMessage(errorMessage);
            _saleValidationService.VerifyAll();
            _salesRepository.VerifyAll();
            _saleNotificationService.VerifyAll();
            _newSaleEventDispatcher.VerifyAll();
            _logger.VerifyAll();
        }

        [Fact]
        public void MakeSaleWithoutTryCatch_WithPendingSale_ShouldExecuteCorrectly()
        {
            var sale = _builder.Create<Sale>();
            sale.Pending = true;
            _saleValidationService
                .Setup(x => x.Validate(sale));
            _salesRepository
                .Setup(x => x.Add(sale));
            _saleNotificationService
                .Setup(x => x.SendNotification(sale.ClientName, It.Is<string>(s => s == PendingNotificationMessage)));
            //dispatch setup is not necessary because when is pending sale, don't process dispatch

            _salesService.MakeSaleWithoutTryCatch(sale);

            _saleValidationService.VerifyAll();
            _salesRepository.VerifyAll();
            _saleNotificationService.VerifyAll();
            _newSaleEventDispatcher.VerifyAll();
            _logger.VerifyAll();
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
            //_logger
            //    .Setup(c => c.Log(
            //        LogLevel.Error,
            //        It.IsAny<EventId>(),
            //        It.Is<object>(v => v.ToString().Contains(errorMessage)),
            //        exception,
            //        (Func<object, Exception, string>)It.IsAny<object>()
            //    ));

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
            //foreach (var sale in sales)
            //{
            //    _salesRepository
            //        .Setup(x => x.Add(sale));
            //}

            _salesService.MakeSales(sales);

            _salesRepository.VerifyAll();
        }

        [Fact]
        public void MakeSales_WhenSalesIsEmpty_ShouldNotCallRepository()
        {
            var sales = Enumerable.Empty<Sale>();
            //var sales = _builder.CreateMany<Sale>(); //the test will not work because it will call the repository

            _salesService.MakeSales(sales);

            _salesRepository.VerifyAll();
        }
    }
}
