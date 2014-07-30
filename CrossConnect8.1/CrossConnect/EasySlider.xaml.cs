using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CrossConnect
{
    public partial class EasySlider : UserControl
    {
        public double SliderValue
        {
            get
            {
                return this.sliderTextSize.Value;
            }
            set
            {
                sliderTextSize.Value = value;
            }
        }

        public delegate void ValueChangedDelegate(object sender, RangeBaseValueChangedEventArgs e);

        public ValueChangedDelegate ValueChanged;

        public EasySlider()
        {
            InitializeComponent();
        }

        private void ButMore_Click(object sender, RoutedEventArgs e)
        {
            sliderTextSize.Value = (int)sliderTextSize.Value;
            if (sliderTextSize.Value < sliderTextSize.Maximum)
            {
                sliderTextSize.Value++;
            }
        }

        private void ButLess_Click(object sender, RoutedEventArgs e)
        {
            sliderTextSize.Value = (int)sliderTextSize.Value;
            if (sliderTextSize.Value > sliderTextSize.Minimum)
            {
                sliderTextSize.Value--;
            }
        }

        private void SliderTextSizeValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (sliderTextSize != null)
            {
                sliderTextSize.Value = (int)sliderTextSize.Value;
                this.SliderNumber.Text = sliderTextSize.Value.ToString();
                if (ValueChanged != null)
                {
                    ValueChanged(sender, e);
                }
            }
        }
    }

}
