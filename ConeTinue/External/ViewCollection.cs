namespace ConeTinue.External
{
	#region Namespaces
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Windows;
	using System.Windows.Controls;
	using Caliburn.Micro;

	#endregion

	/// <summary>
	///   Class representing a view collection that gets populated through a view-model collection.
	/// </summary>
	/// <remarks>
	///   The usage of this class should be restricted to the cases where it is not possible to inject view directly into a <see
	///    cref = "System.Windows.Controls.ItemsControl" />.
	/// </remarks>
	public sealed class ViewCollection : Freezable, IList<FrameworkElement>, INotifyCollectionChanged, INotifyPropertyChanged, IList
	{
		#region Static Fields
		/// <summary>
		///   The DisplayLocation dependency property.
		/// </summary>
		public static readonly DependencyProperty DisplayLocationProperty = DependencyProperty.Register("DisplayLocation", typeof(DependencyObject), typeof(ViewCollection), new FrameworkPropertyMetadata(null, OnDisplayLocationChanged));

		/// <summary>
		///   The Context dependency property.
		/// </summary>
		public static readonly DependencyProperty ContextProperty = DependencyProperty.Register("Context", typeof(object), typeof(ViewCollection), new FrameworkPropertyMetadata(null, OnContextChanged));

		/// <summary>
		///   The indexer property changed args.
		/// </summary>
		private static readonly PropertyChangedEventArgs _indexerArgs = new PropertyChangedEventArgs("[]");

		/// <summary>
		///   The count property changed args.
		/// </summary>
		private static readonly PropertyChangedEventArgs _countArgs = new PropertyChangedEventArgs("Count");

		/// <summary>
		///   The Source dependency property.
		/// </summary>
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(ViewCollection), new FrameworkPropertyMetadata(null, OnSourceChanged), OnValidateSourceProperty);
		#endregion

		#region Static Members
		/// <summary>
		///   Handles changes to the DisplayLocation property.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnDisplayLocationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			((ViewCollection)obj).OnDisplayLocationChanged(e);
		}

		/// <summary>
		///   Handles changes to the Context property.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			((ViewCollection)obj).OnContextChanged(e);
		}

		/// <summary>
		///   Called when source property has to be validated.
		/// </summary>
		/// <param name = "value">The value.</param>
		/// <returns><c>True</c> if the value is valid; otherwise, <c>false</c>.</returns>
		private static bool OnValidateSourceProperty(object value)
		{
			return value == null || (value is INotifyPropertyChanged && value is IEnumerable);
		}

		/// <summary>
		///   Handles changes to the Source property.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			((ViewCollection)obj).OnSourceChanged(e);
		}
		#endregion

		#region Events
		/// <summary>
		///   Occurs when the collection changes.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		///   Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Fields
		/// <summary>
		///   The collection used to store the elements.
		/// </summary>
		private readonly List<FrameworkElement> _items;
		#endregion

		#region Properties
		/// <summary>
		///   Gets or sets the display location of the views.
		/// </summary>
		/// <value>The display location of the views.</value>
		public DependencyObject DisplayLocation
		{
			get { return (DependencyObject)GetValue(DisplayLocationProperty); }
			set { SetValue(DisplayLocationProperty, value); }
		}

		/// <summary>
		///   Gets or sets the views context.
		/// </summary>
		/// <value>The views context.</value>
		public object Context
		{
			get { return GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		/// <summary>
		///   Gets or sets the source of the collection.
		/// </summary>
		/// <value>The source of the collection.</value>
		public object Source
		{
			get { return GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		/// <summary>
		///   Gets or sets the item at the specified index.
		/// </summary>
		/// <returns>The item at the specified index.</returns>
		/// <exception cref = "T:System.ArgumentOutOfRangeException"><paramref name = "index" /> is not a valid index.</exception>
		/// <exception cref = "T:System.NotSupportedException">The property is set (the collection is read-only).</exception>
		FrameworkElement IList<FrameworkElement>.this[int index]
		{
			get { return this[index]; }
			set { throw new NotSupportedException("Collection is read-only"); }
		}

		/// <summary>
		///   Gets the item at the specified index.
		/// </summary>
		/// <returns>The item at the specified index.</returns>
		/// <exception cref = "T:System.ArgumentOutOfRangeException"><paramref name = "index" /> is not a valid index.</exception>
		public FrameworkElement this[int index]
		{
			get
			{
				ReadPreamble();
				return _items[index];
			}
		}

		/// <summary>
		///   Gets the number of items in the collection.
		/// </summary>
		/// <returns>The number of items in the collection.</returns>
		public int Count
		{
			get
			{
				ReadPreamble();
				return _items.Count;
			}
		}

		/// <summary>
		///   Gets a value used to determine if the collection is read-only.
		/// </summary>
		/// <returns><c>True</c> if the collection is read-only; otherwise, <c>false</c>.</returns>
		bool ICollection<FrameworkElement>.IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		///   Gets a value indicating whether the <see cref = "T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		/// <returns>true if the <see cref = "T:System.Collections.IList" /> has a fixed size; otherwise, false.</returns>
		bool IList.IsFixedSize
		{
			get { return false; }
		}

		/// <summary>
		///   Gets a value used to determine if the collection is read-only.
		/// </summary>
		/// <returns><c>True</c> if the collection is read-only; otherwise, <c>false</c>.</returns>
		bool IList.IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		///   Gets or sets the item at the specified index.
		/// </summary>
		/// <returns>The item .</returns>
		/// <exception cref = "T:System.ArgumentOutOfRangeException"><paramref name = "index" /> non è un indice valido nell'interfaccia <see
		///    cref = "T:System.Collections.Generic.IList`1" />.</exception>
		/// <exception cref = "T:System.NotSupportedException">The property is set and the collection is read-only.</exception>
		object IList.this[int index]
		{
			get { return this[index]; }
			set { throw new NotSupportedException("Collection is read-only"); }
		}

		/// <summary>
		///   Gets a value indicating whether access to the <see cref = "T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		/// <returns>true if access to the <see cref = "T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				ReadPreamble();
				return ((ICollection)_items).IsSynchronized;
			}
		}

		/// <summary>
		///   Gets an object that can be used to synchronize access to the <see cref = "T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>An object that can be used to synchronize access to the <see cref = "T:System.Collections.ICollection" />.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				ReadPreamble();
				return ((ICollection)_items).SyncRoot;
			}
		}
		#endregion

		/// <summary>
		///   Initializes a new instance of the <see cref = "ViewCollection" /> class.
		/// </summary>
		public ViewCollection()
		{
			WritePreamble();
			_items = new List<FrameworkElement>();
			WritePostscript();
		}

		#region IList Members
		/// <summary>
		///   Adds an item to the <see cref = "T:System.Collections.IList" />.
		/// </summary>
		/// <param name = "value">The object to add to the <see cref = "T:System.Collections.IList" />.</param>
		/// <returns>
		///   The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
		/// </returns>
		/// <exception cref = "T:System.NotSupportedException">The <see cref = "T:System.Collections.IList" /> is read-only.-or- The <see
		///    cref = "T:System.Collections.IList" /> has a fixed size. </exception>
		int IList.Add(object value)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Clears all the items from the collection.
		/// </summary>
		/// <exception cref = "T:System.NotSupportedException">The collection is read-only. </exception>
		void IList.Clear()
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Determines whether the <see cref = "T:System.Collections.IList" /> contains a specific value.
		/// </summary>
		/// <param name = "value">The object to locate in the <see cref = "T:System.Collections.IList" />.</param>
		/// <returns>
		///   true if the <see cref = "T:System.Object" /> is found in the <see cref = "T:System.Collections.IList" />; otherwise, false.
		/// </returns>
		bool IList.Contains(object value)
		{
			return Contains((FrameworkElement)value);
		}

		/// <summary>
		///   Determines the index of a specific item in the <see cref = "T:System.Collections.IList" />.
		/// </summary>
		/// <param name = "value">The object to locate in the <see cref = "T:System.Collections.IList" />.</param>
		/// <returns>
		///   The index of <paramref name = "value" /> if found in the list; otherwise, -1.
		/// </returns>
		int IList.IndexOf(object value)
		{
			return IndexOf((FrameworkElement)value);
		}

		/// <summary>
		///   Inserts an item to the <see cref = "T:System.Collections.IList" /> at the specified index.
		/// </summary>
		/// <param name = "index">The zero-based index at which <paramref name = "value" /> should be inserted.</param>
		/// <param name = "value">The object to insert into the <see cref = "T:System.Collections.IList" />.</param>
		/// <exception cref = "T:System.ArgumentOutOfRangeException">
		///   <paramref name = "index" /> is not a valid index in the <see cref = "T:System.Collections.IList" />. </exception>
		/// <exception cref = "T:System.NotSupportedException">The <see cref = "T:System.Collections.IList" /> is read-only.-or- The <see
		///    cref = "T:System.Collections.IList" /> has a fixed size. </exception>
		/// <exception cref = "T:System.NullReferenceException">
		///   <paramref name = "value" /> is null reference in the <see cref = "T:System.Collections.IList" />.</exception>
		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Removes the first occurrence of a specific object from the <see cref = "T:System.Collections.IList" />.
		/// </summary>
		/// <param name = "value">The object to remove from the <see cref = "T:System.Collections.IList" />.</param>
		/// <exception cref = "T:System.NotSupportedException">The <see cref = "T:System.Collections.IList" /> is read-only.-or- The <see
		///    cref = "T:System.Collections.IList" /> has a fixed size. </exception>
		void IList.Remove(object value)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Removes the element at the specified index.
		/// </summary>
		/// <param name = "index">The zero-based index to remove the item at.</param>
		/// <exception cref = "T:System.ArgumentOutOfRangeException">The index is not valid.</exception>
		/// <exception cref = "T:System.NotSupportedException">The collection is read-only.</exception>
		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Copies the elements of the <see cref = "T:System.Collections.ICollection" /> to an <see cref = "T:System.Array" />, starting at a particular <see
		///    cref = "T:System.Array" /> index.
		/// </summary>
		/// <param name = "array">The one-dimensional <see cref = "T:System.Array" /> that is the destination of the elements copied from <see
		///    cref = "T:System.Collections.ICollection" />. The <see cref = "T:System.Array" /> must have zero-based indexing.</param>
		/// <param name = "index">The zero-based index in <paramref name = "array" /> at which copying begins.</param>
		/// <exception cref = "T:System.ArgumentNullException">
		///   <paramref name = "array" /> is null. </exception>
		/// <exception cref = "T:System.ArgumentOutOfRangeException">
		///   <paramref name = "index" /> is less than zero. </exception>
		/// <exception cref = "T:System.ArgumentException">
		///   <paramref name = "array" /> is multidimensional.-or- The number of elements in the source <see
		///    cref = "T:System.Collections.ICollection" /> is greater than the available space from <paramref name = "index" /> to the end of the destination <paramref
		///    name = "array" />. </exception>
		/// <exception cref = "T:System.ArgumentException">The type of the source <see cref = "T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref
		///    name = "array" />. </exception>
		void ICollection.CopyTo(Array array, int index)
		{
			ReadPreamble();
			((ICollection)_items).CopyTo(array, index);
		}
		#endregion

		#region IList<FrameworkElement> Members
		/// <summary>
		///   Determines the index of an item.
		/// </summary>
		/// <param name = "item">The item to retrieve the index for.</param>
		/// <returns>
		///   The index of the item or -1 if the item was not present in the collection.
		/// </returns>
		public int IndexOf(FrameworkElement item)
		{
			ReadPreamble();
			return _items.IndexOf(item);
		}

		/// <summary>
		///   Inserts an item into the collection.
		/// </summary>
		/// <param name = "index">The index to insert the item at.</param>
		/// <param name = "item">The item to insert.</param>
		/// <exception cref = "System.NotSupportedException">The collection is read-only.</exception>
		void IList<FrameworkElement>.Insert(int index, FrameworkElement item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Removes an item from the collection.
		/// </summary>
		/// <param name = "index">The index to remove the item from.</param>
		/// <exception cref = "System.NotSupportedException">The collection is read-only.</exception>
		void IList<FrameworkElement>.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Adds an item to the collection.
		/// </summary>
		/// <param name = "item">The item to add.</param>
		/// <exception cref = "T:System.NotSupportedException">The collection is read-only. </exception>
		void ICollection<FrameworkElement>.Add(FrameworkElement item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Removes all items from the collection.
		/// </summary>
		/// <exception cref = "T:System.NotSupportedException">The collection is read-only. </exception>
		void ICollection<FrameworkElement>.Clear()
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Determines if the collection contains the specified item.
		/// </summary>
		/// <param name = "item">The item to check.</param>
		/// <returns>
		///   <c>True</c> if the item is contained in the collection; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(FrameworkElement item)
		{
			ReadPreamble();
			return _items.Contains(item);
		}

		/// <summary>
		///   Copies the elements to the specified array.
		/// </summary>
		/// <param name = "array">The array.</param>
		/// <param name = "arrayIndex">The index of the array to start the copy to.</param>
		public void CopyTo(FrameworkElement[] array, int arrayIndex)
		{
			ReadPreamble();
			_items.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///   Removes the first occurrence of the specified object in the collection.
		/// </summary>
		/// <param name = "item">The item to be removed.</param>
		/// <returns>
		///   <c>True</c> if the item was found and removed; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref = "T:System.NotSupportedException">The collection si read-only.</exception>
		bool ICollection<FrameworkElement>.Remove(FrameworkElement item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		/// <summary>
		///   Restituisce un enumeratore che consente di scorrere l'insieme.
		/// </summary>
		/// <returns>
		///   Interfaccia <see cref = "T:System.Collections.Generic.IEnumerator`1" /> che può essere utilizzata per scorrere l'insieme.
		/// </returns>
		public IEnumerator<FrameworkElement> GetEnumerator()
		{
			ReadPreamble();
			return _items.GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		/// <summary>
		///   Provides derived classes an opportunity to handle changes to the DisplayLocation property.
		/// </summary>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private void OnDisplayLocationChanged(DependencyPropertyChangedEventArgs e)
		{
			Reset();
		}

		/// <summary>
		///   Provides derived classes an opportunity to handle changes to the Context property.
		/// </summary>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private void OnContextChanged(DependencyPropertyChangedEventArgs e)
		{
			Reset();
		}

		/// <summary>
		///   Resets the collection.
		/// </summary>
		private void Reset()
		{
			var collection = (IEnumerable)Source;
			WritePreamble();
			_items.Clear();
			WritePostscript();
			OnCollectionResetted();

			if (collection != null)
				AddItems(collection);
		}

		/// <summary>
		///   Provides derived classes an opportunity to handle changes to the Source property.
		/// </summary>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private void OnSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldSource = (INotifyCollectionChanged)e.OldValue;
			if (oldSource != null)
				oldSource.CollectionChanged -= OnSourceCollectionChanged;

			var newSource = (INotifyCollectionChanged)e.NewValue;
			if (newSource != null)
				newSource.CollectionChanged += OnSourceCollectionChanged;

			Reset();
		}

		/// <summary>
		///   Called when the source collection has changed.
		/// </summary>
		/// <param name = "sender">The sender.</param>
		/// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
		private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				Reset();
			else
			{
				if (e.OldItems != null)
					RemoveItems(e.OldItems);
				if (e.NewItems != null)
					AddItems(e.NewItems);
			}
		}

		/// <summary>
		///   Removes the items from the collection.
		/// </summary>
		/// <param name = "oldItems">The old items.</param>
		private void RemoveItems(IEnumerable oldItems)
		{
			foreach (var oldItem in oldItems)
			{
				WritePreamble();
				FrameworkElement element = null;
				for (var i = _items.Count - 1; i >= 0; i--)
				{
					var current = _items[i];
					if (current.DataContext == oldItem)
					{
						element = current;
						_items.RemoveAt(i);
						break;
					}
				}
				WritePostscript();

				if (element != null)
					OnCollectionChanged(NotifyCollectionChangedAction.Remove, element);
			}
		}

		/// <summary>
		///   Adds the items to the collection.
		/// </summary>
		/// <param name = "newItems">The new items.</param>
		private void AddItems(IEnumerable newItems)
		{
			foreach (var newItem in newItems)
			{
				var view = ViewLocator.LocateForModel(newItem, null, Context);
				ViewModelBinder.Bind(newItem, view, Context);

				var element = view as FrameworkElement;

				if (element == null)
				{
					var contentControl = new ContentControl
					{
						Content = view
					};
					element = contentControl;
				}

				WritePreamble();
				_items.Add(element);
				WritePostscript();

				OnCollectionChanged(NotifyCollectionChangedAction.Add, element);
			}
		}

		/// <summary>
		///   Called when the collection has changed.
		/// </summary>
		/// <param name = "action">The action.</param>
		/// <param name = "element">The element.</param>
		private void OnCollectionChanged(NotifyCollectionChangedAction action, FrameworkElement element)
		{
			if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove)
				throw new NotSupportedException("Only Add and Remove operations are supported");

			OnPropertyChanged(_countArgs);
			OnPropertyChanged(_indexerArgs);
			var collectionChanged = CollectionChanged;
			if (collectionChanged != null)
				collectionChanged(this, new NotifyCollectionChangedEventArgs(action, element));
		}

		/// <summary>
		///   Called when the collection has been resetted.
		/// </summary>
		private void OnCollectionResetted()
		{
			OnPropertyChanged(_countArgs);
			OnPropertyChanged(_indexerArgs);
			var collectionChanged = CollectionChanged;
			if (collectionChanged != null)
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		///   Raises the <see cref = "PropertyChanged" /> event.
		/// </summary>
		/// <param name = "args">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
		private void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			var propertyChanged = PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, args);
		}

		/// <summary>
		///   When implemented in a derived class, creates a new instance of the <see cref = "T:System.Windows.Freezable" /> derived class.
		/// </summary>
		/// <returns>
		///   The new instance.
		/// </returns>
		protected override Freezable CreateInstanceCore()
		{
			return new ViewCollection();
		}
	}
}
