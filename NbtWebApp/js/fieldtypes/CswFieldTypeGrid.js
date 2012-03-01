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
            var propDiv = o.propDiv;
            propDiv.empty();

            var propVals = o.propData.values;
            var gridMode = Csw.string(propVals.gridmode);
            var maxRows = Csw.string(propVals.maxrows);
            var viewid = Csw.string(propVals.viewid).trim();

            if (o.EditMode === Csw.enums.editMode.AuditHistoryInPopup || o.Multi) {
                propDiv.append('[Grid display disabled]');
            } else {

                var makeFullGrid = function () {
                    'use strict';
                    var menuDiv = propDiv.div({ ID: Csw.controls.dom.makeId(o.ID, 'grid_as_fieldtype_menu') });
                    propDiv.br();
                    var searchDiv = propDiv.div({ ID: Csw.controls.dom.makeId(o.ID, 'grid_as_fieldtype_search') });
                    propDiv.br();
                    var gridDiv = propDiv.div({ ID: Csw.controls.dom.makeId(o.ID, 'grid_as_fieldtype') });


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
                            makeGridMenu(menuDiv, o, gridOpts, grid, viewid, searchDiv);
                        }
                    };
                    gridDiv.$.CswNodeGrid('init', gridOpts);
                };

                var makeSmallGrid = function () {
                    'use strict';
                    Csw.ajax.post({
                        url: Csw.enums.ajaxUrlPrefix + 'getThinGrid',
                        data: {
                            ViewId: viewid,
                            IncludeNodeKey: o.cswnbtnodekey,
                            MaxRows: maxRows
                        },
                        success: function (data) {
                            propDiv.thinGrid({
                                rows: data.rows,
                                onLinkClick: function () {
                                    /* CswDialog.open makeeFullGrid */
                                }
                            });
                        }
                    });
                };

                switch (gridMode.toLowerCase()) {
                    case 'small':
                        makeSmallGrid();
                        break;
                    default:
                        makeFullGrid();
                        break;
                }

            } // if(o.EditMode !== Csw.enums.editMode.AuditHistoryInPopup)
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    function makeGridMenu(menuDiv, o, gridOpts, cswGrid, viewid, searchDiv) {
        //Case 21741
        if (o.EditMode !== Csw.enums.editMode.PrintReport) {
            menuDiv.$.CswMenuMain({
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
                            $.extend(s, gridOpts);
                            s.viewid = searchviewid;
                            o.refreshGrid(s, cswGrid);
                        };

                        var onClearSubmit = function (parentviewid) {
                            var s = {};
                            $.extend(s, gridOpts);
                            s.viewid = parentviewid;
                            o.refreshGrid(s, cswGrid);
                        };

                        searchDiv.empty();
                        searchDiv.$.CswSearch({ parentviewid: viewid,
                            cswnbtnodekey: o.cswnbtnodekey,
                            ID: searchDiv.getId(),
                            onSearchSubmit: onSearchSubmit,
                            onClearSubmit: onClearSubmit
                        });
                    },
                    onGenericSearch: null /*not possible here*/
                },
                onEditView: function () {
                    if (Csw.isFunction(o.onEditView)) {
                        o.onEditView(viewid);
                    }
                }
            }); // CswMenuMain
        } // if( o.EditMode !== Csw.enums.editMode.PrintReport )
    }


    // Method calling logic
    $.fn.CswFieldTypeGrid = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
