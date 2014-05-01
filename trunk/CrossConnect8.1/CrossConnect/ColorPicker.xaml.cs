// <copyright file="ColorPicker.xaml.cs" company="Thomas Dilts">
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

    using Windows.UI;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;

    public partial class ColorPicker : UserControl
    {
        #region Constructors and Destructors

        public ColorPicker()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Delegates

        public delegate void ColorChanged(Color color);

        #endregion

        #region Public Events

        public event ColorChanged ColorChangedHolder;

        #endregion

        #region Public Properties

        public Color ColorPicked
        {
            get
            {
                byte R, G, B, A;

                A = Convert.ToByte(this.ASlider.Value);
                R = Convert.ToByte(this.RSlider.Value);
                G = Convert.ToByte(this.GSlider.Value);
                B = Convert.ToByte(this.BSlider.Value);

                return Color.FromArgb(A, R, G, B);
            }
            set
            {
                this.ASlider.Value = value.A;
                this.RSlider.Value = value.R;
                this.GSlider.Value = value.G;
                this.BSlider.Value = value.B;
            }
        }

        public string TextCaption
        {
            get
            {
                return this.Title.Text;
            }
            set
            {
                this.Title.Text = value;
            }
        }

        #endregion

        #region Methods

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            byte R, G, B, A;

            A = Convert.ToByte(this.ASlider.Value);
            R = Convert.ToByte(this.RSlider.Value);
            G = Convert.ToByte(this.GSlider.Value);
            B = Convert.ToByte(this.BSlider.Value);
            Color newColor = Color.FromArgb(A, R, G, B);
            this.sliderbackground.Background = new SolidColorBrush(newColor);
            if (this.ColorChangedHolder != null)
            {
                this.ColorChangedHolder(newColor);
            }
        }

        #endregion
    }
}