/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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

            var reinitGrid;
            function makeGridMenu(menuDiv, o, gridOpts, cswGrid, viewid) {
                //Case 21741
                if (o.EditMode !== Csw.enums.editMode.PrintReport) {

                    var menuOpts = { 
                        width: 150,
                        ajax: { 
                            urlMethod: 'getMainMenu', 
                            data: {
                                ViewId: viewid,
                                SafeNodeKey: o.cswnbtnodekey,
                                PropIdAttr: o.ID,
                                LimitMenuTo: '',
                                ReadOnly: o.ReadOnly
                            }
                        },
                        onAlterNode: function () { reinitGrid(); },
                        onMultiEdit: function () { cswGrid.toggleShowCheckboxes(); },
                        onEditView: function () { Csw.tryExec(o.onEditView, viewid); },
                        onSaveView: null,
                        onPrintView: function () { cswGrid.print(); },
                        Multi: false,
                        nodeTreeCheck: ''
                    };
                    Csw.composites.menu( menuDiv, menuOpts );

                } // if( o.EditMode !== Csw.enums.editMode.PrintReport )
            } // makeGridMenu()


            var propDiv = o.propDiv;
            propDiv.empty();

            var propVals = o.propData.values;
            var gridMode = Csw.string(propVals.gridmode);
            var maxRows = Csw.string(propVals.maxrows);
            var viewid = Csw.string(propVals.viewid).trim();

            if (o.EditMode === Csw.enums.editMode.AuditHistoryInPopup || o.Multi) {
                propDiv.append('[Grid display disabled]');
            } else {

                var makeFullGrid = function (viewid, newDiv) {
                    'use strict';
                    newDiv.empty();
                    var menuDiv = newDiv.div({ ID: Csw.makeId(o.ID, 'grid_as_fieldtype_menu') }).css({ height: '25px' });
                    //newDiv.br();
                    var filterDiv = newDiv.div({ ID: Csw.makeId(o.ID, 'grid_as_fieldtype_filter') });
                    //newDiv.br();
                    var gridDiv = newDiv.div({ ID: Csw.makeId(o.ID, 'grid_as_fieldtype') });
                    reinitGrid = (function () {
                        return function () {
                            makeFullGrid(viewid, newDiv);
                        };
                    } ());
                    Csw.nbt.viewFilters({
                        ID: o.ID + '_viewfilters',
                        parent: filterDiv,
                        viewid: viewid,
                        onEditFilters: function (newviewid) {
                            makeFullGrid(newviewid, newDiv);
                        } // onEditFilters
                    }); // viewFilters

                    var gridOpts = {
                        ID: o.ID + '_fieldtypegrid',
//                        resizeWithParent: false,
//                        resizeWithParentElement: $('#nodetabs_props'),
                        viewid: viewid,
                        nodeid: o.nodeid,
                        cswnbtnodekey: o.cswnbtnodekey,
                        readonly: o.ReadOnly,
                        reinit: false,
                        EditMode: o.EditMode,
                        onEditNode: function () {
                            //o.onReload();
                            reinitGrid();
                        },
                        onDeleteNode: function () {
                            //o.onReload();
                            reinitGrid();
                        },
                        onSuccess: function (grid) {
                            makeGridMenu(menuDiv, o, gridOpts, grid, viewid);
                        }
                    };
                    gridDiv.$.CswNodeGrid('init', gridOpts);
                };

                var makeSmallGrid = function () {
                    'use strict';
                    Csw.ajax.post({
                        urlMethod: 'getThinGrid',
                        data: {
                            ViewId: viewid,
                            IncludeNodeKey: o.cswnbtnodekey,
                            MaxRows: maxRows
                        },
                        success: function (data) {
                            propDiv.thinGrid({
                                rows: data.rows,
                                onLinkClick: function () {
                                    $.CswDialog('OpenEmptyDialog', {
                                        title: o.nodename + ' ' + o.propData.name,
                                        onOpen: function (dialogDiv) {
                                            makeFullGrid(viewid, dialogDiv);
                                        },
                                        onClose: o.onReload
                                    }
                                    );
                                }
                            });
                        }
                    });
                };

                var makeLinkGrid = function () {
                    'use strict';
                    Csw.ajax.post({
                        urlMethod: 'getGridRowCount',
                        data: {
                            ViewId: viewid,
                            IncludeNodeKey: o.cswnbtnodekey
                        },
                        success: function (data) {
                            propDiv.linkGrid({
                                rowCount: data.rowCount,
                                linkText: '',
                                onLinkClick: function () {
                                    $.CswDialog('OpenEmptyDialog', {
                                        title: o.propData.name,
                                        onOpen: function (dialogDiv) {
                                            makeFullGrid(viewid, dialogDiv);
                                        },
                                        onClose: o.onReload
                                    }
                                    );
                                }
                            });
                        }
                    });
                };

                switch (gridMode.toLowerCase()) {
                    case 'small':
                        makeSmallGrid();
                        break;
                    case 'link':
                        makeLinkGrid();
                        break;
                    default:
                        makeFullGrid(viewid, propDiv);
                        break;
                }

            } // if(o.EditMode !== Csw.enums.editMode.AuditHistoryInPopup)
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };


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
