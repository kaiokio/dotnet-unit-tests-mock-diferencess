using AutoFixture;
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
    public class SalesServiceStrictWithMockRepositoryTests
    {
        private const string NotificationMessage = "Successful Sale";
        private readonly SalesService _salesService;
        private readonly MockRepository _mockBuilder;
        private readonly Mock<ISaleValidationService> _saleValidationService;
        private readonly Mock<ISalesRepository> _salesRepository;
        private readonly Mock<ISaleNotificationService> _saleNotificationService;
        private readonly Mock<INewSaleEventDispatcher> _newSaleEventDispatcher;
        private readonly Mock<ILogger<SalesService>> _logger;
        private readonly Fixture _builder = new Fixture();
       
        public SalesServiceStrictWithMockRepositoryTests()
        {
            _mockBuilder = new MockRepository(MockBehavior.Strict);
            _saleValidationService = _mockBuilder.Create<ISaleValidationService>();
            _salesRepository = _mockBuilder.Create<ISalesRepository>();
            _saleNotificationService = _mockBuilder.Create<ISaleNotificationService>();
            _newSaleEventDispatcher = _mockBuilder.Create<INewSaleEventDispatcher>();
            _logger = _mockBuilder.Create<ILogger<SalesService>>();
            _builder.Customize<Sale>(x => x.With(s => s.Pending, false));

            _salesService = new SalesService(
                _saleValidationService.Object,
                _salesRepository.Object,
                _saleNotificationService.Object,
                //_newSaleEventDispatcher.Object,
                _logger.Object);
        }

        [Fact]
        public void MakeSaleWithoutTryCatch_ShouldExecuteCorrectly()
        {
            var sale = _builder.Create<Sale>();
            _saleValidationService
                .Setup(x => x.Validate(sale));
            _salesRepository
                .Setup(x => x.Add(sale));
            _saleNotificationService
                .Setup(x => x.SendNotification(sale.ClientName, NotificationMessage));

            _salesService.MakeSaleWithoutTryCatch(sale);

            _mockBuilder.VerifyAll();
        }

        [Fact]
        public void MakeSales_WhenSalesIsEmpty_ShouldNotCallRepository()
        {
            //var sales = Enumerable.Empty<Sale>();
            var sales = _builder.CreateMany<Sale>(); //the test will not work because it will call the repository

            _salesService.MakeSales(sales);

            _mockBuilder.VerifyAll();
        }
    }
}
