/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.childContents = Csw.properties.register('childContents',
        function(nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                var cswPrivate = Csw.object();

                cswPrivate.loadNode = function (nodeid) {
                    editLink.show();
                    cswPrivate.childContentsDiv.empty();

                    Csw.layouts.tabsAndProps(cswPrivate.childContentsDiv, {
                        name: 'tabsAndProps',
                        tabState: {
                            ShowAsReport: false,
                            nodeid: nodeid,
                            EditMode: Csw.enums.editMode.Edit,
                            ReadOnly: true,
                            showSaveButton: false
                        },
                        showTitle: false,
                        onInitFinish: function () {
                        },
                        forceReadOnly: true
                    });
                }; // loadNode()

                nodeProperty.propDivTbl = nodeProperty.propDiv.table();

                var nsOptions = Csw.object();
                nsOptions.selectedNodeId = nodeProperty.propData.values.relatednodeid;
                nsOptions.nodeTypeId = nodeProperty.propData.values.nodetypeid;
                nsOptions.objectClassId = nodeProperty.propData.values.objectclassid;
                nsOptions.allowAdd = nodeProperty.propData.values.allowadd;
                nsOptions.options = nodeProperty.propData.values.options;
                nsOptions.useSearch = false; // Csw.bool(nodeProperty.propData.values.usesearch);
                nsOptions.cellCol = 1;
                nsOptions.selectedNodeType = null;
                nsOptions.addImage = null;
                nsOptions.onSelectNode = function(nodeObj) {
                    //Case 29390: No need to save or sync with other instances of this prop
                    cswPrivate.loadNode(nodeObj.nodeid);

                };
                nsOptions.onAfterAdd = cswPrivate.loadNode;
                nsOptions.relatedTo = {};
                nsOptions.relatedTo.relatednodeid = nodeProperty.tabState.nodeid;
                nsOptions.relatedTo.relatednodetypeid = nodeProperty.tabState.nodetypeid;
                nsOptions.relatedTo.relatednodename = nodeProperty.tabState.nodename;
                nsOptions.isRequired = nodeProperty.isRequired();
                nsOptions.isMulti = nodeProperty.isMulti();
                nsOptions.isReadOnly = false; // nodeProperty.isReadOnly();
                nsOptions.isClickable = nodeProperty.tabState.EditMode !== Csw.enums.editMode.AuditHistoryInPopup; //case 28180 - relationships not clickable from audit history popup

                nsOptions.doGetNodes = false;
                nsOptions.showSelectOnLoad = true;

                var nodeSelect = nodeProperty.propDivTbl.cell(1, 1).nodeSelect(nsOptions);

                var editLink = nodeProperty.propDivTbl.cell(1, 2).buttonExt({
                    name: 'childContentsEdit',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                    size: 'small',
                    enabledText: 'Edit Selected',
                    onClick: function() {
                        var nodeid = nodeSelect.selectedNodeId();
                        $.CswDialog('EditNodeDialog', {
                            currentNodeId: nodeid,
                            nodenames: [nodeSelect.selectedName()],
                            onEditNode: function() {
                                // refresh
                                cswPrivate.loadNode(nodeid);
                            }
                        }); // CswDialog
                    } // onClick
                }); // link
                cswPrivate.childContentsDiv = nodeProperty.propDiv.div();

                if (false === Csw.isNullOrEmpty(nsOptions.selectedNodeId)) {
                    cswPrivate.loadNode(nsOptions.selectedNodeId);
                } else {
                    editLink.hide();
                }
            }; // render()
            
            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());

