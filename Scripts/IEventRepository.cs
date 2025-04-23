using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts
{
    public interface IEventRepository
    {
        List<FootballEvent> GetEvents(DateTime date);
        void AddEvent(FootballEvent footballEvent);
        void UpdateEvent(FootballEvent footballEvent);
        void DeleteEvent(FootballEvent footballEvent);
    }
}
