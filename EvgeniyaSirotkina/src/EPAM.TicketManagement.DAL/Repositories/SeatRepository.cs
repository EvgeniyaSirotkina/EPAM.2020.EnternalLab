using System.Collections.Generic;
using System.Data.SqlClient;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

namespace EPAM.TicketManagement.DAL.Repositories
{
    internal class SeatRepository : IRepository<Seat>
    {
        private readonly string _connectionString;

        public SeatRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Seat item)
        {
            var queryString = "INSERT INTO [dbo].[Seat] VALUES(@areaId,@row,@number)";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@areaId", item.AreaId);
                    command.Parameters.AddWithValue("@row", item.Row);
                    command.Parameters.AddWithValue("@number", item.Number);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            var queryString = "DELETE FROM [dbo].[Seat] WHERE Id = @id";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Seat> GetAll()
        {
            var queryString = "SELECT [Id], [AreaId], [Row], [Number] " +
                                 "FROM [dbo].[Seat]";
            var seats = new List<Seat>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            seats.Add(new Seat
                            {
                                Id = (int)reader["Id"],
                                AreaId = (int)reader["AreaId"],
                                Row = (int)reader["Row"],
                                Number = (int)reader["Number"],
                            });
                        }
                    }
                }
            }

            return seats;
        }

        public Seat GetById(int id)
        {
            var queryString = "SELECT [Id], [AreaId], [Row], [Number] " +
                                 "FROM [dbo].[Seat] " +
                                 "WHERE Id = @id";
            Seat seat = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            seat = new Seat
                            {
                                Id = (int)reader["Id"],
                                AreaId = (int)reader["AreaId"],
                                Row = (int)reader["Row"],
                                Number = (int)reader["Number"],
                            };
                        }
                    }
                }
            }

            return seat;
        }

        public void Update(Seat item)
        {
            var queryString = "UPDATE [dbo].[Seat] SET " +
                                "[AreaId] = @areaId, " +
                                "[Row] = @row, " +
                                "[Number] = @number " +
                                "WHERE [Id] = @id";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@id", item.Id);
                    command.Parameters.AddWithValue("@areaId", item.AreaId);
                    command.Parameters.AddWithValue("@row", item.Row);
                    command.Parameters.AddWithValue("@number", item.Number);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
