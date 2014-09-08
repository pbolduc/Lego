using System;
using System.Collections.Generic;
using System.Threading;
using Lego.Graphite;
using Metrics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lego.Tests
{
    [TestClass]
    public class GraphiteReporterTests
    {
        [TestMethod]
        public void GraphiteReporter()
        {
            var done = new ManualResetEvent(false);

            List<Tuple<string, string, long>> sent = new List<Tuple<string, string, long>>();

            var graphiteMock = new Mock<IGraphite>();
            graphiteMock.Setup(g => g.Connect());
            graphiteMock.Setup(g => g.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
                .Callback((string name, string value, long timestamp) => sent.Add(Tuple.Create(name, value, timestamp)));

            graphiteMock.Setup(g => g.Close()).Callback(() => done.Set());

            IGraphite graphite = graphiteMock.Object;

            GraphiteReporter metricsReporter = new GraphiteReporter(graphite);

            CreateMetrics();
            Metric.Config.WithReporting(config => config.WithReporter("graphite", () => metricsReporter, TimeSpan.FromSeconds(1)).WithConsoleReport(TimeSpan.FromSeconds(1)));

            bool isDone = done.WaitOne(TimeSpan.FromSeconds(60));

            Assert.IsTrue(isDone);
        }

        private void CreateMetrics()
        {
            Random random = new Random(0);

            var counter = Metric.Counter("counter.test", Unit.None);
            var guage = Metric.Gauge("gauge.test", random.NextDouble, Unit.None);
            var meter = Metric.Meter("meter.test", Unit.None);
            var histogram = Metric.Histogram("histogram.test", Unit.None);
            var timer = Metric.Timer("timer.test", Unit.None, SamplingType.FavourRecent, TimeUnit.Milliseconds);

            for (int i = 0; i < 5; i++)
            {
                using (timer.NewContext())
                {
                    counter.Increment(random.Next(1, 10));
                    histogram.Update(random.Next(1, 10));
                    meter.Mark();
                }
            }           
        }
    }
}