using System.Windows;
using System.Windows.Controls;

namespace FLauncher.CC
{
    /// <summary>
    /// Interaction logic for filterItems.xaml
    /// </summary>
    public partial class filterItems : UserControl
    {
        public filterItems()
        {
            InitializeComponent();
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(filterItems));


    }
}
