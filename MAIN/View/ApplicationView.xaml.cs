using MAIN.ViewModel;
using System.Windows;

namespace MAIN
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : Window
    {
        public ApplicationView()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();
        }
    }
}
