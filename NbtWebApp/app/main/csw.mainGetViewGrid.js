/// <reference path="CswApp-vsdoc.js" />

(function _getViewGrid() {

    Csw.main.onReady.then(function() {

        Csw.main.register('getViewGrid', function (options) {
            var o = {
                viewid: '',
                nodeid: '',
                showempty: false,
                nodekey: '',
                doMenuRefresh: true,
                onAddNode: '',
                onEditNode: '',
                onDeleteNode: '',
                onRefresh: '',
                unhideallgridcols: false
            };

            Csw.extend(o, options);

            // Defaults
            var getEmptyGrid = (Csw.bool(o.showempty));
            if (Csw.isNullOrEmpty(o.nodeid)) {
                o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
            }
            if (Csw.isNullOrEmpty(o.nodekey)) {
                o.nodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
            }
            if (false === Csw.isNullOrEmpty(o.viewid)) {
                Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
            }

            o.onEditNode = function () { return nodeGrid.grid.reload(true); };
            o.onDeleteNode = function () { return nodeGrid.grid.reload(true); };
            o.onRefresh = function (options) {
                Csw.main.clear({ centertop: true, centerbottom: true });
                Csw.clientChanges.unsetChanged();
                Csw.main.is.multi = false; // semi-kludge for multi-edit batch op
                return Csw.main.refreshSelected(options);
            };
            Csw.main.clear({ centertop: true, centerbottom: true });

            Csw.nbt.viewFilters({
                name: 'main_viewfilters',
                parent: Csw.main.centerTopDiv,
                viewid: o.viewid,
                onEditFilters: function (newviewid) {
                    var newopts = o;
                    newopts.viewid = newviewid;
                    // set the current view to be the session view, so filters are saved
                    Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.grid.name);
                    return Csw.main.getViewGrid(newopts);
                } // onEditFilters
            }); // viewFilters
            var div = Csw.main.centerBottomDiv.div({ suffix: window.Ext.id() });
            div.empty();
            var nodeGrid = Csw.nbt.nodeGrid(div, {
                viewid: o.viewid,
                nodeid: o.nodeid,
                nodekey: o.nodekey,
                showempty: getEmptyGrid,
                unhideallgridcols: o.unhideallgridcols,
                onEditNode: o.onEditNode,
                onDeleteNode: o.onDeleteNode,
                onRefresh: o.onRefresh,
                onSuccess: function (thisNodeGrid) {
                    if (o.doMenuRefresh) {
                        return Csw.main.refreshMainMenu({
                            viewid: o.viewid,
                            viewmode: Csw.enums.viewMode.grid.name,
                            nodeGrid: thisNodeGrid
                        });
                    }
                },
                onEditView: function (viewid) {
                   return Csw.main.handleAction({
                        'actionname': 'Edit_View',
                        'ActionOptions': {
                            viewid: viewid,
                            viewmode: Csw.enums.viewMode.grid.name,
                            startingStep: 2,
                            IgnoreReturn: true
                        }
                    });
                }
            });

            return nodeGrid;
        });

    });
}());