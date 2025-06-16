using System;

namespace Assets.Scripts.Helpers
{
    public class EventsLib
    {
        public static readonly GenericEvent<EventArgs> CourseDetailsWindowClosed = new GenericEvent<EventArgs>();
    }
}
