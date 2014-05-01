#region Header

// <copyright file="ImageButton.cs" company="Thomas Dilts">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ImageButton : Button
    {
        #region Fields

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(ImageSource), typeof(ImageButton), null);
        public static readonly DependencyProperty PressedImageProperty = DependencyProperty.Register(
            "PressedImage", typeof(ImageSource), typeof(ImageButton), null);

        #endregion Fields

        #region Constructors

        public ImageButton()
        {
            // Set template for the control
            DefaultStyleKey = typeof(ImageButton);
        }

        #endregion Constructors

        #region Properties

        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }

            set
            {
                SetValue(ImageProperty, value);
            }
        }

        public ImageSource PressedImage
        {
            get
            {
                return (ImageSource)GetValue(PressedImageProperty);
            }

            set
            {
                SetValue(PressedImageProperty, value);
            }
        }

        #endregion Properties
    }
}