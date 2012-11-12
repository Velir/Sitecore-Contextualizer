using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.SharedSource.Contextualizer.CustomItems.Filters;

namespace Sitecore.SharedSource.Contextualizer.Filters
{
	public class TemplateFilter : IFilter
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
			TemplateFilterItem filterItem = args.FilterItem;

			//check if our filter item is null
			if (filterItem.IsNull())
			{
				return;
			}

			//get our exclude templates to filter off of
			List<Item> excludeTemplates = new List<Item>();
			if (filterItem.ExcludeTemplates != null)
			{
				excludeTemplates.AddRange(filterItem.ExcludeTemplates.GetItems());
			}

			//go through each exclude filter and if our item is of an exclude template filter
			//then set hide to true and return.  Exclude will take precedence over include.
			foreach (Item excludeTemplate in excludeTemplates)
			{
				if (contentItem.IsOfTemplate(excludeTemplate.ID.ToString()))
				{
					args.HideCommand = true;
					return;
				}
			}

			//get our include templates to filter off of
			List<Item> includeTemplates = new List<Item>();
			if (filterItem.IncludeTemplates != null)
			{
				includeTemplates.AddRange(filterItem.IncludeTemplates.GetItems());
			}

			//if we have include templates then we need to check each template and make sure our
			//content item is at least of one of those template types.
			foreach (Item includeTemplate in includeTemplates)
			{
				if (contentItem.IsOfTemplate(includeTemplate.ID.ToString()))
				{
					args.HideCommand = false;
					return;
				}
			}

			//if we get here then our template was neither included or excluded.
			//if we have specified include templates, then we should hide by default
			//as we only want to show templates that were in our include list.
			if (includeTemplates.Any())
			{
				args.HideCommand = true;
				return;
			}

			//by default just include our item.  We know it wasn't excluded and we don't have
			//a specific include list
			args.HideCommand = false;
		}

		#endregion
	}
}
