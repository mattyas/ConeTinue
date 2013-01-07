using System;

namespace ConeTinue.Domain
{
	[Serializable]
	public class TestKey : IEquatable<TestKey>
	{
		public string FullName { get; set; }
		public TestAssembly TestAssembly { get; set; }

		public bool Equals(TestKey other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(FullName, other.FullName) && TestAssembly.Equals(other.TestAssembly);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TestKey) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (FullName.GetHashCode()*397) ^ TestAssembly.GetHashCode();
			}
		}

		public static bool operator ==(TestKey left, TestKey right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TestKey left, TestKey right)
		{
			return !Equals(left, right);
		}
	}
}