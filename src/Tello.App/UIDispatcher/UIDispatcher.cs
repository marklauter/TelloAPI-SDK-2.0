using System;
using System.Threading;

namespace Tello.App
{
    //todo: i have no idea if this will work or how to test without an actual UWP app running
    internal class UIDispatcher : IUIDispatcher
    {
        private readonly SynchronizationContext _context;

        private UIDispatcher()
        {
        }

        /// <summary>
        /// pass main thread context
        /// </summary>
        /// <param name="context"></param>
        public UIDispatcher(SynchronizationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Invoke(Action<object> action, object state)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _context.Post(new SendOrPostCallback(action), state);
        }

        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _context.Post(new SendOrPostCallback(state => { action(); }), null);
        }
    }
}
