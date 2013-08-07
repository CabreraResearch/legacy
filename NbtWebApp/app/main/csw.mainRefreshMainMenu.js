/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    Csw.main.onReady.then(function() {

        Csw.main.register('refreshMainMenu', function (options) {
            var o = {
                parent: Csw.main.mainMenuDiv,
                viewid: '',
                viewmode: '',
                nodeid: '',
                nodekey: '',
                nodetypeid: '',
                propid: '',
                limitMenuTo: '',
                readonly: false
            };
            Csw.extend(o, options);

            //Csw.main.mainMenuDiv.empty();
            if (Csw.main.mainMenu) {
                Csw.main.mainMenu.abort();
            }

            var menuOpts = {
                width: '',
                ajax: {
                    urlMethod: 'getMainMenu',
                    data: {
                        ViewId: o.viewid,
                        SafeNodeKey: o.nodekey,
                        NodeTypeId: o.nodetypeid,
                        PropIdAttr: o.propid,
                        LimitMenuTo: o.limitMenuTo,
                        ReadOnly: o.readonly,
                        NodeId: o.nodeid
                    }
                },
                onAlterNode: function (nodeid, nodekey) {
                    var state = Csw.clientState.getCurrent();
                    Csw.main.refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true, 'searchid': state.searchid });
                },
                onMultiEdit: function () {
                    switch (o.viewmode) {
                        case Csw.enums.viewMode.grid.name:
                            o.nodeGrid.grid.toggleShowCheckboxes();
                            break;
                        default:
                            Csw.publish('CswMultiEdit', {
                                multi: Csw.main.is.toggleMulti(),
                                nodeid: o.nodeid,
                                viewid: o.viewid
                            });
                            //refreshSelected({ nodeid: o.nodeid, viewmode: o.viewmode, nodekey: o.nodekey });
                            break;
                    } // switch
                },
                onEditView: function () {
                    Csw.main.handleAction({
                        'actionname': 'Edit_View',
                        'ActionOptions': {
                            'viewid': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId),
                            'viewmode': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode)
                        }
                    });
                },
                onSaveView: function (newviewid) {
                    Csw.main.handleItemSelect({ 'viewid': newviewid, 'viewmode': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode) });
                },
                onPrintView: function () {
                    switch (o.viewmode) {
                        case Csw.enums.viewMode.grid.name:
                            if (false == Csw.isNullOrEmpty(o.nodeGrid.grid)) {
                                o.nodeGrid.grid.print();
                            }
                            break;
                        default:
                            Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'View Printing is not enabled for views of type ' + o.viewmode));
                            break;
                    }
                },
                Multi: Csw.main.is.multi,
                viewMode: o.viewmode,
                nodeTreeCheck: Csw.main.mainTree,
                nodeGrid: o.nodeGrid
            };

            Csw.main.mainMenu = o.parent.menu(menuOpts);
            return Csw.main.mainMenu;
        });

    });
}());