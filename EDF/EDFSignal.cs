using System;
using System.Collections.Generic;
using System.Linq;

namespace EDFCSharp
{
    /// <summary>
    /// Ordinary signal representing a single channel of data stored in the EDF+ file.
    /// Ordinary signals can be recorded from different sources (e.g. "EEG FP7", "EEG C3", "DC01").
    /// They are all have a fixed number of samples in each data record. Data records usually represent a fixed time interval and they are stored predetermined, ordered sequence in the EDF file.
    /// </summary>
    public class EDFSignal : IEDFBaseSignal<short>
    {
        private List<DateTimeOffset> _times;
        public int Index { get; set; }

        public FixedLengthString Label { get; } = new FixedLengthString(HeaderItems.Label);

        public FixedLengthString TransducerType { get; } = new FixedLengthString(HeaderItems.TransducerType);

        public FixedLengthString PhysicalDimension { get; } = new FixedLengthString(HeaderItems.PhysicalDimension);

        public FixedLengthDouble PhysicalMinimum { get; } = new FixedLengthDouble(HeaderItems.PhysicalMinimum);

        public FixedLengthDouble PhysicalMaximum { get; } = new FixedLengthDouble(HeaderItems.PhysicalMaximum);

        public FixedLengthInt DigitalMinimum { get; } = new FixedLengthInt(HeaderItems.DigitalMinimum);

        public FixedLengthInt DigitalMaximum { get; } = new FixedLengthInt(HeaderItems.DigitalMaximum);

        public FixedLengthString Prefiltering { get; } = new FixedLengthString(HeaderItems.Prefiltering);

        public FixedLengthInt NumberOfSamplesInDataRecord { get; } = new FixedLengthInt(HeaderItems.NumberOfSamplesInDataRecord);

        public FixedLengthString Reserved { get; } = new FixedLengthString(HeaderItems.SignalsReserved);
        public double FrequencyInHZ { get; set; }
        public List<short> Samples { get; set; } = new List<short>();
        public List<long> Timestamps { get; set; } = new List<long>();
        public List<double> Values { get; set; } = new List<double>();

        public List<DateTimeOffset> Times
        {
            get
            {
                if (_times == null || _times.Count != Samples.Count)
                {
                    _times = Timestamps.Select(DateTimeOffset.FromUnixTimeMilliseconds).ToList();

                }
                return _times;
            }
            set => _times = value;
        }

        public long SamplesCount => Samples.Count;

        public EDFSignal()
        {

        }
        public EDFSignal(int index, double frequencyInHz)
        {
            Index = index;
            FrequencyInHZ = frequencyInHz;
        }
        /// <summary>
        /// Provided sample value after scaling.
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public double ScaledSample(int aIndex) { return Samples[aIndex] * ScaleFactor(); }

        /// <summary>
        /// Provide sample scaling factor.
        /// </summary>
        /// <returns></returns>
        public double ScaleFactor() { return (PhysicalMaximum.Value - PhysicalMinimum.Value) / (DigitalMaximum.Value - DigitalMinimum.Value); }

        public override string ToString()
        {
            return Label.Value + " " + NumberOfSamplesInDataRecord.Value.ToString() + "/" + Samples.Count().ToString() + " ["
                + string.Join(",", Samples.Skip(0).Take(10).ToArray()) + " ...]";
        }

        public void CalculateAllTimeStamps(DateTime startTime, double frequency, long totalSamples)
        {

        }

        protected bool Equals(EDFSignal other)
        {
            return Index == other.Index && Equals(Label, other.Label) && Equals(TransducerType, other.TransducerType) &&
                   Equals(PhysicalDimension, other.PhysicalDimension) &&
                   Equals(PhysicalMinimum, other.PhysicalMinimum) && Equals(PhysicalMaximum, other.PhysicalMaximum) &&
                   Equals(DigitalMinimum, other.DigitalMinimum) && Equals(DigitalMaximum, other.DigitalMaximum) &&
                   Equals(Prefiltering, other.Prefiltering) &&
                   Equals(NumberOfSamplesInDataRecord, other.NumberOfSamplesInDataRecord) &&
                   Equals(Reserved, other.Reserved) && FrequencyInHZ.Equals(other.FrequencyInHZ) &&
                   Samples.SequenceEqual(other.Samples);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EDFSignal)obj);
        }

        public override int GetHashCode()
        {
#if NET
            var hashCode = new HashCode();
            hashCode.Add(Index);
            hashCode.Add(Label.Value);
            hashCode.Add(TransducerType.Value);
            hashCode.Add(PhysicalDimension.Value);
            hashCode.Add(PhysicalMinimum.Value);
            hashCode.Add(PhysicalMaximum.Value);
            hashCode.Add(DigitalMinimum.Value);
            hashCode.Add(DigitalMaximum.Value);
            hashCode.Add(Prefiltering.Value);
            hashCode.Add(NumberOfSamplesInDataRecord.Value);
            hashCode.Add(Reserved.Value);
            hashCode.Add(FrequencyInHZ);
            hashCode.Add(Samples.Count);
            hashCode.Add(Timestamps.Count);
            return hashCode.ToHashCode();
#else
return 0;
#endif
        }
    }


}
