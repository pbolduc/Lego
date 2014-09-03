using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Lego.PerformanceCounters
{
    /// <summary>
    /// Reads the counters defined in a performance monitor settings file (Performance Monitor).
    /// </summary>
    public class PerformanceMonitorSettingsSource : ICounterSetSource
    {
        private string _filename;

        public PerformanceMonitorSettingsSource(string filename)
        {
            _filename = filename;
        }

        public SamplingCounterSet GetSet()
        {
            TimeSpan samplingRate = TimeSpan.Zero;
            List<string> counters = new List<string>();

            using (var reader = XmlReader.Create(File.OpenText(_filename), new XmlReaderSettings { CloseInput = true, IgnoreWhitespace = true }))
            {
                while (reader.Read())
                {
                    /*
                     * <DataCollectorSet>
                     *  <PerformanceCounterDataCollector>
                     *   <SampleInterval>15</SampleInterval>
                     *   <Counter>\Processor(*)\% Privileged Time</Counter>
                     */
                    if (reader.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    switch (reader.Name)
                    {
                        case "PARAM":
                            reader.MoveToNextAttribute();
                            if (reader.Name == "NAME")
                            {
                                if (Regex.IsMatch(reader.Value, "Counter\\d+.Path"))
                                {
                                    reader.MoveToNextAttribute();
                                    counters.Add(reader.Value);
                                }
                                else if (reader.Value == "UpdateInterval")
                                {
                                    reader.MoveToNextAttribute();
                                    samplingRate = TimeSpan.FromSeconds(Double.Parse(reader.Value));
                                }
                            }
                            reader.MoveToElement();
                            break;
                    }
                }
            }

            SamplingCounterSet set = new SamplingCounterSet();
            set.SamplingRate = samplingRate;
            set.CounterPaths = counters.ToArray();

            return set;

        }
    }
}