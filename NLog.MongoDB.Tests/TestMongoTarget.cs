﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using Moq;
using NLog.Common;
using NLog.LayoutRenderers;
using NUnit.Framework;
using FluentAssertions;
using MongoDB.Bson;

namespace NLog.MongoDB.Tests
{
    [TestFixture]
    public class TestMongoTarget
    {
        private Mock<IRepositoryProvider> _mockProvider;
        private Mock<IRepository> _mockRepository;
        private MongoServerSettings _settings;

        [SetUp]
        public void TestTarget()
        {
            _mockProvider = new Mock<IRepositoryProvider>();
            _mockRepository = new Mock<IRepository>();

            _settings = new MongoServerSettings();
        }

        [Test]
        public void TestDefaultSettings()
        {
            new MongoDBTarget().Host
                .Should().Be("localhost");
            new MongoDBTarget().Port
                .Should().Be(27017);
            new MongoDBTarget().Database
                .Should().Be("NLog");
        }


        [Test]
        public void TestSettings()
        {
            const string databaseName = "Test";
            const string host = "localhost";
            const int port = 27017;
            const string username = "someUser";
            const string password = "q198743n3d8yh32028##@!";
            const string connectionString = "mongodb://some.server/nlog";
            const string connectionName = "mongodb";

            var target = new MongoDBTarget
                             {
                                 Database = databaseName,
                                 Host = host,
                                 Port = port,
                                 Username = username,
                                 Password = password,
                                 ConnectionString = connectionString,
                                 ConnectionName = connectionName
                             };

            target.Database
                .Should().Be(databaseName);
            target.Host
                .Should().Be(host);
            target.Port
                .Should().Be(port);
            target.Username
                .Should().Be(username);
            target.Password
                .Should().Be(password);
            target.ConnectionString
                .Should().Be(connectionString);
            target.ConnectionName
                .Should().Be(connectionName);
        }


        [Test]
        public void TestRepository()
        {
            const string databaseName = "Test";
            const string host = "localhost";
            const int port = 27017;
            const string username = "someUser";
            const string password = "q198743n3d8yh32028##@!";


            _mockProvider.Setup(
                p => p.GetRepository(It.IsAny<MongoServerSettings>(), It.IsAny<string>()))
                .Returns(_mockRepository.Object)
                .Verifiable();

            var target = new MongoDBTarget
            {
                Database = databaseName,
                Host = host,
                Port = port,
                Username = username,
                Password = password,
                GetProvider = () => _mockProvider.Object
            };

            var eventLogInfo = new LogEventInfo();

            _mockRepository.Setup(
                r => r.Insert(It.IsAny<string>(), It.IsAny<BsonDocument>()))
                .Verifiable();

            target.TestWrite(eventLogInfo);

            _mockProvider.Verify();
            _mockRepository.Verify();

            new MongoDBTarget().Host
                .Should().Be(host);
            new MongoDBTarget().Port
                .Should().Be(port);
            new MongoDBTarget().Database
                .Should().Be("NLog");
        }
    }
}
