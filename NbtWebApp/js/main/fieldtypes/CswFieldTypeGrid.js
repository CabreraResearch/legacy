/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";        
    var pluginName = 'CswFieldTypeGrid';
   
    var methods = {
        'init': function (o) { 
            /// <summary>
            ///   Initializes a jqGrid as an NbtNode Prop
            /// </summary>
            /// <param name="o" type="Object">
            ///     A JSON Object
            /// </param>
            var $Div = $(this);
            $Div.empty();
            var propVals = o.propData.values;
            if (o.EditMode === Csw.enums.editMode.AuditHistoryInPopup || o.Multi) {
                $Div.append('[Grid display disabled]');
            } else {

                var menuDivId = Csw.controls.dom.makeId({prefix: o.ID, ID: 'grid_as_fieldtype_menu'});
                var $MenuDiv = $('<div id="' + menuDivId + '" name="' + menuDivId + '"></div>');

                var searchDivId = Csw.controls.dom.makeId({prefix: o.ID, ID: 'grid_as_fieldtype_search'});
                var $SearchDiv = $('<div id="' + searchDivId + '" name="' + searchDivId + '"></div>');

                var gridDivId = Csw.controls.dom.makeId({prefix: o.ID, ID: 'grid_as_fieldtype'});
                var $GridDiv = $('<div id="' + gridDivId + '" name="' + gridDivId + '"></div>');

                var viewid = Csw.string(propVals.viewid).trim();
                var gridOpts = {
                    ID: o.ID + '_fieldtypegrid',
                    viewid: viewid, 
                    nodeid: o.nodeid, 
                    cswnbtnodekey: o.cswnbtnodekey, 
                    readonly: o.ReadOnly,
                    reinit: false,
                    EditMode: o.EditMode,
                    onEditNode: function () { 
                        o.onReload();
                    },
                    onDeleteNode: function () { 
                        o.onReload();
                    },
                    onSuccess: function (grid) {
                        makeGridMenu($MenuDiv, o, gridOpts, grid, viewid, $SearchDiv);
                    }
                };
                $GridDiv.CswNodeGrid('init', gridOpts);
                
                $Div.append($MenuDiv, $('<br/>'), $SearchDiv, $('<br/>'), $GridDiv);
            } // if(o.EditMode !== Csw.enums.editMode.AuditHistoryInPopup)
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };
    
    function makeGridMenu($MenuDiv, o, gridOpts, cswGrid, viewid, $SearchDiv) {
        //Case 21741
        if (o.EditMode !== Csw.enums.editMode.PrintReport) {
            $MenuDiv.CswMenuMain({
                    viewid: viewid,
                    nodeid: o.nodeid,
                    cswnbtnodekey: o.cswnbtnodekey,
                    propid: o.ID,
                    onAddNode: function () {
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
                    onSearch: {
                        onViewSearch: function () {
                            var onSearchSubmit = function (searchviewid) {
                                var s = {};
                                $.extend(s,gridOpts);
                                s.viewid = searchviewid;
                                refreshGrid(s, cswGrid);
                            };
                                
                            var onClearSubmit = function (parentviewid) {
                                var s = {};
                                $.extend(s,gridOpts);
                                s.viewid = parentviewid;
                                refreshGrid(s, cswGrid);
                            };

                            $SearchDiv.empty();
                            $SearchDiv.CswSearch({parentviewid: viewid,
                                                    cswnbtnodekey: o.cswnbtnodekey,
                                                    ID: $SearchDiv.CswAttrDom('id'),
                                                    onSearchSubmit: onSearchSubmit,
                                                    onClearSubmit: onClearSubmit
                                                    });
                        },
                        onGenericSearch: null /*not possible here*/
                    },
                    onEditView: function () {
                        if(Csw.isFunction(o.onEditView))
                        {
                            o.onEditView(viewid);
                        }
                    }
            }); // CswMenuMain
        } // if( o.EditMode !== Csw.enums.editMode.PrintReport )
    }
    
    
    // Method calling logic
    $.fn.CswFieldTypeGrid = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
