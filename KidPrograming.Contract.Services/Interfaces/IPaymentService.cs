namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreateAsync(decimal amount, DateTimeOffset paymentDate);
    }
}