/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.relationship = Csw.properties.register('relationship',
        function(nodeProperty) {
            'use strict';
            var cswPrivate = {};
            var cswPublic = {
                
            };

            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                var nodeSelect = {};
                nodeSelect.name = nodeProperty.propData.name;
                nodeSelect.selectedNodeId = nodeProperty.propData.values.relatednodeid;
                nodeSelect.selectedNodeLink = nodeProperty.propData.values.relatednodelink;
                nodeSelect.selectedName = nodeProperty.propData.values.name;
                nodeSelect.nodeTypeId = nodeProperty.propData.values.nodetypeid;
                nodeSelect.viewid = nodeProperty.propData.values.viewid;
                nodeSelect.objectClassId = nodeProperty.propData.values.objectclassid;
                nodeSelect.allowAdd = nodeProperty.propData.values.allowadd;
                nodeSelect.options = nodeProperty.propData.values.options;
                nodeSelect.useSearch = nodeProperty.propData.values.usesearch;
                nodeSelect.cellCol = 1;
                nodeSelect.selectedNodeType = null;
                nodeSelect.addImage = null;
                nodeSelect.onAddNodeFunc = function() {
                };
                nodeSelect.onSelectNode = function(nodeObj) {
                    nodeProperty.propData.values.nodeid = nodeObj.nodeid;
                    nodeProperty.propData.values.name = nodeObj.name;
                    nodeProperty.propData.values.relatednodeid = nodeObj.selectedNodeId;
                    nodeProperty.propData.values.relatednodelink = nodeObj.relatednodelink;

                    nodeProperty.broadcastPropChange();
                };

                nodeSelect.relatedTo = {};
                nodeSelect.relatedTo.relatednodeid = nodeProperty.tabState.relatednodeid;
                nodeSelect.relatedTo.relatednodetypeid = nodeProperty.tabState.relatednodetypeid;
                nodeSelect.relatedTo.relatednodename = nodeProperty.tabState.relatednodename;
                nodeSelect.relatedTo.relatedobjectclassid = nodeProperty.tabState.relatedobjectclassid;
                nodeSelect.relationshipNodeTypePropId = nodeProperty.propid;

                nodeSelect.isRequired = nodeProperty.isRequired();
                nodeSelect.isMulti = nodeProperty.isMulti();
                nodeSelect.isReadOnly = nodeProperty.isReadOnly();
                nodeSelect.isClickable = nodeProperty.tabState.EditMode !== Csw.enums.editMode.AuditHistoryInPopup; //case 28180 - relationships not clickable from audit history popup

                nodeSelect.doGetNodes = false;

                nodeSelect.showSelectOnLoad = (function() {
                    return nodeProperty.tabState.EditMode === Csw.enums.editMode.Add ||
                        nodeProperty.isMulti() ||
                        (nodeProperty.isRequired() && Csw.isNullOrEmpty(nodeSelect.selectedNodeId));
                }());

                nodeProperty.propDiv.nodeSelect(nodeSelect);

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());        