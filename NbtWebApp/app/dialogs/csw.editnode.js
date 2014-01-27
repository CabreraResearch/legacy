/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('editnode', function (cswPrivate) {
        'use strict';
        
        var cswPublic = {
            closed: false
        };
        
        var prevNodeId = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
        var prevNodeKey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
        Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeId, cswPrivate.currentNodeId);
        Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeKey, cswPrivate.currentNodeKey);

        var doRefresh = true;

        (function _preCtor() {
            cswPrivate.selectedNodeIds = cswPrivate.selectedNodeIds || Csw.delimitedString();
            cswPrivate.selectedNodeKeys = cswPrivate.selectedNodeKeys|| Csw.delimitedString();
            cswPrivate.currentNodeId = cswPrivate.currentNodeId|| '';
            cswPrivate.currentNodeKey = cswPrivate.currentNodeKey|| '';
            cswPrivate.nodenames = cswPrivate.nodenames|| [];
            cswPrivate.Multi = cswPrivate.Multi || false;
            cswPrivate.ReadOnly = cswPrivate.ReadOnly || false;
            cswPrivate.filterToPropId = cswPrivate.filterToPropId || '';
            cswPrivate.onEditNode = cswPrivate.onEditNode || null;
            cswPrivate.onEditView = cswPrivate.onEditView || null;
            cswPrivate.onRefresh = cswPrivate.onRefresh || null;
            cswPrivate.onClose = cswPrivate.onClose || null;
            cswPrivate.onAfterButtonClick = cswPrivate.onAfterButtonClick || null;
            cswPrivate.date = cswPrivate.date || '';
            cswPrivate.editMode = Csw.enums.editMode.EditInPopup;
            cswPrivate.name = cswPrivate.name || 'EditNode';
            cswPrivate.title = cswPrivate.title || 'Edit';

            var title = Csw.string(cswPrivate.title);
            if (cswPrivate.nodenames.length > 1) {
                title += ': ' + cswPrivate.nodenames.join(', ');
            }
            cswPrivate.title = title;
            cswPublic.title = cswPrivate.title;
        }());

        (function _postCtor() {
            var editDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: 1100,
                height: 700,
                onOpen: function () {
                    var table = editDialog.div.table({ width: '100%' });
                    var tabCell = table.cell(1, 2);
                    tabCell.empty();
                    cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(tabCell, {
                        forceReadOnly: cswPrivate.ReadOnly,
                        Multi: cswPrivate.Multi,
                        tabState: {
                            date: cswPrivate.date,
                            selectedNodeIds: cswPrivate.selectedNodeIds,
                            selectedNodeKeys: cswPrivate.selectedNodeKeys,
                            nodenames: cswPrivate.nodenames,
                            filterToPropId: cswPrivate.filterToPropId,
                            nodeid: cswPrivate.currentNodeId || cswPrivate.selectedNodeIds.first(),
                            nodekey: cswPrivate.currentNodeKey || cswPrivate.selectedNodeKeys.first(),
                            ReadOnly: cswPrivate.ReadOnly,
                            EditMode: cswPrivate.editMode,
                            tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId)
                        },
                        ReloadTabOnSave: true,
                        Refresh: cswPrivate.onRefresh,
                        onEditView: function(viewid) {
                            doRefresh = false; //We're loading the view editor, don't refresh when the dialog closes                        
                            cswPublic.div.$.dialog('close');
                            Csw.tryExec(cswPrivate.onEditView, viewid);
                        },
                        onSave: function(nodeids, nodekeys, tabcount) {
                            Csw.clientChanges.unsetChanged();
                            if (tabcount <= 2 || cswPrivate.Multi) { /* Ignore history tab */
                                if (false === cswPublic.closed) {
                                    editDialog.close();
                                    cswPublic.div.$.dialog('close');
                                }
                            }
                            Csw.tryExec(cswPrivate.onEditNode, nodeids, nodekeys, cswPublic.close);
                        },
                        onBeforeTabSelect: function() {
                            return Csw.clientChanges.manuallyCheckChanges();
                        },
                        onTabSelect: function(tabid) {
                            Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, tabid);
                        },
                        onPropertyChange: function() {
                            Csw.clientChanges.setChanged();
                        },
                        onAfterButtonClick: cswPrivate.onAfterButtonClick
                    });
                },
                onClose: function () {
                    if (false === cswPublic.closed && doRefresh) {
                        //Case 31402 - when we close the dialog, set the cookies to the node on the main screen
                        Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeId, prevNodeId);
                        Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeKey, prevNodeKey);



                        cswPublic.closed = true;
                        cswPublic.tabsAndProps.tearDown();
                        Csw.tryExec(cswPrivate.onClose);
                    }
                }
            });
            editDialog.open();
        }());

        return cswPublic;
    });
}());