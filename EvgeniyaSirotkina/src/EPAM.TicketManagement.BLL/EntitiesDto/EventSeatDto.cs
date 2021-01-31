﻿namespace EPAM.TicketManagement.BLL.EntitiesDto
{
    public class EventSeatDto
    {
        public int Id { get; set; }

        public int EventAreaId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        public SeatState State { get; set; }
    }
}
