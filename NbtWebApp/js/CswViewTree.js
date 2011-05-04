/// <reference path="../jquery/jquery-1.5.2-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	$.fn.CswViewTree = function (options) { 

		var o = {
			ViewUrl: '/NbtWebApp/wsNBT.asmx/getViewTree',
			viewid: '',
			onSelect: function (optSelect) { 
							var o = {
								iconurl: '',
								type: '',
								viewid: '',
								viewname: '',
								viewmode: '',
								actionid: '',
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
		$viewsdiv.contents().remove();
		
		CswAjaxXml({
				url: o.ViewUrl,
				data: '',
				success: function ($xml)
				{
					var strTypes = $xml.find('types').text();
					var jsonTypes = $.parseJSON(strTypes);
					var $treexml = $xml.find('tree').children('root')
					var treexmlstring = xmlToString($treexml);
					
					$viewsdiv.jstree({
						"xml_data": {
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
						"plugins": ["themes", "xml_data", "ui", "types"]
					}).bind('select_node.jstree', 
								function (e, data) {
									var Selected = jsTreeGetSelected($viewsdiv); 
									var optSelect = {
												iconurl: Selected.iconurl,
												type: Selected.$item.attr('type'),
												viewid: Selected.$item.attr('viewid'),
												viewname: Selected.text,
												viewmode: Selected.$item.attr('viewmode'),
												actionid: Selected.$item.attr('actionid'),
												actionurl: Selected.$item.attr('actionurl'),
												reportid: Selected.$item.attr('reportid')
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
