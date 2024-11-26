﻿using System;
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

namespace FLauncher.Views
{
    /// <summary>
    /// Interaction logic for TrackingNumberPlayer.xaml
    /// </summary>
    public partial class TrackingNumberPlayer : Window, INotifyPropertyChanged
    {
        private System.Timers.Timer _timer;
        private LineSeries _series;
        private DateTime _startOfDay;
        private IGameRepository _gameRepo;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly TrackingPlayers trackingPlayers;
        private PlotModel _plotModel;
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

            _gameRepo = new GameRepository();
            trackingPlayers = _gameRepo.GetTrackingFromGame(game).Result;

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

            // Load historical data
            LoadHistoricalData();

            // Setup Timer
            _timer = new System.Timers.Timer(1000); // Update every second
            _timer.Elapsed += UpdatePlot;
            _timer.Start();
        }

        private void LoadHistoricalData()
        {
            // Load historical data from trackingPlayers.TimePlayerChange
            for (int i = 0; i < trackingPlayers.TimePlayerChange.Length; i++)
            {
                var time = trackingPlayers.TimePlayerChange[i];
                var playerCount = trackingPlayers.PlayerChange[i];
                if (time.Date == _startOfDay.Date) // Only include today's data
                {
                    _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), playerCount));
                }
            }

            // Refresh the plot after loading historical data
            PlotModel.InvalidatePlot(true);
        }

        private void UpdatePlot(object sender, ElapsedEventArgs e)
        {
            var currentTime = DateTime.Now;

            // Update with real-time data
            var players = trackingPlayers.CurrentPlayer; // Replace with live data source

            // Add a new data point
            _series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(currentTime), players));

            // Adjust axis range
            var timeAxis = PlotModel.Axes[0] as DateTimeAxis;

            if (timeAxis != null)
            {
                timeAxis.Maximum = DateTimeAxis.ToDouble(currentTime.AddMinutes(1));
                timeAxis.Minimum = DateTimeAxis.ToDouble(_startOfDay);
            }

            // Refresh the plot
            PlotModel.InvalidatePlot(true);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }


}
