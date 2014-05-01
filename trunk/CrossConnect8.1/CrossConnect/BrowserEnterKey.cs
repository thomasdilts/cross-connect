// <copyright file="BrowserEnterKey.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Sword.reader;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.Storage;
    using Windows.System;
    using Windows.UI.Xaml;

    public sealed partial class BrowserTitledWindow
    {
        #region Methods

        private void EnterKeyPopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void EnterKeyPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private async void SaveKey_OnClick(object sender, RoutedEventArgs e)
        {
            bool isGood = await ((BibleZtextReader)this.State.Source).IsCipherKeyGood(this.EnteredKeyText.Text);
            if (isGood)
            {
                ((BibleZtextReader)this.State.Source).Serial.CipherKey = this.EnteredKeyText.Text;
                string filenameComplete = ((BibleZtextReader)this.State.Source).Serial.Path + "CipherKey.txt";
                var fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(filenameComplete.Replace("/", "\\"),CreationCollisionOption.ReplaceExisting);
                fs.Write(Encoding.UTF8.GetBytes(this.EnteredKeyText.Text),0,this.EnteredKeyText.Text.Length);
                fs.Flush();
                this.ForceReload = true;
                this.UpdateBrowser(false);
                this.EnterKeyPopup.IsOpen = false;
            }
            else
            {
                this.ErrorEnterKeyText.Text = Translations.Translate("Invalid key. Try again"); ;
                
            }
        }
        private async void EnterKey_OnClick()
        {
            this.EnterKeyPopup.IsOpen = true;
            MainPageSplit.SideBarShowPopup(this.EnterKeyPopup, this.MainPaneEnterKeyPopup);
            this.ErrorEnterKeyText.Text = Translations.Translate("Enter key"); ;
            this.EnteredKeyText.Text = string.Empty;
            this.SaveKey.Content = Translations.Translate("Save");
            this.EnteredKeyText.Focus(FocusState.Programmatic);
        }



        #endregion
    }
}