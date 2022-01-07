using MySql.Data.MySqlClient;

namespace DiscordBotCS.DatabaseInteraction
{
    public class PlayTimeRecorder
    {
        private static void LogQuery(MySqlCommand command)
        {
            string commandText = command.CommandText;
            foreach (MySqlParameter parameter in command.Parameters)
            {
                commandText = commandText.Replace(parameter.ParameterName, parameter.Value.ToString());
            }
            Console.WriteLine(commandText);
        }

        // TODO: USE ENTITY FRAMEWORK

        private static void DeleteActiveGame(ulong id, ulong serverID, MySqlConnection connection)
        {
            var deleteCommand = "delete from active_games where user_id = @id and server_id = @server_id";
            var command = new MySqlCommand(deleteCommand, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@server_id", serverID);
            LogQuery(command);
            command.ExecuteNonQuery();
        }

        private static void CreateActiveGame(ulong id, ulong serverID, string gameTitle, MySqlConnection connection)
        {
            var now = DateTime.Now;
            string insertCommand = "insert into active_games (user_id, server_id, game_title, time_started) values (@id, @server_id, @title, @now)";
            var command = new MySqlCommand(insertCommand, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@server_id", serverID);
            command.Parameters.AddWithValue("@title", gameTitle);
            command.Parameters.AddWithValue("@now", now);
            LogQuery(command);
            command.ExecuteNonQuery();
        }

        private static MySqlDataReader GetActiveGame(ulong id, ulong serverID, string gameTitle, MySqlConnection connection)
        {
            var query = "select user_id, game_title, time_started from active_games where user_id = @id and server_id = @server_id and game_title = @title order by time_started desc";
            var dbCommand = new MySqlCommand(query, connection);
            dbCommand.Parameters.AddWithValue("@id", id);
            dbCommand.Parameters.AddWithValue("@server_id", serverID);
            dbCommand.Parameters.AddWithValue("@title", gameTitle);
            LogQuery(dbCommand);
            return dbCommand.ExecuteReader();
        }

        private static bool TotalGameTimeExists(ulong id, ulong serverID, string gameTitle, MySqlConnection connection)
        {
            var query = "select count(*) from in_game_time where user_id = @id and server_id = @server_id and game_title = @title";
            var dbCommand = new MySqlCommand(query, connection);
            dbCommand.Parameters.AddWithValue("@id", id);
            dbCommand.Parameters.AddWithValue("@server_id", serverID);
            dbCommand.Parameters.AddWithValue("@title", gameTitle);
            LogQuery(dbCommand);
            var count = (Int64)dbCommand.ExecuteScalar();
            return count > 0;
        }

        private static void CreateGameTimeRecord(ulong id, ulong serverID, string gameTitle, int minutesPlayed, MySqlConnection connection)
        {
            var query = "insert into in_game_time (game_title, user_id, server_id, minutes) values (@title, @id, @server_id, @minPlayed)";
            var dbCommand = new MySqlCommand(query, connection);
            dbCommand.Parameters.AddWithValue("@minPlayed", minutesPlayed);
            dbCommand.Parameters.AddWithValue("@id", id);
            dbCommand.Parameters.AddWithValue("@server_id", serverID);
            dbCommand.Parameters.AddWithValue("@title", gameTitle);
            LogQuery(dbCommand);
            dbCommand.ExecuteNonQuery();
        }

        private static void UpdateGameTimeRecord(ulong id, ulong serverID, string gameTitle, int minutesPlayed, MySqlConnection connection)
        {
            var query = "update in_game_time set minutes = minutes + @minPlayed where user_id = @id and server_id = @server_id and game_title = @title";
            var dbCommand = new MySqlCommand(query, connection);
            dbCommand.Parameters.AddWithValue("@minPlayed", minutesPlayed);
            dbCommand.Parameters.AddWithValue("@id", id);
            dbCommand.Parameters.AddWithValue("@server_id", serverID);
            dbCommand.Parameters.AddWithValue("@title", gameTitle);
            LogQuery(dbCommand);
            dbCommand.ExecuteNonQuery();
        }

        private static bool UserExists(ulong id, MySqlConnection connection)
        {
            var query = "select count(*) from user_header where user_id = @id";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            LogQuery(command);
            var result = (Int64)command.ExecuteScalar();
            if (result > 0)
                return true;
            return false;
        }

        private static void CreateUser(ulong id, string username, MySqlConnection connection)
        {
            var query = "insert into user_header (user_id, user_name) values (@id, @username)";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@username", username);
            LogQuery(command);
            command.ExecuteNonQuery();
        }

        private static void UpdateUserName(ulong id, string username, MySqlConnection connection)
        {
            var query = "update user_header set user_name = @username where user_id = @id";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@username", username);
            LogQuery(command);
            command.ExecuteNonQuery();
        }

        private static bool ServerExists(ulong id, MySqlConnection connection)
        {
            var query = "select count(*) from server_header where server_id = @id";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            LogQuery(command);
            var result = (Int64)command.ExecuteScalar();
            return result > 0;
        }

        private static void UpdateServer(ulong id, string serverName, MySqlConnection connection)
        {
            var query = "update server_header set server_name = @name where server_id = @id";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", serverName);
            command.Parameters.AddWithValue("@id", id);
            LogQuery(command);
            command.ExecuteNonQuery();
        }

        private static void CreateServer(ulong id, string name, MySqlConnection connection)
        {
            var query = "insert into server_header (server_id, server_name) values (@id, @name)";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);
            LogQuery(command);
            command.ExecuteNonQuery();
        }

        private struct ActiveGame
        {
            public ulong Id;
            public string Name;
            public DateTime TimeStarted;
        }

        public static async Task UpdatePlayTime(ulong userID, string username, ulong serverID, string serverName, string? before, string? after)
        {
            await Task.Run(() =>
            {
                var ConnectionString = DiscordBot.Credentials.ConnectionString;
                using var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                // Add / update user_header
                if (UserExists(userID, connection))
                {
                    UpdateUserName(userID, username, connection);
                }
                else
                {
                    CreateUser(userID, username, connection);
                }

                // Add / update server_header
                if (ServerExists(serverID, connection))
                {
                    UpdateServer(serverID, serverName, connection);
                }
                else
                {
                    CreateServer(serverID, serverName, connection);
                }

                // User started playing a game
                if (before == null && after != null)
                {
                    // An active game should not exist, but if it does, delete it
                    DeleteActiveGame(userID, serverID, connection);
                    CreateActiveGame(userID, serverID, after.Substring(0, 30 - (30 - after.Length)), connection);
                }
                // User stopped playing a game
                else if (before != null && after == null)
                {
                    string gameTitle = before.Substring(0, 30 - (30 - before.Length));
                    List<ActiveGame> activeGames = new List<ActiveGame>();
                    using (var activeGamesReader = GetActiveGame(userID, serverID, gameTitle, connection))
                    {
                        if (!activeGamesReader.HasRows)
                        {
                            // User stopped playing a game that bot did not see starting, ignore
                            connection.Close();
                            return;
                        }

                        while (activeGamesReader.Read())
                        {
                            DateTime timeStarted = activeGamesReader.GetDateTime(2);
                            activeGames.Add(new ActiveGame { Id = userID, Name = gameTitle, TimeStarted = timeStarted });
                        }
                    }
                    foreach (var activeGame in activeGames)
                    {
                        var timeEnded = DateTime.Now;

                        var minutesPlayed = (int)((timeEnded - activeGame.TimeStarted).TotalMinutes);

                        // Check if there is a record for this user and game, if not, create it
                        var recordExists = TotalGameTimeExists(userID, serverID, gameTitle, connection);

                        if (!recordExists)
                            CreateGameTimeRecord(userID, serverID, gameTitle, minutesPlayed, connection);
                        else
                            UpdateGameTimeRecord(userID, serverID, gameTitle, minutesPlayed, connection);

                        DeleteActiveGame(userID, serverID, connection);
                    }

                }
                else if (before != null && after != null)
                {
                    if (before == after)
                    {
                        // The update is triggered by something other than game status update, ignore this
                        connection.Close();
                        return;
                    }

                    var beforeGameTitle = before.Substring(0, 30 - (30 - before.Length));
                    var afterGameTitle = after.Substring(0, 30 - (30 - after.Length));

                    // User switched games
                    {   // Update game that was being played before
                        List<ActiveGame> activeGames = new List<ActiveGame>();
                        using (var activeGameReader = GetActiveGame(userID, serverID, beforeGameTitle, connection))
                        {
                            while (activeGameReader.Read())
                            {
                                DateTime timeStarted = activeGameReader.GetDateTime(2);
                                activeGames.Add(new ActiveGame { Id = userID, Name = beforeGameTitle, TimeStarted = timeStarted });
                            }
                        }
                        foreach (var activeGame in activeGames)
                        {
                            var timeEnded = DateTime.Now;
                            var minutesPlayed = (int)((timeEnded - activeGame.TimeStarted).TotalMinutes);

                            // Check if there is a record for this user and game, if not, create it
                            var recordExists = TotalGameTimeExists(userID, serverID, beforeGameTitle, connection);

                            if (!recordExists)
                                CreateGameTimeRecord(userID, serverID, beforeGameTitle, minutesPlayed, connection);
                            else
                                UpdateGameTimeRecord(userID, serverID, beforeGameTitle, minutesPlayed, connection);
                        }                        
                    }

                    {   // Create new active game
                        DeleteActiveGame(userID, serverID, connection);
                        CreateActiveGame(userID, serverID, afterGameTitle, connection);
                    }
                }
                // The update is triggered by something other than game status update, ignore this
                else if (before == null && after == null)
                {
                    connection.Close();
                    return;
                }

                connection.Close();
            });
        }

        public static async Task<List<(string name, int minutesPlayed)>> GetPlayTimeWrappedAsync(ulong id, ulong serverID, int limit = 5)
        {
            var ConnectionString = DiscordBot.Credentials.ConnectionString;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var query = "select game_title, minutes from in_game_time where user_id = @id and server_id = @server_id order by minutes desc limit @limit";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@server_id", serverID);
                command.Parameters.AddWithValue("@limit", limit);
                LogQuery(command);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows)
                    {
                        return new List<(string name, int minutesPlayed)>();
                    }

                    List<(string name, int minutesPlayed)> result = new List<(string name, int minutesPlayed)>();
                    while (reader.Read())
                    {
                        result.Add((reader.GetString(0), reader.GetInt32(1)));
                    }
                    return result;
                }
            }
        }

        public static async Task<List<(string game, int minutes, int numPlayers)>> GetPopularGames(ulong serverID, int limit = 5)
        {
            var ConnectionString = DiscordBot.Credentials.ConnectionString;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "select Game, `Minutes Played`, `Number of Players` from v_popular_games where `Server ID` = @server_id limit @limit";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@server_id", serverID);
                command.Parameters.AddWithValue("@limit", limit);
                LogQuery(command);

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No Rows");
                            return new List<(string game, int minutes, int numPlayers)> { };
                        }

                        var result = new List<(string name, int minutes, int numPlayers)>();
                        while (reader.Read())
                        {
                            result.Add((reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2)));
                        }
                        Console.WriteLine($"result contained {result.Count} games");
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return new List<(string game, int minutes, int numPlayers)> { };
                }
            }
        }
    }
}
