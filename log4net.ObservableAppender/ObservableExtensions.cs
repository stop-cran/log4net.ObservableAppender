using System;
using System.Reactive;
using System.Reactive.Linq;

namespace log4net.ObservableAppender
{
    internal static class ObservableExtensions
    {
        public static IObservable<T> AlsoCompleteOn<T>(this IObservable<T> source, IObservable<T> completionSourse) =>
            source.Materialize()
                .Merge(completionSourse.Materialize()
                    .Where(notification => notification.Kind == NotificationKind.OnCompleted))
            .Dematerialize();
    }
}
