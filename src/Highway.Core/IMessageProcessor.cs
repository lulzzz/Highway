using System.Threading;
using System.Threading.Tasks;

namespace Highway.Core
{
    public interface IMessageProcessor
    {
        Task ProcessAsync<TM>(TM message, CancellationToken cancellationToken = default)
            where TM : IMessage;
    }
}