using FLauncher.Repositories;
using System;
using System.Diagnostics;

namespace FLauncher.Utilities
{
    public static class SessionManager
    {
        public static string LoggedInGamerId { get; private set; }

        // Initialize session
        public static void InitializeSession(Model.User user, IGamerRepository gamerRepo)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            if (string.IsNullOrEmpty(LoggedInGamerId))
            {
                var loggedInGamer = gamerRepo.GetGamerByUser(user);

                if (loggedInGamer == null)
                {
                    throw new InvalidOperationException("Gamer not found for the given user.");
                }

                LoggedInGamerId = loggedInGamer.GamerId;
            }
        }

        // Reset the session (e.g., for logging out)
        public static void ClearSession()
        {
            LoggedInGamerId = null;
            Debug.WriteLine("Session cleared."); // Add this for debugging
        }

    }
}
