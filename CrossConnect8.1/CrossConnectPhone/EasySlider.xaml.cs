using System.Windows;
using System.Windows.Controls;

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

        public delegate void ValueChangedDelegate(object sender, RoutedPropertyChangedEventArgs<double> e);

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
                sliderTextSize.Value --;
            }
        }

        private void SliderTextSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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