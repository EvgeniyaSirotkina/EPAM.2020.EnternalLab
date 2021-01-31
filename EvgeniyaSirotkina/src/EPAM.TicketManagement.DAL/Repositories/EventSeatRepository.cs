using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

namespace EPAM.TicketManagement.DAL.Repositories
{
    internal class EventSeatRepository : IRepository<EventSeat>
    {
        private readonly string _connectionString;

        public EventSeatRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(EventSeat item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("CreateEventSeat", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventAreaId", SqlDbType.Int).Value = item.EventAreaId;
                    command.Parameters.Add("@row", SqlDbType.NVarChar).Value = item.Row;
                    command.Parameters.Add("@number", SqlDbType.Int).Value = item.Number;
                    command.Parameters.Add("@state", SqlDbType.Int).Value = item.State;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("DeleteEventSeat", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventSeatId", SqlDbType.Int).Value = id;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<EventSeat> GetAll()
        {
            var queryString = "SELECT [Id], [EventAreaId], [Row], [Number], [State] " +
                                 "FROM [dbo].[EventSeat]";
            var eventSeatss = new List<EventSeat>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventSeatss.Add(new EventSeat
                            {
                                Id = (int)reader["Id"],
                                EventAreaId = (int)reader["EventAreaId"],
                                Row = (int)reader["Row"],
                                Number = (int)reader["Number"],
                                State = (int)reader["State"],
                            });
                        }
                    }
                }
            }

            return eventSeatss;
        }

        public EventSeat GetById(int id)
        {
            var queryString = "SELECT [Id], [EventAreaId], [Row], [Number], [State] " +
                                 "FROM [dbo].[EventSeat] " +
                                 "WHERE [Id] = @id";
            EventSeat eventSeat = null;

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
                            eventSeat = new EventSeat
                            {
                                Id = (int)reader["Id"],
                                EventAreaId = (int)reader["EventAreaId"],
                                Row = (int)reader["Row"],
                                Number = (int)reader["Number"],
                                State = (int)reader["State"],
                            };
                        }
                    }
                }
            }

            return eventSeat;
        }

        public void Update(EventSeat item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("UpdateEventSeat", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@eventSeatId", SqlDbType.Int).Value = item.Id;
                    command.Parameters.Add("@eventAreaId", SqlDbType.Int).Value = item.EventAreaId;
                    command.Parameters.Add("@row", SqlDbType.NVarChar).Value = item.Row;
                    command.Parameters.Add("@number", SqlDbType.Int).Value = item.Number;
                    command.Parameters.Add("@state", SqlDbType.Int).Value = item.State;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
