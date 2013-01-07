using System;

namespace ConeTinue.Domain
{
	public class NoTestCategory : TestCategory, IEquatable<NoTestCategory>
	{
		public bool Equals(NoTestCategory other)
		{
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NoTestCategory) obj);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public static bool operator ==(NoTestCategory left, NoTestCategory right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(NoTestCategory left, NoTestCategory right)
		{
			return !Equals(left, right);
		}

		private readonly int count;

		public NoTestCategory(int count) : base(string.Empty, count)
		{
			this.count = count;
		}

		public override bool IsCategory { get { return false; } }
		public override string ToString()
		{
			return string.Format("[No category] - {0} Tests", count);
		}
		public override bool Matches(TestItem item)
		{
			return item.Categories.Count == 0;
		}
	}
}