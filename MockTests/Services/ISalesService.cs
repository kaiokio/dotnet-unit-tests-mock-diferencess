using MockTests.Entities;

namespace MockTests.Services
{
    public interface ISalesService
    {
        void MakeSaleWithoutTryCatch(Sale sale);
    }
}