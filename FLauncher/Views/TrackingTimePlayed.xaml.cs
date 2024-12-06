using FLauncher.Model;
using FLauncher.Repositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace FLauncher.Views
{
    public partial class TrackingTimePlayed : Window
    {
        private Task<IEnumerable<TrackingRecords>> trackDatas;
        private readonly IGameRepository _gameRepo;
        private DispatcherTimer _fileReadTimer;
        public PlotModel PlotModel { get; set; }
        private DispatcherTimer _timer;
        private string _jsonFilePath = @"D:\GameTracking.json"; // Path to the JSON file
        private TimeSpan _timeStart; // TimeStart from JSON
        private TimeSpan _timeEnd;
        private double _timePlayed; // TimePlayed from JSON (in seconds)
        private double _timeOut = 1/60  ;
        private int _isPlaying = 0; // Tracks IsPlayed state
        private string _lastTimePlayed = "00:00:00"; // Store the last time played value
        private int counter = 0;


        public TrackingTimePlayed(Gamer gamer, Game game)
        {
            InitializeComponent();
            _gameRepo = new GameRepository();
            trackDatas =  _gameRepo.GetTrackingFromGamerGame(gamer, game);   

              // Initialize PlotModel

              PlotModel = new PlotModel { Title = "Game Time Tracking" };
            var lineSeries = new LineSeries
            {
                Title = "Tracking Line",
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2,
                Color = OxyColors.Blue
            };
            PlotModel.Series.Add(lineSeries);

            // Configure Y-axis Play Time in Minutes)
            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Play Time (Minutes)",
                Minimum = 0,
                Maximum = 24,
                MajorStep = 1
            });

            // Configure X-axis (Start Time (Hours))
            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                   Title = "Start Time (Hours)",
                Minimum = 0,
                Maximum = 60,
                MajorStep = 5
            });

            // Start file watcher
            StartFileReaderTimer();

            // Set DataContext for UI binding
            DataContext = this;
        }

        private void ReadTrackingDataFromFile()
        {
            try
            {
                if (File.Exists(_jsonFilePath))
                {
                    // Open and read the file every second
                    string jsonData = File.ReadAllText(_jsonFilePath);
                    var trackingTime = JsonConvert.DeserializeObject<TrackingTime>(jsonData);

                    if (trackingTime != null)
                    {
                        // Parse and update your fields from the deserialized object
                        if (!TimeSpan.TryParse(trackingTime.TimeStart, out _timeStart))
                        {
                            Console.WriteLine("Invalid TimeStart format.");
                            return;
                        }
                        if (!TimeSpan.TryParse(trackingTime.TimeEnd, out _timeEnd))
                        {
                            Console.WriteLine("Invalid TimeStart format.");
                            return;
                        }

                        // Set total play time in minutes
                        _timePlayed = trackingTime.TimePlayed / 60.0; // Convert seconds to minutes

                        // Set IsPlaying state
                        _isPlaying = trackingTime.IsPlayed;

                        // Update the chart with the historical line
                      
                        // Set the last known time played for later reference
                        _lastTimePlayed = trackingTime.TimeStart;
                        UpdateChartWithHistory();
                        // Start or stop the timer based on IsPlaying
                         StartTimer();
                          
                    }
                    else
                    {
                        Console.WriteLine("Error deserializing JSON data.");
                    }
                }
                else
                {
                    Console.WriteLine("Tracking file not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading the tracking data: {ex.Message}");
            }
        }

        private void UpdateChartWithHistory()
        {
            if (PlotModel.Series[0] is LineSeries lineSeries)
            {
             
                if (counter == 0)
                {

          
                    // Plot a horizontal line from (0, 0) to (start time in minutes, 0)
                    double startTimeInMinutes = _timeStart.TotalMinutes / 60; // Convert start time to hours
                    lineSeries.Points.Add(new DataPoint(0, 0)); // Start at (0, 0)
                    PrintOutRecordData();
                    var currentTime = DateTime.Now;
                    var timeSinceStart = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);
                    var totalSeconds = timeSinceStart.TotalSeconds/3600; // Or use TotalMinutes or TotalHours depending on your needs
                    lineSeries.Points.Add(new DataPoint(totalSeconds,0 )); // Plot total seconds
                    PlotModel.InvalidatePlot(true);

                }
                counter = 1;
            }
        }

        private async void PrintOutRecordData()
        {
            if (PlotModel.Series[0] is LineSeries lineSeries)
            {
                var records = await trackDatas;
               

                // Add each record to the line series
                foreach (var record in records)
                {
                    if (!string.IsNullOrWhiteSpace(record.TimeStart) && !string.IsNullOrWhiteSpace(record.TimeEnd))
                    {
                        if (TimeSpan.TryParse(record.TimeStart, out var startTime) && TimeSpan.TryParse(record.TimeEnd, out var endTime))
                        {
                            // Convert TimeStart to hours for Y-axis
                            double startTimeHours = startTime.TotalHours;
                            double timePlayedMinutes = record.TimePlayed / 60.0;
                            // Convert TimePlayed to minutes for X-axis
                            double endTimeHours = endTime.TotalHours;

                            // Add the data point to the line series
                            lineSeries.Points.Add(new DataPoint(startTimeHours,0 )); 
                            lineSeries.Points.Add(new DataPoint(endTimeHours, timePlayedMinutes));
                            lineSeries.Points.Add(new DataPoint(endTimeHours, 0));
                            PlotModel.InvalidatePlot(true);
                        }
                        else
                        {
                            MessageBox.Show($"Failed to parse TimeStart or TimeEnd: {record.TimeStart}, {record.TimeEnd}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Empty or invalid TimeStart or TimeEnd: {record.TimeStart}, {record.TimeEnd}");
                    }
                }
            }
        }


        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(0.85)
                };
                _timer.Tick += Timer_Tick;
            }
            _timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
             
            if (_isPlaying == 0)
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                TimeSpan tolerance = TimeSpan.FromSeconds(1);
                if (_timeEnd >= now - tolerance && _timeEnd <= now + tolerance)
                {
                    // Game is not playing, so we increment the line only in the Y-axis (start time)
                    _timeOut += 1 / 60.0; // Add 1 second (in minutes)
                                          // Recalculate the Y-Axis value (start time + time played)
                    var endTime = _timeEnd.Add(TimeSpan.FromMinutes(_timeOut));


                    // Update the chart dynamically (X = 0, Y = start time + elapsed time)
                    if (PlotModel.Series[0] is LineSeries lineSeries)
                    {
                        double endTimeHours = endTime.TotalHours; // Convert to hours for Y-axis
                        lineSeries.Points.Add(new DataPoint(endTimeHours, 0)); // X = 0 for no progress in time, Y = updated start time
                        PlotModel.InvalidatePlot(true); // Refresh the chart
                    }
                } else
                {
                    var currentTime = DateTime.Now.TimeOfDay;
                    if (PlotModel.Series[0] is LineSeries lineSeries)
                    {
                        double currentTimeHours = currentTime.TotalHours; // Convert to hours for Y-axis
                        lineSeries.Points.Add(new DataPoint(currentTimeHours, 0)); // X = 0 for no progress in time, Y = updated start time
                        PlotModel.InvalidatePlot(true); // Refresh the chart
                    }

                }
            }
            else
            {
                // Game is playing, increment the play time and update the chart
                _timePlayed += 1 / 60.0; // Add 1 second (in minutes)
                
                var currentStartTime = _timeStart.Add(TimeSpan.FromMinutes(_timePlayed));

                // Update the chart dynamically
                if (PlotModel.Series[0] is LineSeries lineSeries)
                {
                    double currentHours = currentStartTime.TotalHours; // Convert to hours for Y-axis
                    lineSeries.Points.Add(new DataPoint(currentHours, _timePlayed   )); // X = elapsed time, Y = current start time
                    PlotModel.InvalidatePlot(true); // Refresh the chart
                }
            }
        }



        private void StartFileReaderTimer()
        {
            // Initialize the timer to trigger every second
            _fileReadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05) 
            };

            // Event handler for the timer's Tick event
            _fileReadTimer.Tick += FileReadTimer_Tick;

            // Start the timer
            _fileReadTimer.Start();
        }

        // Timer tick event handler that will read the file every second
        private void FileReadTimer_Tick(object sender, EventArgs e)
        {
            ReadTrackingDataFromFile(); // This method will open the file, read, and deserialize
        }



    }


}

