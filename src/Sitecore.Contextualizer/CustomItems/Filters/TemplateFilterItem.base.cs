using Sitecore.Data.Items;
using Sitecore.Data.Fields;

namespace Sitecore.SharedSource.Contextualizer.CustomItems.Filters
{
	public partial class TemplateFilterItem : CustomItem
	{

		#region Inherited Base Templates

		private readonly GenericFilterItem _GenericFilter;
		public GenericFilterItem GenericFilter { get { return _GenericFilter; } }

		#endregion



		public static readonly string TemplateId = "{3D471FDB-8FFA-4E65-8A26-0AF5D326217C}";

		#region Boilerplate CustomItem Code

		public TemplateFilterItem(Item innerItem)
			: base(innerItem)
		{

		}

		public static implicit operator TemplateFilterItem(Item innerItem)
		{
			return innerItem != null ? new TemplateFilterItem(innerItem) : null;
		}

		public static implicit operator Item(TemplateFilterItem customItem)
		{
			return customItem != null ? customItem.InnerItem : null;
		}

		#endregion //Boilerplate CustomItem Code

		#region Field Instance Methods


		public MultilistField IncludeTemplates
		{
			get
			{
				return InnerItem.Fields["Include Templates"];
			}
		}


		public MultilistField ExcludeTemplates
		{
			get
			{
				return InnerItem.Fields["Exclude Templates"];
			}
		}

		#endregion //Field Instance Methods
	}
}