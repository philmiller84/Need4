using Grpc.Core;
using Need4Protocol;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Models;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Need4
{
    
    public class SqliteInMemoryItemsControllerTest : IDisposable
    {
        private readonly DbConnection _connection;

        //public SqliteInMemoryItemsControllerTest()
        //    : base(
        //        new DbContextOptionsBuilder<ItemContext>()
        //            .UseSqlite(CreateInMemoryDatabase())
        //            .Options)
        //{
        //    _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        //}

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();
        
            return connection;
        }

        public void Dispose() => _connection.Dispose();
    }


    public class ServiceHandler //: SharedService.IAsync
    {
        Server server;
        const int Port = 50051;
        public void Startup()
        {
            server = new Server
            {
                Services = { ItemRepository.BindService(new ItemRepositoryImpl()) },
                Ports = { new ServerPort("127.0.0.1", Port, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public void Shutdown() => server.ShutdownAsync().Wait();
    }
}
