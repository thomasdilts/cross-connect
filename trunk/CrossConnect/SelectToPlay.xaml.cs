/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="SelectToPlay.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;

    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    public partial class SelectToPlay : AutoRotatePage
    {
        #region Fields

        private WebClient client = new WebClient();
        private bool isInSelectionChanged = false;

        #endregion Fields

        #region Constructors

        public SelectToPlay()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //nothing to do here
        }

        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                // for debug
                //byte[] buffer=new byte[e.Result.Length];
                //e.Result.Read(buffer, 0, (int)e.Result.Length);
                //System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));

                using (XmlReader reader = XmlReader.Create(e.Result))
                {
                    string src = string.Empty;
                    string name = string.Empty;
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name.ToLower().Equals("source") && reader.HasAttributes)
                                {
                                    name = string.Empty;
                                    src = string.Empty;
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        switch (reader.Name.ToLower())
                                        {
                                            case "src":
                                                src = reader.Value;
                                                break;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());
                                }
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.ToLower().Equals("source") && !string.IsNullOrEmpty(src) && !string.IsNullOrEmpty(name))
                                {
                                    TextBlock block = new TextBlock();
                                    block.Text = name;
                                    block.Style = PageTitle.Style;
                                    block.Tag = src;
                                    block.Margin = new Thickness(0,0,0,10);
                                    block.TextWrapping = TextWrapping.Wrap;
                                    SelectList.Items.Add(block);
                                }
                                break;
                            case XmlNodeType.Text:
                                name += reader.Value;
                                break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Fail(e.ToString());
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + exp.Message);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Select what you want to hear");
            SelectList.Items.Clear();
            //do a download.
            object chapterToHear;
            if (PhoneApplicationService.Current.State.TryGetValue("ChapterToHear", out chapterToHear))
            {
                string url = string.Format(App.displaySettings.soundLink, (int)chapterToHear);
                try
                {
                    Uri source = new Uri(url);

                    client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                    Logger.Debug("download start");
                    client.OpenReadAsync(source);
                    Logger.Debug("DownloadStringAsync returned");
                }
                catch (Exception eee)
                {
                    Logger.Fail(eee.ToString());
                    MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + eee.Message);
                }
            }
            else
            {
            }
        }

        private void SelectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInSelectionChanged)
            {
                return;
            }
            isInSelectionChanged = true;
            string address = (string)((TextBlock)e.AddedItems[0]).Tag;
            MediaPlayerLauncher mediaPlayerLauncher =
                new MediaPlayerLauncher()
            {
                Media = new Uri(address, UriKind.Absolute)
            };
            mediaPlayerLauncher.Show();
            isInSelectionChanged = false;
        }

        #endregion Methods
    }
}