using NUnit.Framework;
using Shouldly;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace log4net.Appender.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private ILog logger;

        public void Configure(string config)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(config)))
                Config.XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), memoryStream);
            logger = LogManager.GetLogger(typeof(ObservableAppenderTests));
        }

        [Test]
        public void ShouldInsertLogger()
        {
            Configure(@"
<log4net>
  <appender name=""ObservableAppender"" type=""log4net.Appender.ObservableAppender, log4net.ObservableAppender"" />
  <root>
    <appender-ref ref=""ObservableAppender"" />
  </root>
</log4net>");
            logger.Logger.Repository.GetAppenders()
                .OfType<ObservableAppender>()
                .ShouldHaveSingleItem();
        }

        [Test]
        public void ShouldPutPersist()
        {
            Configure(@"
<log4net>
  <appender name=""ObservableAppender"" type=""log4net.Appender.ObservableAppender, log4net.ObservableAppender"">
    <Persist value=""true"" />
  </appender>
  <root>
    <appender-ref ref=""ObservableAppender"" />
  </root>
</log4net>");
            logger.Logger.Repository.GetAppenders()
                .OfType<ObservableAppender>()
                .ShouldHaveSingleItem()
                .Persist.ShouldBeTrue();
        }

        [Test]
        public void ShouldPutFilter()
        {
            Configure(@"
<log4net>
  <appender name=""ObservableAppender"" type=""log4net.Appender.ObservableAppender, log4net.ObservableAppender"">
    <filter type=""log4net.Filter.LevelRangeFilter"">
      <param name=""LevelMin"" value=""INFO"" />
      <param name=""LevelMax"" value=""ERROR"" />
    </filter>
  </appender>
  <root>
    <appender-ref ref=""ObservableAppender"" />
  </root>
</log4net>");
            logger.Logger.Repository.GetAppenders()
                .OfType<ObservableAppender>()
                .ShouldHaveSingleItem()
                .FilterHead.ShouldBeOfType<Filter.LevelRangeFilter>();
        }
    }
}
