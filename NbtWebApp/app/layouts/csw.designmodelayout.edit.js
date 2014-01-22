(function () {

    Csw.layouts.register('editNode', function (cswHelpers) {

        var identityTabId = 'identityTab_' + window.Ext.id();
        var tabPanelId = 'tabs_' + window.Ext.id();

        var cswPrivate = {
            renderedTabs: {}
        };
        Csw.extend(cswPrivate, cswHelpers);

        cswPrivate.tabStyle = {
            background: '#F2F5F7'
        };
        cswPrivate.identityTabStyle = {
            background: '#E5F0FF'
        };

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
                                        cswPrivate.sidebar.refreshExistingProperties('Edit', tab.id);
                                        if (!cswPrivate.renderedTabs[tab.id]) {
                                            cswPrivate.renderedTabs[tab.id] = tab;
                                            cswPrivate.renderTab(tab.id, tab.id, cswPrivate.tabStyle);
                                        }
                                    }
                                },//listeners
                                closable: true,
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
                                
                            }
                        }, //listeners
                        reorderable: false
                    });

                    window.Ext.create('Ext.panel.Panel', {
                        renderTo: contentDiv.getId(),
                        bodyStyle: cswPrivate.identityTabStyle,
                        layout: {
                            type: 'vbox',
                            align: 'stretch'    // Each takes up full width
                        },
                        items: [{
                            id: identityTabId,
                            xtype: 'panel',
                            border: 0,
                            bodyStyle: cswPrivate.identityTabStyle
                        }, {
                            id: tabPanelId,
                            border: 0,
                            //bodyStyle: cswPrivate.identityTabStyle,
                            padding: '0 10 10 10',
                            xtype: 'tabpanel',
                            plugins: Ext.create('Ext.ux.TabReorderer', {
                                listeners: {
                                    Drop: function (e, tabStrip, tabObj, oldPosition, newPosition, f) {
                                        if (oldPosition != newPosition) {
                                            Csw.ajaxWcf.post({
                                                urlMethod: 'Design/updateTabOrder',
                                                data: {
                                                    TabId: tabObj.card.id, //for whatever reason, this is where the id we set is being stored
                                                    OldPosition: oldPosition,
                                                    NewPosition: newPosition + 1
                                                },
                                                success: function (response) {
                                                }
                                            });
                                        }
                                    }
                                }
                            }),
                            items: tabs
                        }]
                    });

                    cswPrivate.renderTab(identityTabId, cswPrivate.identityTabId, cswPrivate.identityTabStyle);

                }
            });
        };

        return cswPublic;
    });
})();