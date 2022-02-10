using MockTests.Entities;
using MockTests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockTests.Services
{
    public class SaleValidationService : ISaleValidationService
    {
        public void Validate(Sale sale)
        {
            var errors = GetValidationErrors(sale);

            if (errors.Any())
            {
                throw new Exception($"{nameof(Sale)} is invalid. Validation errors: [{errors.ToPipedMessage()}]");
            }
        }

        private IEnumerable<string> GetValidationErrors(Sale sale)
        {
            if (sale.ClientName.IsNullOrWhiteSpace())
            {
                yield return $"{nameof(Sale.ClientName)} must not be null or empty";
            }

            if (sale.ClientDocument.IsNullOrWhiteSpace())
            {
                yield return $"{nameof(Sale.ClientDocument)} must not be null or empty";
            }

            if (sale.ProductId <= 0)
            {
                yield return $"{nameof(Sale.ProductId)} must be greater than 0";
            }

            if (sale.Value <= 0)
            {
                yield return $"{nameof(Sale.Value)} must be greater than 0";
            }
        }
    }
}
