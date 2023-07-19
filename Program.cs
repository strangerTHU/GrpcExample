using Grpc.Core;
using Hello;
using System;

class GreeterService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request,Grpc.Core.ServerCallContext context)
    {
        Thread.Sleep(1000);
        var reply = new HelloReply
        {
            Reply = $"Hello, {request.Name}!"
        };
        return Task.FromResult(reply);
    }
    public override async Task LotsOfReplies(HelloRequest request,Grpc.Core.IServerStreamWriter<HelloReply> responseStream,Grpc.Core.ServerCallContext context)
    {
        for (var i = 0; i < 10; i++)
        {
            var reply = new HelloReply { Reply = $"Hello, {request.Name}! {i}" };
            await Task.Delay(1000);
            await responseStream.WriteAsync(reply);
        }
    }
    public override async Task<HelloReply>LotsOfGreetings(Grpc.Core.IAsyncStreamReader<HelloRequest> requestStream,Grpc.Core.ServerCallContext context)
    {
        var replyStr = "";
        while (await requestStream.MoveNext())
        {
            Console.WriteLine($"Received message {requestStream.Current.Name}");
            replyStr += $"Hello, {requestStream.Current.Name}! ";
        }
        var reply = new HelloReply
        {
            Reply = replyStr
        };
        return reply;
    }
}

namespace newprogram
{
    class program
    {
        static void main(string[] args)
        {
            try
            {
                Grpc.Core.Server server = new Grpc.Core.Server(new[] { new Grpc.Core.ChannelOption(Grpc.Core.ChannelOptions.SoReuseport, 0) })
                {
                    Services = { Greeter.BindService(new GreeterService()) },
                    Ports = {new Grpc.Core.ServerPort("0.0.0.0", 8888,
                    Grpc.Core.ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("Server begins to listen");
                Console.ReadLine();
                Console.WriteLine("Server ends!");
                server.ShutdownAsync().Wait();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}