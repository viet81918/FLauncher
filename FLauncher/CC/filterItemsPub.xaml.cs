using FLauncher.DAO;
using System.Windows;
using System.Windows.Controls;


namespace FLauncher.CC
{
    
    public partial class filterItemsPub : UserControl
    {
        public filterItemsPub()
        {
            InitializeComponent();
            var publisher = PublisherDAO.Instance.getAll();
            ItemListViewPublisher.ItemsSource = publisher;

            ItemListViewPublisher.SelectionChanged += ItemListViewPublisher_SelectionChanged;
        }
        public event Action<string> selectedPub;
        private void ItemListViewPublisher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemListViewPublisher.SelectedItem != null)
            {
                // Kích hoạt sự kiện GenreSelected và gửi genre được chọn
                selectedPub?.Invoke(ItemListViewPublisher.SelectedItem.ToString());
                Console.WriteLine($"Selected genre: {ItemListViewPublisher.SelectedItem.ToString()}");
            }
            else
            {
                MessageBox.Show("CHUA CHON");
            }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(filterItemsPub));
    }
}
