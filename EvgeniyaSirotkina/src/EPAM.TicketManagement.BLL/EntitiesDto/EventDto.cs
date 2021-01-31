using System;

namespace EPAM.TicketManagement.BLL.EntitiesDto
{
    public class EventDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int LayoutId { get; set; }

        public DateTime EventStart { get; set; }

        public DateTime EventEnd { get; set; }
    }
}
