﻿using Autofac;
using Autofac.Integration.WebApi;
using PlayStudiosApi.Autofac;
using PlayStudiosApi.Service.Autofac;
using PlayStudiosApi.Service.Services;
using PlayStudiosApi.Service.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PlayStudiosApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            InitializeIoC();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void InitializeIoC()
        {
            // Create the container builder
            var builder = new ContainerBuilder();

            // Register Modules
            builder
                .RegisterModule<ApiModule>();

            builder
                .RegisterModule<ServiceModule>();

            builder
                .Register(x =>
                {
                    return new LoggerConfiguration()
                      .WriteTo.File("log.txt",
                                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                      .CreateLogger();
                })
                .As<ILogger>()
                .SingleInstance();

            // Build the container
            var container = builder.Build();

            // Configure the Web Api with the dependency resolver
            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}
