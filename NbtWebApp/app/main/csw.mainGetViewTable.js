/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    Csw.main.onReady.then(function() {

        Csw.main.register('getViewTable', function (options) {
            var o = {
                viewid: '',
                //            nodeid: '',
                //            nodekey: '',
                //			doMenuRefresh: true,
                //			onAddNode: '',
                onEditNode: '',
                onDeleteNode: ''
            };
            if (options) {
                Csw.extend(o, options);
            }

            // Defaults
            if (Csw.isNullOrEmpty(o.nodeid)) {
                o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
            }
            if (Csw.isNullOrEmpty(o.nodekey)) {
                o.nodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
            }
            if (false === Csw.isNullOrEmpty(o.viewid)) {
                Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
            }

            o.onEditNode = function () { Csw.main.getViewTable(o); };
            o.onDeleteNode = function () { Csw.main.getViewTable(o); };

            Csw.main.clear({ centertop: true, centerbottom: true });

            Csw.nbt.viewFilters({
                name: 'main_viewfilters',
                parent: Csw.main.centerTopDiv,
                viewid: o.viewid,
                onEditFilters: function (newviewid) {
                    var newopts = o;
                    newopts.viewid = newviewid;
                    // set the current view to be the session view, so filters are saved
                    Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.table.name);
                    Csw.main.getViewTable(newopts);
                } // onEditFilters
            }); // viewFilters

            return Csw.nbt.nodeTable(Csw.main.centerBottomDiv, {
                viewid: o.viewid,
                //            nodeid: o.nodeid,
                //            nodekey: o.nodekey,
                Multi: Csw.main.is.multi,
                //'onAddNode': o.onAddNode,
                onEditNode: o.onEditNode,
                onDeleteNode: o.onDeleteNode,
                onSuccess: function () {
                    Csw.main.refreshMainMenu({
                        viewid: o.viewid,
                        viewmode: Csw.enums.viewMode.table.name//,
                        //                    nodeid: o.nodeid,
                        //                    nodekey: o.nodekey
                    });
                },
                onNoResults: Csw.main.showDefaultContentTable
            });
        });

    });
}());