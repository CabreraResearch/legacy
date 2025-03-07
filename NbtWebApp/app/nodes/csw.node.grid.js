
(function () {


    Csw.nbt.register('nodeGrid', function (cswParent, params) {

        var cswPublic = {};

        var cswPrivate = {
            selectedRowId: '',
            viewid: '',
            showempty: false,
            name: '',
            title: '',
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
            onButtonRender: function (div, colObj, thisBtn) { // do not override please
                div.empty();
                var menuOptions = [];
                if (thisBtn[0].menuoptions) {
                    menuOptions = thisBtn[0].menuoptions.split(',');
                }
                div.nodeButton({
                    displayName: colObj.header,
                    size: 'small',
                    propId: thisBtn[0].propattr,
                    onRefresh: cswPrivate.onEditNode,
                    mode: thisBtn[0].mode,
                    menuOptions: menuOptions
                });
            },
            showCheckboxes: false,
            height: '',
            includeInQuickLaunch: true,
            unhideallgridcols: false
        };

        Csw.extend(cswPrivate, params);

        cswPrivate.forReporting = (cswPrivate.EditMode === Csw.enums.editMode.PrintReport);

        cswPublic.grid = cswParent.grid({
            name: cswPrivate.name,
            stateId: cswPrivate.name + '_' + cswPrivate.viewid,
            ajax: {
                urlMethod: 'runGrid',
                data: {
                    Title: cswPrivate.title,
                    ViewId: cswPrivate.viewid,
                    IncludeNodeId: Csw.string(cswPrivate.nodeid),
                    IncludeNodeKey: cswPrivate.nodekey,
                    IncludeInQuickLaunch: cswPrivate.includeInQuickLaunch,
                    ForReport: cswPrivate.forReporting
                }
            },
            forceFit: cswPrivate.forceFit,
            usePaging: false === cswPrivate.forReporting,
            showCheckboxes: cswPrivate.showCheckboxes,
            showActionColumn: false === cswPrivate.forReporting, // && false === cswPrivate.readonly, //case 29553
            height: cswPrivate.height,
            canSelectRow: cswPrivate.canSelectRow,
            onSelect: cswPrivate.onSelect,
            unhideallgridcols: cswPrivate.unhideallgridcols,
            onEdit: function (rows) {
                // this works for both Multi-edit and regular
                var nodekeys = Csw.delimitedString(),
                    nodeids = Csw.delimitedString(),
                    nodenames = [],
                    firstNodeId, firstNodeKey;

                Csw.iterate(rows, function (row) {
                    firstNodeId = firstNodeId || row.nodeid;
                    firstNodeKey = firstNodeKey || row.nodekey;
                    nodekeys.add(row.nodekey);
                    nodeids.add(row.nodeid);
                    nodenames.push(row.nodename);
                });

                Csw.dialogs.editnode({
                    currentNodeId: firstNodeId,
                    currentNodeKey: firstNodeKey,
                    selectedNodeIds: nodeids,
                    selectedNodeKeys: nodekeys,
                    nodenames: nodenames,
                    Multi: (nodeids.count() > 1),
                    onEditNode: cswPrivate.onEditNode,
                    onEditView: cswPrivate.onEditView,
                    //onClose: function () {
                    //    cswPublic.reload(true);
                    //},
                    onRefresh: cswPrivate.onRefresh
                });
                
            }, // onEdit
            onDelete: function (rows) {
                // this works for both Multi-edit and regular
                var nodes = {};

                Csw.iterate(rows, function (row) {
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
            }, // onDelete
            onPreview: function (o, nodeObj, event) {
                var preview = Csw.nbt.nodePreview(Csw.main.body, {
                    nodeid: nodeObj.nodeid,
                    nodekey: nodeObj.nodekey,
                    nodename: nodeObj.nodename,
                    event: event
                });
                preview.open();
            },
            onButtonRender: cswPrivate.onButtonRender
        }); // grid()

        cswPublic.getSelectedNodes = function () {
            var nodes = [];
            cswPublic.grid.iterateSelectedRowRaw(function (rawRow) {
                nodes.push({
                    nodeid: rawRow.nodeid,
                    nodename: rawRow.nodename
                });
            });
            return nodes;
        };

        Csw.tryExec(cswPrivate.onSuccess, cswPublic);

        return cswPublic;

    });

}());

