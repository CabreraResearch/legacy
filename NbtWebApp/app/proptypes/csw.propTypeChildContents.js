/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.childContents = Csw.properties.childContents ||
        Csw.properties.register('childContents',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.parentTbl = cswPrivate.parent.table();

                    var nodeSelect = {};
                    nodeSelect.name = Csw.string(cswPublic.data.propData.name).trim();
                    nodeSelect.selectedNodeId = Csw.string(cswPrivate.propVals.relatednodeid).trim();
                    nodeSelect.selectedNodeLink = Csw.string(cswPrivate.propVals.relatednodelink).trim();
                    nodeSelect.selectedName = Csw.string(cswPrivate.propVals.relatednodename).trim();
                    nodeSelect.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
                    //nodeSelect.viewid = Csw.string(cswPrivate.propVals.viewid).trim();
                    nodeSelect.objectClassId = Csw.string(cswPrivate.propVals.objectclassid).trim();
                    nodeSelect.allowAdd = Csw.bool(cswPrivate.propVals.allowadd);
                    nodeSelect.options = cswPrivate.propVals.options;
                    nodeSelect.useSearch = false; // Csw.bool(cswPrivate.propVals.usesearch);
                    nodeSelect.cellCol = 1;
                    nodeSelect.selectedNodeType = null;
                    nodeSelect.addImage = null;
                    nodeSelect.onSelectNode = function (nodeObj) {
                        // Csw.tryExec(cswPublic.data.onChange, nodeObj.nodeid);
                        // cswPublic.data.onPropChange({ nodeid: nodeObj.nodeid, name: nodeObj.name, relatednodeid: nodeObj.selectedNodeId, relatednodelink: nodeObj.relatednodelink });

                        cswPrivate.loadNode(nodeObj.nodeid);

                    };
                    nodeSelect.onAfterAdd = cswPrivate.loadNode;
                    nodeSelect.relatedTo = {};
                    nodeSelect.relatedTo.relatednodeid = cswPublic.data.tabState.nodeid;
                    nodeSelect.relatedTo.relatednodetypeid = cswPublic.data.tabState.nodetypeid;
                    nodeSelect.relatedTo.relatednodename = cswPublic.data.tabState.nodename;
                    //nodeSelect.relatedTo.relatedobjectclassid = cswPublic.data.tabState.relatedobjectclassid;

                    nodeSelect.isRequired = cswPublic.data.isRequired();
                    nodeSelect.isMulti = cswPublic.data.isMulti();
                    nodeSelect.isReadOnly = false; // cswPublic.data.isReadOnly();
                    nodeSelect.isClickable = cswPublic.data.tabState.EditMode !== Csw.enums.editMode.AuditHistoryInPopup; //case 28180 - relationships not clickable from audit history popup

                    nodeSelect.doGetNodes = false;
                    nodeSelect.showSelectOnLoad = true;

                    cswPublic.control = cswPrivate.parentTbl.cell(1, 1).nodeSelect(nodeSelect);

                    cswPublic.editLink = cswPrivate.parentTbl.cell(1, 2).buttonExt({
                        name: 'childContentsEdit',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                        size: 'small',
                        enabledText: 'Edit Selected',
                        onClick: function () {
                            var nodeid = cswPublic.control.selectedNodeId();
                            $.CswDialog('EditNodeDialog', {
                                currentNodeId: nodeid,
                                nodenames: [cswPublic.control.selectedName()],
                                onEditNode: function () {
                                    // refresh
                                    cswPrivate.loadNode(nodeid);
                                }
                            }); // CswDialog
                        } // onClick
                    }); // link
                    cswPrivate.childContentsDiv = cswPrivate.parent.div();

                    if (false === Csw.isNullOrEmpty(nodeSelect.selectedNodeId)) {
                        cswPrivate.loadNode(nodeSelect.selectedNodeId);
                    } else {
                        cswPublic.editLink.hide();
                    }
                }; // render()

                cswPrivate.loadNode = function (nodeid) {
                    cswPublic.editLink.show();
                    cswPrivate.childContentsDiv.empty();

                    Csw.layouts.tabsAndProps(cswPrivate.childContentsDiv, {
                        name: 'tabsAndProps',
                        globalState: {
                            //propertyData: cswDlgPrivate.propertyData,
                            ShowAsReport: false,
                            currentNodeId: nodeid
                        },
                        tabState: {
                            //                            nodetypeid: cswDlgPrivate.nodetypeid,
                            //                            relatednodeid: cswDlgPrivate.relatednodeid,
                            //                            relatednodename: cswDlgPrivate.relatednodename,
                            //                            relatednodetypeid: cswDlgPrivate.relatednodetypeid,
                            //                            relatedobjectclassid: cswDlgPrivate.relatedobjectclassid,
                            EditMode: Csw.enums.editMode.Edit,
                            ReadOnly: true,
                            showSaveButton: false
                        },
                        showTitle: false,
                        //ReloadTabOnSave: false,
                        //onSave: function (nodeid, nodekey, tabcount, nodename) {
                        //                            cswPublic.close();
                        //                            cswPublic.div.$.dialog('close');
                        //                            Csw.tryExec(cswDlgPrivate.onAddNode, nodeid, nodekey, nodename);
                        //                            Csw.tryExec(cswDlgPrivate.onSaveImmediate);
                        //                        },
                        onInitFinish: function () {
                            //openDialog(cswPublic.div, 800, 600, null, cswPublic.title);
                        }
                    });
                }; // loadNode()

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());

