﻿using System;
using System.Linq;
using App.Metrics.Sampling;
using App.Metrics.Sampling.Interfaces;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Facts
{
    public class WeightedSnapshotTests
    {
        private readonly WeightedSnapshot _snapshot = MakeSanpshot(new long[] { 5, 1, 2, 3, 4 }, new double[] { 1, 2, 3, 2, 2 });

        [Fact]
        public void weight_snapshot_big_quantiles_are_the_last_value() { _snapshot.GetValue(1.0).Should().Be(5.0); }

        [Fact]
        public void weight_snapshot_calculates_the_max_of_zero_for_an_empty_snapshot()
        {
            ISnapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            
            snapshot.Max.Should().Be(0);
        }

        [Fact]
        public void weight_snapshot_calculates_the_max_value() { _snapshot.Max.Should().Be(5); }

        [Fact]
        public void weight_snapshot_calculates_the_mean_of_zero_for_an_empty_snapshot()
        {
            ISnapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.Mean.Should().Be(0);
        }

        [Fact]
        public void weight_snapshot_calculates_the_mean_value() { _snapshot.Mean.Should().Be(2.7); }

        [Fact]
        public void weight_snapshot_calculates_the_min_of_zero_for_an_empty_snapshot()
        {
            ISnapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.Min.Should().Be(0);
        }

        [Fact]
        public void weight_snapshot_calculates_the_min_value() { _snapshot.Min.Should().Be(1); }

        [Fact]
        public void weight_snapshot_calculates_the_standard_deviation() { _snapshot.StdDev.Should().BeApproximately(1.2688, 0.0001); }

        [Fact]
        public void weight_snapshot_calculates_the_standard_deviation_of_zero_for_a_single_snapshot()
        {
            ISnapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.StdDev.Should().Be(0);
        }

        [Fact]
        public void weight_snapshot_calculates_the_standard_deviation_of_zero_for_an_empty_snapshot()
        {
            ISnapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.StdDev.Should().Be(0);
        }

        [Fact]
        public void weight_snapshot_has_a_75_percentile() { _snapshot.Percentile75.Should().Be(4.0); }

        [Fact]
        public void weight_snapshot_has_a_95_percentile() { _snapshot.Percentile95.Should().Be(5.0); }

        [Fact]
        public void weight_snapshot_has_a_98_percentile() { _snapshot.Percentile98.Should().Be(5.0); }

        [Fact]
        public void weight_snapshot_has_a_99_percentile() { _snapshot.Percentile99.Should().Be(5.0); }

        [Fact]
        public void weight_snapshot_has_a_999_percentile() { _snapshot.Percentile999.Should().Be(5.0); }

        [Fact]
        public void weight_snapshot_has_a_median_percentile() { _snapshot.Median.Should().Be(3.0); }

        [Fact]
        public void weight_snapshot_has_size() { _snapshot.Size.Should().Be(5); }

        [Fact]
        public void weight_snapshot_has_values() { _snapshot.Values.Should().Equal(new long[] { 1, 2, 3, 4, 5 }); }


        [Fact]
        public void weight_snapshot_small_quantiles_are_the_first_value() { _snapshot.GetValue(0.0).Should().Be(1.0); }

        [Fact]
        public void weighted_snapshot_throws_on_bad_quantile_value()
        {
            ((Action)(() => _snapshot.GetValue(-0.5))).ShouldThrow<ArgumentException>();
            ((Action)(() => _snapshot.GetValue(1.5))).ShouldThrow<ArgumentException>();
            ((Action)(() => _snapshot.GetValue(double.NaN))).ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void can_determine_if_weighted_samples_are_same()
        {
            var first = new WeightedSample(1, null, 1);
            var second = new WeightedSample(1, null, 1);

            first.Should().Be(second);
        }

        [Fact]
        public void can_determine_if_weighted_samples_are_diff()
        {
            var first = new WeightedSample(1, null, 1);
            var second = new WeightedSample(2, null, 1);

            first.Should().NotBe(second);
        }

        private static WeightedSnapshot MakeSanpshot(long[] values, double[] weights)
        {
            if (values.Length != weights.Length)
            {
                throw new ArgumentException("values and weights must have same number of elements");
            }

            var samples = Enumerable.Range(0, values.Length).Select(i => new WeightedSample(values[i], null, weights[i]));

            return new WeightedSnapshot(values.Length, samples);
        }
    }
}