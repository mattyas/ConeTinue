using System;

namespace ConeTinue.Domain
{
	public class TestCategory : IEquatable<TestCategory>
	{
		public bool Equals(TestCategory other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(category, other.category);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TestCategory) obj);
		}

		public override int GetHashCode()
		{
			return (category != null ? category.GetHashCode() : 0);
		}

		public static bool operator ==(TestCategory left, TestCategory right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TestCategory left, TestCategory right)
		{
			return !Equals(left, right);
		}

		public TestCategory(string category, int count)
		{
			this.category = category;
			this.count = count;
		}

		public static TestCategory None(int count) { return new NoTestCategory(count); }

		public virtual bool IsCategory { get { return true; } }
		public override string ToString() { return string.Format("{0} - {1} Tests",category,count); }
		public string Name { get { return category; } }
		public virtual bool Matches(TestItem item)
		{
			return item.Categories.Contains(category);
		}

		private readonly string category;
		private readonly int count;
	}
}