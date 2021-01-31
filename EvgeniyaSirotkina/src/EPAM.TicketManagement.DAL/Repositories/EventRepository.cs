using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

namespace EPAM.TicketManagement.DAL.Repositories
{
    internal class EventRepository : IRepository<EventEntity>
    {
        private readonly string _connectionString;

        public EventRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(EventEntity item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("CreateEvent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@name", SqlDbType.NVarChar).Value = item.Name;
                    command.Parameters.Add("@description", SqlDbType.NVarChar).Value = item.Description;
                    command.Parameters.Add("@layoutId", SqlDbType.Int).Value = item.LayoutId;
                    command.Parameters.Add("@start", SqlDbType.DateTime2).Value = item.EventStart;
                    command.Parameters.Add("@end", SqlDbType.DateTime2).Value = item.EventEnd;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("DeleteEvent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventId", SqlDbType.Int).Value = id;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<EventEntity> GetAll()
        {
            var queryString = "SELECT [Id], [Name], [Description], [LayoutId], [EventStart], [EventEnd] " +
                                 "FROM [dbo].[Event]";
            var events = new List<EventEntity>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(new EventEntity
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                LayoutId = (int)reader["LayoutId"],
                                EventStart = (DateTime)reader["EventStart"],
                                EventEnd = (DateTime)reader["EventEnd"],
                            });
                        }
                    }
                }
            }

            return events;
        }

        public EventEntity GetById(int id)
        {
            var queryString = "SELECT [Id], [Name], [Description], [LayoutId], [EventStart], [EventEnd] " +
                                 "FROM [dbo].[Event] " +
                                 "WHERE [Id] = @id";
            EventEntity eventById = null;

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
                            eventById = new EventEntity
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                LayoutId = (int)reader["LayoutId"],
                                EventStart = (DateTime)reader["EventStart"],
                                EventEnd = (DateTime)reader["EventEnd"],
                            };
                        }
                    }
                }
            }

            return eventById;
        }

        public void Update(EventEntity item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("UpdateEvent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventId", SqlDbType.Int).Value = item.Id;
                    command.Parameters.Add("@name", SqlDbType.NVarChar).Value = item.Name;
                    command.Parameters.Add("@description", SqlDbType.NVarChar).Value = item.Description;
                    command.Parameters.Add("@layoutId", SqlDbType.Int).Value = item.LayoutId;
                    command.Parameters.Add("@start", SqlDbType.DateTime2).Value = item.EventStart;
                    command.Parameters.Add("@end", SqlDbType.DateTime2).Value = item.EventEnd;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
