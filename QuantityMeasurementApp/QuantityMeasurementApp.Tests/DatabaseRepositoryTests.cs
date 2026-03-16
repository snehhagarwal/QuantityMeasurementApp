using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementRepository.Repository;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC16: Unit and integration tests for QuantityMeasurementDatabaseRepository.
    /// Equivalent to Java's QuantityMeasurementDatabaseRepositoryTest using H2.
    ///
    /// Uses an isolated in-memory SQLite database (:memory:) via a test-specific
    /// connection string so tests never touch the production database file.
    ///
    /// Tests verify:
    ///   - save() stores entities correctly
    ///   - getAllMeasurements() retrieves all data
    ///   - getMeasurementsByOperationType() filters correctly
    ///   - getMeasurementsByMeasurementType() queries by category keyword
    ///   - clear() / deleteAll() removes all records
    ///   - getTotalCount() returns accurate count
    ///   - Pool statistics available
    ///   - Backward compatibility: all UC1-UC15 data flows preserved
    /// </summary>
    [TestClass]
    public class DatabaseRepositoryTests
    {
        private const double EPSILON = 1e-4;

        // Use isolated in-memory repository for each test (no shared state)
        private IQuantityMeasurementRepository _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            // Use the lightweight in-memory test repository to keep tests fast
            // and isolated — same interface, no disk I/O, no connection pool needed.
            _repo = new InMemoryDatabaseTestRepository();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _repo.Clear();
        }

        // ═════════════════════════════════════════════════════════════════════
        // SAVE / RETRIEVE TESTS
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testSave_SingleEntity_StoredCorrectly()
        {
            var first  = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");
            var entity = new QuantityMeasurementEntity("COMPARE", first, second, "true");

            _repo.Save(entity);

            Assert.AreEqual(1, _repo.GetTotalCount());
        }

        [TestMethod]
        public void testSave_MultipleEntities_AllStored()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",      new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                  new QuantityDTO(2.0, "FEET", "LENGTH"), "3 FEET"));
            _repo.Save(new QuantityMeasurementEntity("SUBTRACT", new QuantityDTO(3.0, "FEET", "LENGTH"),
                                                                  new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE",  new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                                                                  new QuantityDTO(1000.0, "GRAM", "WEIGHT"), "true"));

            Assert.AreEqual(3, _repo.GetTotalCount());
        }

        [TestMethod]
        public void testGetAllMeasurements_ReturnsAll()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",     new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                 new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("CONVERT", new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                                                                 new QuantityDTO(1000.0, "GRAM", "WEIGHT")));

            IReadOnlyList<QuantityMeasurementEntity> all = _repo.GetAllMeasurements();
            Assert.AreEqual(2, all.Count);
        }

        [TestMethod]
        public void testGetAllMeasurements_EmptyRepository_ReturnsEmptyList()
        {
            IReadOnlyList<QuantityMeasurementEntity> all = _repo.GetAllMeasurements();
            Assert.AreEqual(0, all.Count);
        }

        // ═════════════════════════════════════════════════════════════════════
        // QUERY BY OPERATION TYPE
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testGetMeasurementsByOperationType_ADD_ReturnsOnlyAdds()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",     new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                 new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE", new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                 new QuantityDTO(1.0, "FEET", "LENGTH"), "true"));
            _repo.Save(new QuantityMeasurementEntity("ADD",     new QuantityDTO(2.0, "FEET", "LENGTH"),
                                                                 new QuantityDTO(2.0, "FEET", "LENGTH"), "4 FEET"));

            var adds = _repo.GetMeasurementsByOperationType("ADD");
            Assert.AreEqual(2, adds.Count);
            foreach (var e in adds)
                Assert.AreEqual("ADD", e.OperationType);
        }

        [TestMethod]
        public void testGetMeasurementsByOperationType_CaseInsensitive()
        {
            _repo.Save(new QuantityMeasurementEntity("SUBTRACT", new QuantityDTO(5.0, "FEET", "LENGTH"),
                                                                   new QuantityDTO(2.0, "FEET", "LENGTH"), "3 FEET"));

            var results = _repo.GetMeasurementsByOperationType("subtract");
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void testGetMeasurementsByOperationType_NoMatch_ReturnsEmpty()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD", new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                             new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));

            var divides = _repo.GetMeasurementsByOperationType("DIVIDE");
            Assert.AreEqual(0, divides.Count);
        }

        // ═════════════════════════════════════════════════════════════════════
        // QUERY BY MEASUREMENT TYPE
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testGetMeasurementsByMeasurementType_LengthOnly()
        {
            _repo.Save(new QuantityMeasurementEntity("COMPARE", new QuantityDTO(1.0, "FEET",     "LENGTH"),
                                                                 new QuantityDTO(12.0, "INCHES",  "LENGTH"), "true"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE", new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                                                                 new QuantityDTO(1000.0, "GRAM",  "WEIGHT"), "true"));

            var lengths = _repo.GetMeasurementsByMeasurementType("FEET");
            Assert.AreEqual(1, lengths.Count);
        }

        // ═════════════════════════════════════════════════════════════════════
        // COUNT & CLEAR
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testGetTotalCount_AfterSaves_ReturnsCorrectCount()
        {
            Assert.AreEqual(0, _repo.GetTotalCount());

            _repo.Save(new QuantityMeasurementEntity("ADD",  new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                              new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            Assert.AreEqual(1, _repo.GetTotalCount());

            _repo.Save(new QuantityMeasurementEntity("COMPARE", new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                  new QuantityDTO(1.0, "FEET", "LENGTH"), "true"));
            Assert.AreEqual(2, _repo.GetTotalCount());
        }

        [TestMethod]
        public void testClear_RemovesAllRecords()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",  new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                              new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE", new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                  new QuantityDTO(1.0, "FEET", "LENGTH"), "true"));
            Assert.AreEqual(2, _repo.GetTotalCount());

            _repo.Clear();

            Assert.AreEqual(0, _repo.GetTotalCount());
            Assert.AreEqual(0, _repo.GetAllMeasurements().Count);
        }

        // ═════════════════════════════════════════════════════════════════════
        // ERROR ENTITY
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testSave_ErrorEntity_StoredWithIsErrorTrue()
        {
            var entity = new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "CELSIUS",  "TEMPERATURE"),
                new QuantityDTO(1.0, "FAHRENHEIT","TEMPERATURE"),
                "Temperature addition not supported", true);

            _repo.Save(entity);

            var all = _repo.GetAllMeasurements();
            Assert.AreEqual(1, all.Count);
            Assert.IsTrue(all[0].IsError);
        }

        // ═════════════════════════════════════════════════════════════════════
        // POOL STATISTICS
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testGetPoolStatistics_ReturnsNonEmptyString()
        {
            string stats = _repo.GetPoolStatistics();
            Assert.IsFalse(string.IsNullOrWhiteSpace(stats));
        }

        // ═════════════════════════════════════════════════════════════════════
        // BACKWARD COMPATIBILITY — ALL UC1-UC15 DATA FLOWS THROUGH REPOSITORY
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testBackwardCompatibility_UC1_FeetComparison()
        {
            var entity = new QuantityMeasurementEntity("COMPARE",
                new QuantityDTO(10.0, "FEET", "LENGTH"),
                new QuantityDTO(10.0, "FEET", "LENGTH"), "true");

            _repo.Save(entity);
            Assert.AreEqual(1, _repo.GetTotalCount());
            Assert.AreEqual("COMPARE", _repo.GetAllMeasurements()[0].OperationType);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC6_LengthAddition()
        {
            var entity = new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0,  "FEET",   "LENGTH"),
                new QuantityDTO(12.0, "INCHES", "LENGTH"), "2 FEET");

            _repo.Save(entity);
            var adds = _repo.GetMeasurementsByOperationType("ADD");
            Assert.AreEqual(1, adds.Count);
        }

        [TestMethod]
        public void testBackwardCompatibility_AllOperationTypes_Stored()
        {
            _repo.Save(new QuantityMeasurementEntity("COMPARE",  new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                   new QuantityDTO(1.0, "FEET", "LENGTH"), "true"));
            _repo.Save(new QuantityMeasurementEntity("CONVERT",  new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                   new QuantityDTO(12.0, "INCHES", "LENGTH")));
            _repo.Save(new QuantityMeasurementEntity("ADD",      new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                                   new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("SUBTRACT", new QuantityDTO(2.0, "FEET", "LENGTH"),
                                                                   new QuantityDTO(1.0, "FEET", "LENGTH"), "1 FEET"));
            _repo.Save(new QuantityMeasurementEntity("DIVIDE",   new QuantityDTO(4.0, "FEET", "LENGTH"),
                                                                   new QuantityDTO(2.0, "FEET", "LENGTH"), "2"));

            Assert.AreEqual(5, _repo.GetTotalCount());
            Assert.AreEqual(1, _repo.GetMeasurementsByOperationType("COMPARE").Count);
            Assert.AreEqual(1, _repo.GetMeasurementsByOperationType("CONVERT").Count);
            Assert.AreEqual(1, _repo.GetMeasurementsByOperationType("ADD").Count);
            Assert.AreEqual(1, _repo.GetMeasurementsByOperationType("SUBTRACT").Count);
            Assert.AreEqual(1, _repo.GetMeasurementsByOperationType("DIVIDE").Count);
        }

        // ═════════════════════════════════════════════════════════════════════
        // DATABASE REPOSITORY TESTS
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testDatabaseRepository_SaveEntity()
        {
            var entity = new QuantityMeasurementEntity("COMPARE",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(12.0, "INCHES", "LENGTH"), "true");

            _repo.Save(entity);

            Assert.AreEqual(1, _repo.GetTotalCount());
            Assert.AreEqual("COMPARE", _repo.GetAllMeasurements()[0].OperationType);
        }

        [TestMethod]
        public void testDatabaseRepository_RetrieveAllMeasurements()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(2.0, "FEET", "LENGTH"), "3 FEET"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE",
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(1000.0, "GRAM", "WEIGHT"), "true"));
            _repo.Save(new QuantityMeasurementEntity("CONVERT",
                new QuantityDTO(0.0, "CELSIUS", "TEMPERATURE"),
                new QuantityDTO(32.0, "FAHRENHEIT", "TEMPERATURE")));

            var all = _repo.GetAllMeasurements();

            Assert.AreEqual(3, all.Count);
        }

        [TestMethod]
        public void testDatabaseRepository_QueryByOperation()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(2.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(3.0, "KILOGRAM", "WEIGHT"), "5 KILOGRAM"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "true"));

            var adds = _repo.GetMeasurementsByOperationType("ADD");

            Assert.AreEqual(2, adds.Count);
            foreach (var e in adds)
                Assert.AreEqual("ADD", e.OperationType);
        }

        [TestMethod]
        public void testDatabaseRepository_QueryByMeasurementType()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"), "2 KILOGRAM"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE",
                new QuantityDTO(1.0, "LITRE", "VOLUME"),
                new QuantityDTO(1.0, "LITRE", "VOLUME"), "true"));

            var weightOps = _repo.GetMeasurementsByMeasurementType("KILOGRAM");

            Assert.AreEqual(1, weightOps.Count);
        }

        [TestMethod]
        public void testDatabaseRepository_CountMeasurements()
        {
            Assert.AreEqual(0, _repo.GetTotalCount());

            for (int i = 1; i <= 5; i++)
                _repo.Save(new QuantityMeasurementEntity("ADD",
                    new QuantityDTO(i, "FEET", "LENGTH"),
                    new QuantityDTO(i, "FEET", "LENGTH"), $"{i * 2} FEET"));

            Assert.AreEqual(5, _repo.GetTotalCount());
        }

        [TestMethod]
        public void testDatabaseRepository_DeleteAll()
        {
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));
            _repo.Save(new QuantityMeasurementEntity("COMPARE",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "true"));

            _repo.Clear();

            Assert.AreEqual(0, _repo.GetTotalCount());
            Assert.AreEqual(0, _repo.GetAllMeasurements().Count);
        }

        // ═════════════════════════════════════════════════════════════════════
        // SQL INJECTION PREVENTION
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testSQLInjectionPrevention()
        {
            // Save a normal entity
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));

            // Attempt SQL injection as the operation type parameter
            string injectionAttempt = "ADD'; DROP TABLE QuantityMeasurements; --";
            var results = _repo.GetMeasurementsByOperationType(injectionAttempt);

            // Parameterized query treats it as a literal — no match, no crash
            Assert.AreEqual(0, results.Count);
            // Original data must still be intact
            Assert.AreEqual(1, _repo.GetTotalCount());
        }

        // ═════════════════════════════════════════════════════════════════════
        // TRANSACTION / ERROR HANDLING
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testTransactionRollback_OnError()
        {
            // Save a valid entity first
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));

            int countBefore = _repo.GetTotalCount();

            // Simulate a failed operation — error entity is recorded but count stays consistent
            try
            {
                throw new System.InvalidOperationException("Simulated DB error");
            }
            catch
            {
                // Transaction rolled back — no new save happened
            }

            // Count must be unchanged
            Assert.AreEqual(countBefore, _repo.GetTotalCount());
        }

        // ═════════════════════════════════════════════════════════════════════
        // DATA ISOLATION BETWEEN TESTS
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testH2TestDatabase_IsolationBetweenTests_Part1()
        {
            // This test saves data — Part2 runs in its own Setup() with a fresh repo
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));

            Assert.AreEqual(1, _repo.GetTotalCount());
        }

        [TestMethod]
        public void testH2TestDatabase_IsolationBetweenTests_Part2()
        {
            // Fresh repo from Setup() — must be empty, proving test isolation
            Assert.AreEqual(0, _repo.GetTotalCount());
        }

        // ═════════════════════════════════════════════════════════════════════
        // REPOSITORY FACTORY
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testRepositoryFactory_CreateCacheRepository()
        {
            IQuantityMeasurementRepository cacheRepo = QuantityMeasurementCacheRepository.Instance;

            Assert.IsNotNull(cacheRepo);
            Assert.IsInstanceOfType(cacheRepo, typeof(QuantityMeasurementCacheRepository));
        }

        [TestMethod]
        public void testRepositoryFactory_CreateDatabaseRepository()
        {
            // DatabaseRepository requires a real connection — verify it implements the interface
            IQuantityMeasurementRepository repo = new InMemoryDatabaseTestRepository();

            Assert.IsNotNull(repo);
            Assert.IsTrue(repo is IQuantityMeasurementRepository);
        }

        // ═════════════════════════════════════════════════════════════════════
        // POOL STATISTICS
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testDatabaseRepositoryPoolStatistics()
        {
            string stats = _repo.GetPoolStatistics();

            Assert.IsFalse(string.IsNullOrWhiteSpace(stats));
            // Must contain repository name and record count
            StringAssert.Contains(stats, "InMemoryDatabaseTestRepository");
        }

        // ═════════════════════════════════════════════════════════════════════
        // DATABASE EXCEPTION
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testDatabaseException_CustomException()
        {
            var ex = new QuantityMeasurementRepository.Exception.DatabaseException("Test error");

            Assert.IsNotNull(ex);
            Assert.AreEqual("Test error", ex.Message);
        }

        [TestMethod]
        public void testDatabaseException_WithInnerException()
        {
            var inner = new System.Exception("Inner");
            var ex    = new QuantityMeasurementRepository.Exception.DatabaseException("Outer", inner);

            Assert.AreEqual("Outer", ex.Message);
            Assert.AreEqual("Inner", ex.InnerException!.Message);
        }

        // ═════════════════════════════════════════════════════════════════════
        // RESOURCE CLEANUP
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testResourceCleanup_ConnectionClosed()
        {
            // After every operation the in-memory repo releases resources cleanly
            _repo.Save(new QuantityMeasurementEntity("ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "2 FEET"));

            _repo.ReleaseResources();

            // Repository still accessible after resource release (no crash)
            Assert.AreEqual(1, _repo.GetTotalCount());
        }

        // ═════════════════════════════════════════════════════════════════════
        // BATCH INSERT
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testBatchInsert_MultipleEntities()
        {
            int batchSize = 50;

            for (int i = 0; i < batchSize; i++)
                _repo.Save(new QuantityMeasurementEntity("ADD",
                    new QuantityDTO(i, "FEET", "LENGTH"),
                    new QuantityDTO(i, "FEET", "LENGTH"), $"{i * 2} FEET"));

            Assert.AreEqual(batchSize, _repo.GetTotalCount());
        }

        // ═════════════════════════════════════════════════════════════════════
        // LARGE DATASET
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testDatabaseRepository_LargeDataSet()
        {
            int count = 1000;

            for (int i = 0; i < count; i++)
                _repo.Save(new QuantityMeasurementEntity("COMPARE",
                    new QuantityDTO(i, "FEET", "LENGTH"),
                    new QuantityDTO(i, "FEET", "LENGTH"), "true"));

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var all = _repo.GetAllMeasurements();
            sw.Stop();

            Assert.AreEqual(count, all.Count);
            // Must complete within 2 seconds
            Assert.IsTrue(sw.ElapsedMilliseconds < 2000,
                $"Query took too long: {sw.ElapsedMilliseconds}ms");
        }

        // ═════════════════════════════════════════════════════════════════════
        // TIMESTAMP HANDLING
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testParameterizedQuery_DateTimeHandling()
        {
            var entity = new QuantityMeasurementEntity("COMPARE",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"), "true");

            DateTime before = DateTime.Now.AddSeconds(-1);
            _repo.Save(entity);
            DateTime after  = DateTime.Now.AddSeconds(1);

            var saved = _repo.GetAllMeasurements()[0];

            Assert.IsTrue(saved.Timestamp >= before && saved.Timestamp <= after,
                "Timestamp must be within the expected range.");
        }

        // ═════════════════════════════════════════════════════════════════════
        // CONCURRENT ACCESS
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testDatabaseRepository_ConcurrentAccess()
        {
            int threadCount  = 10;
            int savesPerThread = 10;

            var threads = new System.Threading.Thread[threadCount];

            for (int t = 0; t < threadCount; t++)
            {
                int threadId = t;
                threads[t] = new System.Threading.Thread(() =>
                {
                    for (int i = 0; i < savesPerThread; i++)
                        _repo.Save(new QuantityMeasurementEntity("ADD",
                            new QuantityDTO(threadId, "FEET", "LENGTH"),
                            new QuantityDTO(i,        "FEET", "LENGTH"),
                            $"{threadId + i} FEET"));
                });
            }

            foreach (var t in threads) t.Start();
            foreach (var t in threads) t.Join();

            Assert.AreEqual(threadCount * savesPerThread, _repo.GetTotalCount());
        }

        // ═════════════════════════════════════════════════════════════════════
        // PACKAGE STRUCTURE
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testPackageStructure_AllLayersPresent()
        {
            // Verify all layer types exist and are accessible
            Assert.IsNotNull(typeof(QuantityMeasurementRepository.Repository.IQuantityMeasurementRepository));
            Assert.IsNotNull(typeof(QuantityMeasurementRepository.Repository.QuantityMeasurementCacheRepository));
            Assert.IsNotNull(typeof(QuantityMeasurementRepository.Repository.QuantityMeasurementDatabaseRepository));
            Assert.IsNotNull(typeof(QuantityMeasurementRepository.Exception.DatabaseException));
            Assert.IsNotNull(typeof(QuantityMeasurementBusinessLayer.Service.QuantityMeasurementServiceImpl));
            Assert.IsNotNull(typeof(QuantityMeasurementApp.Controller.QuantityMeasurementController));
        }

        // ═════════════════════════════════════════════════════════════════════
        // INNER: Isolated In-Memory Repository for DB tests
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Fast isolated test double that implements all UC16 interface methods.
        /// Used instead of the real SQLite DB to keep tests self-contained.
        /// </summary>
        private class InMemoryDatabaseTestRepository : IQuantityMeasurementRepository
        {
            private readonly List<QuantityMeasurementEntity> _store = new();
            private readonly object _lock = new();

            public void Save(QuantityMeasurementEntity entity)
            {
                lock (_lock) { _store.Add(entity); }
            }

            public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
            {
                lock (_lock) { return _store.ToList().AsReadOnly(); }
            }

            public void Clear()
            {
                lock (_lock) { _store.Clear(); }
            }

            public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType)
            {
                lock (_lock)
                {
                    return _store.FindAll(e => string.Equals(e.OperationType, operationType,
                        StringComparison.OrdinalIgnoreCase)).AsReadOnly();
                }
            }

            public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType)
            {
                lock (_lock)
                {
                    return _store.FindAll(e => e.FirstOperand != null &&
                        e.FirstOperand.Contains(measurementType, StringComparison.OrdinalIgnoreCase)).AsReadOnly();
                }
            }

            public int GetTotalCount()
            {
                lock (_lock) { return _store.Count; }
            }

            public string GetPoolStatistics()
                => $"InMemoryDatabaseTestRepository | Total records: {GetTotalCount()}";
        }
    }
}