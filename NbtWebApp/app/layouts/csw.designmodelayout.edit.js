(function () {

    Csw.layouts.register('editNode', function (cswHelpers) {

        var identityTabId = 'identityTab_' + window.Ext.id();
        var tabPanelId = 'tabs_' + window.Ext.id();

        var cswPrivate = {
            renderedTabs: {}
        };
        Csw.extend(cswPrivate, cswHelpers);

        var cswPublic = {};

        cswPublic.render = function (div) {
            cswPrivate.getTabsAjax = Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getTabs',
                data: {
                    EditMode: cswPrivate.Layout,
                    NodeId: cswPrivate.nodeId,
                    SafeNodeKey: cswPrivate.nodeKey,
                    Date: new Date().toDateString(),
                    filterToPropId: '',
                    Multi: false,
                    ConfigMode: true
                },
                success: function (data) {
                    div.div({ cssclass: 'CswIdentityTabHeader' }).append(data.node.nodename);
                    var contentDiv = div.div();

                    var tabs = [];
                    for (var tabIdx in data.tabs) {
                        var tabData = data.tabs[tabIdx];
                        if (tabData.name !== 'Identity') {
                            tabs.push({
                                id: tabData.id,
                                title: tabData.name,
                                listeners: {
                                    activate: function (tab) {
                                        cswPrivate.activeTabId = tab.id;
                                        //cswPrivate.sidebar.refreshExistingProperties('Edit', tab.id);
                                        if (!cswPrivate.renderedTabs[tab.id]) {
                                            cswPrivate.renderedTabs[tab.id] = tab;
                                            cswPrivate.renderTab(tab.id, tab.id);
                                        }
                                    }
                                }
                            });
                        } else {
                            cswPrivate.identityTabId = tabData.id;
                        }
                    }
                    tabs.push({
                        id: window.Ext.id(),
                        title: "New Tab (+)",
                        listeners: {
                            activate: function () {
                                //TODO: add new tab to NodeType edit layout
                            }
                        }
                    });

                    window.Ext.create('Ext.panel.Panel', {
                        renderTo: contentDiv.getId(),
                        layout: {
                            type: 'vbox',
                            align: 'stretch'    // Each takes up full width
                        },
                        items: [{
                            id: identityTabId,
                            xtype: 'panel'
                        }, {
                            id: tabPanelId,
                            xtype: 'tabpanel',
                            items: tabs
                        }]
                    });

                    cswPrivate.renderTab(identityTabId, cswPrivate.identityTabId);

                }
            });
        };

        return cswPublic;
    });
})();