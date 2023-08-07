using serverside.Services;
using grpcschema;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

var udsAddress = Path.Combine(Path.GetTempPath(), "socket.tmp");
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenUnixSocket(udsAddress, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    serverOptions.Listen(System.Net.IPAddress.Parse("0.0.0.0"),18686, ListenOptions =>
    {
        ListenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();



// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
