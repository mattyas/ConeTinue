using System;
using System.IO;
using System.Text;

namespace ConeTinue.Domain.CrossDomain
{
	public class InfoWriter : TextWriter
	{
		private readonly Action<string> write;


		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		public InfoWriter(IUpdateStatus updateStatus)
		{
			write = updateStatus.ReportInfo;
		}

		public InfoWriter(StringBuilder stringBuilder)
		{
			write = info => stringBuilder.Append(info);
		}

		public override void Write(string value)
		{
			if (currentTestKey != lastTestKey)
			{
				lastTestKey = currentTestKey;
				if (currentTestKey != null)
				write(string.Format("\r\n[Output from: {0}]\r\n", currentTestKey.FullName));
			}

			var chunkSize = 10000;
			if (value.Length < chunkSize)
				write(value);
			else
			{
				for (int i = 0; i < value.Length; i += chunkSize)
				{
					write(value.Substring(i, Math.Min(chunkSize, value.Length - i)));
				}
			}
		}

		public override void WriteLine(string value)
		{
			Write(value + Environment.NewLine);
		}

		public override void Write(bool value)
		{
			Write(value.ToString());
		}

		public override void Write(char value)
		{
			Write(value.ToString());
		}

		public override void Write(char[] buffer)
		{
			Write(new string(buffer));
		}

		public override void Write(char[] buffer, int index, int count)
		{
			Write(new string(buffer, index, count));
		}

		public override void Write(Decimal value)
		{
			Write(value.ToString());
		}

		public override void Write(double value)
		{
			Write(value.ToString());
		}

		public override void Write(float value)
		{
			Write(value.ToString());
		}

		public override void Write(int value)
		{
			Write(value.ToString());
		}

		public override void Write(long value)
		{
			Write(value.ToString());
		}

		public override void Write(string format, object arg0)
		{
			WriteLine(string.Format(format, arg0));
		}

		public override void Write(string format, object arg0, object arg1)
		{
			WriteLine(string.Format(format, arg0, arg1));
		}

		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			WriteLine(string.Format(format, arg0, arg1, arg2));
		}

		public override void Write(string format, params object[] arg)
		{
			WriteLine(string.Format(format, arg));
		}

		public override void Write(uint value)
		{
			WriteLine(value.ToString());
		}

		public override void Write(ulong value)
		{
			WriteLine(value.ToString());
		}

		public override void Write(object value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine()
		{
			Write(Environment.NewLine);
		}

		public override void WriteLine(bool value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(char value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(char[] buffer)
		{
			WriteLine(new string(buffer));
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			WriteLine(new string(buffer, index, count));
		}

		public override void WriteLine(Decimal value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(double value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(float value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(int value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(long value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(string format, object arg0)
		{
			WriteLine(string.Format(format, arg0));
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			WriteLine(string.Format(format, arg0, arg1));
		}

		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			WriteLine(string.Format(format, arg0, arg1, arg2));
		}

		public override void WriteLine(string format, params object[] arg)
		{
			WriteLine(string.Format(format, arg));
		}

		public override void WriteLine(uint value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(ulong value)
		{
			WriteLine(value.ToString());
		}

		public override void WriteLine(object value)
		{
			WriteLine(value.ToString());
		}

		private TestKey lastTestKey;
		private TestKey currentTestKey;
		public void SetCurrentTest(TestKey testKey)
		{
			currentTestKey = testKey;
		}
	}
}