using Grpc.Core;
using Helloworld;
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
        //        new DbContextOptionsBuilder<BloggingContext>()
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
    class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }

    public class ServiceHandler //: SharedService.IAsync
    {
        Server server;
        const int Port = 50051;
        public void Startup()
        {
            server = new Server
            {
                Services = { Greeter.BindService(new GreeterImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public void Shutdown() => server.ShutdownAsync().Wait();
    }
}
