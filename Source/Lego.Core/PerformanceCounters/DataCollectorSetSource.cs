using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Lego.PerformanceCounters
{
    /// <summary>
    /// Reads the counters defined in a Data Collector Set template (Performance Monitor).
    /// </summary>
    public class DataCollectorSetSource : ICounterSetSource
    {
        private string _filename;

        public DataCollectorSetSource(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Filename cannot be null or empty.", "filename");
            }

            _filename = filename;
        }

        public SamplingCounterSet GetSet()
        {
            if (!File.Exists(_filename))
            {
                throw new FileNotFoundException("Data Collector file not found.", _filename);
            }

            TimeSpan samplingRate = TimeSpan.Zero;
            List<string> counters = new List<string>();

            using (var reader = XmlReader.Create(File.OpenText(_filename), new XmlReaderSettings { CloseInput = true }))
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
                        case "SampleInterval":
                            reader.Read();
                            samplingRate = TimeSpan.FromSeconds(Double.Parse(reader.Value));
                            break;

                        case "Counter":
                            reader.Read();
                            counters.Add(reader.Value);
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