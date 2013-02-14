Sitecore-Contextualizer
=======================

Contextual commands in the right-click menu or ribbon in Sitecore's Content Editor

Installation
============

Installation is simple and only requires you to install the package from src/Sitecore.Contextualizer/Packages.

Configuration
=============

Configuration is also simple and just requires you to create commands and filters. Once created you simply need to assign your filters to your command to hide it in certain situations. By default a command will display unless otherwise filtered and hidden from one of the configured filters.

1.  Navigate to "/sitecore/content/Applications/Content Editor/Filterable Commands" in the Core database.
2.  Create any filters in the Filters folder. These will be used to hide a command in certain situations.
	1.  Generic Filter is used when creating custom filters. See Customization below for more information.
	2.  Hierarchy Filter is used to only display a command for selected items that fall under the configured root item.
	3.  Template Filter is used to only display a command for configured templates.
3.  Create any commands you want to filter in the Commands folder.
	1.  Populate the command name. This is the full command name registered in sitecore (item:cuttoclipboard).
	2.  Assigned any filters used to determine when this command should be displayed.
4.  Test it out. Once configured, head over to the master database and test out your configuration.

Customization
=============

You may need to create your own filter if you can't get the desired result from the provided filters. To do this simply follow the steps below.

Quick Filter
============

To create a filter quickly with hard coded configurations, follow these steps.

1.  Create a new class and implement the IFilter interface.
2.  Implement the public void Process(FilterArgs args) method and add the desired logic.
	1.  FilterArgs.ContentItem - The currently selected sitecore item.
	2.  FilterArgs.FilterItem - The filter sitecore item. This won't be very useful in a quick filter.
	3.  FilterArgs.HideCommand - Property to set to true if the command is to be hidden.
3.  Create a new Generic Filter by following the Configuration steps above.
	1.  Populate the Type property with the namespace,assembly information for your new filter class. You can view the provided Hierarchy Filter for an example.
4.  Apply your new filter to a command by following the Configuration steps above and test it out.

Configurable Filter
===================

To create a filter that can be configurable from the sitecore ui, follow these steps.

1.  Create a new class and implement the IFilter interface.
2.  Implement the public void Process(FilterArgs args) method and add the desired logic.
	1.  FilterArgs.ContentItem - The currently selected sitecore item.
	2.  FilterArgs.FilterItem - The filter sitecore item that will contain any filter configurations you need to read.
	3.  FilterArgs.HideCommand - Property to set to true if the command is to be hidden.
3.  Create a new filter template under "/sitecore/templates/User Defined/Modules/Contextualizer/Filters" in the Core database.
	1.  Assign the Generic Filter template as a base template.
	2.  Add any custom fields that will allow you to configure the filter.
	3.  Populate the Type property in your standard values with the namespace,assembly information for your new filter class. You can view the provided Hierarchy Filter for an example.
4.  Follow the Configuration steps above to create your filter, apply it to a command, and test it out.