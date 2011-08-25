/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	$.fn.CswViewTree = function (options) { 

		var o = {
			ViewUrl: '/NbtWebApp/wsNBT.asmx/getViewTree',
			viewid: '',
			issearchable: false,
			usesession: true,
			onSelect: function (optSelect) { 
							var o = {
								$item: '',
								iconurl: '',
								type: '',
								viewid: '',
								viewname: '',
								viewmode: '',
								actionid: '',
								actionname: '',
								actionurl: '',
								reportid: ''
								};
						},
			onSuccess: null //function() { }
		};

		if (options) {
			$.extend(o, options);
		}

		var $viewsdiv = $(this);
		
		var jsonData = {
			IsSearchable: o.issearchable,
			UseSession: o.usesession
		};
						
		CswAjaxJson({
				url: o.ViewUrl,
				data: jsonData,
				stringify: false,
				success: function (data)
				{
				    var jsonTypes = data.types;
				    var treeData = data.tree;					
					$viewsdiv.jstree({
						"json_data": {
							"data": treeData
						},
						"ui": {
							"select_limit": 1
						},
						"core": {
							"initially_open": ["root"]
						},
						"types": {
							"types": jsonTypes
						},
						"plugins": ["themes", "json_data", "ui", "types"]
					}).bind('select_node.jstree', 
								function () {
									if (isFunction(o.onSelect)) {
									    var selected = jsTreeGetSelected($viewsdiv);
									    var optSelect = {
									        $item: selected.$item,
									        iconurl: selected.iconurl,
									        type: selected.$item.CswAttrXml('viewtype'),
									        viewid: selected.$item.CswAttrXml('viewid'),
									        viewname: selected.text,
									        viewmode: selected.$item.CswAttrXml('viewmode'),
									        actionid: selected.$item.CswAttrXml('actionid'),
									        actionname: selected.$item.CswAttrXml('actionname'),
									        actionurl: selected.$item.CswAttrXml('actionurl'),
									        reportid: selected.$item.CswAttrXml('reportid')
									    };
									    o.onSelect(optSelect); //Selected.SelectedId, Selected.SelectedText, Selected.SelectedIconUrl, Selected.SelectedCswNbtNodeKey
									}
								});

					if (isFunction(o.onSuccess)) {
					    o.onSuccess();  
					} 

				} // success{}
			});
		
		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);
