using System;
using System.Collections.Generic;

namespace Tetros
{

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

    public class Leaderboard
    {
        public List<Score> scores;

        public Leaderboard()
        {
            scores = new List<Score>();
        }

        public bool submitScore(int score, User user)
        {
            lock (scores)
            {
                Score toadd = new Score(score, user);
                scores.Add(toadd);
                return true;
            }
            
        }

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
