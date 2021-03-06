using System;
using System.Threading;
using System.Threading.Tasks;
using Highway.Core.Exceptions;
using Highway.Core.Persistence;
using NSubstitute;
using Xunit;

namespace Highway.Core.Tests
{
    public class SagaRunnerTests 
    {
        [Fact]
        public async Task RunAsync_should_throw_SagaNotFoundException_if_saga_cannot_be_build()
        {
            var message = new StartDummySaga(Guid.NewGuid());
            var messageContext = NSubstitute.Substitute.For<IMessageContext<StartDummySaga>>();
            messageContext.Message.Returns(message);
            
            var sagaFactory = NSubstitute.Substitute.For<ISagaFactory<DummySaga, DummySagaState>>();
            
            var sagaStateService = NSubstitute.Substitute.For<ISagaStateService<DummySaga, DummySagaState>>();

            var uow = NSubstitute.Substitute.For<IUnitOfWork>();
            var transaction = NSubstitute.Substitute.For<ITransaction>();
            uow.StartTransactionAsync(Arg.Any<CancellationToken>())
                .Returns(transaction);
            
            var state = new DummySagaState(message.GetCorrelationId());
            sagaStateService.GetAsync(messageContext, Arg.Any<ITransaction>(), Arg.Any<CancellationToken>())
                .Returns(state);
            
            var sut = new SagaRunner<DummySaga, DummySagaState>(sagaFactory, sagaStateService, uow);

            await Assert.ThrowsAsync<SagaNotFoundException>(() => sut.RunAsync(messageContext, CancellationToken.None));
        }

        [Fact]
        public async Task RunAsync_should_execute_all_registered_sagas()
        {
            var message = new StartDummySaga(Guid.NewGuid());

            var messageContext = NSubstitute.Substitute.For<IMessageContext<StartDummySaga>>();
            messageContext.Message.Returns(message);

            var sagaStateService = NSubstitute.Substitute.For<ISagaStateService<DummySaga, DummySagaState>>();

            var state = new DummySagaState(message.GetCorrelationId());
            sagaStateService.GetAsync(messageContext, Arg.Any<ITransaction>(), Arg.Any<CancellationToken>())
                .Returns(state);

            var saga = NSubstitute.Substitute.ForPartsOf<DummySaga>();
            saga.When(s => s.HandleAsync(Arg.Any<IMessageContext<StartDummySaga>>(), Arg.Any<CancellationToken>()))
                .DoNotCallBase();

            var sagaFactory = NSubstitute.Substitute.For<ISagaFactory<DummySaga, DummySagaState>>();
            sagaFactory.Create(state)
                .Returns(saga);

            var uow = NSubstitute.Substitute.For<IUnitOfWork>();
            var transaction = NSubstitute.Substitute.For<ITransaction>();
            uow.StartTransactionAsync(Arg.Any<CancellationToken>())
                .Returns(transaction);

            var sut = new SagaRunner<DummySaga, DummySagaState>(sagaFactory, sagaStateService, uow);

            await sut.RunAsync(messageContext, CancellationToken.None);

            await saga.Received(1)
                .HandleAsync(messageContext, Arg.Any<CancellationToken>());

        }
    }
}