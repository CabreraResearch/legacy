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
            onClose: function () { },
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

        })();

        cswPrivate.init = function () {
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
                    cswPrivate.init();
                    cswPrivate.sidebar.refreshExistingProperties(cswPrivate.Layout, cswPrivate.activeTabId);
                }
            });
            cswPrivate.contentDiv = cswParent.div();

            var layout = null;
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
                //BZZZZZZZZT - throw error
            }

        };

        cswPrivate.makeSearchNodeLayout = function () {
            var searchTbl = cswPrivate.contentDiv.table();

            var imageCell = searchTbl.cell(2, 1).div().css('width', '200px');
            var labelCell = searchTbl.cell(1, 2).div().css('height', '20px' );
            var propsCell = searchTbl.cell(2, 2).div().css('width', '400px');
            var buttonsCell = searchTbl.cell(2, 3).div().css('width', '400px');
            var disabledButtonsCell = searchTbl.cell(2, 4).div().css('width', '400px');

            Csw.ajaxWcf.post({
                urlMethod: 'Design/GetSearchImageLink',
                data: cswPrivate.nodeId,
                success: function(ret) {
                    imageCell.img({
                        src: ret.imagelink
                    });
                }
            });

            var propsPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: propsCell.getId(),
                border: 0,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });

            var buttonsPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: buttonsCell.getId(),
                border: 0,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });

            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getProps',
                data: {
                    EditMode: cswPrivate.Layout,
                    NodeId: cswPrivate.nodeId,
                    TabId: Csw.int32MinVal,
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
                    //split buttons and every other prop
                    var buttonProps = [];
                    var otherProps = [];
                    Csw.iterate(data.properties, function (prop) {
                        if ('Button' === prop.fieldtype) {
                            buttonProps.push(prop);
                        } else {
                            otherProps.push(prop);
                        }
                    });
                    labelCell.setLabelText(data.node.nodename, false, false);

                    cswPrivate.renderProps(data.node, otherProps, propsPanel.id, '', false);
                    cswPrivate.renderProps(data.node, buttonProps, buttonsPanel.id, '', false);

                } // success
            }); // ajax

            var disabledBtnsTbl = disabledButtonsCell.table({
                cellspacing: 5,
                cellpadding: 5
            });
            disabledBtnsTbl.cell(1, 1).buttonExt({
                isEnabled: false,
                enabledText: 'Details',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil)
            }).disable();

            disabledBtnsTbl.cell(1, 2).buttonExt({
                isEnabled: false,
                enabledText: 'Delete',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash)
            }).disable();
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

                    cswPrivate.renderProps(data.node, data.properties, extid, tabid, true);

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
            var labelDiv = propTbl.cell(1, 1).div().css({ 'padding': '5px 10px', 'width': '150px', 'text-align': 'right' });
            var propDiv = propTbl.cell(1, 2).div().css({ 'padding': '5px 10px' });

            labelDiv.setLabelText(prop.name, prop.required, false); //in design mode, readonly better always be true, but we want required props to have the "*"

            var fieldOpt = cswPrivate.makePropOpt(tabid, node, prop, propDiv);
            Csw.nbt.property(fieldOpt, {});
        };

        cswPrivate.renderProps = function (node, properties, extid, tabid, showAddColBtn) {
            var cols = cswPrivate.howManyCols(properties);
            var propsDiv = cswPrivate.makeDiv(extid);

            var dragPanel = Csw.composites.draggablepanel(propsDiv, {
                columns: cols,
                showAddColumnButton: showAddColBtn,
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
                        showRearrangeButton: (prop.hassubprops || false === Csw.isNullOrEmpty(prop.tabgroup)),
                        showConfigureButton: false,//TODO: when we want to edit NTPs in the layout editor, show this button
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
                                cswPrivate.arrangeDialog(node, prop.subprops, tabid, 'Configure ' + prop.name + ' Subprops');
                            } else if (false === Csw.isNullOrEmpty(prop.tabgroup)) { //render all the grouped properties in a re-arrangable dialog
                                var groupProps = cswPrivate.getPropsInGroup(prop.tabgroup, properties);
                                cswPrivate.arrangeDialog(node, groupProps, tabid, 'Configure ' + prop.tabgroup + ' Props');
                            }
                        },
                        onDrop: function (ext, col, row) {
                            window.Ext.getCmp(extid).doLayout();
                            cswPrivate.saveLayout(dragPanel, node, seenProps, tabid);
                        },
                        onClose: function (draggable) {
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

        cswPrivate.getPropsInGroup = function (group, properties) {
            var groupProps = {};
            for (var propIdx in properties) {
                if (group === properties[propIdx].tabgroup) {
                    groupProps[propIdx] = properties[propIdx];
                }
            }
            return groupProps;
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
                            showConfigureButton: false, //TODO: when we want to edit NTPs in the layout editor, show this button
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
                    Csw.iterate(panel.data, function (panelProp) {
                        propsReq.push({
                            nodetypepropid: panelProp.id.substr(panelProp.id.lastIndexOf('_') + 1), //this id is 'nodes_<nodeid>_<propid>'
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

        cswPublic.getActiveLayout = function () {
            return cswPrivate.Layout;
        };

        cswPublic.refresh = function () {
            cswPrivate.init();
        };

        //#endregion Public

        (function _post() {
            


            cswPrivate.init();
        })();

        return cswPublic;
    });
}());