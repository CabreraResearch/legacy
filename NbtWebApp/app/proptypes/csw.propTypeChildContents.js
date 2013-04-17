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

                nodeProperty.propDivTbl = nodeProperty.propDiv.table();

                var nsOptions = Csw.object();
                nsOptions.name = Csw.string(nodeProperty.propData.name).trim();
                nsOptions.selectedNodeId = Csw.string(nodeProperty.propData.values.relatednodeid).trim();
                nsOptions.selectedNodeLink = Csw.string(nodeProperty.propData.values.relatednodelink).trim();
                nsOptions.selectedName = Csw.string(nodeProperty.propData.values.relatednodename).trim();
                nsOptions.nodeTypeId = Csw.string(nodeProperty.propData.values.nodetypeid).trim();
                //nodeSelect.viewid = Csw.string(nodeProperty.propData.values.viewid).trim();
                nsOptions.objectClassId = Csw.string(nodeProperty.propData.values.objectclassid).trim();
                nsOptions.allowAdd = Csw.bool(nodeProperty.propData.values.allowadd);
                nsOptions.options = nodeProperty.propData.values.options;
                nsOptions.useSearch = false; // Csw.bool(nodeProperty.propData.values.usesearch);
                nsOptions.cellCol = 1;
                nsOptions.selectedNodeType = null;
                nsOptions.addImage = null;
                nsOptions.onSelectNode = function(nodeObj) {
                    // Csw.tryExec(nodeProperty.onChange, nodeObj.nodeid);
                    // nodeProperty.onPropChange({ nodeid: nodeObj.nodeid, name: nodeObj.name, relatednodeid: nodeObj.selectedNodeId, relatednodelink: nodeObj.relatednodelink });

                    cswPrivate.loadNode(nodeObj.nodeid);

                };
                nsOptions.onAfterAdd = cswPrivate.loadNode;
                nsOptions.relatedTo = {};
                nsOptions.relatedTo.relatednodeid = nodeProperty.tabState.nodeid;
                nsOptions.relatedTo.relatednodetypeid = nodeProperty.tabState.nodetypeid;
                nsOptions.relatedTo.relatednodename = nodeProperty.tabState.nodename;
                //nodeSelect.relatedTo.relatedobjectclassid = nodeProperty.tabState.relatedobjectclassid;

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
                    cswPrivate.loadNode(nsOptions.selectedNodeId, editLink);
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

