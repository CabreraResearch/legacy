/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('childContents', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';

            var cswPrivate = Csw.object();

            cswPrivate.loadNode = function (nodeid) {
                cswPrivate.childContentsDiv.empty();
                if (false === nodeProperty.tabState.Config) {
                    if (false === Csw.isNullOrEmpty(editLink)) {
                        editLink.show();
                    }
                    Csw.layouts.tabsAndProps(cswPrivate.childContentsDiv, {
                        name: 'tabsAndProps',
                        tabState: {
                            ShowAsReport: false,
                            nodeid: nodeid,
                            EditMode: nsOptions.isClickable ? Csw.enums.editMode.Edit : nodeProperty.tabState.EditMode,
                            ReadOnly: true,
                            showSaveButton: false
                        },
                        showTitle: false,
                        onInitFinish: function () {
                        },
                        forceReadOnly: true
                    });
                }
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
            nsOptions.onSelectNode = function (nodeObj) {
                if (nodeObj && nodeObj.nodeid) {
                    //Case 29390: No need to save or sync with other instances of this prop
                    cswPrivate.loadNode(nodeObj.nodeid);
                }
            };
            nsOptions.onAfterAdd = cswPrivate.loadNode;
            nsOptions.relatedTo = {};
            nsOptions.relatedTo.relatednodeid = nodeProperty.tabState.nodeid;
            nsOptions.relatedTo.relatednodename = nodeProperty.tabState.nodename;
            nsOptions.denyRelatedAsSelected = true;
            nsOptions.isRequired = nodeProperty.isRequired();
            nsOptions.isMulti = nodeProperty.isMulti();
            nsOptions.isReadOnly = false; // nodeProperty.isReadOnly();
            //case 28180 - relationships not clickable from audit history popup (Case 30496 - or when viewing As Report)
            nsOptions.isClickable = (nodeProperty.tabState.EditMode !== Csw.enums.editMode.AuditHistoryInPopup &&
                                     nodeProperty.tabState.EditMode !== Csw.enums.editMode.PrintReport &&
                                     nodeProperty.tabState.EditMode !== Csw.enums.editMode.Preview &&
                                     false === nodeProperty.tabState.Config);

            nsOptions.doGetNodes = false;
            nsOptions.showSelectOnLoad = true;

            var nodeSelect = nodeProperty.propDivTbl.cell(1, 1).nodeSelect(nsOptions);

            if (nsOptions.isClickable) {
                var editLink = nodeProperty.propDivTbl.cell(1, 2).buttonExt({
                    name: 'childContentsEdit',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                    size: 'small',
                    enabledText: 'Edit Selected',
                    onClick: function () {
                        var nodeid = nodeSelect.selectedNodeId();
                        $.CswDialog('EditNodeDialog', {
                            currentNodeId: nodeid,
                            nodenames: [nodeSelect.selectedName()],
                            onEditNode: function () {
                                // refresh
                                cswPrivate.loadNode(nodeid);
                            }
                        }); // CswDialog
                    } // onClick
                }); // link
            }
            cswPrivate.childContentsDiv = nodeProperty.propDiv.div();

            if (false === Csw.isNullOrEmpty(nsOptions.selectedNodeId)) {
                cswPrivate.loadNode(nsOptions.selectedNodeId);
            } else {
                if (false === Csw.isNullOrEmpty(editLink)) {
                    editLink.hide();
                }
            }
        }; // render()

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());

