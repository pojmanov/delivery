using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoSerive
{
    /// <summary>
    /// Реализация клиента к службе GEO
    /// </summary>
    public class GeoClient : IGeoClient
    {
        private readonly MethodConfig _methodConfig;
        private readonly SocketsHttpHandler _socketsHttpHandler;
        private readonly string _url;

        public GeoClient(IOptions<Settings> options)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(options.Value.GeoServiceGrpcHost));
            _url = options.Value.GeoServiceGrpcHost;

            _socketsHttpHandler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                EnableMultipleHttp2Connections = true
            };

            _methodConfig = new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.5,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                }
            };

        }

        /// <summary>
        /// Получить координаты по адресу
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public async Task<Location> GetLocationAsync(string address, CancellationToken cancellationToken)
        {
            using var channel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions
            {
                HttpHandler = _socketsHttpHandler,
                ServiceConfig = new ServiceConfig { MethodConfigs = { _methodConfig } }
            });

            var client = new GeoApp.Api.Geo.GeoClient(channel);
            var request = new GeoApp.Api.GetGeolocationRequest() { Street = address };
            var reply = await client.GetGeolocationAsync(request, null, DateTime.UtcNow.AddSeconds(5), cancellationToken);
            var result = new Location(reply.Location.X, reply.Location.Y);
            return result;
        }
    }
}
