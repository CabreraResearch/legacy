/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    Csw.main.onReady.then(function () {

        Csw.main.register('getTabs', function (options) {
            Csw.publish('initPropertyTearDown');
            var o = {
                nodeid: '',
                nodekey: '',
                viewid: ''
            };
            Csw.extend(o, options);

            Csw.main.clear({ right: true });

            if (Csw.main.is.oneTimeReset ||
                Csw.isNullOrEmpty(Csw.main.tabsAndProps) ||
                o.viewid !== Csw.main.tabsAndProps.getViewId()) {
                Csw.main.tabsAndProps = Csw.layouts.tabsAndProps(Csw.main.rightDiv, {
                    name: 'nodetabs',
                    tabState: {
                        viewid: o.viewid,
                        ShowCheckboxes: Csw.main.is.multi,
                        tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId),
                        nodeid: o.nodeid,
                        nodekey: o.nodekey
                    },
                    onSave: function () {
                        Csw.clientChanges.unsetChanged();
                    },
                    onBeforeTabSelect: function () {
                        return Csw.clientChanges.manuallyCheckChanges();
                    },
                    Refresh: function (options) {
                        Csw.clientChanges.unsetChanged();
                        Csw.main.is.multi = false; // semi-kludge for multi-edit batch op
                        Csw.main.refreshSelected(options);
                    },
                    onTabSelect: function (tabid) {
                        Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, tabid);
                    },
                    onPropertyChange: function () {
                        Csw.clientChanges.setChanged();
                    },
                    onEditView: function (viewid, viewmode) {
                        Csw.main.handleAction({
                            actionname: 'Edit_View',
                            ActionOptions: {
                                viewid: viewid,
                                viewmode: viewmode || Csw.enums.viewMode.grid.name,
                                startingStep: 2,
                                IgnoreReturn: true
                            }
                        });
                    },
                    nodeTreeCheck: Csw.main.mainTree
                });
            } else {
                Csw.main.tabsAndProps.resetTabs(o.nodeid, o.nodekey);
            }
        });

    });
}());