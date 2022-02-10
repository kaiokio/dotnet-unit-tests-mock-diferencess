using MockTests.Entities;

namespace MockTests.Services
{
    public interface ISaleValidationService
    {
        void Validate(Sale sale);
    }
}