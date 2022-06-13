using System;
using System.Collections.Generic;

namespace Tetros
{

    /// <summary>
    /// Struct to represent a score as it would be wasteful to represent that as an object.
    /// </summary>
    public struct Score : IComparable<Score>
    {
        public int score { get; set; }
        public User user { get; set; }

        public Score(int score, User user)
        {
            this.score = score;
            this.user = user;
        }

        public int CompareTo(Score other)
        {
            if(other.score > this.score)
            {
                return 1;
            }
            else if (other.score < this.score)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Class used to keep track of scores
    /// </summary>
    public class Leaderboard
    {
        // List of Scores
        public List<Score> scores;

        // Constructor
        public Leaderboard()
        {
            scores = new List<Score>();
        }

        // Submits a new score for the defined user
        public bool submitScore(int score, User user)
        {
            lock (scores)
            {
                Score toadd = new Score(score, user);
                scores.Add(toadd);
                return true;
            }
            
        }

        // Submits a new score with the defined username, searches for the user in the userStorage of the referenced Server instance.
        public bool submitScore(int score, string username, Server server)
        {
            lock (scores)
            {
                try
                {
                    Score toadd = new Score(score, server.userStorage.getUser(username));
                    scores.Add(toadd);
                }
                catch (Exception)
                {
                    return false;
                }
                
                return true;
            }
        }

        // Returns a sorted list of scores
        public List<Score> getSortedScores()
        {
            lock (scores)
            {
                List<Score> sortedScores = new List<Score>();
                sortedScores = scores;
                sortedScores.Sort();
                return sortedScores;
            }
        }
    }
}
