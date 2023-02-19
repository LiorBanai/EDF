using System;
using System.Globalization;
using System.Reflection.Emit;

namespace EDFCSharp
{
    /// <summary>
    /// Header of the EDF (European Data Format) file
    /// </summary>
    public class EDFHeader
    {
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        /// <summary>
        /// Version of this data format ("0")
        /// </summary>
        public FixedLengthString Version { get; }
        /// <summary>
        /// Local patient identification
        /// </summary>
        public FixedLengthString PatientID { get; }
        /// <summary>
        /// Local recording identification
        /// </summary>
        public FixedLengthString RecordID { get; }
        /// <summary>
        /// Start date of recording (dd.mm.yy)
        /// </summary>
        public FixedLengthString RecordingStartDate { get; }
        /// <summary>
        /// Start time of recording (hh.mm.ss)
        /// </summary>
        public FixedLengthString RecordingStartTime { get; }
        /// <summary>
        /// Number of bytes in header record
        /// </summary>
        public FixedLengthInt SizeInBytes { get; }
        /// <summary>
        /// 44-byte long reserved section ("EDF+D" for EDF+)
        /// </summary>
        public FixedLengthString Reserved { get; }
        /// <summary>
        /// Number of data records (-1 if unknown)
        /// </summary>
        public FixedLengthLong NumberOfDataRecords { get; }
        /// <summary>
        /// Duration of a data record, in seconds
        /// </summary>
        public FixedLengthDouble RecordDurationInSeconds { get; }
        /// <summary>
        /// Number of signals (ns) in data record
        /// </summary>
        public FixedLengthInt NumberOfSignalsInRecord { get; }
        /// <summary>
        /// Labels of the signals, concatenated
        /// </summary>
        public VariableLengthString Labels { get; }
        /// <summary>
        /// Transducer types of the signals, concatenated (e.g. "AgAgCl electrode")
        /// </summary>
        public VariableLengthString TransducerTypes { get; }
        /// <summary>
        /// Physical dimensions/units of the signals, concatenated (e.g. "uV")
        /// </summary>
        public VariableLengthString PhysicalDimensions { get; }
        /// <summary>
        /// Physical minimums in units of PhysicalDimension, concatenated (e.g. "-500")
        /// </summary>
        public VariableLengthDouble PhysicalMinimums { get; }
        /// <summary>
        /// Physical maximums in units of PhysicalDimension, concatenated (e.g. "500")
        /// </summary>
        public VariableLengthDouble PhysicalMaximums { get; }
        /// <summary>
        /// Digital minimums, concatenated (e.g. "-2048")
        /// </summary>
        public VariableLengthInt DigitalMinimums { get; }
        /// <summary>
        /// Digital maximums, concatenated (e.g. "2047")
        /// </summary>
        public VariableLengthInt DigitalMaximums { get; }
        /// <summary>
        /// Prefilterings, concatenated (e.g. "HP:0.1Hz LP:75Hz")
        /// </summary>
        public VariableLengthString PreFilterings { get; }
        /// <summary>
        /// Number of samples in each data record, concatenated (e.g. "1000")
        /// </summary>
        public VariableLengthInt NumberOfSamplesPerRecord { get; }
        /// <summary>
        /// 32-byte long reserved section, concatenated
        /// </summary>
        public VariableLengthString SignalsReserved { get; }
        public double TotalDurationInSeconds => NumberOfDataRecords.Value * RecordDurationInSeconds.Value;

