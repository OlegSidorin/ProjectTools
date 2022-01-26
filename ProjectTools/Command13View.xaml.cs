using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectTools
{
    /// <summary>
    /// Логика взаимодействия для Command13View.xaml
    /// </summary>
    public partial class Command13View : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9,-]+"); //regex that matches disallowed text
        private CreateVoidsEventHandler _createVoidsEventHandler;
        private ExternalEvent _externalEvent;
        private ExternalCommandData _commandData;
        public CreateVoidsEventHandler CreateVoidsEventHandler
        {
            get { return _createVoidsEventHandler; }
            set
            {
                if (value == _createVoidsEventHandler) return;
                _createVoidsEventHandler = value;
            }
        }
        public ExternalEvent CreateVoidsExternalEvent
        {
            get { return _externalEvent; }
            set
            {
                if (value == _externalEvent) return;
                _externalEvent = value;
            }
        }
        public ExternalCommandData CommandData
        {
            get { return _commandData; }
            set
            {
                if (value == _commandData) return;
                _commandData = value;
            }
        }

        public Command13View()
        {
            InitializeComponent();

            CreateVoidsEventHandler = new CreateVoidsEventHandler();
            CreateVoidsExternalEvent = ExternalEvent.Create(CreateVoidsEventHandler);
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void OnClickApply(object sender, RoutedEventArgs e)
        {
            Command13ViewModel vm = (Command13ViewModel)DataContext;

            CreateVoidsEventHandler.CommandData = CommandData;
            CreateVoidsEventHandler.ViewModel = vm;
            bool wgResult = double.TryParse(vm.WallGap, out double wg);
            CreateVoidsEventHandler.WallGap = 2 * wg.ToFeet();
            bool wiResult = double.TryParse(vm.WallIndent, out double wi);
            CreateVoidsEventHandler.WallIndent = 2 * wi.ToFeet();
            CreateVoidsEventHandler.CommandData = CommandData;

            if (wgResult && wiResult)
            {
                //Close();
                CreateVoidsExternalEvent.Raise();
            }
            else MessageBox.Show("неверное значение зазора или отступа");

        }
    }
}
