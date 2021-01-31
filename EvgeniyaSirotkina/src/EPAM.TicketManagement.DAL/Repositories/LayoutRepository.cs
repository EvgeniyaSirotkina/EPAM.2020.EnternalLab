using System.Collections.Generic;
using System.Data.SqlClient;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

namespace EPAM.TicketManagement.DAL.Repositories
{
    internal class LayoutRepository : IRepository<VenueLayout>
    {
        private readonly string _connectionString;

        public LayoutRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(VenueLayout item)
        {
            var queryString = "INSERT INTO [dbo].[Layout] VALUES(@venueId,@description)";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@venueId", item.VenueId);
                    command.Parameters.AddWithValue("@description", item.Description);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            var queryString = "DELETE FROM [dbo].[Layout] WHERE Id = @id";

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

        public IEnumerable<VenueLayout> GetAll()
        {
            var queryString = "SELECT [Id], [VenueId], [Description] " +
                                 "FROM [dbo].[Layout]";
            var layouts = new List<VenueLayout>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            layouts.Add(new VenueLayout
                            {
                                Id = (int)reader["Id"],
                                VenueId = (int)reader["VenueId"],
                                Description = reader["Description"].ToString(),
                            });
                        }
                    }
                }
            }

            return layouts;
        }

        public VenueLayout GetById(int id)
        {
            var queryString = "SELECT [Id], [VenueId], [Description] " +
                                 "FROM [dbo].[Layout] " +
                                 "WHERE Id = @id";
            VenueLayout layout = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            layout = new VenueLayout
                            {
                                Id = (int)reader["Id"],
                                VenueId = (int)reader["VenueId"],
                                Description = reader["Description"].ToString(),
                            };
                        }
                    }
                }
            }

            return layout;
        }

        public void Update(VenueLayout item)
        {
            var queryString = "UPDATE [dbo].[Layout] SET " +
                                "[VenueId] = @venueId, " +
                                "[Description] = @description, " +
                                "WHERE [Id] = @id";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@id", item.Id);
                    command.Parameters.AddWithValue("@venueId", item.VenueId);
                    command.Parameters.AddWithValue("@description", item.Description);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
