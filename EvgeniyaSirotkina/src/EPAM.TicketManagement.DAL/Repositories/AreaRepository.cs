using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

[assembly: InternalsVisibleTo("EPAM.TicketManagement.IntegrationTests")]

namespace EPAM.TicketManagement.DAL.Repositories
{
    internal class AreaRepository : IRepository<Area>
    {
        private readonly string _connectionString;

        public AreaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Area item)
        {
            var queryString = "INSERT INTO [dbo].[Area] VALUES(@layoutId,@description,@coordX,@coordY)";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@layoutId", item.LayoutId);
                    command.Parameters.AddWithValue("@description", item.Description);
                    command.Parameters.AddWithValue("@coordX", item.CoordX);
                    command.Parameters.AddWithValue("@coordY", item.CoordY);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            var queryString = "DELETE FROM [dbo].[Area] WHERE Id = @id";

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

        public IEnumerable<Area> GetAll()
        {
            var queryString = "SELECT [Id], [LayoutId], [Description], [CoordX], [CoordY] " +
                                 "FROM [dbo].[Area]";
            var areas = new List<Area>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            areas.Add(new Area
                            {
                                Id = (int)reader["Id"],
                                LayoutId = (int)reader["LayoutId"],
                                Description = reader["Description"].ToString(),
                                CoordX = (int)reader["CoordX"],
                                CoordY = (int)reader["CoordY"],
                            });
                        }
                    }
                }
            }

            return areas;
        }

        public Area GetById(int id)
        {
            var queryString = "SELECT [Id], [LayoutId], [Description], [CoordX], [CoordY] " +
                                 "FROM [dbo].[Area] " +
                                 "WHERE Id = @id";
            Area area = null;

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
                            area = new Area
                            {
                                Id = (int)reader["Id"],
                                LayoutId = (int)reader["LayoutId"],
                                Description = reader["Description"].ToString(),
                                CoordX = (int)reader["CoordX"],
                                CoordY = (int)reader["CoordY"],
                            };
                        }
                    }
                }
            }

            return area;
        }

        public void Update(Area item)
        {
            var queryString = "UPDATE [dbo].[Venue] SET " +
                                "[LayoutId] = @layoutId, " +
                                "[Description] = @description, " +
                                "[CoordX] = @coordX " +
                                "[CoordY] = @coordY " +
                                "WHERE [Id] = @id";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@id", item.Id);
                    command.Parameters.AddWithValue("@layoutId", item.LayoutId);
                    command.Parameters.AddWithValue("@description", item.Description);
                    command.Parameters.AddWithValue("@coordX", item.CoordX);
                    command.Parameters.AddWithValue("@coordY", item.CoordY);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
