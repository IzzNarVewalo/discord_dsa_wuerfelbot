using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _04_dsa.Modules;
using Npgsql;

namespace _04_dsa.Services {
    public class DBService {
        private string connString;

        private void LoadConnection() {
            connString = "Host=" + Helper.PostgresData.Address +
                ";Username=" + Helper.PostgresData.Username +
                ";Password=" + Helper.PostgresData.Password +
                ";Database=" + Helper.PostgresData.Database;
        }

        public async Task<List<string>> GetHeldenListe() {
            if (string.IsNullOrEmpty(connString))
                LoadConnection();
            List<string> retval = new List<string>();
            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand($"SELECT h.helden_name FROM helden AS h", conn)) {
                    cmd.Prepare();
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            retval.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return retval;


        }

        public async Task<string> GetHeldenName(ulong discordID) {
            if (string.IsNullOrEmpty(connString))
                LoadConnection();
            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand($"SELECT h.helden_name FROM helden AS h, link AS l WHERE l.helden_id = h.helden_id AND l.discord_id={discordID}", conn)) {
                    cmd.Prepare();
                    using (var reader = await cmd.ExecuteReaderAsync()) {
                        {
                            await reader.ReadAsync();
                            return reader.GetString(0);
                        }
                    }
                }
            }
        }

        public async Task<Dictionary<int, string>> GetHeldenIDUndName(string heldenname) {
            if (string.IsNullOrEmpty(connString))
                LoadConnection();
            var retval = new Dictionary<int, string>();
            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand($"SELECT h.helden_id, h.helden_name FROM helden AS h WHERE (lower(h.helden_name) LIKE '{heldenname}%')", conn)) {
                    cmd.Prepare();
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            retval.Add(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }
            }
            return retval;
        }

        public async Task<KeyValuePair<int, string>> GetHeldenID(ulong discordID) {
            var retval = new KeyValuePair<int, string>();
            if (string.IsNullOrEmpty(connString))
                LoadConnection();
            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();
                // Retrieve all rows
                using (var cmd = new NpgsqlCommand($"SELECT h.helden_id, h.helden_name FROM link AS l, helden as h WHERE l.helden_id = h.helden_id AND l.discord_id = {discordID}", conn)) {
                    cmd.Prepare();
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            retval = new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1));
                        }
                        return retval;
                    }
                }
            }
        }

        public async Task<string> GetHeldenXML(int heldenID) {
            if (string.IsNullOrEmpty(connString))
                LoadConnection();
            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand($"SELECT helden_xml FROM helden WHERE helden_id = {heldenID}", conn)) {
                    cmd.Prepare();
                    using (var reader = cmd.ExecuteReader()) {
                        {
                            while (reader.Read()) {
                                return reader.GetString(0);
                            }
                        }
                    }
                }
            }
            return "";
        }

        public async Task<int> UpdateDsaLink(ulong discordID, int heroID) {
            int retval;
            if (string.IsNullOrEmpty(connString))
                LoadConnection();
            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand()) {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO link (discord_id, helden_id) VALUES (@d, @h) ON CONFLICT (discord_id) DO UPDATE SET helden_id=@h;";
                    cmd.Parameters.AddWithValue("h", heroID);
                    cmd.Parameters.Add(new NpgsqlParameter {
                        ParameterName = "d",
                        //THIS WORKAROUND IS NOT OK
                        Value = (Int64)discordID,
                        DataTypeName = "bigint"
                    });
                    await cmd.PrepareAsync();
                    retval = await cmd.ExecuteNonQueryAsync();
                }
                conn.Close();
            }
            return retval;
        }

        public async Task<int> UpdateHeroXml(int id, string xml) {
            int retval;
            if (string.IsNullOrEmpty(connString))
                LoadConnection();

            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand()) {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO helden (helden_id, helden_xml) VALUES (@i, @x) ON CONFLICT (helden_id) DO UPDATE SET helden_xml=@x;";
                    cmd.Parameters.AddWithValue("i", id);
                    cmd.Parameters.AddWithValue("x", xml);
                    await cmd.PrepareAsync();
                    retval = await cmd.ExecuteNonQueryAsync();
                }
                conn.Close();
            }
            return retval;
        }

        public async Task<int> UpdateHeroName(int id, string name) {
            int retval;
            if (string.IsNullOrEmpty(connString))
                LoadConnection();

            using (var conn = new NpgsqlConnection(connString)) {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand()) {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO helden (helden_id, helden_name) VALUES (@i, @n) ON CONFLICT (helden_id) DO UPDATE SET helden_name=@n;";
                    cmd.Parameters.AddWithValue("i", id);
                    cmd.Parameters.AddWithValue("n", name);
                    await cmd.PrepareAsync();
                    retval = await cmd.ExecuteNonQueryAsync();
                }
                conn.Close();
            }
            return retval;
        }

    }
}
