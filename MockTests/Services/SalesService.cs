using Microsoft.Extensions.Logging;
using MockTests.Entities;
using MockTests.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockTests.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISaleValidationService _saleValidationService;
        private readonly ISalesRepository _salesRepository;
        private readonly ISaleNotificationService _saleNotificationService;
        private readonly INewSaleEventDispatcher _newSaleEventDispatcher;
        private readonly ILogger<SalesService> _logger;

        public SalesService(ISaleValidationService saleValidationService,
            ISalesRepository salesRepository,
            ISaleNotificationService saleNotificationService,
            //INewSaleEventDispatcher newSaleEventDispatcher,
            ILogger<SalesService> logger)
        {
            _saleValidationService = saleValidationService;
            _salesRepository = salesRepository;
            _saleNotificationService = saleNotificationService;
            //_newSaleEventDispatcher = newSaleEventDispatcher;
            _logger = logger;
        }

        public void MakeSaleWithoutTryCatch(Sale sale)
        {
            _saleValidationService.Validate(sale);

            _salesRepository.Add(sale);

            if (sale.Pending)
            {
                _saleNotificationService.SendNotification(sale.ClientName, "Pending Sale");
                return;
            }

            _saleNotificationService.SendNotification(sale.ClientName, "Successful Sale");

            //_newSaleEventDispatcher.DispatchEvent(sale);
        }












        public void MakeSaleWithTryCatch(Sale sale)
        {
            _saleValidationService.Validate(sale);

            _salesRepository.Add(sale);
            
            try
            {
                _saleNotificationService.SendNotification(sale.ClientName, "Successful Sale");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sending notification failed. Error: {ex.Message}");
            }
        }







        public void MakeSales(IEnumerable<Sale> sales)
        {
            foreach (var sale in sales)
            {
                _salesRepository.Add(sale);
            }
        }





        public void MakeSalesWithNotification(IEnumerable<Sale> sales)
        {
            foreach (var sale in sales)
            {
                _salesRepository.Add(sale);
                _saleNotificationService.SendNotification(sale.ClientName, "Successful Sale");
            }
        }
    }
}
