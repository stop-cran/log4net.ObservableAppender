using log4net.Core;
using System;
using System.Reactive.Linq;

namespace log4net.Appender
{
    /// <summary>
    /// In-memory appender, that streams logging events into IObservable&lt;LoggingEvent&gt;.
    /// </summary>
    public class ObservableAppender : AppenderSkeleton
    {
        private event Action<LoggingEvent> OnLoggingEvent;

        private event Action<LoggingEvent> OnCloseEvent;

        private IObservable<LoggingEvent> loggingEventsReplay;

        private IDisposable subscription;

        private IObservable<LoggingEvent> WrapEvent() =>
            Observable
                .FromEvent<LoggingEvent>(
                    d => OnLoggingEvent += d,
                    d => OnLoggingEvent -= d)
                .AlsoCompleteOn(
                    Observable
                        .FromEvent<LoggingEvent>(
                            d => OnCloseEvent += d,
                            d => OnCloseEvent -= d)
                        .Take(1));

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
                        loggingEventsReplay = WrapEvent().Replay().RefCount();
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
        public IObservable<LoggingEvent> LoggingEvents => loggingEventsReplay ?? WrapEvent();

        /// <summary>
        /// An override. Sends an incoming loggingEvent to LoggingEvents observable.
        /// </summary>
        /// <param name="loggingEvent">An incoming logging event</param>
        protected override void Append(LoggingEvent loggingEvent) =>
            OnLoggingEvent?.Invoke(loggingEvent);

        /// <summary>
        /// An override. Completes the LoggingEvents
        /// </summary>
        protected override void OnClose()
        {
            if (Persist)
                subscription.Dispose();
            OnCloseEvent?.Invoke(default);
            base.OnClose();
        }
    }
}
