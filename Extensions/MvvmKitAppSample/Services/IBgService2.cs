using System.Threading.Tasks;
using MvvmKit;

namespace MvvmKitAppSample.Services
{
    public interface IBgService2
    {
        AsyncEvent<string> OnString { get; }

        Task Method();
    }
}