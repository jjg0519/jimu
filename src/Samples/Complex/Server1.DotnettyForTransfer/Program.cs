﻿using System;
using Autofac;
using Jimu;
using Jimu.Common.Logger;
using Jimu.Server;

namespace Server1.DotnettyForTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var containerBuilder = new ContainerBuilder();
            var builder = new ServiceHostServerBuilder(containerBuilder)
                .UseLog4netLogger(new LogOptions
                {
                    EnableConsoleLog = true
                })
                .LoadServices(new[] { "IServices", "Services" })
                .UseDotNettyForTransfer("127.0.0.1", 8005, server => { })
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService", "127.0.0.1:8005")
                             .UseJoseJwtForOAuth<DotNettyAddress>(new Jimu.Server.OAuth.JwtAuthorizationOptions
                             {
                                 ServerIp = "127.0.0.1",
                                 ServerPort = 8005,
                                 SecretKey = "test",
                                 ExpireTimeSpan = new TimeSpan(1, 0, 0),
                                 TokenEndpointPath = "token",
                                 ValidateLifetime = true,
                                 CheckCredential = o =>
                                 {
                                     if (o.UserName == "admin" && o.Password == "admin")
                                     {
                                         o.AddClaim("department", "IT部");
                                     }
                                     else
                                     {
                                         o.Rejected("401", "acount or password incorrect");
                                     }
                                 }
                             })
                ;
            using (var hostJimu = builder.Build())
            {
                hostJimu.Run();
                Console.ReadLine();
            }

        }
    }
}
