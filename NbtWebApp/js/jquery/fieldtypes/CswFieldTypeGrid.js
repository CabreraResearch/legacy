/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.2-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../CswNodeGrid.js" />

; (function ($) { /// <param name="$" type="jQuery" />
		
	var PluginName = 'CswFieldTypeGrid';
   
	var methods = {
		'init': function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
			/// <summary>
            ///   Initializes a jqGrid as an NbtNode Prop
            /// </summary>
            /// <param name="o" type="Object">
            ///     A JSON Object
            /// </param>
            var $Div = $(this);
			$Div.empty();

			if(o.EditMode === EditMode.AuditHistoryInPopup.name)
			{
				$Div.append('[Grid display disabled]');
			} else {

				var MenuDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype_menu'});
				var $MenuDiv = $('<div id="' + MenuDivId + '" name="' + MenuDivId + '"></div>');

				var SearchDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype_search'});
				var $SearchDiv = $('<div id="' + SearchDivId + '" name="' + SearchDivId + '"></div>');

				var GridDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype'});
				var $GridDiv = $('<div id="' + GridDivId + '" name="' + GridDivId + '"></div>');

				var viewid = o.$propxml.children('viewid').text().trim();
            
				var gridOpts = {
					'ID': o.ID,
					'viewid': viewid, 
					'nodeid': o.nodeid, 
					'cswnbtnodekey': o.cswnbtnodekey, 
					'readonly': o.ReadOnly,
					'reinit': false,
					'EditMode': o.EditMode,
					'onEditNode': function() { 
						refreshGrid(gridOpts);
					},
	//                'onAddNode': function() { 
	//                    refreshGrid(gridOpts);
	//                },
					'onDeleteNode': function() { 
						refreshGrid(gridOpts);
					}
				};

				function refreshGrid(options) { 
					var o ={
						reinit: true
					};
					if( options ) $.extend(options,o);
					$GridDiv.CswNodeGrid('init', options);
				};

				$GridDiv.CswNodeGrid('init', gridOpts);
				//Case 21741
				if( o.EditMode !== EditMode.PrintReport.name )
				{
					$MenuDiv.CswMenuMain({
							'viewid': viewid,
							'nodeid': o.nodeid,
							'cswnbtnodekey': o.cswnbtnodekey,
							'propid': o.ID,
							'onAddNode': function (nodeid, cswnbtnodekey)
							{
								refreshGrid(gridOpts);
							},
							'onSearch':
								{
									'onViewSearch': function ()
									{
										var onSearchSubmit = function(searchviewid) {
											var s = {};
											$.extend(s,gridOpts);
											s.viewid = searchviewid;
											refreshGrid(s);
										};
                                
										var onClearSubmit = function(parentviewid) {
											var s = {};
											$.extend(s,gridOpts);
											s.viewid = parentviewid;
											refreshGrid(s);
										};

										$SearchDiv.empty();
										$SearchDiv.CswSearch({'parentviewid': viewid,
															  'cswnbtnodekey': o.cswnbtnodekey,
															  'ID': SearchDivId,
															  'onSearchSubmit': onSearchSubmit,
															  'onClearSubmit': onClearSubmit
															  });
									},
									'onGenericSearch': function () { /*not possible here*/ }
								},
							'onEditView': function (Viewid)
							{
								o.onEditView(viewid);                    
							}
					}); // CswMenuMain
				} // if( o.EditMode !== EditMode.PrintReport.name )
				$Div.append($MenuDiv, $('<br/>'), $SearchDiv, $('<br/>'), $GridDiv);
			} // if(o.EditMode !== EditMode.AuditHistoryInPopup.name)
		},
		save: function(o) {
//                var $TextBox = $propdiv.find('input');
//                $xml.children('barcode').text($TextBox.val());
			}
	};
	
	// Method calling logic
	$.fn.CswFieldTypeGrid = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);
