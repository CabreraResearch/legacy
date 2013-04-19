/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.relationship = Csw.properties.register('relationship',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();
                cswPrivate.relatedNodeId = nodeProperty.propData.values.relatednodeid;
                
                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.relatedNodeId !== val.selectedNodeId) {
                        cswPrivate.relatedNodeId = val.selectedNodeId;
                        updateProp(val);
                    }
                });

                var updateProp = function (val) {
                    nodeProperty.propData.values.nodeid = val.nodeid;
                    nodeProperty.propData.values.name = val.name;
                    nodeProperty.propData.values.relatednodeid = val.selectedNodeId;
                    nodeProperty.propData.values.relatednodelink = val.relatednodelink;
                    nodeSelect.val(val.selectedNodeId);
                };


                var optsNodeSelect = {};
                optsNodeSelect.name = nodeProperty.propData.name;
                optsNodeSelect.selectedNodeId = nodeProperty.propData.values.relatednodeid;
                optsNodeSelect.selectedNodeLink = nodeProperty.propData.values.relatednodelink;
                optsNodeSelect.selectedName = nodeProperty.propData.values.name;
                optsNodeSelect.nodeTypeId = nodeProperty.propData.values.nodetypeid;
                optsNodeSelect.viewid = nodeProperty.propData.values.viewid;
                optsNodeSelect.objectClassId = nodeProperty.propData.values.objectclassid;
                optsNodeSelect.allowAdd = nodeProperty.propData.values.allowadd;
                optsNodeSelect.options = nodeProperty.propData.values.options;
                optsNodeSelect.useSearch = nodeProperty.propData.values.usesearch;
                optsNodeSelect.cellCol = 1;
                optsNodeSelect.selectedNodeType = null;
                optsNodeSelect.addImage = null;
                optsNodeSelect.onAddNodeFunc = function() {
                };
                optsNodeSelect.onSelectNode = function(nodeObj) {
                    nodeProperty.propData.values.nodeid = nodeObj.nodeid;
                    nodeProperty.propData.values.name = nodeObj.name;
                    nodeProperty.propData.values.relatednodeid = nodeObj.selectedNodeId;
                    nodeProperty.propData.values.relatednodelink = nodeObj.relatednodelink;
                    cswPrivate.relatedNodeId = nodeObj.selectedNodeId;
                    
                    nodeProperty.broadcastPropChange(nodeObj);
                };

                optsNodeSelect.relatedTo = {};
                optsNodeSelect.relatedTo.relatednodeid = nodeProperty.tabState.relatednodeid;
                optsNodeSelect.relatedTo.relatednodetypeid = nodeProperty.tabState.relatednodetypeid;
                optsNodeSelect.relatedTo.relatednodename = nodeProperty.tabState.relatednodename;
                optsNodeSelect.relatedTo.relatedobjectclassid = nodeProperty.tabState.relatedobjectclassid;
                optsNodeSelect.relationshipNodeTypePropId = nodeProperty.propid;

                optsNodeSelect.isRequired = nodeProperty.isRequired();
                optsNodeSelect.isMulti = nodeProperty.isMulti();
                optsNodeSelect.isReadOnly = nodeProperty.isReadOnly();
                optsNodeSelect.isClickable = nodeProperty.tabState.EditMode !== Csw.enums.editMode.AuditHistoryInPopup; //case 28180 - relationships not clickable from audit history popup

                optsNodeSelect.doGetNodes = false;

                optsNodeSelect.showSelectOnLoad = (function() {
                    return nodeProperty.tabState.EditMode === Csw.enums.editMode.Add ||
                        nodeProperty.isMulti() ||
                        (nodeProperty.isRequired() && Csw.isNullOrEmpty(optsNodeSelect.selectedNodeId));
                }());

                var nodeSelect = nodeProperty.propDiv.nodeSelect(optsNodeSelect);

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());        