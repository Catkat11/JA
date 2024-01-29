using MAIN.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;

namespace MAIN
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var app = new ApplicationView();
                ApplicationViewModel context = new ApplicationViewModel();
                app.Show();
                app.DataContext = context;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }

        }
    }
}
