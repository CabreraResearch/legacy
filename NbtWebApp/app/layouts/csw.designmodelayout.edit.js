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

        cswPublic.activeTabId = cswPrivate.tabid;
        cswPublic.identityTabId = 0;

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

                    var beforeCloseTab = function (tab) {
                        if (tab.ownerCt.items.length <= 2) {
                            Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Cannot delete the last tab on a layout.'));
                        } else {
                            var confirmDialog = Csw.dialogs.confirmDialog({
                                title: 'Delete Tab',
                                message: 'Are you sure you want to delete this tab?',
                                width: 300,
                                height: 160,
                                onYes: function() {
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'Design/deleteTab',
                                        data: tab.id,
                                        success: function (data) {
                                            var tabstrip = tab.ownerCt;
                                            tabstrip.removeListener('beforeclose', beforeCloseTab); //self referential, ooo. Necessary to not open this dialog again when we remove the tab
                                            tabstrip.remove(tab);
                                            confirmDialog.close();
                                            //if there is only one tab left now, remove the little [X]
                                            if (tabstrip.items.length <= 2) {
                                                tabstrip.items.items.forEach( function (eachTab) {
                                                    eachTab.tab.setClosable(false);
                                                });
                                            }
                                        },
                                    }); //confirm dialog
                                },//onYes
                                onNo: function() {
                                    confirmDialog.close();
                                }
                            });
                        }
                        return false;
                    };//beforeClose

                    var clickTab = function (tab) {
                        cswPrivate.setActiveTabId(tab.id);
                        cswPublic.activeTabId = tab.id;
                        cswPrivate.sidebar.refreshExistingProperties('Edit', tab.id);
                        if (!cswPrivate.renderedTabs[tab.id]) {
                            cswPrivate.renderedTabs[tab.id] = tab;
                            cswPrivate.renderTab(tab.id, tab.id, cswPrivate.tabStyle);
                        }
                    };

                    var tabNo = 0;
                    var activeTab = 0;
                    var tabs = [];
                    for (var tabIdx in data.tabs) {
                        var tabData = data.tabs[tabIdx];
                        if (tabData.name !== 'Identity') {
                            if (tabData.id === cswPrivate.tabid) {
                                activeTab = tabNo;
                            }
                            tabNo++;
                            tabs.push({
                                id: tabData.id,
                                title: tabData.name,
                                listeners: {
                                    activate: clickTab,
                                    beforeclose: beforeCloseTab,

                                },//listeners
                                closable: (Object.keys(data.tabs).length > 2),
                                cswTabNodeId: tabData.tabnodeid
                            });


                        } else {
                            cswPublic.identityTabId = tabData.id;
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
                                                var newTab = tab.ownerCt.add({
                                                    title: fields['Name'].val(),
                                                    id: data.TabId,
                                                    listeners: {
                                                        activate: clickTab,
                                                        beforeclose: beforeCloseTab,
                                                    },//listeners
                                                    closable: true,
                                                });
                                                tab.ownerCt.setActiveTab(newTab);
                                                inputDialog.close();
                                                //if there was previously only 1 tab, re-insert the little [X]
                                                if (tab.ownerCt.items.length == 3) {
                                                    tab.ownerCt.items.items.forEach( function(eachTab) {
                                                        if (eachTab !== tab) {//ignore the 'New Tab (+)' tab
                                                            eachTab.tab.setClosable(true);
                                                        }
                                                    });
                                                }
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
                            activeTab: activeTab,
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
                            tabBar: {
                                items: [
                                    { xtype: 'tbfill' }, //this forces the button below to be on the far right
                                    {
                                        xtype: 'button',
                                        text: 'Configure Selected Tab',
                                        onClick: function () {
                                            var selectedTab = window.Ext.getCmp(cswPublic.activeTabId);
                                            Csw.dialogs.editnode({
                                                currentNodeId: selectedTab.cswTabNodeId,
                                                title: 'Edit Tab: ' + selectedTab.title,
                                                onEditNode: function () {
                                                    cswPrivate.init();
                                                }
                                            });
                                        }
                                    }]
                            },
                            items: tabs
                        }]
                    });

                    cswPrivate.renderTab(identityTabId, cswPublic.identityTabId, cswPrivate.identityTabStyle);

                }
            });
        };

        return cswPublic;
    });
})();