/// <reference path="../jquery/jquery-1.5.2-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />
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

            var MenuDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype_menu'});
            var $MenuDiv = $('<div id="' + MenuDivId + '" name="' + MenuDivId + '"></div>');

            var SearchDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype_search'});
            var $SearchDiv = $('<div id="' + SearchDivId + '" name="' + SearchDivId + '"></div>');

            var GridDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype'});
            var $GridDiv = $('<div id="' + GridDivId + '" name="' + GridDivId + '"></div>');

            var viewid = o.$propxml.children('viewid').text().trim();
            
			$GridDiv.CswNodeGrid('init', {'viewid': viewid, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey, 'readonly': o.ReadOnly});
            $MenuDiv.CswMenuMain({
			        'viewid': viewid,
			        'nodeid': o.nodeid,
			        'cswnbtnodekey': o.cswnbtnodekey,
			        'onAddNode': function (nodeid, cswnbtnodekey)
			        {
                        $GridDiv.CswNodeGrid('init', {'viewid': viewid, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey, 'readonly': o.ReadOnly});
			        },
		            'onSearch':
                        {
                            'onViewSearch': function ()
                            {
	                            var onSearchSubmit = function(view) {
                                    $GridDiv.empty();
                                    $GridDiv.CswNodeGrid('init', {'viewid': view.sessionviewid, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey, 'readonly': o.ReadOnly});
                                };
                                
                                $SearchDiv.empty();
                                $SearchDiv.CswSearch({'viewid': viewid,
                                                      'cswnbtnodekey': o.cswnbtnodekey,
                                                      'ID': SearchDivId,
                                                      'onSearchSubmit': onSearchSubmit
                                                      });
                                
                                if ($SearchDiv.is(':hidden'))
	                            {
	                                $SearchDiv.show();
	                            }
	                            else
	                            {
	                                $SearchDiv.hide();
	                            }
                            },
                            'onGenericSearch': function () { /*not possible here*/ }
                        },
		            'onEditView': function (Viewid)
		            {
                        o.onEditView(viewid);                    
		            }
		    });
			$Div.append($MenuDiv, $('<br/><br/>'), $GridDiv);
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
