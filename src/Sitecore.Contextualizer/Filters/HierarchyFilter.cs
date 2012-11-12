using Sitecore.Data.Items;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.SharedSource.Contextualizer.CustomItems.Filters;

namespace Sitecore.SharedSource.Contextualizer.Filters
{
	public class HierarchyFilter : IFilter
	{
		#region IFilter

		public void Process(FilterArgs args)
		{
			//get current content item from our filter args.
			Item contentItem = args.ContentItem;

			//check if the content item is null
			if (contentItem.IsNull())
			{
				return;
			}

			//get current filter item from our filter args.
			//this will be used to pull out any filter settings.
			HierarchyFilterItem filterItem = args.FilterItem;

			//check if our filter item is null
			if (filterItem.IsNull())
			{
				return;
			}

			//verify that we also have a root item and if not return
			if (filterItem.RootItem == null || filterItem.RootItem.TargetItem.IsNull())
			{
				return;
			}

			//get our root item to filter off of
			Item rootItem = filterItem.RootItem.TargetItem;

			//hide our menu item if our content item is not a descendant of our root item.
			args.HideCommand = !contentItem.Axes.IsDescendantOf(rootItem);
		}

		#endregion
	}
}
