﻿namespace NServiceBus.NHibernate.Tests.SynchronizedStorage
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;
    using Extensibility;
    using NUnit.Framework;
    using Persistence.NHibernate;
    using Transport;

    [TestFixture]
    class NHibernateAmbientTransactionSynchronizedStorageSessionTests : InMemoryDBFixture
    {
        [Test]
        public async Task It_invokes_callbacks_when_session_is_completed()
        {
            using (var scope = new TransactionScope())
            {
                var transportTransaction = new TransportTransaction();
                transportTransaction.Set(Transaction.Current);

                var callbackInvoked = 0;
                var adapter = new NHibernateSynchronizedStorageAdapter(SessionFactory, null);

                using (var storageSession = await adapter.TryAdapt(transportTransaction, new ContextBag()))
                {
                    storageSession.Session(); //Make sure session is initialized
                    storageSession.OnSaveChanges((s, _) =>
                    {
                        callbackInvoked++;
                        return Task.FromResult(0);
                    });
                    storageSession.OnSaveChanges((s, _) =>
                    {
                        callbackInvoked++;
                        return Task.FromResult(0);
                    });

                    await storageSession.CompleteAsync();

                    Assert.AreEqual(2, callbackInvoked);
                }

                scope.Complete();
            }
        }



        [Test]
        public async Task It_does_not_commit_if_callback_throws()
        {
            var entityId = Guid.NewGuid().ToString();

            using (var scope = new TransactionScope())
            {
                var transportTransaction = new TransportTransaction();
                transportTransaction.Set(Transaction.Current);

                var adapter = new NHibernateSynchronizedStorageAdapter(SessionFactory, null);

                using (var storageSession = await adapter.TryAdapt(transportTransaction, new ContextBag()))
                {
                    storageSession.Session().Save(new TestEntity { Id = entityId });
                    storageSession.OnSaveChanges((s, _) =>
                    {
                        throw new Exception("Simulated");
                    });

                    try
                    {
                        await storageSession.CompleteAsync();
                        scope.Complete();
                    }
                    catch (Exception)
                    {
                        //NOOP
                    }
                }

            }

            using (var session = SessionFactory.OpenSession())
            {
                var savedEntity = session.Get<TestEntity>(entityId);
                Assert.IsNull(savedEntity);
            }
        }
    }
}