        public EDFHeader()
        {
            Version = new FixedLengthString(HeaderItems.Version);
            PatientID = new FixedLengthString(HeaderItems.PatientID);
            RecordID = new FixedLengthString(HeaderItems.RecordID);
            RecordingStartDate = new FixedLengthString(HeaderItems.RecordingStartDate);
            RecordingStartTime = new FixedLengthString(HeaderItems.RecordingStartTime);
            SizeInBytes = new FixedLengthInt(HeaderItems.SizeInBytes);
            Reserved = new FixedLengthString(HeaderItems.Reserved);
            NumberOfDataRecords = new FixedLengthLong(HeaderItems.NumberOfDataRecords);
            RecordDurationInSeconds = new FixedLengthDouble(HeaderItems.RecordDurationInSeconds);
            NumberOfSignalsInRecord = new FixedLengthInt(HeaderItems.NumberOfSignalsInRecord);
            Labels = new VariableLengthString(HeaderItems.Label);
            TransducerTypes = new VariableLengthString(HeaderItems.TransducerType);
            PhysicalDimensions = new VariableLengthString(HeaderItems.PhysicalDimension);
            PhysicalMinimums = new VariableLengthDouble(HeaderItems.PhysicalMinimum);
            PhysicalMaximums = new VariableLengthDouble(HeaderItems.PhysicalMaximum);
            DigitalMinimums = new VariableLengthInt(HeaderItems.DigitalMinimum);
            DigitalMaximums = new VariableLengthInt(HeaderItems.DigitalMaximum);
            PreFilterings = new VariableLengthString(HeaderItems.Prefiltering);
            NumberOfSamplesPerRecord = new VariableLengthInt(HeaderItems.NumberOfSamplesInDataRecord);
            SignalsReserved = new VariableLengthString(HeaderItems.SignalsReserved);
        }
        public EDFHeader(string version, string patientId, string recordId, string recordingStartDate, string recordingStartTime,
            short sizeInBytes, string reserved, long numberOfDataRecords, double recordDurationInSeconds,
            short numberOfSignalsInRecord, string[] labels, string[] transducerTypes, string[] physicalDimensions,
            double[] physicalMinimums, double[] physicalMaximums, int[] digitalMinimums, int[] digitalMaximums,
            string[] preFilterings, int[] numberOfSamplesPerRecord, string[] signalsReserved):this()
        {
            Version.Value = version;
            PatientID.Value = patientId;
            RecordID.Value = recordId;
            RecordingStartDate.Value = recordingStartDate;
            RecordingStartTime.Value = recordingStartTime;
            SizeInBytes.Value = sizeInBytes;
            Reserved.Value = reserved;
            NumberOfDataRecords.Value = numberOfDataRecords;
            RecordDurationInSeconds.Value = recordDurationInSeconds;
            NumberOfSignalsInRecord.Value = numberOfSignalsInRecord;
            Labels.Value = labels;
            TransducerTypes.Value = transducerTypes;
            PhysicalDimensions.Value = physicalDimensions;
            PhysicalMinimums.Value = physicalMinimums;
            PhysicalMaximums.Value = physicalMaximums;
            DigitalMinimums.Value = digitalMinimums;
            DigitalMaximums.Value = digitalMaximums;
            PreFilterings.Value = preFilterings;
            NumberOfSamplesPerRecord.Value = numberOfSamplesPerRecord;
            SignalsReserved.Value = signalsReserved;
            StartTime = GetDateTime(RecordingStartDate.Value, RecordingStartTime.Value);
            EndTime = StartTime.AddSeconds(TotalDurationInSeconds);
        }

        private DateTime GetDateTime(string datePart, string timePart)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime date = DateTime.MinValue;
            try
            {
                date = DateTime.ParseExact(datePart, "dd.MM.yy", provider);
            }
            catch (FormatException)
            {
                //do nothing
            }
            DateTime time = DateTime.MinValue;
            try
            {
                time = DateTime.ParseExact(timePart, "HH.mm.ss", provider);
            }
            catch (FormatException)
            {
                //do nothing
            }
            DateTime combined = date.Date.Add(time.TimeOfDay);
            return combined;
        }


        /// <summary>
        /// Provides the time corresponding to the given record index.
        /// </summary>
        /// <param name="aRecordIndex"></param>
        /// <returns></returns>
        private DateTime RecordTime(int aRecordIndex)
        {
            return StartTime.AddSeconds(aRecordIndex * RecordDurationInSeconds.Value);
        }


        /// <summary>
        /// Provides the time corresponding to the given signal sample with millisecond precision.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="sampleIndex"></param>
        /// <returns></returns>
        public DateTime SampleTime(EDFSignal signal, int sampleIndex)
        {
            int recordIndex = sampleIndex / signal.NumberOfSamplesInDataRecord.Value;
            int modulo = sampleIndex % signal.NumberOfSamplesInDataRecord.Value;
            DateTime recordTime = RecordTime(recordIndex);
            // That will only give us milliseconds precision
            DateTime sampleTime = recordTime.AddMilliseconds(RecordDurationInSeconds.Value * 1000 * modulo / signal.NumberOfSamplesInDataRecord.Value);
            return sampleTime;
        }



        /// <summary>
        /// Useful for debug and inspection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strOutput = "";

