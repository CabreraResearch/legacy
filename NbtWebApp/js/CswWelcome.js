﻿; (function ($) {
	var PluginName = "CswWelcome";

	var methods = {
	
		'initTable': function (options) 
			{
				var o = {
					Url: '/NbtWebApp/wsNBT.asmx/getWelcomeItems',
					MoveWelcomeItemUrl: '/NbtWebApp/wsNBT.asmx/moveWelcomeItems',
					RemoveWelcomeItemUrl: '/NbtWebApp/wsNBT.asmx/deleteWelcomeItem',
					onLinkClick: function(optSelect) { }, //viewid, actionid, reportid
					onSearchClick: function(optSelect) { }, //viewid
					onAddClick: function(nodetypeid) { },
					onAddComponent: function() { }
				};

				if (options) {
					$.extend(o, options);
				}
				var $this = $(this);

				CswAjaxXml({
					url: o.Url,
					data: "RoleId=",
					success: function ($xml) {
						var $WelcomeDiv = $('<div id="welcomediv"></div>')
											.appendTo($this)
											.css('text-align', 'center')
											.css('font-size', '1.2em');

						var $table = $WelcomeDiv.CswLayoutTable('init', {
																		 'ID': 'welcometable',
																		 'cellset': { rows: 2, columns: 1 },
																		 'TableCssClass': 'WelcomeTable',
																		 'cellpadding': 10,
																		 'align': 'center',
																		 'onSwap': function(ev, onSwapData) 
																			{ 
																				_onSwap({
																					cellset: onSwapData.cellset, 
																					row: onSwapData.row, 
																					column: onSwapData.column,
																					swapcellset: onSwapData.swapcellset, 
																					swaprow: onSwapData.swaprow, 
																					swapcolumn: onSwapData.swapcolumn,
																					MoveWelcomeItemUrl: o.MoveWelcomeItemUrl
																				 });
																			 },
																		 'showConfigButton': true,
																		 'showAddButton': true,
																		 'showRemoveButton': true,
																		 'onAddClick': function() { $.CswDialog('AddWelcomeItemDialog', { 'onAdd': o.onAddComponent }); },
																		 'onRemove': function(ev, onRemoveData) 
																			{ 
																				_removeItem({
																					cellset: onRemoveData.cellset,
																					row: onRemoveData.row,
																					column: onRemoveData.column,
																					RemoveWelcomeItemUrl: o.RemoveWelcomeItemUrl
																				});
																			}
																		});
				
						$xml.children().each(function() {

							var $item = $(this);
							var $cellset = $table.CswLayoutTable('cellset', $item.attr('displayrow'), $item.attr('displaycol'));
							var $imagecell = $cellset[1][1];
							var $textcell = $cellset[2][1];

							if($item.attr('buttonicon') != undefined && $item.attr('buttonicon') != '')
								$imagecell.append( $('<a href=""><img border="0" src="'+ $item.attr('buttonicon') +'"/></a>') );
					
							var optSelect = {
								type: $item.attr('type'),
								viewmode: $item.attr('viewmode'),
								itemid: $item.attr('itemid'), 
								text: $item.attr('text'), 
								iconurl: $item.attr('iconurl'),
								viewid: $item.attr('viewid'),
								actionid: $item.attr('actionid'),
								reportid: $item.attr('reportid'),
								//nodetypeid: $item.attr('nodetypeid'),
								linktype: $item.attr('linktype')
							};

							switch(optSelect.linktype)
							{
								case 'Link':
									$textcell.append( $('<a href="">' + optSelect.text + '</a>') );
									$textcell.find('a').click(function() { o.onLinkClick(optSelect); return false; });
									$imagecell.find('a').click(function() { o.onLinkClick(optSelect); return false; });
									break;
								case 'Search': 
									$textcell.append( $('<a href="">' + optSelect.text + '</a>') );
									$textcell.find('a').click(function() { o.onSearchClick(optSelect); return false; }); //viewid
									$imagecell.find('a').click(function() { o.onSearchClick(optSelect); return false; }); //viewid
									break;
								case 'Text':
									$textcell.append('<span>' + optSelect.text + '</span>');
									break;
								case 'Add': 
									$textcell.append( $('<a href="">' + optSelect.text + '</a>') );
									$textcell.find('a').click(function() { o.onAddClick($item.attr('nodetypeid')); return false; }); 
									$imagecell.find('a').click(function() { o.onAddClick($item.attr('nodetypeid')); return false; });
									break;
							}

							$textcell.append('<input type="hidden" welcomeid="' + $item.attr('welcomeid') + '" />');
						}); // each
				
					} // success{}
				}); // CswAjaxXml
			}, // initTable

		'getAddItemForm': function(options) 
			{
				var o = { 
					'AddWelcomeItemUrl': '/NbtWebApp/wsNBT.asmx/addWelcomeItem',
					'onAdd': function() { }
				};
				if(options) {
					$.extend(o, options);
				}

				var $parent = $(this);
				var $table = $parent.CswTable('init', { ID: 'addwelcomeitem_tbl' });

				var $typeselect_label = $('<span>Type:</span>')
										.appendTo($table.CswTable('cell', 1, 1));
				var $typeselect = $('<select id="welcome_type" name="welcome_type"></select>')
										.appendTo($table.CswTable('cell', 1, 2));
				$typeselect.append('<option value="Add" selected>Add</option>');
				$typeselect.append('<option value="Link">Link</option>');
				$typeselect.append('<option value="Search">Search</option>');
				$typeselect.append('<option value="Text">Text</option>');

				var $viewselect_label = $('<span>View:</span>')
										.appendTo($table.CswTable('cell', 2, 1))
										.hide();
				var $viewselect = $table.CswTable('cell', 2, 2).CswViewSelect({
																				'ID': 'welcome_viewsel',
																				//'viewid': '',
																				//'onSelect': function(optSelect) { },
																			})
										.hide();

				var $ntselect_label = $('<span>Add New:</span>')
										.appendTo($table.CswTable('cell', 3, 1))
				var $ntselect = $table.CswTable('cell', 3, 2).CswNodeTypeSelect({
																	'ID': 'welcome_ntsel'
																});

				var $welcometext_label = $('<span>Text:</span>')
										.appendTo($table.CswTable('cell', 4, 1))
				var $welcometext = $('<input type="text" id="welcome_text" value="" />')
										.appendTo($table.CswTable('cell', 4, 2));

				var $buttonsel_label = $('<span>Use Button:</span>')
										.appendTo($table.CswTable('cell', 5, 1))
				var $buttonsel = $('<select id="welcome_button" />')
										.appendTo($table.CswTable('cell', 5, 2));
				$buttonsel.append('<option value="blank.gif"></option>');

				var $buttonimg = $('<img id="welcome_btnimg" />')
										.appendTo( $table.CswTable('cell', 6, 2) );

				var $addbutton = $('<input type="button" />')
                $addbutton.CswButton('init', {ID: 'welcome_add', 
                                            enabledText: 'Add', 
                                            disabledText: 'Adding', 
                                            onclick: function() { 
										            _addItem({ 
													            'AddWelcomeItemUrl': o.AddWelcomeItemUrl,
													            'type': $typeselect.val(),
													            'viewid': $viewselect.CswViewSelect('value'),
													            'nodetypeid': $ntselect.CswNodeTypeSelect('value'),
													            'text': $welcometext.val(),
													            'iconfilename': $buttonsel.val(),
													            'onSuccess': o.onAdd
												            });
									            }
                                            });
                $table.CswTable('cell', 7, 2).append($addbutton);

				$buttonsel.change(function(event) { 
					$buttonimg.attr('src', 'Images/biggerbuttons/' + $buttonsel.val()); 
				});

				$typeselect.change(function() 
									{ 
										_onTypeChange({
											//'$table': $table,
											//'$typeselect_label': $typeselect_label,
											'$typeselect': $typeselect,
											'$viewselect_label': $viewselect_label,
											'$viewselect': $viewselect,
											'$ntselect_label': $ntselect_label,
											'$ntselect': $ntselect,
//											'$welcometext_label': $welcometext_label,
//											'$welcometext': $welcometext,
											'$buttonsel_label': $buttonsel_label,
											'$buttonsel': $buttonsel,
											'$buttonimg': $buttonimg, 
//											'$addbutton': $addbutton
										});
									});

				CswAjaxXml({ 
							'url': '/NbtWebApp/wsNBT.asmx/getWelcomeButtonIconList',
							'success': function($xml) { 
										$xml.children().each(function() {
											var $icon = $(this);
											var filename = $icon.attr('filename');
											if(filename != 'blank.gif') 
											{
												$buttonsel.append('<option value="'+ filename +'">'+ filename +'</option>');
											}
										}); // each
									} // success
							}); // CswAjaxXml
			} // getAddItemForm
		
	};

	
	// Method calling logic
	$.fn.CswWelcome = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

	function _removeItem(removedata)
	{
		var r = {
					'cellset': '',
					'row': '',
					'column': '',
					'RemoveWelcomeItemUrl': '',
					'onSuccess': function() { }
				};
		if(removedata) {
			$.extend(r, removedata);
		}
        var $textcell = $(r.cellset[2][1]);
        if($textcell.length > 0)
        {
            var welcomeid = $textcell.children('input').attr('welcomeid');
		
			CswAjaxJSON({
				url: r.RemoveWelcomeItemUrl,
				data: '{ "RoleId": "", "WelcomeId": "'+ welcomeid +'" }',
				success: function (result) 
					{
						r.onSuccess();
					}
	        });

		} // if($textcell.length > 0)
	} // _removeItem()

	function _addItem(addoptions)
	{
		var a = {
			'AddWelcomeItemUrl': '', 
			'type': '',
			'viewid': '',
			'nodetypeid': '',
			'text': '',
			'iconfilename': '',
			'onSuccess': function() { }
		}
		if(addoptions){
			$.extend(a, addoptions);
		}

        CswAjaxJSON({
			url: a.AddWelcomeItemUrl,
			data: '{ "RoleId": "", "Type": "' + a.type + '", "ViewId": "' + a.viewid + '", "NodeTypeId": "' + a.nodetypeid + '", "Text": "' + a.text + '", "IconFileName": "' + a.iconfilename + '" }',
			success: function (result) 
				{
					a.onSuccess();
                }
        });

	} // _addItem()

    function _onSwap(onSwapData)
    {
		var s = {
					cellset: '',
					row: '',
					column: '',
					swapcellset: '',
					swaprow: '',
					swapcolumn: '',
					MoveWelcomeItemUrl: ''
				};
		if(onSwapData) {
			$.extend(s, onSwapData);
		}

        _moveItem(s.MoveWelcomeItemUrl, s.cellset, s.swaprow, s.swapcolumn);
        _moveItem(s.MoveWelcomeItemUrl, s.swapcellset, s.row, s.column);
    } // onSwap()

    function _moveItem(MoveWelcomeItemUrl, cellset, newrow, newcolumn)
    {
        var $textcell = $(cellset[2][1]);
        if($textcell.length > 0)
        {
            var welcomeid = $textcell.children('input').attr('welcomeid');
            CswAjaxJSON({
				url: MoveWelcomeItemUrl,
				data: '{ "RoleId": "", "WelcomeId": "'+ welcomeid +'", "NewRow": "' + newrow + '", "NewColumn": "' + newcolumn + '" }',
				success: function (result) {
                            }
            });
        }
    } // _moveItem()

	function _onTypeChange(options)
	{
		var o = {
			//$table: '',
			//$typeselect_label: '',
			$typeselect: '',
			$viewselect_label: '', 
			$viewselect: '',
			$ntselect_label: '',
			$ntselect: '',
//			$welcometext_label: '',
//			$welcometext: '',
			$buttonsel_label: '',
			$buttonsel: '',
			$buttonimg: ''//,
//			$addbutton: '',
		};
		if(options) {
			$.extend(o, options);
		}

		switch(o.$typeselect.val())
		{
			case "Add":
				o.$viewselect_label.hide();
				o.$viewselect.hide();
				o.$ntselect_label.show();
				o.$ntselect.show();
				o.$buttonsel_label.show();
				o.$buttonsel.show();
				break;
			case "Link":
				o.$viewselect_label.show();
				o.$viewselect.show();
				o.$ntselect_label.hide();
				o.$ntselect.hide();
				o.$buttonsel_label.show();
				o.$buttonsel.show();
				break;
			case "Search":
				o.$viewselect_label.show();
				o.$viewselect.show();
				o.$ntselect_label.hide();
				o.$ntselect.hide();
				o.$buttonsel_label.show();
				o.$buttonsel.show();
				break;
			case "Text":
				o.$viewselect_label.hide();
				o.$viewselect.hide();
				o.$ntselect_label.hide();
				o.$ntselect.hide();
				o.$buttonsel_label.hide();
				o.$buttonsel.hide();
				break;
		} // switch

	} // _onTypeChange()

})(jQuery);


