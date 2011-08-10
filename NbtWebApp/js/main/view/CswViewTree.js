/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

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
			onSuccess: function() { }
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
				    log(data);
				    var strTypes = $xml.find('types').text();
					var jsonTypes = $.parseJSON(strTypes);
					var $treexml = $xml.find('tree').children('root');
					var treexmlstring = xmlToString($treexml);
					
					$viewsdiv.jstree({
						"json_data": {
							"data": treexmlstring,
							"xsl": "nest"
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
								function (e, data) {
									var Selected = jsTreeGetSelected($viewsdiv); 
									var optSelect = {
												$item: Selected.$item,
												iconurl: Selected.iconurl,
												type: Selected.$item.CswAttrXml('viewtype'),
												viewid: Selected.$item.CswAttrXml('viewid'),
												viewname: Selected.text,
												viewmode: Selected.$item.CswAttrXml('viewmode'),
												actionid: Selected.$item.CswAttrXml('actionid'),
												actionname: Selected.$item.CswAttrXml('actionname'),
												actionurl: Selected.$item.CswAttrXml('actionurl'),
												reportid: Selected.$item.CswAttrXml('reportid')
											};
									o.onSelect(optSelect); //Selected.SelectedId, Selected.SelectedText, Selected.SelectedIconUrl, Selected.SelectedCswNbtNodeKey
								});

					o.onSuccess();

				} // success{}
			});
		
		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);
