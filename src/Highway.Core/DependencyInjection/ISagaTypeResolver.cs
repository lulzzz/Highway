using System;
using System.Collections.Generic;

namespace Highway.Core.DependencyInjection
{
    public interface ISagaTypeResolver
    {
        //TODO: don't allow multiple registrations. Only one saga can be registered to handle a message
        IEnumerable<(Type sagaType, Type sagaStateType)> Resolve<TM>() where TM : IMessage;
        void Register(Type messageType, (Type sagaType, Type sagaStateType) types);
    }
}