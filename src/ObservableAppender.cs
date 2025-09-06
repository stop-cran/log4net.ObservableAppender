using log4net.Core;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace log4net.Appender
{
    /// <summary>
    /// In-memory appender, that streams logging events into IObservable&lt;LoggingEvent&gt;.
    /// </summary>
    public class ObservableAppender : AppenderSkeleton
    {
        private readonly Subject<LoggingEvent> loggingEventsSubject = new Subject<LoggingEvent>();
        private IObservable<LoggingEvent> loggingEventsReplay;
        private IDisposable subscription;

        /// <summary>
        /// False - (default) do not persist any logging events. Every subscriber of LoggingEvents will recieve only ongoing logging events, without ones that occurred before subscription.
        /// True - persist all logging events in memory. Every new LoggingEvents subscriber gets all past logs first, and then ongoing logs.
        /// Occurs until repository shutdown/appender close or longer in case still there are any subscriptions.
        /// </summary>
        public bool Persist
        {
            get => loggingEventsReplay != null;
            set
            {
                if (value ^ Persist)
                    if (value)
                    {
                        // Keep alive until no subscribers left.
                        loggingEventsReplay = loggingEventsSubject.Replay().RefCount();
                        // Subscribe internally to keep alive at least until OnClose.
                        subscription = loggingEventsReplay.Subscribe();
                    }
                    else
                    {
                        subscription.Dispose();
                        loggingEventsReplay = null;
                    }
            }
        }

        /// <summary>
        /// An observable, that contains all logging events matched,
        /// optionally including history (i.e. the events came before the subscription to this observable).
        /// Completes on the repository shutdown or an explicit Close() call.
        /// </summary>
        public IObservable<LoggingEvent> LoggingEvents => loggingEventsReplay ?? loggingEventsSubject;

        /// <summary>
        /// An override. Sends an incoming loggingEvent to LoggingEvents observable.
        /// </summary>
        /// <param name="loggingEvent">An incoming logging event</param>
        protected override void Append(LoggingEvent loggingEvent) =>
            loggingEventsSubject.OnNext(loggingEvent);

        /// <summary>
        /// An override. Completes the LoggingEvents
        /// </summary>
        protected override void OnClose()
        {
            if (Persist)
                subscription.Dispose();
            loggingEventsSubject.OnCompleted();
            loggingEventsSubject.Dispose();
            base.OnClose();
        }
    }
}
