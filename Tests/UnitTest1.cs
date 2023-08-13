using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using EDFCSharp;

namespace EDFSharpTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Read_File()
        {
           string file =  Path.Combine(Environment.CurrentDirectory, "files", "Female57yrs 07-MAR-2009 00h00m00s APSG.edf");
           var edf2 = new EDFFile(file);
        }
        [TestMethod]
        public void Test_WriteRead_EDF_signals_only()
        {
            var ecgSig = new EDFSignal(0,10);
            ecgSig.Label.Value = "ECG";
            ecgSig.NumberOfSamplesInDataRecord.Value = 10; //Small number of samples for testing
            ecgSig.PhysicalDimension.Value = "mV";
            ecgSig.DigitalMinimum.Value = -2048;
            ecgSig.DigitalMaximum.Value = 2047;
            ecgSig.PhysicalMinimum.Value = -10.2325;
            ecgSig.PhysicalMaximum.Value = 10.2325;
            ecgSig.TransducerType.Value = "UNKNOWN";
            ecgSig.Prefiltering.Value = "UNKNOWN";
            ecgSig.Reserved.Value = "RESERVED";
            ecgSig.Samples = new List<short> { 100, 50, 23, 75, 12, 88, 73, 12, 34, 83 };

            var soundSig = new EDFSignal(1,10);
            soundSig.Label.Value = "SOUND";
            soundSig.NumberOfSamplesInDataRecord.Value = 10;//Small number of samples for testing
            soundSig.PhysicalDimension.Value = "mV";
            soundSig.DigitalMinimum.Value = -2048;
            soundSig.DigitalMaximum.Value = 2047;
            soundSig.PhysicalMinimum.Value = -44;
            soundSig.PhysicalMaximum.Value = 44.0;
            soundSig.TransducerType.Value = "UNKNOWN";
            soundSig.Prefiltering.Value = "UNKNOWN";
            soundSig.Samples = new List<short> { 11, 200, 300, 123, 87, 204, 145, 234, 222, 75 };
            soundSig.Reserved.Value = "RESERVED";

            var signals = new EDFSignal[2] { ecgSig, soundSig };

            var h = new EDFHeader();
            h.RecordDurationInSeconds.Value = 1;
            h.Version.Value = "0";
            h.PatientID.Value = "TEST PATIENT ID";
            h.RecordID.Value = "TEST RECORD ID";
            h.RecordingStartDate.Value = "11.11.16"; //dd.mm.yy
            h.RecordingStartTime.Value = "12.12.12"; //hh.mm.ss
            h.Reserved.Value = "RESERVED";
            h.NumberOfDataRecords.Value = 1;
            h.NumberOfSignalsInRecord.Value = (short)2;
            h.SignalsReserved.Value = Enumerable.Repeat("RESERVED", h.NumberOfSignalsInRecord.Value).ToArray();

            var edf1 = new EDFFile(h, signals, new List<AnnotationSignal>());

            string edfFilePath = @"test1.EDF";
            edf1.Save(edfFilePath);

            //Read the file back
            var edf2 = new EDFFile(edfFilePath);

            Assert.AreEqual(edf2.Header.Version.ToAscii(), edf1.Header.Version.ToAscii());
            Assert.AreEqual(edf2.Header.PatientID.ToAscii(), edf1.Header.PatientID.ToAscii());
            Assert.AreEqual(edf2.Header.RecordID.ToAscii(), edf1.Header.RecordID.ToAscii());
            Assert.AreEqual(edf2.Header.RecordingStartDate.ToAscii(), edf1.Header.RecordingStartDate.ToAscii());
            Assert.AreEqual(edf2.Header.RecordingStartTime.ToAscii(), edf1.Header.RecordingStartTime.ToAscii());
            Assert.AreEqual(edf2.Header.Reserved.ToAscii(), edf1.Header.Reserved.ToAscii());
            Assert.AreEqual(edf2.Header.NumberOfDataRecords.ToAscii(), edf1.Header.NumberOfDataRecords.ToAscii());
            Assert.AreEqual(edf2.Header.SignalsReserved.ToAscii(), edf1.Header.SignalsReserved.ToAscii());
            Assert.AreEqual(edf2.Signals[0].Samples.Count, edf1.Signals[0].Samples.Count);
            Assert.IsTrue(edf2.Signals[0].Equals(edf1.Signals[0]));
            Assert.IsTrue(edf2.Signals[1].Equals(edf1.Signals[1]));
            System.IO.File.Delete(edfFilePath);
        }

        [TestMethod]
        public void ReadSignalOnlyFile1()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "files", "signals_only.EDF");
            if (!File.Exists(filename))
            {
                return;
            }
            var edf = new EDFFile(filename);
            Console.WriteLine(edf.ToString());
            Console.WriteLine(edf.Header.StartTime);
            Console.WriteLine(edf.Header.EndTime);
            Console.WriteLine(edf.Header.TotalDurationInSeconds);
            TimeSpan t = TimeSpan.FromSeconds(edf.Header.TotalDurationInSeconds);
            Console.WriteLine(t);
        }
        [TestMethod]
        public void ReadSignalOnlyFile2()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "files", "signals_only2.EDF");
            if (!File.Exists(filename))
            {
                return;
            }
            var edf = new EDFFile(filename);
            Console.WriteLine(edf.ToString());
            Console.WriteLine(edf.Header.StartTime);
            Console.WriteLine(edf.Header.EndTime);
            Console.WriteLine(edf.Header.TotalDurationInSeconds);
            TimeSpan t = TimeSpan.FromSeconds(edf.Header.TotalDurationInSeconds);
            Console.WriteLine(t);
        }
        [TestMethod]
        public void ReadSampleFile()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "files", "sample_ecg.EDF");
            if (!File.Exists(filename))
            {
                return;
            }
            var edf = new EDFFile(filename);
            Console.WriteLine(edf.ToString());
            Console.WriteLine(edf.Header.StartTime);
            Console.WriteLine(edf.Header.EndTime);
            Console.WriteLine(edf.Header.TotalDurationInSeconds);
            TimeSpan t = TimeSpan.FromSeconds(edf.Header.TotalDurationInSeconds);
            Console.WriteLine(t);
        }
        [TestMethod]
        public void ReadAndSaveAnnotationOnlyFile()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "files", "annotations.EDF");
            if (!File.Exists(filename))
            {
                return;
            }
            var edf = new EDFFile(filename);
            Assert.IsTrue(edf.AnnotationSignals.Count == 8);
            Assert.IsTrue(edf.AnnotationSignals[0].SamplesCount == 145);

            string file2 = "test2.edf";
            edf.Save(file2);

            var edf2 = new EDFFile(file2);
            Assert.IsTrue( edf2.Header.Equals( edf.Header ),          "Saved Header does not match original" );
            Assert.IsTrue( edf2.Signals.SequenceEqual( edf.Signals ), "Saved Signals do not match original" );
            Assert.AreEqual( edf.AnnotationSignals.Count, edf2.AnnotationSignals.Count, "Saved file contains a different number of Annotation Signals than original." );
            
            for( int i = 0; i < edf.AnnotationSignals.Count; i++ )
            {
                var original = edf.AnnotationSignals[ i ];
                var compare  = edf2.AnnotationSignals[ i ];

                Assert.IsTrue( original.SamplesCount == compare.SamplesCount, "Annotation signals contain a differing number of samples" );

                for( int j = 0; j < original.SamplesCount; j++ )
                {
                    Assert.AreEqual( original.Samples[ j ].ToString().Trim(), compare.Samples[ j ].ToString().Trim(), false, "Stored Annotation samples differ" );
                }
            }
        }
        [TestMethod]
        public void ReadAnnotationAndSignalsFile()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "files", "annotations_and_signals.EDF");
            if (!File.Exists(filename))
            {
                return;
            }
            var edf = new EDFFile(filename);
            Assert.IsTrue(edf.AnnotationSignals.Sum(a => a.SamplesCount) == 2);

        }
        [TestMethod]
        public void ReadAnnotationAndSignalsFile2()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "files", "annotations_and_signals2.EDF");
            if (!File.Exists(filename))
            {
                return;
            }
            var edf = new EDFFile(filename);
            Assert.IsTrue(edf.AnnotationSignals.Sum(a => a.SamplesCount) == 25);
        }
        [TestMethod]
        public void ReadTemplateFile()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "files", "template.EDF");
            if (!File.Exists(filename))
            {
                return;
            }
            var edf = new EDFFile(filename);
            Console.WriteLine(edf.ToString());
            Console.WriteLine(edf.Header.StartTime);
            Console.WriteLine(edf.Header.EndTime);
            Console.WriteLine(edf.Header.TotalDurationInSeconds);
            TimeSpan t = TimeSpan.FromSeconds(edf.Header.TotalDurationInSeconds);
            Console.WriteLine(t);
        }
    }
}
