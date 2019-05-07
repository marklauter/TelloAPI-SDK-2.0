using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.CompilerServices;

namespace Repository.Sqlite.Test
{
    [TestClass]
    public class SqliteRepositoryTests
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

        private long GetFileSize([CallerMemberName]string callerName = null)
        {
            var path = Path.GetTempPath();
            var fileName = $"{callerName}.sqlite";
            var fullPath = Path.Combine(path, fileName);

            return File.Exists(fullPath)
                ? new FileInfo(fullPath).Length
                : 0L;
        }

        [TestMethod]
        public void SqliteRepository_CTOR_formats_connectionstring_creates_connection_and_creates_dbfile()
        {
            using (var repo = CreateRepository()) { }
        }

        [TestMethod]
        public void SqliteRepository_CreateCatalog_adds_new_table()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<TestEntity>());
            }
        }

        [TestMethod]
        public void SqliteRepository_NewEntity_creates_new_entity_inserts_into_table_and_updates_primarykey_id()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<TestEntity>());

                var entity = repo.NewEntity<TestEntity>();
                Assert.AreEqual(1, entity.Id);
                Assert.IsNull(entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(1));

                var name = "Jello Baby";
                entity = repo.NewEntity<TestEntity>(name);
                Assert.AreEqual(2, entity.Id);
                Assert.AreEqual(name, entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(2));
            }
        }

        [TestMethod]
        public void SqliteRepository_NewEntity_delete_removes_all_data()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<TestEntity>());

                var entity = repo.NewEntity<TestEntity>();
                Assert.AreEqual(1, entity.Id);
                Assert.IsNull(entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(1));

                var name = "Jello Baby";
                entity = repo.NewEntity<TestEntity>(name);
                Assert.AreEqual(2, entity.Id);
                Assert.AreEqual(name, entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(2));

                Assert.AreEqual(2, repo.Delete<TestEntity>());
                Assert.IsNull(repo.Read<TestEntity>(1));
                Assert.IsNull(repo.Read<TestEntity>(2));
            }
        }

        [TestMethod]
        public void SqliteRepository_NewEntity_delete_removes_record_matching_pk()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<TestEntity>());

                var entity = repo.NewEntity<TestEntity>();
                Assert.AreEqual(1, entity.Id);
                Assert.IsNull(entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(1));

                var name = "Jello Baby";
                entity = repo.NewEntity<TestEntity>(name);
                Assert.AreEqual(2, entity.Id);
                Assert.AreEqual(name, entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(2));

                Assert.AreEqual(1, repo.Delete<TestEntity>(1));
                Assert.IsNull(repo.Read<TestEntity>(1));
                Assert.IsNotNull(repo.Read<TestEntity>(2));
            }
        }

        [TestMethod]
        public void SqliteRepository_NewEntity_delete_removes_record_matching_expression()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<TestEntity>());

                var entity = repo.NewEntity<TestEntity>();
                Assert.AreEqual(1, entity.Id);
                Assert.IsNull(entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(1));

                var name = "Jello Baby";
                entity = repo.NewEntity<TestEntity>(name);
                Assert.AreEqual(2, entity.Id);
                Assert.AreEqual(name, entity.Name);
                Assert.IsNotNull(repo.Read<TestEntity>(2));

                Assert.AreEqual(1, repo.Delete<TestEntity>(e => e.Id == 1));
                Assert.IsNull(repo.Read<TestEntity>(1));
                Assert.IsNotNull(repo.Read<TestEntity>(2));
            }
        }

        [TestMethod]
        public void SqliteRepository_NewEntity_shrink_recovers_disk_space()
        {
            var fileSize = 0L;
            using (var repo = CreateRepository())
            {
                fileSize = GetFileSize();

                Assert.IsTrue(repo.CreateCatalog<TestEntity>());

                for (var i = 0; i < 400; ++i)
                {
                    Assert.AreEqual(i + 1, repo.NewEntity<TestEntity>().Id);
                }
            }
            var newSize = GetFileSize();
            Assert.AreNotEqual(fileSize, newSize);
            fileSize = newSize;

            // delete does not recover space
            using (var repo = CreateRepository(false))
            {
                Assert.AreEqual(400, repo.Delete<TestEntity>());
            }
            newSize = GetFileSize();
            Assert.AreEqual(fileSize, newSize);
            fileSize = newSize;

            // shrink recovers space, but connection must be disposed to see result
            using (var repo = CreateRepository(false))
            {
                repo.Shrink();
                newSize = GetFileSize();
                Assert.AreEqual(fileSize, newSize);
            }
            newSize = GetFileSize();
            Assert.IsTrue(fileSize > newSize);
        }

        [TestMethod]
        public void SqliteRepository_Read_retrieves_correct_entity()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<TestEntity>());

                var entity = repo.NewEntity<TestEntity>();
                Assert.AreEqual(1, entity.Id);
                Assert.IsNull(entity.Name);
                entity = repo.Read<TestEntity>(1);
                Assert.IsNotNull(entity);
                Assert.AreEqual(1, entity.Id);
                Assert.IsNull(entity.Name);

                var name = "Jello Baby";
                entity = repo.NewEntity<TestEntity>(name);
                Assert.AreEqual(2, entity.Id);
                Assert.AreEqual(name, entity.Name);
                entity = repo.Read<TestEntity>(2);
                Assert.IsNotNull(entity);
                Assert.AreEqual(2, entity.Id);
                Assert.AreEqual(name, entity.Name);
            }
        }

        [TestMethod]
        public void SqliteRepository_Update_changes_entity()
        {
            using (var repo = CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<TestEntity>());

                var name = "Jello Baby";
                var entity = repo.NewEntity<TestEntity>(name);
                Assert.AreEqual(1, entity.Id);
                Assert.AreEqual(name, entity.Name);

                entity = repo.Read<TestEntity>(1);
                Assert.IsNotNull(entity);
                Assert.AreEqual(1, entity.Id);
                Assert.AreEqual(name, entity.Name);

                name += "_changed";
                entity.Name = name;
                Assert.AreEqual(1, repo.Update(entity));

                entity = repo.Read<TestEntity>(1);
                Assert.IsNotNull(entity);
                Assert.AreEqual(1, entity.Id);
                Assert.AreEqual(name, entity.Name);
            }
        }
    }
}
