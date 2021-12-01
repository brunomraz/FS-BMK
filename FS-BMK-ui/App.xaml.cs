﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FS_BMK_ui.ViewModels;
using FS_BMK_ui.Views;

namespace FS_BMK_ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IDialogService dialogService = new DialogService(MainWindow);

            dialogService.Register<GraphWindowViewModel, GraphWindow>();

            var viewModel = new OptimizationSuspensionViewModel();
            var view = new MainWindow { DataContext = viewModel };

            view.ShowDialog();
        }
    }
}
