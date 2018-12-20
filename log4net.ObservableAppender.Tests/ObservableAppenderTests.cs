using log4net.Core;
using log4net.Filter;
using Moq;
using NUnit.Framework;
using System;
using System.Reflection;

namespace log4net.ObservableAppender.Tests
{
    [TestFixture]
    public class ObservableAppenderTests
    {
        private ObservableAppender appender;
        private Mock<IObserver<LoggingEvent>> mock;
        private ILog logger;

        [SetUp]
        public void Configure()
        {
            appender = new ObservableAppender();
            mock = new Mock<IObserver<LoggingEvent>>();

            Config.BasicConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), appender);
            logger = LogManager.GetLogger(typeof(ObservableAppenderTests));
        }

        [Test]
        public void ShouldExposeLoggedMessages()
        {
            appender.LoggingEvents.Subscribe(mock.Object);
            logger.Debug("test");

            mock.Verify(x => x.OnNext(It.IsAny<LoggingEvent>()));
        }

        [Test]
        public void ShouldCompleteOnShutDown()
        {
            appender.LoggingEvents.Subscribe(mock.Object);

            logger.Logger.Repository.Shutdown();

            mock.Verify(x => x.OnCompleted());
        }

        [Test]
        public void ShouldNotCompleteBeforeShutDown()
        {
            appender.LoggingEvents.Subscribe(mock.Object);

            mock.Verify(x => x.OnCompleted(), Times.Never());
        }

        [Test]
        public void ShouldStoreOldMessagesIfReplay()
        {
            appender.Persist = true;

            logger.Debug("test");

            appender.LoggingEvents.Subscribe(mock.Object);

            mock.Verify(x => x.OnNext(It.IsAny<LoggingEvent>()));
        }

        [Test]
        public void ShouldNotStoreOldMessages()
        {
            logger.Debug("test");

            appender.LoggingEvents.Subscribe(mock.Object);

            mock.Verify(x => x.OnNext(It.IsAny<LoggingEvent>()), Times.Never());
        }

        [Test]
        public void ShouldFilterOut()
        {
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = Level.Info,
                LevelMax = Level.Error
            });
            appender.LoggingEvents.Subscribe(mock.Object);
            logger.Debug("test");

            mock.Verify(x => x.OnNext(It.IsAny<LoggingEvent>()), Times.Never());
        }

        [Test]
        public void ShouldFilterIn()
        {
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = Level.Info,
                LevelMax = Level.Error
            });
            appender.LoggingEvents.Subscribe(mock.Object);
            logger.Info("test");

            mock.Verify(x => x.OnNext(It.IsAny<LoggingEvent>()));
        }

        [Test]
        public void ShouldFilterOutByThreshold()
        {
            appender.Threshold = Level.Warn;
            appender.LoggingEvents.Subscribe(mock.Object);
            logger.Debug("test");

            mock.Verify(x => x.OnNext(It.IsAny<LoggingEvent>()), Times.Never());
        }

        [Test]
        public void ShouldFilterInByThreshold()
        {
            appender.Threshold = Level.Warn;
            appender.LoggingEvents.Subscribe(mock.Object);
            logger.Warn("test");

            mock.Verify(x => x.OnNext(It.IsAny<LoggingEvent>()));
        }
    }
}
