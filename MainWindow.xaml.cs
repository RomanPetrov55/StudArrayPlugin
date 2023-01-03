using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TSDI = Tekla.Structures.Dialog;

namespace StudArrayPlugin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : TSDI.PluginWindowBase
    {
        MainWindowViewModel dataViewModel { get; set; }
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            dataViewModel = viewModel;
        }

        private void WpfOkApplyModifyGetOnOffCancel_ApplyClicked(object sender, EventArgs e)
        {
            this.Apply();
        }

        private void WpfOkApplyModifyGetOnOffCancel_CancelClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WpfOkApplyModifyGetOnOffCancel_GetClicked(object sender, EventArgs e)
        {
            this.Get();
        }

        private void WpfOkApplyModifyGetOnOffCancel_ModifyClicked(object sender, EventArgs e)
        {
            this.Modify();
        }

        private void WpfOkApplyModifyGetOnOffCancel_OkClicked(object sender, EventArgs e)
        {
            this.Apply();
            this.Close();
        }

        private void WpfOkApplyModifyGetOnOffCancel_OnOffClicked(object sender, EventArgs e)
        {
            this.ToggleSelection();
        }

        private void MaterialCatalog_SelectClicked(object sender, EventArgs e)
        {
            this.MaterialCatalog.SelectedMaterial = this.dataViewModel.Material;
        }

        private void MaterialCatalog_SelectionDone(object sender, EventArgs e)
        {
            this.dataViewModel.Material = this.MaterialCatalog.SelectedMaterial;
        }
    }
}
