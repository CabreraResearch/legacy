
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswNodeGrid';

    var cswPrivate = {
        selectedRowId: ''
    };

    var methods = {

        'init': function (options) {

            var o = {
                runGridUrl: 'runGrid',
                viewid: '',
                showempty: false,
                name: '',
                nodeid: '',
                nodekey: '',
                reinit: false,
                forceFit: false,
                EditMode: Csw.enums.editMode.Edit,
                readonly: false,
                onEditNode: null,
                onDeleteNode: null,
                canSelectRow: false,
                onSelect: null,
                onSuccess: null,
                onEditView: null,
                onRefresh: null,
                height: '',
                includeInQuickLaunch: true
            };

            if (options) {
                Csw.extend(o, options);
            }
            var $parent = $(this);

            if (o.reinit) $parent.empty();

            var forReporting = (o.EditMode === Csw.enums.editMode.PrintReport),
                ret;

            /* fetchGridSkeleton */
            (function () {

                var parent = Csw.literals.factory($parent);
                ret = parent.grid({
                                    name: o.name,
                                    stateId: o.viewid,
                                    ajax: {
                                        urlMethod: o.runGridUrl,
                                        data: {
                                            ViewId: o.viewid,
                                            IncludeNodeKey: o.nodekey,
                                            IncludeInQuickLaunch: o.includeInQuickLaunch,
                                            ForReport: forReporting
                                        }
                                    },
                                    forceFit: o.forceFit,
                                    usePaging: false === forReporting,
                                    showActionColumn: false === forReporting && false === o.readonly,
                                    height: o.height,
                                    canSelectRow: o.canSelectRow,
                                    onSelect: o.onSelect,
                                    onEdit: function(rows) {
                                        // this works for both Multi-edit and regular
                                        var nodekeys = Csw.delimitedString(),
                                            nodeids = Csw.delimitedString(),
                                            nodenames = [],
                                            firstNodeId, firstNodeKey;
    
                                        Csw.each(rows, function(row) {
                                            firstNodeId = firstNodeId || row.nodeid;
                                            firstNodeKey = firstNodeKey || row.nodekey;
                                            nodekeys.add(row.nodekey);
                                            nodeids.add(row.nodeid);
                                            nodenames.push(row.nodename);
                                        });

                                        $.CswDialog('EditNodeDialog', {
                                            currentNodeId: firstNodeId,
                                            currentNodeKey: firstNodeKey,
                                            selectedNodeIds: nodeids,
                                            selectedNodeKeys: nodekeys,
                                            nodenames: nodenames,
                                            Multi: (nodeids.count() > 1),
                                            onEditNode: o.onEditNode,
                                            onEditView: o.onEditView,
                                            onRefresh:  o.onRefresh
                                        });
                                    }, // onEdit
                                    onDelete: function(rows) {
                                        // this works for both Multi-edit and regular
                                        var nodes = { };
    
                                        Csw.each(rows, function(row) {
                                            nodes[row.nodeid] = {
                                                nodeid: row.nodeid,
                                                nodekey: row.nodekey,
                                                nodename: row.nodename
                                            };
                                        });

                                        $.CswDialog('DeleteNodeDialog', {
                                            nodes: nodes,
                                            onDeleteNode: o.onDeleteNode,
                                            Multi: (nodes.length > 1),
                                            publishDeleteEvent: false
                                        });
                                    } // onDelete
                                }); // grid()

                Csw.tryExec(o.onSuccess, ret);

            })();
            return ret;
        } // 'init'
    }; // methods

    $.fn.CswNodeGrid = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }
    };

})(jQuery);

