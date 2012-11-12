using System;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.SharedSource.Contextualizer.CustomItems;
using Sitecore.SharedSource.Contextualizer.CustomItems.Filters;
using Sitecore.SharedSource.Contextualizer.Utils;

namespace Sitecore.SharedSource.Contextualizer.CustomSitecore.Events
{
	public class ClearContextualMenuCache
	{
		protected void OnItemSaved(object sender, EventArgs args)
		{
			//if our args are null then just return
			if (args == null)
			{
				return;
			}

			//get our item that is being saved
			Item savedItem = Event.ExtractParameter(args, 0) as Item;

			//if we don't have an item then just return
			if (savedItem.IsNull())
			{
				return;
			}

			//if our saved item is a menu item or menu item filter then clear our cache
			if (savedItem.IsOfTemplate(CommandItem.TemplateId) || savedItem.IsOfTemplate(GenericFilterItem.TemplateId, 2))
			{
				CacheUtil.ClearAll();
			}
		}
	}
}
