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
            onClose: function () { }
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
                var closeBtnDiv = cswParent.div().css('float', 'right');
                closeBtnDiv.buttonExt({
                    enabledText: 'Close Design Mode',
                    onClick: function () {
                        cswPrivate.onClose();
                    }
                });
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

        cswPrivate.makeAddNodeLayout = function () {
            cswPrivate.nameDiv.append('Add Node Layout');
            var addPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: cswPrivate.contentDiv.getId(),
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
                                for (var subPropIdx in prop.subprops) {
                                    var subProp = prop.subprops[subPropIdx];
                                    seenProps[subProp.id] = subProp;
                                    cswPrivate.renderPropDiv(tabid, node, subProp, subPropsDiv);
                                } //for subProp in subProps
                                //subDragPanel.doLayout();
                            }
                        },
                        onRearrange: function () {

                            if (prop.hassubprops) { //we render the sub properties in a re-arrangable dialog
                                cswPrivate.arrangeDialog(node, prop.subprops, tabid, 'Configure ' + prop.name + ' Subprops');

                                //var rearrangeSubpropsDialog = Csw.layouts.dialog({
                                //    title: 'Configure ' + prop.name + ' Subprops',
                                //    width: 800,
                                //    height: 400,
                                //    onOpen: function () {
                                //        var subDragPanel = Csw.composites.draggablepanel(rearrangeSubpropsDialog.div, {
                                //            columns: 1, //We force all sub props to be in a single column
                                //            showAddColumnButton: false,
                                //        });
                                //
                                //        var keys = Object.keys(prop.subprops).sort(function (a, b) { //sort subprops by row
                                //            var prop1 = prop.subprops[a];
                                //            var prop2 = prop.subprops[b];
                                //            if (prop1.displayrow > prop2.displayrow) return -1;
                                //            if (prop1.displayrow < prop2.displayrow) return 1;
                                //            return 0;
                                //        });
                                //        for (var i = 0; i < keys.length; i++) {
                                //            var subProp = prop.subprops[keys[i]];
                                //            seenProps['sub_' + subProp.id] = subProp;
                                //
                                //            subDragPanel.addItemToCol(0, {
                                //                id: 'sub_' + subProp.id,
                                //                showRearrangeButton: false,
                                //                showConfigureButton: true,
                                //                showCloseButton: false,
                                //                render: function (subExtEl, subCswEl) {
                                //                    var propTbl = subCswEl.table();
                                //                    var propDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });
                                //
                                //                    cswPrivate.renderPropDiv(tabid, node, subProp, propDiv);
                                //                },
                                //                onDrop: function () {
                                //                    cswPrivate.saveLayout(subDragPanel, node, seenProps, tabid);
                                //                }
                                //            });
                                //        } //for subProp in subProps
                                //        Csw.publish('render_' + node.nodeid + '_' + tabid);
                                //    }
                                //});
                                //rearrangeSubpropsDialog.open();
                            } else if (false === Csw.isNullOrEmpty(prop.tabgroup)) { //render all the grouped properties in a re-arrangable dialog
                                var groupProps = {};
                                for (var propIdx in properties) {
                                    if (prop.tabgroup === properties[propIdx].tabgroup) {
                                        groupProps[propIdx] = properties[propIdx];
                                    }
                                }

                                cswPrivate.arrangeDialog(node, groupProps, tabid, 'Configure ' + prop.tabgroup + ' Props');
                            }

                        },
                        onDrop: function (ext, col, row) {
                            window.Ext.getCmp(extid).doLayout();
                            cswPrivate.saveLayout(dragPanel, node, seenProps, tabid);
                        },
                        onClose: function (draggable) {
                            var doomedProp = seenProps[draggable.id];
                            if ((false === doomedProp.required && 'Add' === cswPrivate.Layout) || 'Add' !== cswPrivate.Layout) {
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
                window.Ext.getCmp(extid).doLayout();
            }, 2000);
        };

        cswPrivate.arrangeDialog = function (node, props, tabid, title) {
            var seenProps = {};

            var rearrangeGroupPropDialog = Csw.layouts.dialog({
                title: title,
                width: 800,
                height: 400,
                onOpen: function () {
                    var groupDragPanel = Csw.composites.draggablepanel(rearrangeGroupPropDialog.div, {
                        columns: 1, //We force all grouped props to be in a single column
                        showAddColumnButton: false,
                    });

                    groupDragPanel.allowDrag(false); //TODO: enable drag for sub/tabgroup props

                    for (var groupIdx in props) {
                        var groupProp = props[groupIdx];
                        seenProps['group_' + groupProp.id] = groupProp;

                        groupDragPanel.addItemToCol(0, {
                            id: 'group_' + groupProp.id,
                            showRearrangeButton: false,
                            showConfigureButton: true,
                            showCloseButton: false,
                            render: function (subExtEl, subCswEl) {
                                var propTbl = subCswEl.table();
                                var propDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });

                                cswPrivate.renderPropDiv(tabid, node, groupProp, propDiv);
                            },
                            onDrop: function () {
                                cswPrivate.saveLayout(groupDragPanel, node, seenProps, tabid);
                            }
                        });
                    } //for subProp in subProps
                    Csw.publish('render_' + node.nodeid + '_' + tabid);
                }
            });
            rearrangeGroupPropDialog.open();
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

        cswPublic.tearDown = function () {
            cswParent.empty();
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

        return cswPublic;
    });
}());