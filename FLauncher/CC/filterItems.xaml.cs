using FLauncher.DAO;
using System.Windows;
using System.Windows.Controls;


namespace FLauncher.CC
{
    
    public partial class filterItems : UserControl
    {
        public filterItems()
        {
            InitializeComponent();

            var genres = GenreDAO.Instance.GetAllTypeGenres();
            ////var publisher = PublisherDAO.Instance.getAll(); 
            ItemListView.ItemsSource = genres;

            ItemListView.SelectionChanged += ItemListView_SelectionChanged;
        }
        // Sự kiện khi trạng thái CheckBox thay đổi
        public event Action<List<string>> selectedGenre;

        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selectedGenres = new List<string>();

            // Duyệt qua tất cả các item trong ListView
            foreach (var item in ItemListView.SelectedItems)
            {
                // Mỗi item là một genre, ta sẽ lấy nội dung của nó
                if (item != null)
                {
                    selectedGenres.Add(item.ToString());
                }
            }

            // Gửi danh sách các genre đã chọn
            selectedGenre?.Invoke(selectedGenres);

        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(filterItems));

        public bool IsMouseOver => ListItems.IsMouseOver;
    }
}
