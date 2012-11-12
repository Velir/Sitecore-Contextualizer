using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.CodeDom.Scripts;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pipelines.GetQueryState;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.SharedSource.Contextualizer.CustomItems;
using Sitecore.SharedSource.Contextualizer.Filters;
using Sitecore.SharedSource.Contextualizer.Utils;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.SharedSource.Contextualizer.CustomSitecore.Pipelines
{
	public class FilterMenuItems
	{
		/// <summary>
		/// The id for the parent folder containing our commands to filter on.
		/// </summary>
		protected string CommandsParentFolderId = "{3C16B828-9F51-422B-9A02-B87F49C05C6A}";

		/// <summary>
		/// Method that will be called when a command item (contextual menu item, ribbon button, etc.) is about to be rendered.
		/// </summary>
		/// <param name="args"></param>
		public void Process(GetQueryStateArgs args)
		{
			//if we don't have a command context then return.  the command context will contain all the info we need.
			if (args.CommandContext == null)
			{
				return;
			}

			//get our current content item that was selected to display the menu.
			Item contentItem = null;
			if (args.CommandContext.Items != null && args.CommandContext.Items.Any())
			{
				contentItem = args.CommandContext.Items.FirstOrDefault();
			}

			//get our parent item that will contain all of the filterable commands
			Item parentMenuItem = Context.Database.GetItem(ID.Parse(CommandsParentFolderId));

			//if we didn't find our parent item we can't do anything, just return.
			if (parentMenuItem.IsNull())
			{
				return;
			}

			//get our list of menu items from our context menu parent.
			//first try to pull from cache based on the parent name,
			//otherwise get from parent item.
			Dictionary<string, object> menuItemArgs = new Dictionary<string, object> { { "ParentMenuItem", parentMenuItem } };
			Dictionary<string, Item> menuItems = CacheUtil.GetFromCacheWithParams<Dictionary<string, Item>>(parentMenuItem.Name,
																											new TimeSpan(0, 8, 0),
																											GetMenuItems,
																											menuItemArgs);

			//get our command name from the current menu item
			string commandName = args.CommandName;

			//if we don't have a command name then just return
			if (string.IsNullOrEmpty(commandName))
			{
				return;
			}

			//try to get the command item by using our command name to match against the
			//list of commands pulled from the cache.  If we couldn't find one then return.
			Item rawCommandItem;
			if (!menuItems.TryGetValue(commandName.ToLowerInvariant(), out rawCommandItem))
			{
				return;
			}

			//get our command item as the proper custom item type
			CommandItem commandItem = rawCommandItem;

			//get our list of filters assigned to our command.
			MultilistField filters = commandItem.Filters;

			//if we don't have any filters then just return.
			if (filters == null)
			{
				return;
			}

			//loop through each filter, get our filter class through reflection,
			//and execute our filter.  If any filters hide our menu item, then set
			//the menu item to hidden.
			foreach (Item filterItem in filters.GetItems())
			{
				//if our filter is null then just continue
				if (filterItem.IsNull())
				{
					continue;
				}

				//get our filter code.  This sitecore method will work only if
				//our Item has a Type field.  Any class specified in this Type
				//field will be created here.
				IFilter filter = ItemScripts.CreateObject(filterItem) as IFilter;

				//if we don't have filter code then just continue
				if (filter == null)
				{
					continue;
				}

				//create our filter args for passing into our filter.
				//we pass the actual filter item into our filter code so it can use
				//any custom fields needed to perform the filtering.
				FilterArgs filterArgs = new FilterArgs(contentItem, filterItem);

				//call our filter
				filter.Process(filterArgs);

				//if our filter set our visibility to hidden then hide our menu item and return
				if (filterArgs.HideCommand)
				{
					//hide our menu item.
					args.CommandState = CommandState.Hidden;

					//return as we don't need to do any further checking.
					return;
				}
			}
		}

		/// <summary>
		/// Will get the menu items from the context menu parent provided.
		/// </summary>
		/// <param name="args">Method arguments.</param>
		/// <returns>All menu items from the provided context menu parent folder.</returns>
		private Dictionary<string, Item> GetMenuItems(Dictionary<string, object> args)
		{
			//make sure we are provided args
			if (args == null)
			{
				return null;
			}

			//get the parent item provided
			Item parentItem = args["ParentMenuItem"] as Item;
			if (parentItem.IsNull())
			{
				return null;
			}

			//go through each child item and add them to our dictionary.
			Dictionary<string, Item> commandItems = new Dictionary<string, Item>(parentItem.Children.Count);
			foreach (Item childItem in parentItem.Children)
			{
				//if we are not dealing with a command item then just continue.
				if (!childItem.IsOfTemplate(CommandItem.TemplateId))
				{
					continue;
				}

				//get our command item as the proper custom item type
				CommandItem commandItem = childItem;

				//get our command text so we can pull out our command name
				string commandText = commandItem.CommandName;

				//if we don't have command text then just continue
				if (string.IsNullOrEmpty(commandText))
				{
					continue;
				}

				//strip off any parameter string to get our command name.
				int index = commandText.IndexOf("(");
				if (index >= 0)
				{
					commandText = StringUtil.Left(commandText, index);
				}

				//add our command item to our dictionary
				commandItems.Add(commandText.ToLowerInvariant(), commandItem);
			}

			//return our command items
			return commandItems;
		}
	}
}
