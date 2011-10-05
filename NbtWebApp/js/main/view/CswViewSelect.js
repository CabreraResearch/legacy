/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="CswViewTree.js" />


; (function ($) {
	var PluginName = "CswViewSelect";

	var methods = {
		'init': function(options) 
			{

				var o = {
					ID: '',
					//viewid: '',
					onSelect: null,
//				        function() { 
//						var x = {
//									iconurl: '',
//									type: '',
//									viewid: '',
//									viewname: '',
//									viewmode: '',
//									actionid: '',
//									actionname: '',
//									actionurl: '',
//									reportid: ''
//								};
//					},
					onSuccess: null, //function() {},
					ClickDelay: 300,
					issearchable: false,
					usesession: true
				};

				if (options) {
					$.extend(o, options);
				}

				var $selectdiv = $(this);
				
				var $viewtreediv = $('<div/>');
				$selectdiv.CswComboBox('init', { ID: o.ID + '_combo', 
												 TopContent: 'Select a View',
												 SelectContent: $viewtreediv,
												 Width: '266px' });

				$viewtreediv.CswViewTree({ 
											onSelect: function(optSelect) 
												{ 
													_onTreeSelect({
																	ID: o.ID,
																	ClickDelay: o.ClickDelay,
																	$item: optSelect.$item,
																	iconurl: optSelect.iconurl,
																	type: optSelect.type,
																	viewid: optSelect.viewid,
																	viewname: optSelect.viewname,
																	viewmode: optSelect.viewmode,
																	actionid: optSelect.actionid,
																	actionname: optSelect.actionname,
																	actionurl: optSelect.actionurl,
																	reportid: optSelect.reportid,
																	onSelect: o.onSelect,
																	$selectdiv: $selectdiv
																});
												}, 
											onSuccess: o.onSuccess,
											issearchable: o.issearchable,
											usesession: o.usesession 
										});
				return $selectdiv;
			},

		'value': function() 
			{
				var $selectdiv = $(this);
				return {
					'type': $selectdiv.CswAttrXml('selectedType'),
					'value': $selectdiv.CswAttrXml('selectedValue')
				};
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
				$item: '',
				iconurl: '',
				type: '',
				viewid: '',
				viewname: '',
				viewmode: '',
				actionid: '',
				actionname: '',
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
		$icondiv.css('width', '16px');
		$icondiv.css('height' ,'16px');

		var $cell2 = $table.CswTable('cell', 1, 2);
		$cell2.append(x.viewname);

		x.$selectdiv.CswComboBox( 'TopContent', $newTopContent );
		x.$selectdiv.CswAttrXml('selectedType', x.type);
		switch(x.type.toLowerCase())
		{
			case 'view':
				x.$selectdiv.CswAttrXml('selectedValue', x.viewid);
				break;
			case 'action':
				x.$selectdiv.CswAttrXml('selectedValue', x.actionid);
				break;
			case 'report':
				x.$selectdiv.CswAttrXml('selectedValue', x.reportid);
				break;
		}

		setTimeout(function() { x.$selectdiv.CswComboBox( 'toggle'); }, x.ClickDelay);
		x.onSelect({
					iconurl: x.iconurl,
					type: x.type,
					viewid: x.viewid,
					viewname: x.viewname,
					viewmode: x.viewmode,
					actionid: x.actionid,
					actionname: x.actionname,
					actionurl: x.actionurl,
					reportid: x.reportid
					});
	} // _onTreeSelect()
		

}) (jQuery);

