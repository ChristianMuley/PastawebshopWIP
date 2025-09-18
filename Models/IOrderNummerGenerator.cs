namespace Pastashop.Models;

public interface IOrderNummerGenerator
{
    Task<string> GenerateAsync();
}