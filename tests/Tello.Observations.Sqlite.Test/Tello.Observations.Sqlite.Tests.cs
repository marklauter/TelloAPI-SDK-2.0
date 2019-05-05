using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Repository.Sqlite;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Tello.Controller;

namespace Tello.Observations.Sqlite.Test
{
    [TestClass]
    public class TelloObservationsSqliteTests
    {
        private IRepository CreateRepository(bool deleteFile = true, [CallerMemberName]string callerName = null)
        {
            var path = Path.GetTempPath();
            var fileName = $"{callerName}.sqlite";
            var fullPath = Path.Combine(path, fileName);

            if (deleteFile && File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            var repo = new SqliteRepository((path, fileName));
            Assert.IsTrue(File.Exists(fullPath));
            return repo;
        }

        [TestMethod]
        public void Session_NewEntity_Insert_Update_Read()
        {
            using(var repo= CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start);
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(0, session.Duration.TotalMilliseconds);

                session.Duration = TimeSpan.FromSeconds(1);
                Assert.AreEqual(1, repo.Update(session));

                session = repo.Read<Session>(1);
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);
            }
        }

        [TestMethod]
        public void ObservationGroup_NewEntity_Insert_Update_Read()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);
            }
        }

        [TestMethod]
        public void TelloCommandObservation_NewEntity_Insert_Update_Read()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());
                Assert.IsTrue(repo.CreateCatalog<CommandObservation>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                var args = new CommandResponseReceivedArgs(
                    Messaging.TelloCommands.Takeoff, 
                    "ok", 
                    start, 
                    TimeSpan.FromSeconds(1));
                var observation = new CommandObservation(group, args);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<CommandObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(start, observation.Initiated);
                Assert.AreEqual(Messaging.TelloCommands.Takeoff, observation.Command);
                Assert.AreEqual("ok", observation.Response);
            }
        }


    }
}
