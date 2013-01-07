using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading;
using Caliburn.Micro;
using Action = System.Action;

namespace ConeTinue.Domain
{
	public class DelayedPropertyChangedBase : PropertyChangedBase
	{
		static DelayedPropertyChangedBase()
		{
			timer = new Timer(_ =>
				{
					if (itemsToNotify.Count == 0)
						return;
					
					foreach (var item in itemsToNotify.ToArray())
					{
						Action dummy;
						itemsToNotify.TryRemove(item.Key, out dummy);
						item.Value();
					}
				});
			
			timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(250));
		}
		
		private static readonly ConcurrentDictionary<Tuple<DelayedPropertyChangedBase, string>,System.Action> itemsToNotify = new ConcurrentDictionary<Tuple<DelayedPropertyChangedBase, string>, Action>();
		private static readonly Timer timer;

		public override void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
		{
			var propertyName = property.GetMemberInfo().Name;
			itemsToNotify.TryAdd(new Tuple<DelayedPropertyChangedBase, string>(this, propertyName), () => base.NotifyOfPropertyChange(propertyName));
		}

		public override void NotifyOfPropertyChange(string propertyName)
		{
			itemsToNotify.TryAdd(new Tuple<DelayedPropertyChangedBase, string>(this, propertyName), () => base.NotifyOfPropertyChange(propertyName));
		}
	}
}