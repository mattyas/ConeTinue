using System;
using System.IO;
using Caliburn.Micro;

namespace ConeTinue.Domain
{
	[Serializable]
	public class TestAssembly : PropertyChangedBase, IEquatable<TestAssembly>
	{
		private readonly string assemblyPath;
		private int number;

		public TestAssembly()
		{

		}
		public TestAssembly(string assemblyPath)
		{
			this.assemblyPath = assemblyPath;
		}


		public virtual bool IsAssembly { get { return true; } }
		public string AssemblyPath { get { return assemblyPath; } }
		public string AssemblyDirectory { get { return Path.GetDirectoryName(Path.GetFullPath(assemblyPath)); } }

		public int Number
		{
			get { return number; }
			set
			{
				if (value == number) return;
				number = value;
				NotifyOfPropertyChange(() => Number);
			}
		}

		public string AssemblyFileName
		{
			get { return Path.GetFileName(assemblyPath); }
		}

		public bool Equals(TestAssembly other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(assemblyPath, other.assemblyPath);
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TestAssembly) obj);
		}

		public override int GetHashCode()
		{
			return assemblyPath.GetHashCode();
		}

		public static bool operator ==(TestAssembly left, TestAssembly right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TestAssembly left, TestAssembly right)
		{
			return !Equals(left, right);
		}
	}

	public class NoTestAssembly : TestAssembly, IEquatable<NoTestAssembly>
	{
		public NoTestAssembly() : base(string.Empty)
		{
			
		}
		public bool Equals(NoTestAssembly other)
		{
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NoTestAssembly) obj);
		}

		public override int GetHashCode()
		{
			return 56;
		}

		public static bool operator ==(NoTestAssembly left, NoTestAssembly right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(NoTestAssembly left, NoTestAssembly right)
		{
			return !Equals(left, right);
		}

		public override bool IsAssembly
		{
			get
			{
				return false;
			}
		}
	}
}