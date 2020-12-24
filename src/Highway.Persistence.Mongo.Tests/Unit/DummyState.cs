﻿using System;
using Highway.Core;

namespace Highway.Persistence.Mongo.Tests.Unit
{
    public class DummyState : SagaState
    {
        public DummyState(Guid id, string foo, int bar)
        {
            Id = id;
            Foo = foo;
            Bar = bar;
        }

        public Guid Id { get; }
        public string Foo { get; }
        public int Bar { get; }
    }
}