using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace LanProject
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            var settings = new CefSettings();
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = CustomProtocolSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new CustomProtocolSchemeHandlerFactory(),
                IsCSPBypassing = true
            });

            settings.LogSeverity = LogSeverity.Error;
            if (!Cef.IsInitialized)
                Cef.Initialize(settings);
        }
    }
    public class CustomProtocolSchemeHandler : ResourceHandler
    {
        public CustomProtocolSchemeHandler()
        {
        }

        public override CefSharp.CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            return CefSharp.CefReturnValue.Continue;

        }
    }

    public class CustomProtocolSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "customFileProtocol";

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new CustomProtocolSchemeHandler();
        }
    }
}
