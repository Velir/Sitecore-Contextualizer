Usage Instructions:
1) Login to Sitecore, switch to core database, and navigate to "/sitecore/content/Applications/Content Editor/Filterable Commands".
2) Create any filters necessary for hiding your menu items.
   a) Generic filter can be used for a custom built filter.
   b) Template filter can be used to only show commands for specific templates.
   c) Hierarchy filter can be used to only show commands under a specific node.
3) Create any commands you would like to filter and assign filters to it.  An example command name is "item:cuttoclipboard".

Development Instructions:
1) Login to Sitecore, switch to core database, and navigate to "/sitecore/templates/User Defined/Modules/Contextualizer/Filters".
2) Create any new filter templates with custom fields and assign the "Generic Filter" template as a base template.
3) Create a new class that inherits Velir.SitecorePlugins.ContextualMenu.Filters.IFilter and set this class as a standard value for the Type field in your template.
4) Start using the new filter by following the usage instructions above.