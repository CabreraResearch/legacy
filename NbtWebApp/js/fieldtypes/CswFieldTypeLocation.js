; (function ($) {
		
	$.fn.CswFieldTypeLocation = function (method) {

		var PluginName = 'CswFieldTypeLocation';
		var options = [];

		var methods = {
			init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 
			
				var $Div = $(this);
				$Div.children().remove();

				//var NodeId = $xml.children('nodeid').text().trim();
				var Name = o.$propxml.children('name').text().trim();
				var Path = o.$propxml.children('path').text().trim();
				var ViewId = o.$propxml.children('viewid').text().trim();

				if(o.ReadOnly)
				{
					$Div.append(Path);
				}
				else 
				{
					var $mytable = makeTable(o.ID + '_tbl').appendTo($Div);

					var $selectcell = getTableCell($mytable, 1, 1);
					var $selectdiv = $('<div class="locationselect" value="'+ o.nodeid +'"/>' )
										.appendTo($selectcell);
					options = {
						'$selectdiv': $selectdiv,
						itemid: '',
						text: '',
						iconurl: ''
					};
					var $locationtree = $('<div />').CswNodeTree('init', { 'ID': o.ID,
																viewid: ViewId,
																nodeid: o.nodeid,
																onSelectNode: function(options) //itemid, text, iconurl
																	{ onTreeSelect(options); onchange(); }, //$selectdiv, itemid, text, iconurl
																SelectFirstChild: false
																});

					$selectdiv.CswComboBox('init', { 'ID': o.ID + '_combo', 
											'TopContent': Name,
											'SelectContent': $locationtree,
											'Width': '266px' });

					var $addcell = getTableCell($mytable, 1, 2);
					var $AddButton = $('<div />').appendTo($addcell);
					$AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
												AlternateText: "Add New",
												onClick: onAdd 
												});

//                        if(o.Required)
//                        {
//                            $SelectBox.addClass("required");
//                        }
				}

			},
			save: function(o) { //($propdiv, $xml
					var $selectdiv = o.$propdiv.find('.locationselect');
					o.$propxml.children('nodeid').text($selectdiv.attr('value'));
				}
		};
	
	
		function onTreeSelect(o) //$selectdiv, itemid, text, iconurl
		{
			if(options)
			{
				$.extend(o,options);
			}
			o.$selectdiv.attr('value', o.itemid);
			o.$selectdiv.CswComboBox( 'TopContent', o.text );
			setTimeout(function() { o.$selectdiv.CswComboBox( 'close'); }, 300);
		}
		
		function onAdd($ImageDiv)
		{
			alert('This function has not been implemented yet.');
		}

		// Method calling logic
		if ( methods[method] ) {
			return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
			return methods.init.apply( this, arguments );
		} else {
			$.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);
