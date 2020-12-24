﻿using System.Threading;
using System.Threading.Tasks;

namespace Highway.Core
{
    public interface ISubscriber<TM>
        where TM : IMessage
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}