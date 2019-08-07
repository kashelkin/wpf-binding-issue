using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfBinding.MVVM;
using WpfBinding.ViewModels;
using WpfBinding.Views;

namespace WpfBinding
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static WindowManager WindowManager { get; } = new WindowManager();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WindowManager.RegisterWindowType<MainViewModel, MainWindow>();
            WindowManager.RegisterWindowType<ChildViewModel, ChildWindow>();

            WindowManager.ShowPresentation(new MainViewModel());
        }
    }
}
