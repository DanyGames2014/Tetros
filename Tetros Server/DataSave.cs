using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetros
{
    public static class DataSave
    {
        public enum SaveMethod{
            File = 0,
            Database = 1
        }

        /// <summary>
        /// Saves the userdata from Server
        /// </summary>
        /// <param name="server">Reference to the server to save data from</param>
        /// <returns>Returns true if save was succesful</returns>
        public static bool Save(Server server)
        {
            bool SavingEnabled;
            string SaveDirectory;
            SaveMethod SaveMethod;

            string userFileName = "users.ttr";
            string leaderboardFileName = "leaderboard.ttr";


            // Get values from App.config
            try
            {
                SavingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("UserdataStorageEnabled"));
                SaveDirectory = ConfigurationManager.AppSettings.Get("UserdataStorageDirectory").Trim().Replace(@"/","").Replace(@"\","");
                switch (ConfigurationManager.AppSettings.Get("UserdataStorageMethod"))
                {
                    case "File":
                        SaveMethod = SaveMethod.File;
                        break;

                    default:
                        break;
                }
                
            }
            catch (Exception e)
            {
                server.logger.WriteError("There was an Error parsing User Data config options.", e);
                SavingEnabled = true;
                SaveDirectory = "Userdata";
                SaveMethod = SaveMethod.File;
            }


            // Warn the server admin if saving is disabled as this may not be the desired behaviour.
            if (!SavingEnabled)
            {
                server.logger.WriteWarn("User Data Saving is disabled");
            }

            // Get The current working directory
            string currentDir = Directory.GetCurrentDirectory();
            string userdataDir = currentDir + Path.DirectorySeparatorChar + ConfigurationManager.AppSettings.Get("UserdataStorageDirectory") + Path.DirectorySeparatorChar;
            if (!Directory.Exists(userdataDir))
            {
                Directory.CreateDirectory(userdataDir);
            }

            // Write Users
            StreamWriter sw_user;
            try
            {
                sw_user = new StreamWriter(userdataDir + @"\" + userFileName);
                sw_user.AutoFlush = true;

                foreach (var item in server.userStorage.users)
                {
                    sw_user.Write(item.Value.username + ";");
                    var HashBytes = Encoding.UTF8.GetBytes(item.Value.passwordHash);
                    sw_user.Write(HashBytes[0]);
                    bool firstByte = true;
                    foreach (var itemHash in HashBytes)
                    {
                        if (!firstByte)
                        {
                            sw_user.Write(":" + itemHash);
                        }
                        else
                        {
                            firstByte = false;
                        }
                    }
                    sw_user.Write(";" + item.Value.passwordSalt + ";");
                    sw_user.Write(item.Value.level);
                    sw_user.WriteLine("");
                }

                sw_user.Close();
            }
            catch (Exception e)
            {
                server.logger.WriteError("There was an error creating the " + userFileName + " file", e);
            }

            // Write Leaderboard
            StreamWriter sw_leaderboard;
            try
            {
                sw_leaderboard = new StreamWriter(userdataDir + @"\" + leaderboardFileName);
                sw_leaderboard.AutoFlush = true;

                foreach (var item in server.leaderboard.scores)
                {
                    sw_leaderboard.WriteLine(item.user.username + ";" + item.score);
                }

                sw_leaderboard.Close();
            }
            catch (Exception e)
            {
                server.logger.WriteError("There was an error creating the " + leaderboardFileName + " file", e);
            }

            return true;
        }

        public static void Load(Server server)
        {
            bool SavingEnabled;
            string SaveDirectory;
            SaveMethod SaveMethod;

            string userFileName = "users.ttr";
            string leaderboardFileName = "leaderboard.ttr";

            // Get values from App.config
            try
            {
                SavingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("UserdataStorageEnabled"));
                SaveDirectory = ConfigurationManager.AppSettings.Get("UserdataStorageDirectory").Trim().Replace(@"/", "").Replace(@"\", "");
                switch (ConfigurationManager.AppSettings.Get("UserdataStorageMethod"))
                {
                    case "File":
                        SaveMethod = SaveMethod.File;
                        break;

                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                server.logger.WriteError("There was an Error parsing User Data config options.", e);
                SavingEnabled = true;
                SaveDirectory = "Userdata";
                SaveMethod = SaveMethod.File;
            }

            // Get The current working directory
            string currentDir = Directory.GetCurrentDirectory();
            string userdataDir = currentDir + Path.DirectorySeparatorChar + ConfigurationManager.AppSettings.Get("UserdataStorageDirectory") + Path.DirectorySeparatorChar;
            if (!Directory.Exists(userdataDir))
            {
                Directory.CreateDirectory(userdataDir);
            }

            // Load Users
            try
            {
                StreamReader sr_user = new(userdataDir + @"\" + userFileName);

                string[] usersRaw = sr_user.ReadToEnd().Trim().Split(Environment.NewLine);

                sr_user.Close();

                foreach (var item in usersRaw)
                {
                    if(item != String.Empty)
                    {
                        string[] userRaw = item.Trim().Split(";");
                        string username = userRaw[0];
                        string[] passHashRaw = userRaw[1].Split(":");
                        string passSalt = userRaw[2];
                        string levelT = userRaw[3];
                        AccessLevel accessLevel = AccessLevel.User;

                        if(levelT == "Admin")
                        {
                            accessLevel = AccessLevel.Admin;
                        }
                        else
                        {
                            accessLevel = AccessLevel.User;
                        }

                        byte[] passHashB = new byte[passHashRaw.Length];
                        for (int i = 0; i < passHashRaw.Length; i++)
                        {
                            passHashB[i] = Convert.ToByte(passHashRaw[i]);
                        }
                        string passHash = Encoding.UTF8.GetString(passHashB);

                        User tempUser = new User(username, passHash, passSalt, accessLevel);
                        server.userStorage.addUser(tempUser);
                    }
                }

                
                
            }
            catch (FileNotFoundException)
            {
                server.logger.WriteError(userFileName + " Not Found or Accesible. Creating a New One.");
            }
            catch (Exception ex)
            {
                server.logger.WriteError("There was an error parsing " + userFileName, ex);
            }



            // Load Leaderboards
            try
            {
                StreamReader sr_leaderboard = new(userdataDir + @"\" + leaderboardFileName);

                string[] leaderboardRaw = sr_leaderboard.ReadToEnd().Trim().Split(Environment.NewLine);

                sr_leaderboard.Close();

                foreach (var item in leaderboardRaw)
                {
                    if(item != String.Empty)
                    {
                        string[] entryRaw = item.Split(";");

                        server.leaderboard.submitScore(Convert.ToInt32(entryRaw[1]), entryRaw[0], server);
                    }

                }

                
            }
            catch (FileNotFoundException)
            {
                server.logger.WriteError(leaderboardFileName + " Not Found or Accesible. Creating a New One.");
            }
            catch(Exception ex)
            {
                server.logger.WriteError("There was an error parsing " + leaderboardFileName, ex);
            }
            finally
            {
            }


        }
    }
}
