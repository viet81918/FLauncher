using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.DAO;

namespace FLauncher.Views
{
    public partial class TrackingNumberPlayer : Window, INotifyPropertyChanged
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private System.Timers.Timer _timer;
        private LineSeries _series;
        private DateTime _startOfDay;
        public event PropertyChangedEventHandler PropertyChanged;
        private TrackingPlayers trackingPlayers;
        private DateTime _lastUpdateTime;
        private int _lastPlayerCount;
        private PlotModel _plotModel;
        private Game _game;

        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }

        public TrackingNumberPlayer(Game game)
        {
            InitializeComponent();
            DataContext = this;

   
            _game = game;

            // Start the async initialization process
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                TrackingDAO DAO = new TrackingDAO();
                trackingPlayers = await DAO.GetTrackingFromGame(_game);

                // Start time is set to midnight of the current day
                _startOfDay = DateTime.Today;

                // Initialize PlotModel
                PlotModel = new PlotModel { Title = "Player Tracking (Since Midnight)" };

                // Setup Axes
                var timeAxis = new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    StringFormat = "HH:mm:ss",
                    Title = "Time",
                    IntervalType = DateTimeIntervalType.Hours,
                    IsPanEnabled = true,
                    IsZoomEnabled = true,
                    Minimum = DateTimeAxis.ToDouble(_startOfDay),
                    Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddMinutes(1))
                };

                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Number of Players",
                    Minimum = 0
                };

                PlotModel.Axes.Add(timeAxis);
                PlotModel.Axes.Add(valueAxis);

                // Initialize Line Series
                _series = new LineSeries
                {
                    Title = "Players Over Time",
                    MarkerType = MarkerType.Circle
                };

                PlotModel.Series.Add(_series);

                // Load historical data asynchronously
                await LoadHistoricalDataAsync();

                // Setup Timer
                _timer = new System.Timers.Timer(1000); // Update every second
                _timer.Elapsed += UpdatePlot;
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing tracking player: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadHistoricalDataAsync()
        {
            // Initialize the last update time and player count
            _lastUpdateTime = _startOfDay;
            _lastPlayerCount = 0;

            // Load historical data from trackingPlayers.TimePlayerChange
            for (int i = 0; i < trackingPlayers.TimePlayerChange.Length; i++)
            {
                var time = trackingPlayers.TimePlayerChange[i];
                var playerCount = trackingPlayers.PlayerChange[i];

                if (time.Date == _startOfDay.Date) // Only include today's data
                {
                    if (i == 0)
                    {
                        // Initialize the first horizontal segment
                        _lastPlayerCount = playerCount;
                        _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_startOfDay), playerCount));
                    }

                    // Add vertical line if the player count changes
                    if (playerCount != _lastPlayerCount)
                    {
                        _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), _lastPlayerCount));
                        _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), playerCount));
                        _lastPlayerCount = playerCount;
                    }
                }

                // Always extend the horizontal line to the current time
                _lastUpdateTime = time;
                _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), playerCount));
            }

            // Refresh the plot after loading historical data
            PlotModel.InvalidatePlot(true);
        }

        private async void UpdatePlot(object sender, ElapsedEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                var currentTime = DateTime.Now;

                // Fetch the latest data asynchronously
                TrackingDAO DAO = new TrackingDAO();
                trackingPlayers = await DAO.GetTrackingFromGame(_game);

                var currentPlayerCount = trackingPlayers.CurrentPlayer;

                // Check if the player count has changed
                if (currentPlayerCount == _lastPlayerCount && currentTime == _lastUpdateTime)
                {
                    return; // No change, skip updating
                }

                // Clear the series if needed (optional but ensures consistency)
                if (_lastPlayerCount != currentPlayerCount)
                {
                    _series.Points.Clear();
                    await LoadHistoricalDataAsync(); // Reload full historical data
                }

                // Update the series with the latest point
                if (currentPlayerCount == _lastPlayerCount)
                {
                    _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(currentTime), _lastPlayerCount));
                }
                else
                {
                    _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(currentTime), _lastPlayerCount)); // Vertical line start
                    _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(currentTime), currentPlayerCount)); // Vertical line end
                    _lastPlayerCount = currentPlayerCount;
                }

                // Update the last update time
                _lastUpdateTime = currentTime;

                // Adjust axis range
                if (PlotModel.Axes[0] is DateTimeAxis timeAxis)
                {
                    timeAxis.Maximum = DateTimeAxis.ToDouble(currentTime.AddMinutes(1));
                    timeAxis.Minimum = DateTimeAxis.ToDouble(_startOfDay);
                }

                // Refresh the plot
                PlotModel.InvalidatePlot(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating plot: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
