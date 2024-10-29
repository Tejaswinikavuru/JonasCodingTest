using AutoMapper;
using BusinessLayer;
using DataAccessLayer.Database;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Repositories;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebApi.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(WebApi.App_Start.NinjectWebCommon), "Stop")]

namespace WebApi.App_Start
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Filters;
    using BusinessLayer.Model.Interfaces;
    using BusinessLayer.Services;
    using FluentValidation;
    using FluentValidation.WebApi;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using Ninject.WebApi.DependencyResolver;
    using NLog;
    using WebApi.Models;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                // Set Ninject as the Dependency Resolver
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

                // FluentValidation Integration
                FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration);

                RegisterServices(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // AutoMapper Configuration
            kernel.Bind<IMapper>().ToMethod(context =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<BusinessProfile>();      // Business mapping profile
                    cfg.AddProfile<AppServicesProfile>();   // Application services profile
                    cfg.ConstructServicesUsing(t => kernel.Get(t));
                });

                return config.CreateMapper();
            }).InSingletonScope();

            // NLog Configuration
            kernel.Bind<ILogger>().ToMethod(context => LogManager.GetCurrentClassLogger()).InSingletonScope();

            // Global Exception Filter
            kernel.Bind<ExceptionFilterAttribute>().To<GlobalExceptionFilter>().InSingletonScope();

            // FluentValidation Validator Binding
            kernel.Bind<IValidator<CompanyDto>>().To<CompanyInfoValidator>();
            kernel.Bind<IValidator<EmployeeDto>>().To<EmployeeInfoValidator>();

            // Bind the service and repository layers
            kernel.Bind<ICompanyService>().To<CompanyService>();
            kernel.Bind<ICompanyRepository>().To<CompanyRepository>();
            kernel.Bind<IEmployeeService>().To<EmployeeService>();
            kernel.Bind<IEmployeeRepository>().To<EmployeeRepository>();

            // Example of InMemoryDatabase usage
            kernel.Bind(typeof(IDbWrapper<>)).To(typeof(InMemoryDatabase<>));
        }
    }
}
