﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prometheus.Advanced.DataContracts;
using Prometheus.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Prometheus.Tests
{
    public sealed class AsciiFormatterTests
    {
        [DataTestMethod]
        [DataRow("simple-label-value-1")]
        [DataRow("with\nlinebreaks")]
        [DataRow("with\nlinebreaks and \\slashes and quotes \"")]
        public void family_should_be_formatted_to_one_line(string labelValue)
        {
            using (var ms = new MemoryStream())
            {
                var metricFamily = new MetricFamily
                {
                    name = "family1",
                    help = "help",
                    type = MetricType.COUNTER,
                };

                var metricCounter = new Advanced.DataContracts.Counter { value = 100 };
                metricFamily.metric.Add(new Metric
                {
                    counter = metricCounter,
                    label = new List<LabelPair>
                    {
                        new LabelPair {name = "label1", value = labelValue }
                    }
                });

                AsciiFormatter.Format(ms, new[]
                {
                    metricFamily
                });

                using (var sr = new StringReader(Encoding.UTF8.GetString(ms.ToArray())))
                {
                    var linesCount = 0;
                    var line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        linesCount += 1;
                    }
                    Assert.AreEqual(3, linesCount);
                }
            }
        }
    }
}
