﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EDFCSharp
{
    public class EDFFile : IDisposable
    {
        public EDFHeader Header { get; set; }
        public EDFSignal[] Signals { get; set; }
        public List<TAL> Annotations { get; set; }
        public List<AnnotationSignal> AnnotationSignals { get; set; }
        private Reader Reader { get; set; }

        public EDFFile()
        {
            Annotations = new List<TAL>();
            AnnotationSignals = new List<AnnotationSignal>();
        }
        public EDFFile(string filePath) : this()
        {
            ReadAll(filePath);
        }

        public EDFFile(byte[] edfBytes) : this()
        {
            ReadAll(edfBytes);
        }


        public override string ToString()
        {
            return $@"Header: {Header}";
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edfBase64"></param>
        public void ReadBase64(string edfBase64)
        {
            byte[] edfBytes = Convert.FromBase64String(edfBase64);
            ReadAll(edfBytes);
        }

        /// <summary>
        /// Open the given EDF file, read its header and allocate corresponding Signal objects.
        /// </summary>
        /// <param name="filePath"></param>
        public void Open(string filePath)
        {
            // Open file
            Reader = new Reader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
            Header = Reader.ReadHeader();
            Signals = Reader.AllocateSignals(Header);

        }

        /// <summary>
        /// Read the signal at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void ReadSignal(int index)
        {
            Reader.ReadSignal(Header, Signals[index]);
        }

        /// <summary>
        /// Read the signal matching the given name.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public EDFSignal ReadSignal(string match)
        {
            var signal = Signals.FirstOrDefault(s => s.Label.Value.Equals(match));
            if (signal == null)
            {
                return null;
            }

            Reader.ReadSignal(Header, signal);
            return signal;
        }

        public static EDFHeader ReadHeader(string filename)
        {
            using (var reader = new Reader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                return reader.ReadHeader();
            }
        }
        /// <summary>
        /// Read the whole file into memory
        /// </summary>
        /// <param name="edfFilePath"></param>
        public void ReadAll(string edfFilePath)
        {
            using (var reader = new Reader(File.Open(edfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                Header = reader.ReadHeader();
                var result = reader.ReadSignalsAndAnnotations(Header);
                Signals = result.Signals;
                Annotations = result.Annotations;
            }
        }

        /// <summary>
        /// Read a whole EDF file from a memory buffer. 
        /// </summary>
        /// <param name="edfBytes"></param>
        public void ReadAll(byte[] edfBytes)
        {
            using (var reader = new Reader(edfBytes))
            {
                Header = reader.ReadHeader();
                var result = reader.ReadSignalsAndAnnotations(Header);
                Signals = result.Signals;
                Annotations = result.Annotations;
            }
        }

        public void Save(string filePath)
        {
            if (Header == null) return;

            using (var writer = new EDFWriter(File.Open(filePath, FileMode.Create)))
            {
                writer.WriteEDF(this, filePath);
            }
        }
    }
}
