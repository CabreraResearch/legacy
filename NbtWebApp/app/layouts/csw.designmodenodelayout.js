(function () {


    Csw.layouts.register('designmodenodelayout', function (cswParent, options) {

        var identityTabId = 'identityTab_' + window.Ext.id();
        var tabPanelId = 'tabs_' + window.Ext.id();

        var cswPrivate = {
            nodeId: '',
            nodeKey: '',
            nodeTypeId: '',
            nodeName: 'Sample Name',
            tabs: {},

            identityTabId: ''
        };
        if (options) {
            Csw.extend(cswPrivate, options);
        }

        var renderedTabs = {};

        (function _pre() {
            Csw.main.clear({ right: true });

            cswPrivate.getTabsAjax = Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getTabs',
                data: {
                    EditMode: 'Edit',
                    NodeId: cswPrivate.nodeId,
                    SafeNodeKey: cswPrivate.nodeKey,
                    Date: new Date().toDateString(),
                    filterToPropId: '',
                    Multi: false,
                    ConfigMode: true
                },
                success: function (data) {
                    var tabs = [];
                    for (var tabIdx in data.tabs) {
                        var tabData = data.tabs[tabIdx];
                        if (tabData.name !== 'Identity') {
                            tabs.push({
                                id: tabData.id,
                                title: tabData.name,
                                listeners: {
                                    activate: function (tab) {
                                        if (!renderedTabs[tab.id]) {
                                            renderedTabs[tab.id] = tab;
                                            cswPrivate.renderTab(tab.id, tab.id);
                                            return true;
                                        } else {
                                            return false;
                                        }
                                    }
                                }
                            });
                        } else {
                            cswPrivate.identityTabId = tabData.id;
                        }
                    }

                    window.Ext.create('Ext.panel.Panel', {
                        renderTo: Csw.main.rightDiv.getId(),
                        layout: {
                            align: 'stretch',    // Each takes up full width
                            padding: 1
                        },
                        height: 700,
                        items: [{
                            id: identityTabId,
                            xtype: 'panel',
                            height: 200,
                            border: 1
                        }, {
                            id: tabPanelId,
                            xtype: 'tabpanel',
                            height: 500,
                            border: 1,
                            items: tabs
                        }]
                    });

                }
            });

        })();

        cswPrivate.makeDiv = function (extId) {
            var tabPanel = window.Ext.getCmp(extId);
            var cswEl = Csw.domNode({
                ID: tabPanel.body.id,
                el: tabPanel.body.dom
            });

            return cswEl.div();
        };

        cswPrivate.howManyCols = function (properties) {
            var cols = 0;
            Csw.iterate(properties, function (prop) {
                if (prop.displaycol > cols) {
                    cols = prop.displaycol;
                }
            });
            return cols;
        };

        cswPrivate.renderTab = function (extid, tabid) {
            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getProps',
                data: {
                    EditMode: 'Edit',
                    NodeId: cswPrivate.nodeId,
                    TabId: tabid,
                    SafeNodeKey: cswPrivate.nodeKey,
                    NodeTypeId: cswPrivate.nodeTypeId,
                    Date: new Date().toDateString(),
                    Multi: false,
                    filterToPropId: '',
                    ConfigMode: true,
                    RelatedNodeId: '',
                    GetIdentityTab: true,
                    ForceReadOnly: true
                },
                success: function (data) {

                    cswPrivate.renderProps(data.node, data.properties, extid, tabid);

                } // success{}
            }); // ajax
        };

        cswPrivate.renderProps = function (node, properties, extid, tabid) {
            var cols = cswPrivate.howManyCols(properties);
            var identityTabDiv = cswPrivate.makeDiv(extid);

            var dragPanel = Csw.composites.draggablepanel(identityTabDiv, {
                columns: cols,
                border: 0
            });

            for (var propIdx in properties) {
                var prop = properties[propIdx];
                var realCol = prop.displaycol - 1; //server starts cols at 1, dragpanel starts at 0
                dragPanel.addItemToCol(realCol, {
                    render: function (extEl, cswEl) {

                        var propTbl = cswEl.table();
                        var labelDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });
                        var propDiv = propTbl.cell(1, 2).div().css({ 'padding': '5px 10px' });

                        labelDiv.setLabelText(prop.name, prop.required, true); //in design mode, readonly better always be true OR ELSE!!

                        var fieldOpt = Csw.nbt.propertyOption({
                            isMulti: false,
                            fieldtype: prop.fieldtype,
                            tabState: {
                                nodeid: node.nodeid,
                                nodename: node.nodename,
                                EditMode: Csw.enums.editMode.PrintReport, //TODO: do we want PrintReport?
                                ReadOnly: true,
                                Config: true,
                                showSaveButton: false,
                                relatednodeid: '',
                                relatednodename: '',
                                nodetypeid: node.nodetypeid
                            },
                            tabid: tabid,
                            identityTabId: cswPrivate.identityTabId,
                            propid: prop.id,
                            propData: prop,
                            Required: Csw.bool(prop.required),
                            issaveprop: prop.issaveprop
                        }, propDiv);

                        Csw.nbt.property(fieldOpt, {});

                        //TODO: subprops go here
                    }
                });
            }

            //trigger the prop render events:
            Csw.publish('render_' + node.nodeid + '_' + tabid);

            //TODO: fix this hack - we need to wait for all property ajax requests to finish before calling doLayout()
            Csw.defer(function () {
                dragPanel.doLayout(); //fix our layout
            }, 2000);
        };

        (function _post() {

            cswPrivate.getTabsAjax.then(function () {
                cswPrivate.renderTab(identityTabId, cswPrivate.identityTabId);
            });


        })();
    });
}());