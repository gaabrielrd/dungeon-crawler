using System;
using System.Collections.Generic;
using DungeonCrawler.Core.Services;

namespace DungeonCrawler.Core.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public void Publish<TEvent>(TEvent gameEvent)
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                ((Action<TEvent>)handlers)?.Invoke(gameEvent);
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var eventType = typeof(TEvent);
            _handlers[eventType] = _handlers.TryGetValue(eventType, out var existing)
                ? Delegate.Combine(existing, handler)
                : handler;
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var eventType = typeof(TEvent);
            if (!_handlers.TryGetValue(eventType, out var existing))
            {
                return;
            }

            var updated = Delegate.Remove(existing, handler);
            if (updated == null)
            {
                _handlers.Remove(eventType);
                return;
            }

            _handlers[eventType] = updated;
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}
