// <copyright file="BadgeHelper.cs" company="Thomas Dilts">
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
    using Windows.Data.Xml.Dom;
    using Windows.UI.Notifications;

    public enum Glyphs
    {
        none,

        activity,

        alert,

        available,

        away,

        busy,

        newMessage,

        paused,

        playing,

        unavailable,

        error,

        attention,
    }

    public class BadgeHelper
    {
        #region Public Methods and Operators

        public static void UpdateBadge(Glyphs glyph)
        {
            // Get badge xml content 
            XmlDocument content = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);
            IXmlNode element = content.GetElementsByTagName("badge")[0];

            // Set new glyph value
            element.Attributes[0].NodeValue = glyph.ToString();

            // Update badge
            var notification = new BadgeNotification(content);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(notification);
        }

        #endregion
    }
}