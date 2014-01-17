(function () {


    Csw.layouts.register('designmodenodelayout', function (cswParent, options) {

        var identityTabId = 'identityTab_' + window.Ext.id();
        var tabPanelId = 'tabs_' + window.Ext.id();

        var cswPrivate = {
            nodeId: '',
            nodeKey: '',
            nodeTypeId: '',
            identityTabId: '',
            Layout: 'Edit',
            sidebar: {},
            activeTabId: ''
        };
        if (options) {
            Csw.extend(cswPrivate, options);
        }

        var cswPublic = {};
        var renderedTabs = {};

        (function _pre() {

            cswParent = cswParent || Csw.main.rightDiv;

            var init = function () {
                Csw.main.clear({ right: true });
                var layoutSelectDiv = cswParent.div().css('float', 'right');
                layoutSelectDiv.setLabelText('Select Layout:', false, false);
                layoutSelectDiv.select({
                    values: ['Edit', 'Add', 'Search', 'Preview'],
                    selected: cswPrivate.Layout,
                    onChange: function (val) {
                        cswPrivate.Layout = val;
                        init();
                        if (val === 'Edit') {
                            cswPrivate.makeEditNodeLayout();
                        } else if (val === 'Add') {
                            cswPrivate.makeAddNodeLayout();
                        } else if (val === 'Search') {
                            cswPrivate.Layout = 'Table'; //TODO: rename Table layout to Search
                            cswPrivate.makeSearchNodeLayout();
                        } else if (val === 'Preview') {
                            cswPrivate.makePreviewNodeLayout();
                        }
                    }
                });
                cswPrivate.nameDiv = cswParent.div({ cssclass: 'CswIdentityTabHeader' });
                cswPrivate.contentDiv = cswParent.div();
            };
            init();

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
                    cswPrivate.nameDiv.append(data.node.nodename);

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
            var cols = 0; //We always need a col
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

        cswPrivate.makePropOpt = function (tabid, node, prop, propDiv) {
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

        cswPrivate.renderPropDiv = function (tabid, node, prop, div) {
            var propTbl = div.table();
            var labelDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });
            var propDiv = propTbl.cell(1, 2).div().css({ 'padding': '5px 10px' });

            labelDiv.setLabelText(prop.name, prop.required, false); //in design mode, readonly better always be true, but we want required props to have the "*"

            var fieldOpt = cswPrivate.makePropOpt(tabid, node, prop, propDiv);
            Csw.nbt.property(fieldOpt, {});
        };

        cswPrivate.renderProps = function (node, properties, extid, tabid) {
            var cols = cswPrivate.howManyCols(properties);
            var propsDiv = cswPrivate.makeDiv(extid);

            var dragPanel = Csw.composites.draggablepanel(propsDiv, {
                columns: cols,
                border: 0
            });
            dragPanel.allowDrag(false);

            var seenProps = {};
            Csw.iterate(properties, function (prop) {
                //for (var propIdx in properties) {
                //var prop = properties[propIdx];
                if (!seenProps[prop.id]) {
                    seenProps[prop.id] = prop;
                    var realCol = prop.displaycol - 1; //server starts cols at 1, dragpanel starts at 0
                    var subDragPanel;
                    dragPanel.addItemToCol(realCol, {
                        id: prop.id,
                        render: function (extEl, cswEl) {

                            var propTbl = cswEl.table();
                            var propDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });
                            var subPropsDiv = propTbl.cell(2, 2).div().css({
                                'border': '1px solid #ccc'
                            });

                            cswPrivate.renderPropDiv(tabid, node, prop, propDiv);

                            if (prop.hassubprops) {
                                var subCols = cswPrivate.howManyCols(prop.subprops);
                                subDragPanel = Csw.composites.draggablepanel(subPropsDiv, {
                                    columns: subCols
                                });

                                for (var subPropIdx in prop.subprops) {
                                    var subProp = prop.subprops[subPropIdx];
                                    seenProps[subProp.id] = subProp;

                                    var subRealCol = subProp.displaycol - 1;
                                    subDragPanel.addItemToCol(subRealCol, {
                                        render: function (extRenderTo, cswRenderTo) {
                                            seenProps[subProp.id] = subProp;
                                            cswPrivate.renderPropDiv(tabid, node, subProp, cswRenderTo);
                                        },
                                        onDrop: function (el, col, row) {
                                            subDragPanel.doLayout();
                                            dragPanel.doLayout();
                                            //TODO: update subprops position in layout
                                        }
                                    });
                                } //for subProp in subProps
                                subDragPanel.doLayout();
                            }
                        },
                        onDrop: function (ext, col, row) {
                            cswPrivate.saveLayout(dragPanel, node, seenProps, tabid);
                        },
                        onClose: function (draggable) {
                            var doomedProp = seenProps[draggable.id];
                            if (false === doomedProp.required && 'Add' === cswPrivate.Layout) {
                                var confirm = Csw.dialogs.confirmDialog({
                                    title: 'Remove Property From Layout',
                                    message: 'Are you sure you want to remove this property from the layout?',
                                    height: 200,
                                    width: 300,
                                    onYes: function () {
                                        var doomedPropsCollection = [{
                                            nodetypepropid: doomedProp.id.substr(doomedProp.id.lastIndexOf('_') + 1),
                                            displaycol: Csw.int32MinVal,
                                            displayrow: Csw.int32MinVal
                                        }];
                                        dragPanel.removeDraggableFromCol(realCol, draggable.id);
                                        cswPrivate.removePropsFromLayout(node, doomedPropsCollection, tabid, function () {
                                            cswPrivate.saveLayout(dragPanel, node, seenProps, tabid);
                                        });
                                        confirm.close();
                                    },
                                    onNo: function () {
                                        confirm.close();
                                    }
                                });
                            } else {
                                var alert = Csw.dialogs.alert({
                                    title: 'Error removing property',
                                    message: 'Cannot remove required properties from the Add layout'
                                });
                                alert.open();
                            }
                        }
                    });
                } //if (!seenProps[prop.id)
            }); //for prop in properties


            dragPanel.allowDrag(true);
            //trigger the prop render events
            Csw.publish('render_' + node.nodeid + '_' + tabid);

            //TODO: fix this hack - we need to wait for all property ajax requests (like grid) to finish before calling doLayout()
            Csw.defer(function () {
                dragPanel.doLayout(); //fix our layout
            }, 2000);
        };

        cswPrivate.saveLayout = function (dragPanel, node, props, tabid) {
            var propsReq = [];
            var numCols = dragPanel.getNumCols();
            for (var i = 0; i < numCols; i++) {
                var propPanels = dragPanel.getItemsInCol(i);
                var thisCol = i + 1;
                var thisRow = 1;
                Csw.iterate(propPanels, function (panel) {
                    var panelProp = props[panel.id];
                    propsReq.push({
                        nodetypepropid: panelProp.id.substr(panelProp.id.lastIndexOf('_') + 1), //this id is 'nodes_<nodeid>_<propid>'
                        displaycol: thisCol,
                        displayrow: thisRow
                    });
                    thisRow++;
                });
            }
            Csw.ajaxWcf.post({
                urlMethod: 'Design/updateLayout',
                data: {
                    layout: cswPrivate.Layout,
                    nodetypeid: node.nodetypeid,
                    tabid: tabid,
                    props: propsReq
                },
                success: function (response) {
                    //nothing to do here
                }
            });
        };

        cswPrivate.removePropsFromLayout = function (node, props, tabid, onSuccess) {
            Csw.ajaxWcf.post({
                urlMethod: 'Design/removePropsFromLayout',
                data: {
                    layout: cswPrivate.Layout,
                    nodetypeid: node.nodetypeid,
                    tabid: tabid,
                    props: props
                },
                success: function (response) {
                    Csw.tryExec(onSuccess);
                }
            });
        };
        
        //#region Public

        cswPublic.tearDown = function () {
            cswParent.empty();
        };

        cswPublic.setSidebar = function (sidebar) {
            cswPrivate.sidebar = sidebar;
        };
        
        cswPublic.getActiveTabId = function () {
            return cswPrivate.activeTabId;
        };

        //#endregion Public

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

        return cswPublic;
    });
}());