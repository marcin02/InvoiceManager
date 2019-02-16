using AppUI.ViewModels;
using Caliburn.Micro;
using DataAccess;
using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppUI
{
    public class Bootstrapper : BootstrapperBase
    {
        #region Constructor

        public Bootstrapper()
        {
            Initialize();
        }

        #endregion

        #region Objects

        private SimpleContainer _container = new SimpleContainer();

        #endregion

        #region Dependecy Injection Config

        protected override void Configure()
        {
            _container.Instance(_container);

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

            _container.PerRequest<ISqliteDataAccess, SqliteDataAccess>();
            _container.PerRequest<IQueries, Queries>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return base.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        #endregion

        #region StarUp

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }


        #endregion
    }
}
