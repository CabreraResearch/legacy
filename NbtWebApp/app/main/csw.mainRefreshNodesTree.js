/// <reference path="CswApp-vsdoc.js" />

(function _mainTearDown() {

    Csw.main.onReady.then(function() {
        
        Csw.main.register('onSelectTreeNode', function (options) {
            //if (debugOn()) Csw.debug.log('Main.onSelectTreeNode()');
            if (Csw.clientChanges.manuallyCheckChanges()) {
                var o = {
                    tree: null,
                    viewid: '',
                    nodeid: '',
                    nodename: '',
                    iconurl: '',
                    nodekey: ''
                };
                Csw.extend(o, options);

                Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeId, o.nodeid);
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeKey, o.nodekey);

                if (o.nodeid !== '' && o.nodeid !== 'root') {
                    Csw.main.getTabs({
                        viewid: o.viewid,
                        nodeid: o.nodeid,
                        nodekey: o.nodekey
                    });
                    Csw.main.refreshMainMenu({
                        parent: o.tree.menuDiv,
                        viewid: o.viewid,
                        viewmode: Csw.enums.viewMode.tree.name,
                        nodeid: o.nodeid,
                        nodekey: o.nodekey
                    });
                } else {
                    Csw.main.showDefaultContentTree({ viewid: o.viewid, viewmode: Csw.enums.viewMode.tree.name });
                    Csw.main.refreshMainMenu({
                        parent: o.tree.menuDiv,
                        viewid: o.viewid,
                        viewmode: Csw.enums.viewMode.tree.name,
                        nodeid: '',
                        nodekey: ''
                    });
                }
            }
        }); // onSelectTreeNode()

        Csw.main.register('refreshNodesTree', function (options) {
            var o = {
                'nodeid': '',
                'nodekey': '',
                'nodename': '',
                'showempty': false,
                'forsearch': false,
                'iconurl': '',
                'viewid': '',
                'viewmode': 'tree',
                'IncludeNodeRequired': false
            };
            Csw.extend(o, options);

            if (Csw.isNullOrEmpty(o.nodeid)) {
                o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
                if (Csw.isNullOrEmpty(o.nodekey)) {
                    o.nodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
                }
            }
            if (Csw.isNullOrEmpty(o.viewid)) {
                o.viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
            }

            Csw.main.clear({ left: true });

            Csw.nbt.viewFilters({
                name: 'main_viewfilters',
                parent: Csw.main.leftDiv,
                viewid: o.viewid,
                onEditFilters: function (newviewid) {
                    var newopts = o;
                    newopts.viewid = newviewid;
                    // set the current view to be the session view, so filters are saved
                    Csw.clientState.setCurrentView(newviewid, o.viewmode);
                    Csw.main.refreshNodesTree(newopts);
                } // onEditFilters
            }); // viewFilters

            Csw.main.mainTree = Csw.nbt.nodeTreeExt(Csw.main.leftDiv, {
                forSearch: o.forsearch,
                onBeforeSelectNode: Csw.clientChanges.manuallyCheckChanges,
                onSelectNode: function (optSelect) {
                    Csw.main.onSelectTreeNode({
                        tree: Csw.main.mainTree,
                        viewid: optSelect.viewid,
                        nodeid: optSelect.nodeid,
                        nodekey: optSelect.nodekey
                    });
                },
                isMulti: Csw.main.is.multi,
                state: {
                    viewId: o.viewid,
                    viewMode: o.viewmode,
                    nodeId: o.nodeid,
                    nodeKey: o.nodekey,
                    includeInQuickLaunch: true,
                    includeNodeRequired: o.IncludeNodeRequired,
                    onViewChange: function (newviewid, newviewmode) {
                        Csw.clientState.setCurrentView(newviewid, newviewmode);
                    }
                }
            });
        }); // refreshNodesTree()

    });
}());