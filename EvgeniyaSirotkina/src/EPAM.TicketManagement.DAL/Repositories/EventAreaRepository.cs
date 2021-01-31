using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

namespace EPAM.TicketManagement.DAL.Repositories
{
    internal class EventAreaRepository : IRepository<EventArea>
    {
        private readonly string _connectionString;

        public EventAreaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(EventArea item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("CreateEventArea", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventId", SqlDbType.Int).Value = item.EventId;
                    command.Parameters.Add("@description", SqlDbType.NVarChar).Value = item.Description;
                    command.Parameters.Add("@coordX", SqlDbType.Int).Value = item.CoordX;
                    command.Parameters.Add("@coordY", SqlDbType.Int).Value = item.CoordY;
                    command.Parameters.Add("@price", SqlDbType.Int).Value = item.Price;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("DeleteEventArea", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventAreaId", SqlDbType.Int).Value = id;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<EventArea> GetAll()
        {
            var queryString = "SELECT [Id], [EventId], [Description], [CoordX], [CoordY], [Price] " +
                                 "FROM [dbo].[EventArea]";
            var eventAreas = new List<EventArea>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventAreas.Add(new EventArea
                            {
                                Id = (int)reader["Id"],
                                EventId = (int)reader["EventId"],
                                Description = reader["Description"].ToString(),
                                CoordX = (int)reader["CoordX"],
                                CoordY = (int)reader["CoordY"],
                                Price = (decimal)reader["Price"],
                            });
                        }
                    }
                }
            }

            return eventAreas;
        }

        public EventArea GetById(int id)
        {
            var queryString = "SELECT [Id], [EventId], [Description], [CoordX], [CoordY], [Price] " +
                                 "FROM [dbo].[EventArea] WHERE [Id] = @id";
            EventArea eventArea = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            eventArea = new EventArea
                            {
                                Id = (int)reader["Id"],
                                EventId = (int)reader["EventId"],
                                Description = reader["Description"].ToString(),
                                CoordX = (int)reader["CoordX"],
                                CoordY = (int)reader["CoordY"],
                                Price = (decimal)reader["Price"],
                            };
                        }
                    }
                }
            }

            return eventArea;
        }

        public void Update(EventArea item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("UpdateEventArea", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventAreaId", SqlDbType.Int).Value = item.Id;
                    command.Parameters.Add("@eventId", SqlDbType.Int).Value = item.EventId;
                    command.Parameters.Add("@description", SqlDbType.NVarChar).Value = item.Description;
                    command.Parameters.Add("@coordX", SqlDbType.Int).Value = item.CoordX;
                    command.Parameters.Add("@coordY", SqlDbType.Int).Value = item.CoordY;
                    command.Parameters.Add("@price", SqlDbType.Int).Value = item.Price;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
