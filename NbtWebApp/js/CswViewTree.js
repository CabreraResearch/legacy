; (function ($) {
	$.fn.CswViewTree = function (options) {

		var o = {
			ViewUrl: '/NbtWebApp/wsNBT.asmx/getViewTree',
			viewid: '',
			onSelect: function (optSelect) { } // nodeid, nodename, iconurl, cswnbtnodekey
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
									var Selected = jsTreeGetSelected($viewsdiv, ''); 
									var optSelect = {
												type: Selected.SelectedType,
												viewmode: Selected.SelectedViewMode,
												itemid: Selected.SelectedId, 
												text: Selected.SelectedText, 
												iconurl: Selected.SelectedIconUrl,
												viewid: Selected.SelectedViewId,
												nodeid: Selected.SelectedId, 
												nodename: Selected.SelectedText, 
												cswnbtnodekey: Selected.SelectedCswNbtNodeKey												
											 };
									o.onSelect(optSelect); //Selected.SelectedId, Selected.SelectedText, Selected.SelectedIconUrl, Selected.SelectedCswNbtNodeKey
								});

				} // success{}
			});
		
		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);
