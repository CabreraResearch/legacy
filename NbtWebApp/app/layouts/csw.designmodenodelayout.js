(function () {


    Csw.layouts.register('designmodenodelayout', function (cswParent, options) {

        var identityTabId = 'identityTab_' + window.Ext.id();
        var tabPanelId = 'tabs_' + window.Ext.id();

        var cswPrivate = {
            nodeId: '',
            nodeKey: '',
            nodeTypeId: '',
            identityTabId: '',
            tabid: '',
            Layout: 'Edit',
            onClose: function () { },
            sidebar: {}
        };
        if (options) {
            Csw.extend(cswPrivate, options);
        }

        var cswPublic = {};
        var layout = null;

        (function _pre() {

            cswParent = cswParent || Csw.main.rightDiv;

        })();

        cswPrivate.setActiveTabId = function (tabid) {
            cswPrivate.tabid = tabid;
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

        cswPrivate.renderTab = function (extid, tabid, style) {
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

                    cswPrivate.renderProps(data.node, data.properties, extid, tabid, style);

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
            var labelDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px', 'width': '180px', 'text-align': 'right' });
            var propDiv = propTbl.cell(1, 2).div().css({ 'padding': '5px 10px' });

            labelDiv.setLabelText(prop.name, prop.required, false); //in design mode, readonly better always be true, but we want required props to have the "*"

            var fieldOpt = cswPrivate.makePropOpt(tabid, node, prop, propDiv);
            Csw.nbt.property(fieldOpt, {});
        };

        cswPrivate.renderProps = function (node, properties, extid, tabid, style) {
            var cols = cswPrivate.howManyCols(properties);
            var propsDiv = cswPrivate.makeDiv(extid);

            var dragPanel = Csw.composites.draggablepanel(propsDiv, {
                columns: cols,
                border: 0,
                bodyStyle: style
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
                        showRearrangeButton: (prop.hassubprops || false === Csw.isNullOrEmpty(prop.tabgroup)),
                        showConfigureButton: Csw.isNullOrEmpty(prop.tabgroup),
                        render: function (extEl, cswEl) {

                            var propTbl = cswEl.table();
                            var propDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });
                            var subPropsDiv = propTbl.cell(2, 2).div().css({
                                'border': '1px solid #ccc'
                            });

                            extEl.data = [];
                            if (Csw.isNullOrEmpty(prop.tabgroup)) {
                                cswPrivate.renderPropDiv(tabid, node, prop, propDiv);
                                extEl.data.push(prop);
                            } else {
                                var fieldSet = propDiv.fieldSet();
                                fieldSet.legend({ value: prop.tabgroup });
                                propDiv = fieldSet.div();
                                var groupProps = cswPrivate.getPropsInGroup(prop.tabgroup, properties);

                                for (var groupIdx in groupProps) {
                                    var groupProp = groupProps[groupIdx];
                                    seenProps[groupProp.id] = groupProp;
                                    cswPrivate.renderPropDiv(tabid, node, groupProp, propDiv);
                                    extEl.data.push(groupProp);
                                }
                            }

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
                                cswPrivate.arrangeDialog(node, prop.subprops, tabid, 'Configure ' + prop.name + ' Subprops', style);
                            } else if (false === Csw.isNullOrEmpty(prop.tabgroup)) { //render all the grouped properties in a re-arrangable dialog
                                var groupProps = cswPrivate.getPropsInGroup(prop.tabgroup, properties);
                                cswPrivate.arrangeDialog(node, groupProps, tabid, 'Configure ' + prop.tabgroup + ' Props', style);
                            }
                        },
                        onConfigure: cswPrivate.onConfigure,
                        onDrop: function (ext, col, row) {
                            window.Ext.getCmp(extid).doLayout();
                            cswPrivate.saveLayout(dragPanel, node, seenProps, tabid);
                        },
                        onClose: function (draggable) {
                            cswPrivate.onRemove(dragPanel, draggable, realCol, tabid, node, seenProps);


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

        cswPrivate.onRemove = function (dragPanel, draggable, col, tabid, node, props) {
            var canRemove = true;
            var doomedPropsCollection = [];
            Csw.iterate(draggable.data, function (doomedProp) {
                doomedPropsCollection.push({
                    nodetypepropid: doomedProp.id.substr(doomedProp.id.lastIndexOf('_') + 1),
                    displaycol: Csw.int32MinVal,
                    displayrow: Csw.int32MinVal
                });
                if (doomedProp.required && 'Add' === cswPrivate.Layout) {
                    canRemove = false;
                }
            });

            if (canRemove) {
                var confirm = Csw.dialogs.confirmDialog({
                    title: 'Remove Property From Layout',
                    message: 'Are you sure you want to remove this property from the layout?',
                    height: 200,
                    width: 300,
                    onYes: function () {
                        dragPanel.removeDraggableFromCol(col, draggable.id);
                        cswPrivate.removePropsFromLayout(node, doomedPropsCollection, tabid, function () {
                            cswPrivate.saveLayout(dragPanel, node, props, tabid);
                        });
                        cswPrivate.sidebar.refreshExistingProperties(cswPrivate.Layout, layout.activeTabId);
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
        };

        cswPrivate.onConfigure = function (draggable, onSave) {
            var propToConfigure = draggable.data[0];
            Csw.dialogs.editnode({
                currentNodeId: propToConfigure.propnodeid,
                title: 'Edit Property: ' + propToConfigure.name,
                onEditNode: function () {
                    Csw.tryExec(onSave);
                    cswPublic.init();
                }
            });
        };

        cswPrivate.getPropsInGroup = function (group, properties) {
            var groupProps = {};
            for (var propIdx in properties) {
                if (group === properties[propIdx].tabgroup) {
                    groupProps[propIdx] = properties[propIdx];
                }
            }
            return groupProps;
        };

        cswPrivate.arrangeDialog = function (node, props, tabid, title, style) {
            var seenProps = {};

            var rearrangeGroupPropDialog = Csw.layouts.dialog({
                title: title,
                width: 800,
                height: 400,
                onOpen: function () {

                    var div = rearrangeGroupPropDialog.div;
                    if (false === Csw.isNullOrEmpty(props[Object.keys(props)[0]].tabgroup)) {
                        var fieldSet = div.fieldSet();
                        fieldSet.legend({ value: props[Object.keys(props)[0]].tabgroup });
                        div = fieldSet.div();
                    }

                    var groupDragPanel = Csw.composites.draggablepanel(div, {
                        columns: 1, //We force all grouped props to be in a single column
                        bodyStyle: style
                    });

                    groupDragPanel.allowDrag(false); //TODO: enable drag for sub/tabgroup props

                    for (var groupIdx in props) {
                        var groupProp = props[groupIdx];
                        seenProps['group_' + groupProp.id] = groupProp;

                        groupDragPanel.addItemToCol(0, {
                            id: 'group_' + groupProp.id,
                            showRearrangeButton: false,
                            showConfigureButton: true,
                            showCloseButton: true,
                            render: function (subExtEl, subCswEl) {
                                var propTbl = subCswEl.table();
                                var propDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px' });

                                subExtEl.data = [groupProp];
                                cswPrivate.renderPropDiv(tabid, node, groupProp, propDiv);
                            },
                            onConfigure: function (draggable) {
                                cswPrivate.onConfigure(draggable, rearrangeGroupPropDialog.close);
                            },
                            onDrop: function () {
                                cswPrivate.saveLayout(groupDragPanel, node, seenProps, tabid);
                            },
                            onClose: function (draggable) {
                                cswPrivate.onRemove(groupDragPanel, draggable, 0, tabid, node, props);
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
                    Csw.iterate(panel.data, function (panelProp) {
                        propsReq.push({
                            nodetypepropid: panelProp.id.substr(panelProp.id.lastIndexOf('_') + 1), //panelProp.id is 'nodes_<nodeid>_<propid>' - we just want the propid
                            tabgroup: panelProp.tabgroup,
                            displaycol: thisCol,
                            displayrow: thisRow
                        });
                    });
                    thisRow++;
                });
            }
            Csw.ajaxWcf.post({
                urlMethod: 'Design/updateLayout',
                data: {
                    layout: cswPrivate.Layout,
                    nodetypeid: node.nodetypeid,
                    tabid: (false === Csw.isNullOrEmpty(tabid) ? tabid : Csw.int32MinVal),
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
                    tabid: (false === Csw.isNullOrEmpty(tabid) ? tabid : Csw.int32MinVal),
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
        
        cswPublic.getIdentityTabId = function () {
            return layout.identityTabId;
        };

        cswPublic.getActiveTabId = function () {
            return layout.activeTabId;
        };

        cswPublic.getActiveLayout = function () {
            return cswPrivate.Layout;
        };

        cswPublic.refresh = function () {
            cswPublic.init();
        };

        cswPublic.init = function () {
            cswParent.empty();

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
                    cswPublic.init();
                    cswPrivate.sidebar.refreshExistingProperties(cswPrivate.Layout, layout.activeTabId);
                    cswPrivate.sidebar.toggleIdentityTabOption(cswPrivate.Layout === 'Edit');
                }
            });
            cswPrivate.contentDiv = cswParent.div();

            if (cswPrivate.Layout === 'Edit') {
                layout = Csw.layouts.editNode(cswPrivate);
            } else if (cswPrivate.Layout === 'Add') {
                layout = Csw.layouts.addNode(cswPrivate);
            } else if (cswPrivate.Layout === 'Search') {
                cswPrivate.Layout = 'Table'; //"Search" layout is really just a table layout
                layout = Csw.layouts.searchNode(cswPrivate);
            } else if (cswPrivate.Layout === 'Preview') {
                layout = Csw.layouts.previewNode(cswPrivate);
            }

            if (null !== layout) {
                layout.render(cswPrivate.contentDiv);
            } else {
                Csw.error.showError({
                    name: 'Layout error',
                    type: 'js',
                    message: 'Error rendering ' + cswPrivate.Layout + ' layout',
                    detail: "'" + cswPrivate.Layout + "' is not a valid layout"
                });
            }

        };

        //#endregion Public

        (function _post() {

        })();

        return cswPublic;
    });
}());