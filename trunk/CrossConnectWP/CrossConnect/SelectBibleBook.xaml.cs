#region Header

// <copyright file="SelectBibleBook.xaml.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using Sword.reader;
    using CrossConnect.readers;

    /// <summary>
    /// The select bible book.
    /// </summary>
    public partial class SelectBibleBook
    {
        #region Fields

        /// <summary>
        /// The color conversion scheme.
        /// </summary>
        private static readonly Color[] ColorConversionScheme = {
                                                                    (Color)
                                                                    Application.Current.Resources["PhoneForegroundColor"
                                                                        ], Color.FromArgb(255, 0xD2, 0x69, 0x1E),
                                                                    // chochlate
                                                                    Color.FromArgb(255, 0x00, 0xce, 0xd1),
                                                                    // dark turqoise
                                                                    Color.FromArgb(255, 0xff, 0x14, 0x39), // deep pink
                                                                    Color.FromArgb(255, 0xa9, 0xa9, 0xa9), // darkgrey
                                                                    Color.FromArgb(255, 0xb8, 0x86, 0x0b),
                                                                    // darkgoldenrod
                                                                    Color.FromArgb(255, 0x8f, 0xbc, 0x8f),
                                                                    // dark seagreen
                                                                    Color.FromArgb(255, 0x8a, 0x2b, 0xe2), // blueviolet
                                                                    Color.FromArgb(255, 0xff, 0x8c, 0x00), // dark orange
                                                                    Color.FromArgb(255, 0x00, 0xbf, 0xff),
                                                                    // deep skyblue
                                                                    Color.FromArgb(255, 0xdc, 0x14, 0x3c) // crimson
                                                                };

        /// <summary>
        /// The _using now spec.
        /// </summary>
        private ButtonWindowSpecs _usingNowSpec;

        private int _selectBibleBookSecondSelection;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectBibleBook"/> class.
        /// </summary>
        public SelectBibleBook()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// The reload window.
        /// </summary>
        /// <param name="buttonWindow">
        /// The button window.
        /// </param>
        public void ReloadWindow(ButtonWindowSpecs buttonWindow)
        {
            try
            {
                if (buttonWindow != null)
                {
                    int butWidth = 96;
                    int butHeight = 70;
                    switch (buttonWindow.ButSize)
                    {
                        case ButtonSize.Large:
                            butWidth = 200;
                            butHeight = 70;
                            break;
                        case ButtonSize.Medium:
                            butWidth = 96;
                            butHeight = 70;
                            break;
                        case ButtonSize.Small:
                            if (buttonWindow.NumButtons < 50)
                            {
                                double sideLength =
                                    Math.Sqrt(
                                        (Application.Current.Host.Content.ActualWidth
                                          * Application.Current.Host.Content.ActualHeight) / buttonWindow.NumButtons)
                                    / 1.15;
                                if (sideLength * 2
                                    >
                                    (Application.Current.Host.Content.ActualWidth
                                     > Application.Current.Host.Content.ActualHeight
                                         ? Application.Current.Host.Content.ActualHeight
                                         : Application.Current.Host.Content.ActualWidth))
                                {
                                    sideLength = sideLength / 2;
                                }

                                butWidth = (int)sideLength;
                                butHeight = (int)sideLength;
                            }

                            break;
                    }
                    string titletext = buttonWindow.Title;
                    switch(buttonWindow.Title)
                    {
                        case "Select a book to view":
                            titletext = "Select book";
                            break;
                        case "Select a chapter to view":
                            titletext = "Select chapter";
                            break;
                        case "Select a verse to view":
                            titletext = "Select a verse";
                            break;
                    }
                    PageTitle.Text = Translations.Translate(titletext);
                    _usingNowSpec = buttonWindow;
                    ScrollContentGrid.Children.Clear();
                    ScrollContentGrid.ColumnDefinitions.Clear();
                    ScrollContentGrid.RowDefinitions.Clear();
                    double screenWidth = 0;
                    var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                    if (phoneApplicationFrame != null)
                    {
                        PageOrientation orient = phoneApplicationFrame.Orientation;
                        if (orient == PageOrientation.Portrait || orient == PageOrientation.PortraitDown
                            || orient == PageOrientation.PortraitUp || orient == PageOrientation.None)
                        {
                            screenWidth = Application.Current.Host.Content.ActualWidth;
                        }
                        else
                        {
                            screenWidth = Application.Current.Host.Content.ActualHeight;
                        }
                    }

                    int numCols = (int)screenWidth / butWidth;
                    if (numCols == 0)
                    {
                        numCols = 1;
                    }

                    int numRows = buttonWindow.NumButtons / numCols;
                    for (int i = 0; i <= numRows; i++)
                    {
                        var row = new RowDefinition { Height = GridLength.Auto };
                        ScrollContentGrid.RowDefinitions.Add(row);

                        for (int j = 0; j < numCols && ((i * numCols) + j) < buttonWindow.NumButtons; j++)
                        {
                            var col = new ColumnDefinition { Width = GridLength.Auto };
                            ScrollContentGrid.ColumnDefinitions.Add(col);
                            var but = new Button
                                {
                                    Content = buttonWindow.Text[(i * numCols) + j],
                                    Margin = new Thickness(-9, -9, -9, -9),
                                    MaxHeight = butHeight,
                                    MinHeight = butHeight,
                                    MaxWidth = butWidth,
                                    MinWidth = butWidth,
                                    FontSize = (int)(butHeight / 3.5),
                                    Visibility = Visibility.Visible
                                };
                            if (buttonWindow.Colors != null)
                            {
                                but.Foreground =
                                    new SolidColorBrush(ColorConversionScheme[buttonWindow.Colors[(i * numCols) + j]]);
                            }

                            Grid.SetRow(but, i);
                            Grid.SetColumn(but, j);
                            ScrollContentGrid.Children.Add(but);
                            switch (buttonWindow.Stage)
                            {
                                case 0:
                                    but.Click += (RoutedEventHandler)this.FirstClick;
                                    break;
                                case 1:
                                    but.Click += (RoutedEventHandler)this.SecondClick;
                                    break;
                                case 2:
                                    but.Click += (RoutedEventHandler)this.ThirdClick;
                                    break;
                                case 3:
                                    but.Click += (RoutedEventHandler)this.FourthClick;
                                    break;
                            }
                            but.Tag = buttonWindow.Value[(i * numCols) + j];
                        }
                    }

                    LayoutRoot.UpdateLayout();
                    scrollViewer1.UpdateLayout();
                    ScrollContentGrid.UpdateLayout();
                }
            }
            catch (Exception eee)
            {
                Debug.WriteLine("strange crash =" + eee.Message + "; " + eee.StackTrace);
            }
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            ReloadWindow(App.OpenWindows[(int)openWindowIndex].State.Source.GetButtonWindowSpecs(0, 0, Translations.IsoLanguageCode));
        }

        private void PhoneApplicationPageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            ReloadWindow(_usingNowSpec);
        }

        private void FirstClick(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.State["SelectBibleBookFirstSelection"] = 0;
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            var source = App.OpenWindows[(int)openWindowIndex].State.Source;
            Sword.versification.CanonBookDef book = null;
            if (source is BibleZtextReader)
            {
                book = ((BibleZtextReader)App.OpenWindows[(int)openWindowIndex].State.Source).canon.GetBookFromBookNumber((int)((Button)sender).Tag);
            }
            //may need to hide chapter...
            var stage = (book == null || book.NumberOfChapters > 1)
                && !(App.OpenWindows[(int)openWindowIndex].State.Source is RawGenTextReader)
                && !(App.OpenWindows[(int)openWindowIndex].State.Source is DailyPlanReader) ? 1 : 2;
            var stageInfo = (int)((Button)sender).Tag;
            if (stage == 2)
            {
                if (source is BibleZtextReader)
                {
                    _selectBibleBookSecondSelection = book.VersesInChapterStartIndex; //first chapter. there is only one chapter
                    stageInfo = book.VersesInChapterStartIndex;
                }
                else
                {
                    _selectBibleBookSecondSelection = stageInfo;
                }
                ButtonWindowSpecs specs2 = App.OpenWindows[(int)openWindowIndex].State.Source.GetButtonWindowSpecs(
                    stage, stageInfo,Translations.IsoLanguageCode);
                if (specs2 != null)
                {
                    ReloadWindow(specs2);
                    return;
                }

            }
            

            PhoneApplicationService.Current.State["SelectBibleBookFirstSelection"] = stageInfo;

            ButtonWindowSpecs specs = App.OpenWindows[(int)openWindowIndex].State.Source.GetButtonWindowSpecs(
                stage, stageInfo, Translations.IsoLanguageCode);
            if (specs != null)
            {
                PhoneApplicationService.Current.State["SelectBibleBookFirstSelection"] = (int)((Button)sender).Tag;
                ReloadWindow(specs);
            }
            else
            {
                App.OpenWindows[(int)openWindowIndex].State.Source.MoveChapterVerse(book.ShortName1, (int)((Button)sender).Tag - book.VersesInChapterStartIndex, 0, false, App.OpenWindows[(int)openWindowIndex].State.Source);
                PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void SecondClick(object sender, RoutedEventArgs e)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            ButtonWindowSpecs specs = App.OpenWindows[(int)openWindowIndex].State.Source.GetButtonWindowSpecs(
                2, (int)((Button)sender).Tag,Translations.IsoLanguageCode);
            if (specs != null)
            {
                _selectBibleBookSecondSelection = (int)((Button)sender).Tag;
                ReloadWindow(specs);
            }
            else
            {
                var bookname = string.Empty;
                var chapter = 0;
                if (App.OpenWindows[(int)openWindowIndex].State.Source is BibleZtextReader)
                {
                    var book = ((BibleZtextReader)App.OpenWindows[(int)openWindowIndex].State.Source).canon.GetBookFromAbsoluteChapter((int)((Button)sender).Tag);
                    bookname = book.ShortName1;
                    chapter = (int)((Button)sender).Tag - book.VersesInChapterStartIndex;
                }
                else
                {
                    chapter = (int)((Button)sender).Tag;
                } 
                
                App.OpenWindows[(int)openWindowIndex].State.Source.MoveChapterVerse(
                    bookname,
                    chapter,
                    0,
                    false,
                    App.OpenWindows[(int)openWindowIndex].State.Source);
                PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void ThirdClick(object sender, RoutedEventArgs e)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            ButtonWindowSpecs specs = App.OpenWindows[(int)openWindowIndex].State.Source.GetButtonWindowSpecs(
                3, (int)((Button)sender).Tag, Translations.IsoLanguageCode);
            if (specs != null)
            {
                PhoneApplicationService.Current.State["SelectBibleBookThirdSelection"] = (int)((Button)sender).Tag;
                ReloadWindow(specs);
            }
            else
            {
                var bookname = string.Empty;
                var chapter = 0;
                if (App.OpenWindows[(int)openWindowIndex].State.Source is BibleZtextReader)
                {
                    var book = ((BibleZtextReader)App.OpenWindows[(int)openWindowIndex].State.Source).canon.GetBookFromAbsoluteChapter(this._selectBibleBookSecondSelection);
                    bookname = book.ShortName1;
                    chapter = this._selectBibleBookSecondSelection - book.VersesInChapterStartIndex;
                }
                else
                {
                    chapter = this._selectBibleBookSecondSelection;
                } 
                
                App.OpenWindows[(int)openWindowIndex].State.Source.MoveChapterVerse(
                    bookname,
                    chapter,
                    (int)((Button)sender).Tag,
                    false,
                    App.OpenWindows[(int)openWindowIndex].State.Source);
                PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void FourthClick(object sender, RoutedEventArgs e)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            var bookname = string.Empty;
            var chapter = 0;
            if (App.OpenWindows[(int)openWindowIndex].State.Source is BibleZtextReader)
            {
                var book = ((BibleZtextReader)App.OpenWindows[(int)openWindowIndex].State.Source).canon.GetBookFromAbsoluteChapter((int)((Button)sender).Tag);
                bookname = book.ShortName1;
                chapter = (int)((Button)sender).Tag - book.VersesInChapterStartIndex;
            }
            else
            {
                chapter = (int)((Button)sender).Tag;
            }
            App.OpenWindows[(int)openWindowIndex].State.Source.MoveChapterVerse(
                bookname,
                chapter,
                0,
                false,
                App.OpenWindows[(int)openWindowIndex].State.Source);
            PhoneApplicationService.Current.State["skipWindowSettings"] = true;
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #endregion Methods
    }
}