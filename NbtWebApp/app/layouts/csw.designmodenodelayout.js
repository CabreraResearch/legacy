(function () {


    Csw.layouts.register('designmodenodelayout', function (cswParent, options) {

        var identityTabId = 'identityTab_' + window.Ext.id();
        var tabPanelId = 'tabs_' + window.Ext.id();

        var cswPrivate = {
            nodeId: '',
            nodeKey: '',
            nodeTypeId: '',
            identityTabId: '',
            Layout: 'Edit'
        };
        if (options) {
            Csw.extend(cswPrivate, options);
        }

        var renderedTabs = {};

        (function _pre() {
            Csw.main.clear({ right: true });
            cswPrivate.nameDiv = Csw.main.rightDiv.div({ cssclass: 'CswIdentityTabHeader' });

            cswPrivate.contentDiv = Csw.main.rightDiv.div();

        })();

        cswPrivate.makeEditNodeLayout = function () {
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
                    //cswPrivate.nameDiv.append(data.node.nodename);

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
                        renderTo: cswPrivate.contentDiv.getId(),
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

                    cswPrivate.renderTab(identityTabId, cswPrivate.identityTabId);

                }
            });
        };

        cswPrivate.makeAddNodeLayout = function () {
            cswPrivate.nameDiv.append('Add Node Layout');
            var addPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: cswPrivate.contentDiv.getId(),
                height: 700,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });
            cswPrivate.renderTab(addPanel.id, Csw.int32MinVal);
        };

        cswPrivate.makePreviewNodeLayout = function () {
            cswPrivate.nameDiv.append('Preview Node Layout');
            var addPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: cswPrivate.contentDiv.getId(),
                height: 700,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });
            cswPrivate.renderTab(addPanel.id, Csw.int32MinVal);
        };

        //TODO: fix search (aka Table) layouts then make this layout
        cswPrivate.makeSearchNodeLayout = function () {
            cswPrivate.nameDiv.append('Search Node Layout');
            var addPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: cswPrivate.contentDiv.getId(),
                height: 700,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });
            cswPrivate.renderTab(addPanel.id, Csw.int32MinVal);
        };

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
                    EditMode: cswPrivate.Layout,
                    NodeId: cswPrivate.nodeId,
                    TabId: tabid,
                    SafeNodeKey: cswPrivate.nodeKey,
                    NodeTypeId: cswPrivate.nodeTypeId,
                    Date: new Date().toDateString(),
                    Multi: false,
                    filterToPropId: '',
                    ConfigMode: true,
                    RelatedNodeId: '',
                    GetIdentityTab: false,
                    ForceReadOnly: false
                },
                success: function (data) {

                    cswPrivate.renderProps(data.node, data.properties, extid, tabid);

                } // success{}
            }); // ajax
        };

        cswPrivate.makePropOpt = function(tabid, node, prop, propDiv) {
            var fieldOpt = Csw.nbt.propertyOption({
                isMulti: false,
                fieldtype: prop.fieldtype,
                tabState: {
                    nodeid: node.nodeid,
                    nodename: node.nodename,
                    EditMode: Csw.enums.editMode.PrintReport,
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
            return fieldOpt;
        };

        cswPrivate.renderProps = function (node, properties, extid, tabid) {
            var cols = cswPrivate.howManyCols(properties);
            var identityTabDiv = cswPrivate.makeDiv(extid);

            var dragPanel = Csw.composites.draggablepanel(identityTabDiv, {
                columns: cols,
                border: 0
            });

            var seenProps = {};
            for (var propIdx in properties) {
                var prop = properties[propIdx];
                if (!seenProps[prop.id]) {
                    seenProps[prop.id] = prop;
                    var realCol = prop.displaycol - 1; //server starts cols at 1, dragpanel starts at 0
                    dragPanel.addItemToCol(realCol, {
                        render: function(extEl, cswEl) {

                            var propTbl = cswEl.table();
                            var labelDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });
                            var propDiv = propTbl.cell(1, 2).div().css({ 'padding': '5px 10px' });
                            var subPropsDiv = propTbl.cell(2, 2).div();

                            labelDiv.setLabelText(prop.name, prop.required, true); //in design mode, readonly better always be true OR ELSE!!

                            var fieldOpt = cswPrivate.makePropOpt(tabid, node, prop, propDiv);
                            Csw.nbt.property(fieldOpt, {});

                            if (prop.hassubprops) {
                                var subPropTbl = subPropsDiv.table().css('border', '1px solid #ccc');
                                var idx = 1;
                                for (var subPropIdx in prop.subprops) {
                                    var subProp = prop.subprops[subPropIdx];
                                    seenProps[subProp.id] = subProp;
                                    
                                    var subLabelDiv = subPropTbl.cell(idx, 1).div().css({ 'padding': '5px 10px' });
                                    var subPropDiv = subPropTbl.cell(idx, 2).div().css({ 'padding': '5px 10px' });
                                    idx++;

                                    subLabelDiv.setLabelText(subProp.name, subProp.required, true);
                                    var subFieldOpt = cswPrivate.makePropOpt(tabid, node, subProp, subPropDiv);
                                    Csw.nbt.property(subFieldOpt, {});
                                }
                            }
                        },
                        onDrop: function(col, row) {
                            //TODO: save prop in new layout
                        }
                    });
                }
            }

            //trigger the prop render events:
            Csw.publish('render_' + node.nodeid + '_' + tabid);

            //TODO: fix this hack - we need to wait for all property ajax requests to finish before calling doLayout()
            Csw.defer(function () {
                dragPanel.doLayout(); //fix our layout
            }, 2000);
        };

        (function _post() {

            if (cswPrivate.Layout === 'Edit') {
                cswPrivate.makeEditNodeLayout();
            } else if (cswPrivate.Layout === 'Add') {
                cswPrivate.makeAddNodeLayout();
            } else if (cswPrivate.Layout === 'Preview') {
                cswPrivate.makePreviewNodeLayout();
            } else if (cswPrivate.Layout === 'Search') {
                cswPrivate.Layout = 'Table';
                cswPrivate.makeSearchNodeLayout();
            }
        })();
    });
}());