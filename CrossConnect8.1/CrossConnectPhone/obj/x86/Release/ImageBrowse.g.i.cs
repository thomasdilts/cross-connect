﻿#pragma checksum "C:\Users\Thomas\Documents\GitHubVisualStudio\cross-connect\CrossConnect8.1\CrossConnectPhone\ImageBrowse.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "98A22241DB7EF3EFFE2F94823AC823BD"
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
using Microsoft.Phone.Shell;
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
    
    
    public partial class ImageBrowse : CrossConnect.AutoRotatePage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.Image ImagePane;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton ButPrevious;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton ButSelect;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton ButNext;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/CrossConnect;component/ImageBrowse.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ImagePane = ((System.Windows.Controls.Image)(this.FindName("ImagePane")));
            this.ButPrevious = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("ButPrevious")));
            this.ButSelect = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("ButSelect")));
            this.ButNext = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("ButNext")));
        }
    }
}
