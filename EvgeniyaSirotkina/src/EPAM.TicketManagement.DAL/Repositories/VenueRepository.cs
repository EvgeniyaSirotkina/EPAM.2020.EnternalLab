using System.Collections.Generic;
using System.Data.SqlClient;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

namespace EPAM.TicketManagement.DAL.Repositories
{
    internal class VenueRepository : IRepository<Venue>
    {
        private readonly string _connectionString;

        public VenueRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Venue item)
        {
            var queryString = "INSERT INTO [dbo].[Venue] VALUES(@name,@description,@adress,@phone)";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@name", item.Name);
                    command.Parameters.AddWithValue("@description", item.Description);
                    command.Parameters.AddWithValue("@adress", item.Address);
                    command.Parameters.AddWithValue("@phone", item.Phone);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            var queryString = "DELETE FROM [dbo].[Venue] WHERE Id = @id";

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

        public IEnumerable<Venue> GetAll()
        {
            var queryString = "SELECT [Id], [Name], [Description], [Address], [Phone] " +
                              "FROM [dbo].[Venue]";
            var venues = new List<Venue>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            venues.Add(new Venue
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Address = reader["Address"].ToString(),
                                Phone = reader["Phone"].ToString(),
                            });
                        }
                    }
                }
            }

            return venues;
        }

        public Venue GetById(int id)
        {
            var queryString = "SELECT [Id], [Name], [Description], [Address], [Phone] " +
                                 "FROM [dbo].[Venue] " +
                                 "WHERE Id = @id";
            Venue venue = null;

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
                            venue = new Venue
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Address = reader["Address"].ToString(),
                                Phone = reader["Phone"].ToString(),
                            };
                        }
                    }
                }
            }

            return venue;
        }

        public void Update(Venue item)
        {
            var queryString = "UPDATE [dbo].[Venue] SET " +
                                "[Name] = @name, " +
                                "[Description] = @description, " +
                                "[Adress] = @adress, " +
                                "[Phone] = @phone " +
                                "WHERE [Id] = @id";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@id", item.Id);
                    command.Parameters.AddWithValue("@name", item.Name);
                    command.Parameters.AddWithValue("@description", item.Description);
                    command.Parameters.AddWithValue("@adress", item.Address);
                    command.Parameters.AddWithValue("@phone", item.Phone);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
