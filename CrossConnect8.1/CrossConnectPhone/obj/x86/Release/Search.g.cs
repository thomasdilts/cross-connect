﻿#pragma checksum "C:\Users\Thomas\Documents\GitHubVisualStudio\cross-connect\CrossConnect8.1\CrossConnectPhone\Search.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "17F5598198FC6EC64E9EF5767EAA3DC1"
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
    
    
    public partial class Search : CrossConnect.AutoRotatePage {
        
        internal System.Windows.Controls.ScrollViewer scrollViewer1;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.StackPanel ContentPanel;
        
        internal System.Windows.Controls.Button butSearch;
        
        internal System.Windows.Controls.TextBox SearchText;
        
        internal System.Windows.Controls.RadioButton FastSearch;
        
        internal System.Windows.Controls.Button butFastHelp;
        
        internal System.Windows.Controls.RadioButton SlowSearch;
        
        internal System.Windows.Controls.TextBlock SearchWhereText;
        
        internal System.Windows.Controls.RadioButton wholeBible;
        
        internal System.Windows.Controls.RadioButton oldTestement;
        
        internal System.Windows.Controls.RadioButton newTEstement;
        
        internal System.Windows.Controls.RadioButton Chapter;
        
        internal System.Windows.Controls.TextBlock SearchByText;
        
        internal System.Windows.Controls.RadioButton OneOrMoreWords;
        
        internal System.Windows.Controls.RadioButton AllWords;
        
        internal System.Windows.Controls.RadioButton ExactMatch;
        
        internal Microsoft.Phone.Controls.ToggleSwitch IgnoreCase;
        
        internal System.Windows.Controls.ProgressBar progressBar1;
        
        internal System.Windows.Controls.Button butHelp;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/CrossConnect;component/Search.xaml", System.UriKind.Relative));
            this.scrollViewer1 = ((System.Windows.Controls.ScrollViewer)(this.FindName("scrollViewer1")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ContentPanel = ((System.Windows.Controls.StackPanel)(this.FindName("ContentPanel")));
            this.butSearch = ((System.Windows.Controls.Button)(this.FindName("butSearch")));
            this.SearchText = ((System.Windows.Controls.TextBox)(this.FindName("SearchText")));
            this.FastSearch = ((System.Windows.Controls.RadioButton)(this.FindName("FastSearch")));
            this.butFastHelp = ((System.Windows.Controls.Button)(this.FindName("butFastHelp")));
            this.SlowSearch = ((System.Windows.Controls.RadioButton)(this.FindName("SlowSearch")));
            this.SearchWhereText = ((System.Windows.Controls.TextBlock)(this.FindName("SearchWhereText")));
            this.wholeBible = ((System.Windows.Controls.RadioButton)(this.FindName("wholeBible")));
            this.oldTestement = ((System.Windows.Controls.RadioButton)(this.FindName("oldTestement")));
            this.newTEstement = ((System.Windows.Controls.RadioButton)(this.FindName("newTEstement")));
            this.Chapter = ((System.Windows.Controls.RadioButton)(this.FindName("Chapter")));
            this.SearchByText = ((System.Windows.Controls.TextBlock)(this.FindName("SearchByText")));
            this.OneOrMoreWords = ((System.Windows.Controls.RadioButton)(this.FindName("OneOrMoreWords")));
            this.AllWords = ((System.Windows.Controls.RadioButton)(this.FindName("AllWords")));
            this.ExactMatch = ((System.Windows.Controls.RadioButton)(this.FindName("ExactMatch")));
            this.IgnoreCase = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("IgnoreCase")));
            this.progressBar1 = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBar1")));
            this.butHelp = ((System.Windows.Controls.Button)(this.FindName("butHelp")));
        }
    }
}
