﻿// <copyright file="MediaPlayerChooseBook.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

namespace CrossConnect
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using CrossConnect.readers;

    using Sword.reader;

    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;
    using Sword.versification;

    public sealed partial class MediaPlayerWindow
    {
        #region Static Fields

        private static readonly Color[] ColorConversionScheme =
            {
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(255, 0xD2, 0x69, 0x1E), // chochlate
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

        #endregion

        #region Fields

        private int _selectBibleBookSecondSelection;

        private ButtonWindowSpecs _usingNowSpec;

        #endregion

        #region Public Methods and Operators

        public event EventHandler HitButtonBigger;

        public static void MakeSurePopupIsOnScreen(
            double popupWindowHeight,
            double popupWindowWidth,
            FrameworkElement container,
            FrameworkElement topObject,
            Popup popup)
        {
            Rect windowBounds = Window.Current.Bounds;
            UIElement rootVisual = Window.Current.Content;

            GeneralTransform gt = container.TransformToVisual(rootVisual);

            Point absolutePosition = gt.TransformPoint(new Point(0, 0));
            GeneralTransform gt2 = popup.TransformToVisual(container);
            Point pos = gt2.TransformPoint(new Point(0, 0));

            // place it over the container object which is usually the button that pressed to show the screen
            popup.VerticalOffset = -pos.Y;
            if ((windowBounds.Height - absolutePosition.Y) < (popupWindowHeight + pos.Y))
            {
                double moreHeightAdjust = popupWindowHeight - (windowBounds.Height - absolutePosition.Y);
                popup.VerticalOffset = -moreHeightAdjust - pos.Y;
            }

            // place it over the container object which is usually the button that pressed to show the screen
            popup.HorizontalOffset = -pos.X;
            if ((windowBounds.Width - absolutePosition.X) < (popupWindowWidth + pos.X))
            {
                double moreWidthAdjust = popupWindowWidth - (windowBounds.Width - absolutePosition.X);
                popup.HorizontalOffset = -moreWidthAdjust - pos.X;
            }
            //make sure it is not off the screen to the left
            GeneralTransform gt3 = popup.TransformToVisual(rootVisual);
            Point posAbs = gt3.TransformPoint(new Point(0, 0));
            if (posAbs.X < 0)
            {
                popup.HorizontalOffset = -posAbs.X;
            }

            //make sure it is not off the screen to the right
            if ((popup.HorizontalOffset + posAbs.X + popupWindowWidth) > windowBounds.Width)
            {
                popup.HorizontalOffset = windowBounds.Width - (popupWindowWidth + posAbs.X);
            }
        }

        public void ReloadBookPopupWindow(ButtonWindowSpecs buttonWindow)
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
                            butHeight = 45;
                            break;
                        case ButtonSize.Medium:
                            butWidth = 85;
                            butHeight = 45;
                            break;
                        case ButtonSize.Small:
                            butWidth = 85;
                            butHeight = 45;

                            break;
                    }

                    this.ListTitle.Text = Translations.Translate(buttonWindow.Title);
                    this._usingNowSpec = buttonWindow;
                    this.ScrollContentGrid.Children.Clear();
                    this.ScrollContentGrid.ColumnDefinitions.Clear();
                    this.ScrollContentGrid.RowDefinitions.Clear();
                    Rect bounds = Window.Current.Bounds;

                    int numCols = (int)bounds.Width / butWidth;
                    if (numCols == 0)
                    {
                        numCols = 1;
                    }

                    if (numCols > 10)
                    {
                        numCols = 10;
                    }

                    int numRows = buttonWindow.NumButtons / numCols;

                    for (int i = 0; i <= numRows; i++)
                    {
                        var row = new RowDefinition { Height = GridLength.Auto };
                        this.ScrollContentGrid.RowDefinitions.Add(row);

                        for (int j = 0; j < numCols && ((i * numCols) + j) < buttonWindow.NumButtons; j++)
                        {
                            var col = new ColumnDefinition { Width = GridLength.Auto };
                            this.ScrollContentGrid.ColumnDefinitions.Add(col);
                            var but = new Button
                                          {
                                              Content = buttonWindow.Text[(i * numCols) + j],
                                              //Margin = new Thickness(1, 1, 1, 1),
                                              MaxHeight = butHeight,
                                              MinHeight = butHeight,
                                              MaxWidth = butWidth,
                                              MinWidth = butWidth,
                                              FontSize = (butHeight / 2),
                                              Visibility = Visibility.Visible
                                          };
                            if (buttonWindow.Colors != null
                                && !ColorConversionScheme[buttonWindow.Colors[(i * numCols) + j]].Equals(
                                    Color.FromArgb(0, 0, 0, 0)))
                            {
                                but.Background =
                                    new SolidColorBrush(ColorConversionScheme[buttonWindow.Colors[(i * numCols) + j]]);
                            }

                            Grid.SetRow(but, i);
                            Grid.SetColumn(but, j);
                            this.ScrollContentGrid.Children.Add(but);
                            switch (buttonWindow.Stage)
                            {
                                case 0:
                                    but.Click += (RoutedEventHandler)this.FirstClick;
                                    break;
                                case 1:
                                    but.Click += (RoutedEventHandler)this.SecondClick;
                                    break;
                            }
                            but.Tag = buttonWindow.Value[(i * numCols) + j];
                        }
                    }
                    double popupWindowHeight = numRows * butHeight + 50;
                    this.SelectBookScrollViewer.Height = popupWindowHeight;
                    if (popupWindowHeight > bounds.Height)
                    {
                        this.SelectBookScrollViewer.Height = bounds.Height - 40;
                    }

                    this.ScrollContentGrid.UpdateLayout();
                    MakeSurePopupIsOnScreen(
                        this.ScrollContentGrid.ActualHeight,
                        this.ScrollContentGrid.ActualWidth,
                        this.ButMenu,
                        this,
                        this.BookPopup);
                }
            }
            catch (Exception eee)
            {
                Debug.WriteLine("strange crash =" + eee.Message + "; " + eee.StackTrace);
            }
        }

        #endregion

        #region Methods
        /*
        private void ButSearch_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.SearchPopup.IsOpen)
            {
                this.ListChapter.Content = Translations.Translate("Select a chapter to view");
                this.ListBook.Content = Translations.Translate("Select a book to view");
                var canonKjv = CanonManager.GetCanon("KJV");

                //may need to hide chapter...
                bool showChapter = true;
                foreach (var book in canonKjv.BookByShortName)
                {
                    if (((MediaReader)this._state.Source).Info.Chapter == book.Value.VersesInChapterStartIndex)
                    {
                        showChapter = book.Value.NumberOfChapters > 1;
                        break;
                    }                    
                }

                this.ListChapter.Visibility = showChapter ? Visibility.Visible : Visibility.Collapsed;

                this.SubMenuSearchPopup.SelectedItem = null;
                this.SearchPopup.IsOpen = true;
                this.SubMenuSearchPopup.UpdateLayout();
                MakeSurePopupIsOnScreen(
                    this.SubMenuSearchPopup.ActualHeight,
                    this.SubMenuSearchPopup.ActualWidth,
                    this.ButSearch,
                    this,
                    this.SearchPopup);
            }
        }
        */
        private void FirstClick(object sender, RoutedEventArgs e)
        {
            ButtonWindowSpecs specs = this._state.Source.GetButtonWindowSpecs(1, (int)((Button)sender).Tag,Translations.IsoLanguageCode);
            if (specs != null)
            {
                this.ReloadBookPopupWindow(specs);
            }
            else
            {
                // go directly to verse
                specs = this._state.Source.GetButtonWindowSpecs(2, (int)((Button)sender).Tag,Translations.IsoLanguageCode);
                if (specs != null)
                {
                    this._selectBibleBookSecondSelection = (int)((Button)sender).Tag;
                    this.ReloadBookPopupWindow(specs);
                }
                else
                {
                    var canonKjv = CanonManager.GetCanon("KJV");
                    var book = canonKjv.GetBookFromAbsoluteChapter((int)((Button)sender).Tag);
                    this._state.Source.MoveChapterVerse(book.ShortName1,(int)((Button)sender).Tag, 0, false, this._state.Source);

                    this.BookPopup.IsOpen = false;
                    this.SearchPopup.IsOpen = false;
                    Info.Chapter = (int)((Button)sender).Tag;
                    this.RestartToThisMedia();
                }
            }
        }

        private void MenuPopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void MenuPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void SearchPopup_OnClosed(object sender, object e)
        {
            if (!this.SearchPopup.IsOpen && !this.BookPopup.IsOpen)
            {
                App.ShowUserInterface(true);
            }
        }

        private void SearchPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void SecondClick(object sender, RoutedEventArgs e)
        {
            var canonKjv = CanonManager.GetCanon("KJV");
            var book = canonKjv.GetBookFromAbsoluteChapter((int)((Button)sender).Tag);
            this._state.Source.MoveChapterVerse(book.ShortName1, (int)((Button)sender).Tag, 0, false, this._state.Source);
            this.BookPopup.IsOpen = false;
            this.SearchPopup.IsOpen = false;
            Info.Chapter = (int)((Button)sender).Tag;
            this.RestartToThisMedia();
        }

        private void SubMenuSearchPopup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemSelected = (ListBoxItem)e.AddedItems.FirstOrDefault();
            if (itemSelected == null)
            {
                return;
            }
            switch (itemSelected.Name)
            {
                case "ListBook":
                    if (!this.BookPopup.IsOpen)
                    {
                        this.BookPopup.IsOpen = true;
                        this.SearchPopup.IsOpen = false;
                    }
                    this.ReloadBookPopupWindow(this._state.Source.GetButtonWindowSpecs(0, 0, Translations.IsoLanguageCode));
                    break;
                case "ListChapter":
                    {
                        if (!this.BookPopup.IsOpen)
                        {
                            this.BookPopup.IsOpen = true;
                            this.SearchPopup.IsOpen = false;
                        }

                        ButtonWindowSpecs specs = this._state.Source.GetButtonWindowSpecs(
                            1, Info.Chapter, Translations.IsoLanguageCode);
                        if (specs != null)
                        {
                            this.ReloadBookPopupWindow(specs);
                        }
                        else
                        {
                            this.RestartToThisMedia();

                            this.BookPopup.IsOpen = false;
                            this.SearchPopup.IsOpen = false;
                            this.UpdateBrowser(false);
                        }
                    }
                    break;
            }
            this.SubMenuSearchPopup.SelectedItem = null;
        }

        #endregion
    }
}