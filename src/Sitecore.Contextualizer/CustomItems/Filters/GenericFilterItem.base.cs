using Sitecore.Data.Items;

namespace Sitecore.SharedSource.Contextualizer.CustomItems.Filters
{
	public partial class GenericFilterItem : CustomItem
	{
		public static readonly string TemplateId = "{CD422DF6-C5A0-4966-A772-14628CC6D106}";

		#region Boilerplate CustomItem Code

		public GenericFilterItem(Item innerItem)
			: base(innerItem)
		{

		}

		public static implicit operator GenericFilterItem(Item innerItem)
		{
			return innerItem != null ? new GenericFilterItem(innerItem) : null;
		}

		public static implicit operator Item(GenericFilterItem customItem)
		{
			return customItem != null ? customItem.InnerItem : null;
		}

		#endregion //Boilerplate CustomItem Code

		#region Field Instance Methods


		public string Type
		{
			get
			{
				return InnerItem["Type"];
			}
		}

		#endregion //Field Instance Methods
	}
}