            strOutput += "\n---------- Header ---------\n";
            strOutput += "8b\tVersion [" + Version.Value + "]\n";
            strOutput += "80b\tPatient ID [" + PatientID.Value + "]\n";
            strOutput += "80b\tRecording ID [" + RecordID.Value + "]\n";
            strOutput += "8b\tRecording start date [" + RecordingStartDate.Value + "]\n";
            strOutput += "8b\tRecording start time [" + RecordingStartTime.Value + "]\n";
            strOutput += "8b\tHeader size (bytes) [" + SizeInBytes.Value + "]\n";
            strOutput += "44b\tReserved [" + Reserved.Value + "]\n";
            strOutput += "8b\tRecord count [" + NumberOfDataRecords.Value + "]\n";
            strOutput += "8b\tRecord duration in seconds [" + RecordDurationInSeconds.Value + "]\n";
            strOutput += "4b\tSignal count [" + NumberOfSignalsInRecord.Value + "]\n\n";
            //strOutput += "First record time: " + FirstRecordTime + "\n\n";

            // For each signal
            for (int i = 0; i < NumberOfSignalsInRecord.Value; i++)
            {
                strOutput += "\tSignal " + i + ": " + Labels.Value[i] + "\n\n";
                //strOutput += "\tLabel [" + Signals.Labels.Value[i] + "]\n";
                strOutput += "\t\tTransducer type [" + TransducerTypes.Value[i] + "]\n";
                strOutput += "\t\tPhysical dimension [" + PhysicalDimensions.Value[i] + "]\n";
                strOutput += "\t\tPhysical minimum [" + PhysicalMinimums.Value[i] + "]\n";
                strOutput += "\t\tPhysical maximum [" + PhysicalMaximums.Value[i] + "]\n";
                strOutput += "\t\tDigital minimum [" + DigitalMinimums.Value[i] + "]\n";
                strOutput += "\t\tDigital maximum [" + DigitalMaximums.Value[i] + "]\n";
                strOutput += "\t\tPrefiltering [" + PreFilterings.Value[i] + "]\n";
                strOutput += "\t\tSample count per record [" + NumberOfSamplesPerRecord.Value[i] + "]\n";
                strOutput += "\t\tSignals reserved [" + SignalsReserved.Value[i] + "]\n\n";
            }

            strOutput += "\n-----------------------------------\n";

            return strOutput;
        }

        protected bool Equals(EDFHeader other)
        {
            return StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime) &&
                   Equals(Version, other.Version) && 
                   Equals(PatientID, other.PatientID) &&
                   Equals(RecordID, other.RecordID) && 
                   Equals(RecordingStartDate, other.RecordingStartDate) &&
                   Equals(RecordingStartTime, other.RecordingStartTime) && 
                   Equals(SizeInBytes, other.SizeInBytes) &&
                   Equals(Reserved, other.Reserved) && 
                   Equals(NumberOfDataRecords, other.NumberOfDataRecords) &&
                   Equals(RecordDurationInSeconds, other.RecordDurationInSeconds) &&
                   Equals(NumberOfSignalsInRecord, other.NumberOfSignalsInRecord) && 
                   Equals(Labels, other.Labels) &&
                   Equals(TransducerTypes, other.TransducerTypes) &&
                   Equals(PhysicalDimensions, other.PhysicalDimensions) &&
                   Equals(PhysicalMinimums, other.PhysicalMinimums) &&
                   Equals(PhysicalMaximums, other.PhysicalMaximums) && 
                   Equals(DigitalMinimums, other.DigitalMinimums) &&
                   Equals(DigitalMaximums, other.DigitalMaximums) && 
                   Equals(PreFilterings, other.PreFilterings) &&
                   Equals(NumberOfSamplesPerRecord, other.NumberOfSamplesPerRecord) &&
                   Equals(SignalsReserved, other.SignalsReserved);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EDFHeader)obj);
        }

        public override int GetHashCode()
        {
#if NET
            var hashCode = new HashCode();
            hashCode.Add(StartTime);
            hashCode.Add(EndTime);
            hashCode.Add(Version);
            hashCode.Add(PatientID);
            hashCode.Add(RecordID);
            hashCode.Add(RecordingStartDate);
            hashCode.Add(RecordingStartTime);
            hashCode.Add(SizeInBytes);
            hashCode.Add(Reserved);
            hashCode.Add(NumberOfDataRecords);
            hashCode.Add(RecordDurationInSeconds);
            hashCode.Add(NumberOfSignalsInRecord);
            hashCode.Add(Labels);
            hashCode.Add(TransducerTypes);
            hashCode.Add(PhysicalDimensions);
            hashCode.Add(PhysicalMinimums);
            hashCode.Add(PhysicalMaximums);
            hashCode.Add(DigitalMinimums);
            hashCode.Add(DigitalMaximums);
            hashCode.Add(PreFilterings);
            hashCode.Add(NumberOfSamplesPerRecord);
            hashCode.Add(SignalsReserved);
            return hashCode.ToHashCode();
#else
return 0;
#endif
        }
    }
}
