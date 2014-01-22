(function () {

    Csw.layouts.register('editNode', function (cswHelpers) {

        var identityTabId = 'identityTab_' + window.Ext.id();
        var tabPanelId = 'tabs_' + window.Ext.id();

        var cswPrivate = {
            renderedTabs: {}
        };
        Csw.extend(cswPrivate, cswHelpers);

        var cswPublic = {};

        cswPublic.activeTabId = 0;

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

                    var beforeCloseTab = function(tab) {
                        var confirmDialog = Csw.dialogs.confirmDialog({
                            title: 'Delete Tab',
                            message: 'Are you sure you want to delete this tab?',
                            width: 300,
                            height: 160,
                            onYes: function() {
                                Csw.ajaxWcf.post({
                                    urlMethod: 'Design/deleteTab',
                                    data: tab.id,
                                    success: function(data) {
                                        tab.ownerCt.removeListener('beforeclose', beforeCloseTab); //self referential, ooo. Necessary to not open this dialog again when we remove the tab
                                        tab.ownerCt.remove(tab);
                                        confirmDialog.close();
                                    },
                                }); //confirm dialog
                            },//onYes
                            onNo: function() {
                                confirmDialog.close();
                            }
                        });
                        return false;
                    }//beforeClose

                    var clickTab = function(tab) {
                        cswPublic.activeTabId = tab.id;
                        cswPrivate.sidebar.refreshExistingProperties('Edit', tab.id);
                        if (!cswPrivate.renderedTabs[tab.id]) {
                            cswPrivate.renderedTabs[tab.id] = tab;
                            cswPrivate.renderTab(tab.id, tab.id);
                        }
                    };
                    
                    var tabs = [];
                    for (var tabIdx in data.tabs) {
                        var tabData = data.tabs[tabIdx];
                        if (tabData.name !== 'Identity') {
                            tabs.push({
                                id: tabData.id,
                                title: tabData.name,
                                listeners: {
                                    activate: clickTab,
                                    beforeclose: beforeCloseTab,

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
                            activate: function (tab) {
                                var inputDialog = Csw.dialogs.inputDialog({
                                    title: 'Create New Tab',
                                    message: '',
                                    fields: {
                                        'Name': Csw.enums.inputTypes.text,
                                    },
                                    onOk: function (fields) {
                                        Csw.ajaxWcf.post({
                                            urlMethod: 'Design/createNewTab',
                                            data: {
                                                NodetypeId: cswPrivate.nodeTypeId,
                                                Name: fields['Name'].val(),
                                                Order: tab.ownerCt.items.length,
                                            },
                                            success: function (data) {
                                                tab.ownerCt.add({
                                                    title: fields['Name'].val(),
                                                    id: data.TabId,
                                                    listeners: {
                                                        activate: clickTab,
                                                        beforeclose: beforeCloseTab,
                                                    },//listeners
                                                    closable: true,
                                                });
                                                inputDialog.close();
                                            }
                                        });
                                    },
                                });
                            }
                        }, //listeners
                        reorderable: false
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

                    cswPrivate.renderTab(identityTabId, cswPrivate.identityTabId);

                }
            });
        };

        return cswPublic;
    });
})();