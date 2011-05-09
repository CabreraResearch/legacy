; (function ($) {
	var PluginName = "CswViewSelect";

	var methods = {
		'init': function(options) 
			{

				var o = {
					ID: '',
					//viewid: '',
					onSelect: function(optSelect) { 
						var x = {
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
					onSuccess: function() {},
					ClickDelay: 300,
                    issearchable: false,
                    usesession: true,
				};

				if (options) {
					$.extend(o, options);
				}

				var $selectdiv = $(this);
				
				$viewtreediv = $('<div/>');
				$selectdiv.CswComboBox('init', { 'ID': o.ID + '_combo', 
													'TopContent': 'Select a View',
													'SelectContent': $viewtreediv,
													'Width': '266px' });

				$viewtreediv.CswViewTree({ 
											'onSelect': function(optSelect) 
												{ 
													_onTreeSelect({
																	'ID': o.ID,
																	'ClickDelay': o.ClickDelay,
																	'iconurl': optSelect.iconurl,
																	'type': optSelect.type,
																	'viewid': optSelect.viewid,
																	'viewname': optSelect.viewname,
																	'viewmode': optSelect.viewmode,
																	'actionid': optSelect.actionid,
																	'actionurl': optSelect.actionurl,
																	'reportid': optSelect.reportid,
																	'onSelect': o.onSelect,
																	'$selectdiv': $selectdiv
																});
												}, 
											'onSuccess': o.onSuccess,
                                            'issearchable': o.issearchable,
                                            'usesession': o.usesession 
										});
				return $selectdiv;
			},

		'value': function() 
			{
				var $selectdiv = $(this);
				return $selectdiv.CswAttrDom('selectedValue');
			}
	};
	
	// Method calling logic
	$.fn.CswViewSelect = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
		
	function _onTreeSelect(optSelect)
	{
		var x = {
				ID: '',
				ClickDelay: 300,
				iconurl: '',
				type: '',
				viewid: '',
				viewname: '',
				viewmode: '',
				actionid: '',
				actionurl: '',
				reportid: '',
				onSelect: function() { },
				$selectdiv: ''
				};
		if(optSelect){
			$.extend(x, optSelect);
		}

		var $newTopContent = $('<div></div>');
        var $table = $newTopContent.CswTable('init', { ID: x.ID + 'selectedtbl' });
		var $cell1 = $table.CswTable('cell', 1, 1);
		var $icondiv = $('<div />').appendTo($cell1);
		$icondiv.css('background-image', x.iconurl);
		$icondiv.css('width', '18px');
		$icondiv.css('height' ,'18px');

		var $cell2 = $table.CswTable('cell', 1, 2);
		$cell2.append(x.viewname);

		x.$selectdiv.CswComboBox( 'TopContent', $newTopContent );
		x.$selectdiv.CswAttrDom('selectedValue', x.viewid);
		setTimeout(function() { x.$selectdiv.CswComboBox( 'toggle'); }, x.ClickDelay);
		x.onSelect({
					iconurl: x.iconurl,
					type: x.type,
					viewid: x.viewid,
					viewname: x.viewname,
					viewmode: x.viewmode,
					actionid: x.actionid,
					actionurl: x.actionurl,
					reportid: x.reportid
					});
	} // _onTreeSelect()
		

}) (jQuery);

