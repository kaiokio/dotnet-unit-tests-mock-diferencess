using MockTests.Entities;

namespace MockTests.Services
{
    public interface INewSaleEventDispatcher
    {
        void DispatchEvent(Sale sale);
    }
}