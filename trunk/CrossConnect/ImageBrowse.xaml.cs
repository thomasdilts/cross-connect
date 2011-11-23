#region Header

// <copyright file="ImageBrowse.xaml.cs" company="Thomas Dilts">
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

#endregion Header

namespace CrossConnect
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Microsoft.Phone.Shell;

    public partial class ImageBrowse
    {
        #region Fields

        private string[] _imageNames = new string[0];
        private int _nowShowingPicture;

        #endregion Fields

        #region Constructors

        public ImageBrowse()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePageBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select an image");

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = Translations.Translate("Previous");
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = Translations.Translate("Save");
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).Text = Translations.Translate("Next");

            var root = IsolatedStorageFile.GetUserStoreForApplication();
            if (root.DirectoryExists(App.WebDirIsolated + "/images"))
            {
                try
                {
                    _imageNames = root.GetFileNames(App.WebDirIsolated + "/images/*.*");
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee);
                }
            }
            if (_imageNames == null || _imageNames.Count()==0)
            {
                //if (NavigationService.CanGoBack)
                //{
                //    NavigationService.GoBack();
                //}
            }
            else
            {
                LoadCurrentImage();
            }
        }

        private void ButNextClick(object sender, EventArgs e)
        {
            _nowShowingPicture++;
            if (_nowShowingPicture >= _imageNames.Count())
                _nowShowingPicture = 0;
            LoadCurrentImage();
        }

        private void ButPreviousClick(object sender, EventArgs e)
        {
            _nowShowingPicture--;
            if (_nowShowingPicture < 0)
                _nowShowingPicture = _imageNames.Count() - 1;
            if (_nowShowingPicture < 0)
                _nowShowingPicture = 0;
            LoadCurrentImage();
        }

        private void ButSelectClick(object sender, EventArgs e)
        {
            if (_imageNames.Count() > _nowShowingPicture && !string.IsNullOrEmpty(_imageNames[_nowShowingPicture]))
            {
                PhoneApplicationService.Current.State["ImageBrowserSelected"] = _imageNames[_nowShowingPicture];
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void LoadCurrentImage()
        {
            if (_imageNames.Count() > _nowShowingPicture && !string.IsNullOrEmpty(_imageNames[_nowShowingPicture]))
            {
                //read from isolated storage.
                using (var isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageRoot.FileExists(App.WebDirIsolated + "/images/" + _imageNames[_nowShowingPicture]))
                    {
                        try
                        {
                            using (IsolatedStorageFileStream fStream =
                                isolatedStorageRoot.OpenFile(App.WebDirIsolated + "/images/" + _imageNames[_nowShowingPicture], FileMode.Open))
                            {
                                var buffer = new byte[10000];
                                int len;
                                var ms = new MemoryStream();
                                while ((len = fStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                                {
                                    ms.Write(buffer, 0, len);
                                }
                                fStream.Close();
                                ms.Position = 0;
                                var bitImage = new BitmapImage();
                                bitImage.SetSource(ms);
                                ImagePane.Source = bitImage;
                            }
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}