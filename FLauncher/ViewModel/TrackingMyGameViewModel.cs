using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class TrackingMyGameViewModel
    {
        public string GameName { get; set; }  // Name of the game
        public string GameImage { get; set; }
        public double TotalPlayingHours { get; set; }  // Total hours played for the game
        public string LastPlayedDate { get; set; }  // Date when the game was last played
        
    }

}
