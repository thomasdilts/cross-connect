// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageBrowse.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The image browse.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

// <copyright file="ImageBrowse.xaml.cs" company="Thomas Dilts">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Microsoft.Phone.Shell;

    /// <summary>
    /// The image browse.
    /// </summary>
    public partial class ImageBrowse
    {
        #region Constants and Fields

        /// <summary>
        /// The _image names.
        /// </summary>
        private string[] _imageNames = new string[0];

        /// <summary>
        /// The _now showing picture.
        /// </summary>
        private int _nowShowingPicture;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBrowse"/> class.
        /// </summary>
        public ImageBrowse()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The auto rotate page back key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
        }

        /// <summary>
        /// The auto rotate page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            this.PageTitle.Text = Translations.Translate("Select an image");

            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Text = Translations.Translate("Previous");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).Text = Translations.Translate("Save");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[2]).Text = Translations.Translate("Next");

            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            if (root.DirectoryExists(App.WebDirIsolated + "/images"))
            {
                try
                {
                    this._imageNames = root.GetFileNames(App.WebDirIsolated + "/images/*.*");
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee);
                }
            }

            if (this._imageNames == null || this._imageNames.Count() == 0)
            {
                // if (NavigationService.CanGoBack)
                // {
                // NavigationService.GoBack();
                // }
            }
            else
            {
                this.LoadCurrentImage();
            }
        }

        /// <summary>
        /// The but next click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButNextClick(object sender, EventArgs e)
        {
            this._nowShowingPicture++;
            if (this._nowShowingPicture >= this._imageNames.Count())
            {
                this._nowShowingPicture = 0;
            }

            this.LoadCurrentImage();
        }

        /// <summary>
        /// The but previous click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButPreviousClick(object sender, EventArgs e)
        {
            this._nowShowingPicture--;
            if (this._nowShowingPicture < 0)
            {
                this._nowShowingPicture = this._imageNames.Count() - 1;
            }

            if (this._nowShowingPicture < 0)
            {
                this._nowShowingPicture = 0;
            }

            this.LoadCurrentImage();
        }

        /// <summary>
        /// The but select click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSelectClick(object sender, EventArgs e)
        {
            if (this._imageNames.Count() > this._nowShowingPicture
                && !string.IsNullOrEmpty(this._imageNames[this._nowShowingPicture]))
            {
                PhoneApplicationService.Current.State["ImageBrowserSelected"] =
                    this._imageNames[this._nowShowingPicture];
                if (this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack();
                }
            }
        }

        /// <summary>
        /// The load current image.
        /// </summary>
        private void LoadCurrentImage()
        {
            if (this._imageNames.Count() > this._nowShowingPicture
                && !string.IsNullOrEmpty(this._imageNames[this._nowShowingPicture]))
            {
                // read from isolated storage.
                using (IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (
                        isolatedStorageRoot.FileExists(
                            App.WebDirIsolated + "/images/" + this._imageNames[this._nowShowingPicture]))
                    {
                        try
                        {
                            using (
                                IsolatedStorageFileStream fStream =
                                    isolatedStorageRoot.OpenFile(
                                        App.WebDirIsolated + "/images/" + this._imageNames[this._nowShowingPicture], 
                                        FileMode.Open))
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
                                this.ImagePane.Source = bitImage;
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

        #endregion
    }
}