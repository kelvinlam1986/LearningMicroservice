using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace Contracts.Common.Events
{
    public class EventEntity<T> : EntityBase<T>, IEventEntity
    {
        private readonly List<BaseEvent> _events = new List<BaseEvent>();

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _events.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _events.Clear();
        }

        public IReadOnlyCollection<BaseEvent> DomainEvents()
        {
            return _events.AsReadOnly();
        }

        public void RemoveDomainEvent(BaseEvent domainEvent)
        {
            _events.Remove(domainEvent);
        }
    }
}
