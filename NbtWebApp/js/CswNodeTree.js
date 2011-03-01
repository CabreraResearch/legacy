; (function ($) {
	
	var PluginName = 'CswNodeTree';

	var methods = {
		init: function(options) {
		
			var o = {
				ID: '', 
				TreeUrl: '/NbtWebApp/wsNBT.asmx/getTree',
				viewid: '',
				nodeid: '',
				cswnbtnodekey: '',
				onSelectNode: function(nodeid, nodename, iconurl) { },
				SelectFirstChild: true
			};

			if (options) {
				$.extend(o, options);
			}

			var IDPrefix = o.ID + '_';
			var $treediv = $('<div id="'+ IDPrefix + '" class="treediv" />')
							.appendTo($(this));

			CswAjaxXml({
				url: o.TreeUrl,
				data: 'ViewId=' + o.viewid + '&IDPrefix=' + IDPrefix,
				success: function ($xml) {
					var selectid;
					if(o.nodeid != undefined && o.nodeid != '') 
						selectid = IDPrefix + o.nodeid;
					else
					{
						if(o.SelectFirstChild)
							selectid = $xml.find('item').first().find('item').first().attr('id');
						else
							selectid = IDPrefix + 'root';
					}

					// make sure selected item is visible
					var $selecteditem = $xml.find('item[id="'+ selectid + '"]');
					var $itemparents = $selecteditem.parents('item').andSelf();
					var initiallyOpen = new Array();
					var i = 0;
					$itemparents.each(function() { initiallyOpen[i] = $(this).attr('id'); i++; });

					var strTypes = $xml.find('types').text();
					var jsonTypes = $.parseJSON(strTypes);
					var $treexml = $xml.find('tree').children('root')
					var treexmlstring = xmlToString($treexml);

					$treediv.jstree({
						"xml_data": {
							"data": treexmlstring,
							"xsl": "nest"
						},
						"ui": {
							"select_limit": 1,
							"initially_select": selectid
						},
						"core": {
							"initially_open": initiallyOpen
						},
						"types": {
							"types": jsonTypes
						},
						"plugins": ["themes", "xml_data", "ui", "types"]
					}).bind('select_node.jstree', 
									function (e, data) {
										var Selected = jsTreeGetSelected($treediv, IDPrefix); 
										o.onSelectNode(Selected.SelectedId, Selected.SelectedText, Selected.SelectedIconUrl);
									});

					// DO NOT define an onSuccess() function here that interacts with the tree.
					// The tree has initalization events that appear to happen asynchronously,
					// and thus having an onSuccess() function that changes the selected node will
					// cause a race condition.

				} // success{}
			});
		},

		'selectNode': function(newnodeid) {
			var $treediv = $(this).children('.treediv')
			var IDPrefix = $treediv.attr('id');
			$treediv.jstree('select_node', '#' + IDPrefix + newnodeid);
		}
	};

	// Method calling logic
	$.fn.CswNodeTree = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);

