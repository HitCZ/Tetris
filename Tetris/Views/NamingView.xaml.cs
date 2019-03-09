using System.Windows.Input;
using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for Naming.xaml
    /// </summary>
    public partial class NamingView
    {

        #region Properties

        public NamingViewModel ViewModel
        {
            get => DataContext as NamingViewModel;
            set => DataContext = value;
        }

        #endregion Properties

        #region Constructor

        public NamingView()
        {
            InitializeComponent();

            ViewModel = new NamingViewModel();

            ViewModel.CloseAction += Close;
            NameBox.Focus();
            NameBox.KeyDown += NameBox_KeyDown;
        }

        #endregion Constructor

        private void NameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (ViewModel.ConfirmCommand.CanExecute(null))
                ViewModel.ConfirmCommand.Execute(null);
        }

    }
}
