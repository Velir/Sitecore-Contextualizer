using Sitecore.Data.Items;

namespace Sitecore.SharedSource.Contextualizer.Filters
{
	public class FilterArgs
	{
		#region Properties

		private Item _contentItem;
		/// <summary>
		/// The content item that was clicked when opening
		/// the context menu.
		/// </summary>
		public Item ContentItem
		{
			get
			{
				//return our content item
				return _contentItem;
			}
		}

		private Item _filterItem;
		/// <summary>
		/// The filter item that contains all the settings
		/// for the current filter.
		/// </summary>
		public Item FilterItem
		{
			get
			{
				//return our filter item
				return _filterItem;
			}
		}

		/// <summary>
		/// Whether or not we will hide our command.
		/// </summary>
		public bool HideCommand { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Will create a new filter args object for passing into
		/// a menu item filter.
		/// </summary>
		/// <param name="contentItem">The currently selected content item.</param>
		/// <param name="filterItem">The current filter containing any filter settings.</param>
		public FilterArgs(Item contentItem, Item filterItem)
		{
			this._contentItem = contentItem;
			this._filterItem = filterItem;
		}

		#endregion
	}
}
