/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswGrid.js" />
/// <reference path="../node/CswNodeGrid.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeGrid';
   
    var methods = {
        'init': function(o) { 
            /// <summary>
            ///   Initializes a jqGrid as an NbtNode Prop
            /// </summary>
            /// <param name="o" type="Object">
            ///     A JSON Object
            /// </param>
            var $Div = $(this);
            $Div.empty();
            var propVals = o.propData.values;
            if (o.EditMode === EditMode.AuditHistoryInPopup.name || o.Multi) {
                $Div.append('[Grid display disabled]');
            } else {

                var menuDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype_menu'});
                var $MenuDiv = $('<div id="' + menuDivId + '" name="' + menuDivId + '"></div>');

                var searchDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype_search'});
                var $SearchDiv = $('<div id="' + searchDivId + '" name="' + searchDivId + '"></div>');

                var gridDivId = makeId({prefix: o.ID, ID: 'grid_as_fieldtype'});
                var $GridDiv = $('<div id="' + gridDivId + '" name="' + gridDivId + '"></div>');

                var viewid = tryParseString(propVals.viewid).trim();
                var cswGrid;
                var gridOpts = {
                    ID: o.ID + '_fieldtypegrid',
                    viewid: viewid, 
                    nodeid: o.nodeid, 
                    cswnbtnodekey: o.cswnbtnodekey, 
                    readonly: o.ReadOnly,
                    reinit: false,
                    EditMode: o.EditMode,
                    onEditNode: function() { 
                        //refreshGrid(gridOpts, cswGrid);
                        o.onReload();
                    },
    //                'onAddNode': function() { 
    //                    refreshGrid(gridOpts);
    //                },
                    onDeleteNode: function() { 
                        //refreshGrid(gridOpts, cswGrid);
                        o.onReload();
                    },
                    onSuccess: function (grid) {
                        makeGridMenu($MenuDiv, o, gridOpts, grid, viewid);
                    }
                };

                cswGrid = $GridDiv.CswNodeGrid('init', gridOpts);
                
                $Div.append($MenuDiv, $('<br/>'), $SearchDiv, $('<br/>'), $GridDiv);
            } // if(o.EditMode !== EditMode.AuditHistoryInPopup.name)
        },
        save: function(o) {
//          var $TextBox = $propdiv.find('input');
//          $xml.children('barcode').text($TextBox.val());
            preparePropJsonForSave(o.propData);
        }
    };
    
//    function refreshGrid(options, cswGrid) { 
//		var g = {
//			gridOpts: {
//				reinit: true,
//				multiselect: false
//			}
//		};
//		if( options ) $.extend(options,g);
//		cswGrid.changeGridOpts(g);
//	};
    
    function makeGridMenu($MenuDiv, o, gridOpts, cswGrid, viewid) {
        //Case 21741
        if (o.EditMode !== EditMode.PrintReport.name) {
            $MenuDiv.CswMenuMain({
                    viewid: viewid,
                    nodeid: o.nodeid,
                    cswnbtnodekey: o.cswnbtnodekey,
                    propid: o.ID,
                    onAddNode: function () {
                        //refreshGrid(gridOpts, cswGrid);
                        o.onReload();
                    },
                    onPrintView: function () {
                        cswGrid.print();    
                    },
                    onMultiEdit: function () {
                        var multi = (false === cswGrid.isMulti());
                        var g = {
                            gridOpts: {
                                multiselect: multi
                            }
                        };
                        cswGrid.changeGridOpts(g);
                    },
//                    onSearch: {
//						onViewSearch: function () {
//							var onSearchSubmit = function(searchviewid) {
//								var s = {};
//								$.extend(s,gridOpts);
//								s.viewid = searchviewid;
//								refreshGrid(s, cswGrid);
//							};
//                                
//							var onClearSubmit = function(parentviewid) {
//								var s = {};
//								$.extend(s,gridOpts);
//								s.viewid = parentviewid;
//								refreshGrid(s, cswGrid);
//							};

//							$SearchDiv.empty();
//							$SearchDiv.CswSearch({parentviewid: viewid,
//													cswnbtnodekey: o.cswnbtnodekey,
//													ID: searchDivId,
//													onSearchSubmit: onSearchSubmit,
//													onClearSubmit: onClearSubmit
//													});
//						},
//						onGenericSearch: function () { /*not possible here*/ }
//					},
                    onEditView: function () {
                        if(isFunction(o.onEditView))
                        {
                            o.onEditView(viewid);
                        }
                    }
            }); // CswMenuMain
        } // if( o.EditMode !== EditMode.PrintReport.name )
    }
    
    
    // Method calling logic
    $.fn.CswFieldTypeGrid = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
