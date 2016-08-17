using System.Threading.Tasks;

namespace Qb.Core46Api.Services
{
    public interface ISmsSender
    {
        Task<bool> SendSms(string message, string toNumber);
    }
}