
(function () {

    Csw.nbt.nodeGrid = Csw.nbt.nodeGrid ||
        Csw.nbt.register('nodeGrid', function(cswParent, params) {

            var cswPublic = { };

            var cswPrivate = {
                selectedRowId: '',
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

            Csw.extend(cswPrivate, params);

            cswPrivate.forReporting = (cswPrivate.EditMode === Csw.enums.editMode.PrintReport);


            /* fetchGridSkeleton */
            (function() {

                cswPublic = cswParent.grid({
                    name: cswPrivate.name,
                    stateId: cswPrivate.viewid,
                    ajax: {
                        urlMethod: cswPrivate.runGridUrl,
                        data: {
                            ViewId: cswPrivate.viewid,
                            IncludeNodeKey: cswPrivate.nodekey,
                            IncludeInQuickLaunch: cswPrivate.includeInQuickLaunch,
                            ForReport: cswPrivate.forReporting
                        }
                    },
                    forceFit: cswPrivate.forceFit,
                    usePaging: false === cswPrivate.forReporting,
                    showActionColumn: false === cswPrivate.forReporting && false === cswPrivate.readonly,
                    height: cswPrivate.height,
                    canSelectRow: cswPrivate.canSelectRow,
                    onSelect: cswPrivate.onSelect,
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
                            onEditNode: cswPrivate.onEditNode,
                            onEditView: cswPrivate.onEditView,
                            onRefresh: cswPrivate.onRefresh
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
                            onDeleteNode: cswPrivate.onDeleteNode,
                            Multi: (nodes.length > 1),
                            publishDeleteEvent: false
                        });
                    } // onDelete
                }); // grid()

                Csw.tryExec(cswPrivate.onSuccess, cswPublic);

            })();
            return cswPublic;

        });
}());

