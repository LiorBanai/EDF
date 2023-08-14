using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDFCSharp
{
	/// <summary>
	/// Used to encode EDF+ patient identification subfields into Header.PatientID
	/// </summary>
	public class PatientIdentification
	{
		#region Public properties 

		public string Code { get; set; }
		public string Sex { get; set; }
		public DateTime? BirthDate { get; set; }
		public string PatientName { get; set; }

		public List<KeyValuePair<string,string>> AdditionalSubfields { get; set;  }

		#endregion

		#region Constructors 

		public PatientIdentification()
		{
			AdditionalSubfields = new List<KeyValuePair<string, string>>();
		}

		public PatientIdentification( string Code, string Sex, DateTime? BirthDate, string PatientName)
		{
			this.Code = Code;
			this.Sex = Sex;
			this.BirthDate = BirthDate;
			this.PatientName = PatientName;

			AdditionalSubfields = new List<KeyValuePair<string, string>>();
		}

		#endregion

		#region Public functions 

		public void AddField( string key, string value )
		{
			AdditionalSubfields.Add(new KeyValuePair<string, string>(key, value));
		}

		public static PatientIdentification Parse(string buffer, bool throwOnFormatInvalid = true)
		{
			if (TryParse(buffer, out PatientIdentification result))
			{
				return result;
			}

			if (throwOnFormatInvalid)
			{
				throw new FormatException($"The value '{buffer}' does not appear to be a valid format for {nameof(PatientIdentification)}");
			}

			return null;
		}

		public static bool TryParse( string buffer, out PatientIdentification result )
		{
			result = null;

			var parts = buffer.Split(' ');
			if ( parts.Length < 4 ) 
			{  
				return false; 
			}

			string code = parts[0].Replace( '_', ' ' ); 
			string sex = parts[1].Substring( 0, 1 );
			string patientName = parts[3].Replace('_', ' ');
			DateTime? birthDate = null;

			if (string.Compare(parts[2], "X", true) != 0)
			{
				if (!DateTime.TryParse(parts[2], out DateTime parsedDate))
				{
					return false;
				}
				birthDate = parsedDate;
			}

			if( string.Compare(code, "X", StringComparison.Ordinal ) == 0 ) { code = string.Empty; }
			if( string.Compare(sex, "X", StringComparison.Ordinal) == 0 ) { sex = string.Empty; }
			if( string.Compare(patientName, "X", StringComparison.Ordinal) == 0 ) { patientName = string.Empty; }

			var additionalFields = new List<KeyValuePair<string, string>>( parts.Length - 4 );

			for (int i = 4; i < parts.Length; i++)
			{
				var field = parts[i];

				int splitPos = field.IndexOf('=');
				if( splitPos == -1 )
				{
					return false;
				}

				var newPair = new KeyValuePair<string, string>(
					field.Substring(0, splitPos).Replace( '_', ' ' ), 
					field.Substring(splitPos + 1).Replace('_', ' ')
					);

				additionalFields.Add( newPair );
			}

			result = new PatientIdentification(code, sex, birthDate, patientName)
			{
				AdditionalSubfields = additionalFields
			};

			return true;
        }

		#endregion

		#region Base type overrides 

		public override string ToString()
		{
			var buffer = new StringBuilder();

			buffer.Append(string.IsNullOrEmpty(Code) ? "X" : Code?.Replace(' ', '_'));
			buffer.Append(' ');

			buffer.Append(string.IsNullOrEmpty(Sex) ? "X" : Sex?.Substring(0, 1));
			buffer.Append(' ');

			buffer.Append(BirthDate?.ToString("dd-MMM-yyyy") ?? "X");
			buffer.Append(' ');

			buffer.Append(string.IsNullOrEmpty(PatientName) ? "X" : PatientName?.Replace(' ', '_'));

			foreach( var pair in AdditionalSubfields )
			{
				buffer.Append(' ');
				buffer.Append(pair.Key.Replace(' ', '_'));
				buffer.Append('=');
				buffer.Append(pair.Value.Replace(' ', '_'));
			}

			return buffer.ToString();
		}

		#endregion

		#region Implicit Type Conversion 

		public static implicit operator FixedLengthString( PatientIdentification value )
		{
			return new FixedLengthString(HeaderItems.PatientID) 
			{  
				Value = value.ToString() 
			};	
		}

		public static implicit operator PatientIdentification( FixedLengthString value )
		{
			return Parse(value.Value);
		}

		#endregion
	}
}
