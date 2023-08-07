using System.Net.Sockets;
using FluentAssertions;
using Grpc.Net.Client;

namespace clientside;

public class UnitTest1
{
    public static readonly string UDSAddress = Path.Combine(Path.GetTempPath(), "socket.tmp");
    public static readonly string TCPAddress = "http://127.0.0.1:18686";

    [Fact]
    public async Task TCPTest()
    {
        
        var channel = GrpcChannel.ForAddress(TCPAddress);

        var req = new grpcschema.HelloRequest() { Name = "てぃーしーぴぃそけっと" };

        var client = new grpcschema.Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(req);

        reply.Message.Should().Be("Hello " + req.Name);
    }

    [Fact]
    public async Task UDSTest()
    {
        var udsEndPoint = new UnixDomainSocketEndPoint(UDSAddress);
        var connectionFactory = new UnixDomainSocketsConnectionFactory(udsEndPoint);
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = connectionFactory.ConnectAsync
        };

        var channel = GrpcChannel.ForAddress(
            TCPAddress, //←これはなんでもいい(無視される)
             new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler
        });

        var req = new grpcschema.HelloRequest() { Name = "ゆにっくすどめいんそけっと" };

        var client = new grpcschema.Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(req);

        reply.Message.Should().Be("Hello " + req.Name);
    }

}