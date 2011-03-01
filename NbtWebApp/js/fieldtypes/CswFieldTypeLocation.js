; (function ($) {
		
	$.fn.CswFieldTypeLocation = function (method) {

		var PluginName = 'CswFieldTypeLocation';

		var methods = {
			init: function(nodepk, $xml) {
			
					var $Div = $(this);
					$Div.children().remove();

					var ID = $xml.attr('id');
					var Required = ($xml.attr('required') == "true");
					var ReadOnly = ($xml.attr('readonly') == "true");

					var NodeId = $xml.children('nodeid').text().trim();
					var CswNbtNodeKey = $xml.children('cswnbtnodekey').text().trim();
					var Name = $xml.children('name').text();
					var Path = $xml.children('path').text();
					var ViewId = $xml.children('viewid').text();

					if(ReadOnly)
					{
						$Div.append(Path);
					}
					else 
					{
						var $mytable = makeTable(ID + '_tbl').appendTo($Div);

						var $selectcell = getTableCell($mytable, 1, 1);
						var $selectdiv = $('<div class="locationselect" value="'+ NodeId +'"/>' )
										   .appendTo($selectcell);

						var $locationtree = $('<div />').CswNodeTree('init', { 'ID': ID,
																	viewid: ViewId,
																	nodeid: NodeId,
																	cswnbtnodekey: CswNbtNodeKey,
																	onSelectNode: function(itemid, text, iconurl) { onTreeSelect($selectdiv, itemid, text, iconurl); },
																	SelectFirstChild: false
																 });

						$selectdiv.CswComboBox('init', { 'ID': ID + '_combo', 
											   'TopContent': Name,
											   'SelectContent': $locationtree,
											   'Width': '266px' });

						var $addcell = getTableCell($mytable, 1, 2);
						var $AddButton = $('<div />').appendTo($addcell);
						$AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
													AlternateText: "Add New",
													onClick: onAdd 
												  });

//                        if(Required)
//                        {
//                            $SelectBox.addClass("required");
//                        }
					}

				},
			save: function($propdiv, $xml) {
					var $selectdiv = $propdiv.find('.locationselect');
					$xml.children('nodeid').text($selectdiv.attr('value'));
				}
		};
	
	
		function onTreeSelect($selectdiv, itemid, text, iconurl)
		{
			$selectdiv.attr('value', itemid);
			$selectdiv.CswComboBox( 'TopContent', text );
			setTimeout(function() { $selectdiv.CswComboBox( 'close'); }, 300);
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
