﻿namespace NServiceBus.NHibernate.Tests
{
    using global::NHibernate.Dialect;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.NHibernate;
    using NUnit.Framework;
    using Particular.Approvals;

    [TestFixture]
    public class DDL
    {
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Outbox_MsSql2012()
        {
            var script = ScriptGenerator<MsSql2012Dialect>.GenerateOutboxScript();
            Approver.Verify(script);
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Outbox_Oracle10g()
        {
            var script = ScriptGenerator<Oracle10gDialect>.GenerateOutboxScript();
            Approver.Verify(script);
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Subscriptions_MsSql2012()
        {
            var script = ScriptGenerator<MsSql2012Dialect>.GenerateSubscriptionStoreScript();
            Approver.Verify(script);
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Subscriptions_Oracle10g()
        {
            var script = ScriptGenerator<Oracle10gDialect>.GenerateSubscriptionStoreScript();
            Approver.Verify(script);
        }

#if !NETCOREAPP
        // This test is ignored for .NETCore because of the unstable foreign key name generation in NHibernate https://github.com/nhibernate/nhibernate-core/issues/1769
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Sagas_MsSql2012()
        {
            var script = ScriptGenerator<MsSql2012Dialect>.GenerateSagaScript<MySaga>();
            Approver.Verify(script);
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Sagas_Oracle10g()
        {
            var script = ScriptGenerator<Oracle10gDialect>.GenerateSagaScript<MySaga>();
            Approver.Verify(script);
        }
#endif
    }

    class MySaga : Saga<MySaga.SagaData>, IAmStartedByMessages<MyMessage>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
            mapper.ConfigureMapping<MyMessage>(m => m.UniqueId).ToSaga(s => s.UniqueId);
        }

        public class SagaData : ContainSagaData
        {
            public virtual string UniqueId { get; set; }
            public virtual IList<CollectionEntry> Entries { get; set; }
            public virtual IList<CollectionEntryWithoutId> EntriesWithoutId { get; set; }
        }

        public class CollectionEntry
        {
            public virtual Guid Id { get; set; }
            public virtual decimal Value { get; set; }
        }

        public class CollectionEntryWithoutId
        {
            public virtual decimal Value1 { get; set; }
            public virtual decimal Value2 { get; set; }
        }

        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }

    class MyMessage : IMessage
    {
        public string UniqueId { get; set; }
    }
}
