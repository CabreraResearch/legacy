; (function ($) {
		
	$.fn.CswFieldTypeLocation = function (method) {

		var PluginName = 'CswFieldTypeLocation';

		var methods = {
			init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 
			
				var $Div = $(this);
				$Div.contents().remove();

				var NodeId = o.$propxml.children('nodeid').text().trim();
				var NodeKey = o.$propxml.children('nodekey').text().trim();
				var Name = o.$propxml.children('name').text().trim();
				var Path = o.$propxml.children('path').text().trim();
				var ViewId = o.$propxml.children('viewid').text().trim();

				if(o.ReadOnly)
				{
					$Div.append(Path);
				}
				else 
				{
                    var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

					var $pathcell = $table.CswTable('cell', 1, 1);
					$pathcell.CswAttrDom('colspan', '2');
					$pathcell.append(Path + "<br/>");

					var $selectcell = $table.CswTable('cell', 2, 1);
					var $selectdiv = $('<div class="locationselect" value="'+ NodeId +'"/>' )
										.appendTo($selectcell);

					var $locationtree = $('<div />').CswNodeTree('init', { 'ID': o.ID,
																			viewid: ViewId,
																			nodeid: NodeId,
																			cswnbtnodekey: NodeKey,
																			onSelectNode: function(optSelect)
																							{
																								onTreeSelect($selectdiv, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, o.onchange); 
																							}, 
																			SelectFirstChild: false,
																			UsePaging: false
																		});
	
					$selectdiv.CswComboBox( 'init', {	'ID': o.ID + '_combo', 
														'TopContent': Name,
														'SelectContent': $locationtree,
														'Width': '266px' 
													});

//					var $addcell = $table.CswTable('cell', 2, 2);
//					var $AddButton = $('<div />').appendTo($addcell);
//					$AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
//												AlternateText: "Add New",
//												onClick: function($ImageDiv) { 
//														onAdd();
//														return CswImageButton_ButtonType.None;
//													}
//												});

//                        if(o.Required)
//                        {
//                            $SelectBox.addClass("required");
//                        }
				}

			},
			save: function(o) { //($propdiv, $xml
					var $selectdiv = o.$propdiv.find('.locationselect');
					o.$propxml.children('nodeid').text($selectdiv.val());
				}
		};
	
	
		function onTreeSelect($selectdiv, itemid, text, iconurl, onchange)
		{
			if(itemid === 'root') itemid = '';   // case 21046
			$selectdiv.CswComboBox( 'TopContent', text );
			if($selectdiv.val() !== itemid)
			{
				$selectdiv.val(itemid);
				onchange();
			}
			setTimeout(function() { $selectdiv.CswComboBox( 'close'); }, 300);
		}
		
//		function onAdd()
//		{
//			alert('This function has not been implemented yet.');
//		}

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
