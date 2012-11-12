using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.Contextualizer.CustomItems.Filters
{
	public partial class HierarchyFilterItem : CustomItem
	{

		#region Inherited Base Templates

		private readonly GenericFilterItem _GenericFilter;
		public GenericFilterItem GenericFilter { get { return _GenericFilter; } }

		#endregion



		public static readonly string TemplateId = "{0CB6AF49-4E30-4CBD-94D2-24D352BA7097}";

		#region Boilerplate CustomItem Code

		public HierarchyFilterItem(Item innerItem)
			: base(innerItem)
		{

		}

		public static implicit operator HierarchyFilterItem(Item innerItem)
		{
			return innerItem != null ? new HierarchyFilterItem(innerItem) : null;
		}

		public static implicit operator Item(HierarchyFilterItem customItem)
		{
			return customItem != null ? customItem.InnerItem : null;
		}

		#endregion //Boilerplate CustomItem Code

		#region Field Instance Methods


		public ReferenceField RootItem
		{
			get
			{
				return InnerItem.Fields["Root Item"];
			}
		}

		#endregion //Field Instance Methods
	}
}
