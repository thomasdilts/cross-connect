using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
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
///
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///  
namespace CrossConnect
{
    /// <summary>
    /// ImageButton control
    /// </summary>
    public class ImageButton : Button
    {
        /// <summary>
        /// Initializes new instance of ImageButton class
        /// </summary>
        public ImageButton()
        {
            // Set template for the control
            DefaultStyleKey = typeof(ImageButton);
        }

        /// <summary>
        /// Normal State Image dependency property
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), null);

        /// <summary>
        /// Normal State Image property
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)this.GetValue(ImageProperty); }
            set { this.SetValue(ImageProperty, value); }

        }

        /// <summary>
        /// Pressed State Image dependency property
        /// </summary>
        public static readonly DependencyProperty PressedImageProperty = DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(ImageButton), null);

        /// <summary>
        /// Pressed State Image property
        /// </summary>
        public ImageSource PressedImage
        {
            get { return (ImageSource)this.GetValue(PressedImageProperty); }
            set { this.SetValue(PressedImageProperty, value); }

        }
        
    }
}
