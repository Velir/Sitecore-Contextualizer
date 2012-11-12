using Sitecore.Data.Items;
using Sitecore.Data.Fields;

namespace Sitecore.SharedSource.Contextualizer.CustomItems
{
	public partial class CommandItem : CustomItem
	{

		#region Inherited Base Templates

		#endregion



		public static readonly string TemplateId = "{AEE09A03-6B16-4731-BD16-E158AF6209BF}";

		#region Boilerplate CustomItem Code

		public CommandItem(Item innerItem)
			: base(innerItem)
		{

		}

		public static implicit operator CommandItem(Item innerItem)
		{
			return innerItem != null ? new CommandItem(innerItem) : null;
		}

		public static implicit operator Item(CommandItem customItem)
		{
			return customItem != null ? customItem.InnerItem : null;
		}

		#endregion //Boilerplate CustomItem Code

		#region Field Instance Methods


		public string CommandName
		{
			get
			{
				return InnerItem["Command Name"];
			}
		}

		public MultilistField Filters
		{
			get
			{
				return InnerItem.Fields["Filters"];
			}
		}

		#endregion //Field Instance Methods
	}
}