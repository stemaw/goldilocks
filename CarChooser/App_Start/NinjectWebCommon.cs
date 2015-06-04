using System.Web.SessionState;
using CarChooser.Data;
using CarChooser.Domain;
using CarChooser.Domain.Audit;
using CarChooser.Domain.ScoreStrategies;
using CarChooser.Web.Mappers;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CarChooser.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CarChooser.Web.App_Start.NinjectWebCommon), "Stop")]

namespace CarChooser.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
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
            kernel.Bind<ISearchCars>().To<SearchService>().InSingletonScope();
            kernel.Bind<IGetCars>().To<CarRepository>().InSingletonScope();
            kernel.Bind<IMapCarVMs>().To<CarVMMapper>().InSingletonScope();
            kernel.Bind<IMapSearchRequests>().To<SearchMapper>().InSingletonScope();
            kernel.Bind<IMapSearchVMs>().To<SearchVMMapper>().InSingletonScope();
            kernel.Bind<IManageCars>().To<CarService>().InSingletonScope();
            kernel.Bind<IMapCarRatings>().To<CarRatingsMapper>().InSingletonScope();
            kernel.Bind<IFilter>().To<AdaptiveScorer>();
            kernel.Bind<IRecordDecisions>().To<DecisionRepository>();
        }        
    }
}
