using System.Windows;

namespace FPVNoiseDetector
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public UpdateWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }
    }
}
