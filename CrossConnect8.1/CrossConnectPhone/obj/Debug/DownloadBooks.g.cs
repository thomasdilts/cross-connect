﻿#pragma checksum "C:\Users\Thomas\Documents\GitHubVisualStudio\cross-connect\CrossConnect8.1\CrossConnectPhone\DownloadBooks.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0F735B490F7E71768E6CE448AB0F848B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CrossConnect;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace CrossConnect {
    
    
    public partial class DownloadBooks : CrossConnect.AutoRotatePage {
        
        internal System.Windows.Controls.ScrollViewer scrollViewer1;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.ProgressBar WaitingForDownload;
        
        internal System.Windows.Controls.TextBlock ServerMessage;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Grid DocumentPanel;
        
        internal Microsoft.Phone.Controls.ListPicker selectServer;
        
        internal System.Windows.Controls.Button butDownload;
        
        internal System.Windows.Controls.ProgressBar progressBarGetBookList;
        
        internal Microsoft.Phone.Controls.ListPicker selectType;
        
        internal Microsoft.Phone.Controls.ListPicker selectLangauge;
        
        internal Microsoft.Phone.Controls.ListPicker selectBook;
        
        internal System.Windows.Controls.Grid BookPanel;
        
        internal System.Windows.Controls.Button butDownloadBook;
        
        internal System.Windows.Controls.ProgressBar progressBarGetBook;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/CrossConnect;component/DownloadBooks.xaml", System.UriKind.Relative));
            this.scrollViewer1 = ((System.Windows.Controls.ScrollViewer)(this.FindName("scrollViewer1")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.WaitingForDownload = ((System.Windows.Controls.ProgressBar)(this.FindName("WaitingForDownload")));
            this.ServerMessage = ((System.Windows.Controls.TextBlock)(this.FindName("ServerMessage")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.DocumentPanel = ((System.Windows.Controls.Grid)(this.FindName("DocumentPanel")));
            this.selectServer = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("selectServer")));
            this.butDownload = ((System.Windows.Controls.Button)(this.FindName("butDownload")));
            this.progressBarGetBookList = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBarGetBookList")));
            this.selectType = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("selectType")));
            this.selectLangauge = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("selectLangauge")));
            this.selectBook = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("selectBook")));
            this.BookPanel = ((System.Windows.Controls.Grid)(this.FindName("BookPanel")));
            this.butDownloadBook = ((System.Windows.Controls.Button)(this.FindName("butDownloadBook")));
            this.progressBarGetBook = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBarGetBook")));
        }
    }
}

