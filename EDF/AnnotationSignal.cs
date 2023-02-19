using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EDFCSharp
{
    /// <summary>
    /// Annotation signal representing annotations in the EDF+ file ("EDF Annotations").
    /// Non-annotation signal time offsets are also stored in the annotations signal as the first items in their TALs.
    /// </summary>
    public class AnnotationSignal : IEDFBaseSignal<TAL>
    {
        /// <summary>
        /// Provided sample value after scaling.
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns></returns>
        public TAL ScaledSample(int aIndex) { return Samples[aIndex]; }

        /// <summary>
        /// Provide sample scaling factor.
        /// </summary>
        /// <returns></returns>
        public double ScaleFactor() { return (PhysicalMaximum.Value - PhysicalMinimum.Value) / (DigitalMaximum.Value - DigitalMinimum.Value); }

        public override string ToString()
        {
            return Label.Value + " " + NumberOfSamplesInDataRecord.Value.ToString() + "/" + Samples.Count().ToString() + " ["
                + string.Join<TAL>(",", Samples.Skip(0).Take(10).ToArray()) + " ...]";
        }

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

        public List<TAL> Samples { get; set; } = new List<TAL> { };
        public long SamplesCount => Samples.Count;

        public AnnotationSignal(int numberOfSamplesInDataRecord)
        {
            /* /// https://www.edfplus.info/specs/edfplus.html#annotationssignal section 2.2.1
             * For the sake of EDF compatibility, the fields 'digital minimum' and 'digital maximum' must be filled with -32768 and 32767, respectively. 
             * The 'Physical maximum' and 'Physical minimum' fields must contain values that differ from each other. 
             * The other fields of this signal are filled with spaces*/
            Label.Value = EDFConstants.AnnotationLabel;
            DigitalMinimum.Value = -32768;
            DigitalMaximum.Value = 32767;
            PhysicalMinimum.Value = -1;
            PhysicalMaximum.Value = 1;
            PhysicalDimension.Value = string.Empty;
            TransducerType.Value = string.Empty;
            Prefiltering.Value = string.Empty;
            Reserved.Value = string.Empty;
            NumberOfSamplesInDataRecord.Value = numberOfSamplesInDataRecord;
        }

        protected bool Equals(AnnotationSignal other)
        {
            return Index == other.Index && Equals(Label, other.Label) && Equals(TransducerType, other.TransducerType) &&
                   Equals(PhysicalDimension, other.PhysicalDimension) &&
                   Equals(PhysicalMinimum, other.PhysicalMinimum) && Equals(PhysicalMaximum, other.PhysicalMaximum) &&
                   Equals(DigitalMinimum, other.DigitalMinimum) && Equals(DigitalMaximum, other.DigitalMaximum) &&
                   Equals(Prefiltering, other.Prefiltering) &&
                   Equals(NumberOfSamplesInDataRecord, other.NumberOfSamplesInDataRecord) &&
                   Equals(Reserved, other.Reserved) && Samples.SequenceEqual(other.Samples);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AnnotationSignal)obj);
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
            hashCode.Add(NumberOfSamplesInDataRecord);
            hashCode.Add(Reserved.Value);
            hashCode.Add(Samples.Count);
            return hashCode.ToHashCode();
#else
            return 0;
#endif
        }
    }

}
