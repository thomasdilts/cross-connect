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
