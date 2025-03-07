/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/app/ChemSW.js" />

(function () {

    /**
     *  
     * @class
     * @classdesc Node Selects are used to drive picklists in wizards, relationship and child contents properties.
    */

    Csw.composites.register('nodeSelect', function (cswParent, cswPrivate) {
        'use strict';

        //#region _preCtor

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.$parent = cswPrivate.$parent || cswParent.$;
            cswPrivate.name = cswPrivate.name || '';
            cswPrivate.nodesUrlMethod = cswPrivate.nodesUrlMethod || 'Nodes/get';

            cswPrivate.labelText = cswPrivate.labelText || null;
            cswPrivate.excludeNodeTypeIds = cswPrivate.excludeNodeTypeIds || '';
            // Note for excludeNodeIds: This also filters these nodes from search results
            // in the case that the nodeselect is a Search button
            cswPrivate.excludeNodeIds = cswPrivate.excludeNodeIds || [];
            cswPrivate.selectedNodeId = cswPrivate.selectedNodeId || '';
            cswPrivate.selectedName = cswPrivate.selectedName || '';
            cswPrivate.viewid = cswPrivate.viewid || '';

            cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || '';
            cswPrivate.objectClassId = cswPrivate.objectClassId || '';
            cswPrivate.propertySetId = cswPrivate.propertySetId || '';
            cswPrivate.objectClassName = cswPrivate.objectClassName || '';
            cswPrivate.addNodeDialogTitle = cswPrivate.addNodeDialogTitle || '';

            cswPrivate.relatedTo = cswPrivate.relatedTo || {};
            cswPrivate.relatedTo.relatednodeid = cswPrivate.relatedTo.relatednodeid || '';
            cswPrivate.relatedTo.relatednodename = cswPrivate.relatedTo.relatednodename || '';
            cswPrivate.relationshipNodeTypePropId = cswPrivate.relationshipNodeTypePropId || '';

            cswPrivate.cellCol = cswPrivate.cellCol || 1;
            cswPrivate.width = cswPrivate.width || '200px';

            cswPrivate.onSelectNode = cswPrivate.onSelectNode || function () { };
            cswPrivate.onSuccess = cswPrivate.onSuccess || function () { };

            //cswPrivate.addNewOption = cswPrivate.addNewOption; // || false;
            cswPrivate.allowAdd = cswPrivate.allowAdd; // || false;
            cswPrivate.onAfterAdd = cswPrivate.onAfterAdd;

            cswPrivate.isRequired = cswPrivate.isRequired; // || false;
            cswPrivate.isMulti = cswPrivate.isMulti; // || false;
            cswPrivate.isReadOnly = cswPrivate.isReadOnly; // || false;
            cswPrivate.showSelectOnLoad = cswPrivate.showSelectOnLoad; // || true;
            cswPrivate.isClickable = cswPrivate.isClickable; // ||true;
            cswPrivate.useSearch = cswPrivate.useSearch;
            cswPrivate.usePreview = cswPrivate.usePreview;
            cswPrivate.options = cswPrivate.options || [];
            cswPrivate.extraOptions = cswPrivate.extraOptions || [];

            // Case 30408
            cswPrivate.showRemoveIcon = cswPrivate.showRemoveIcon || false;
            cswPrivate.wasNodeLinkModified = false; // Used to validate when there is no nodeselect only search button
            cswPrivate.overrideNodelinkValidation = cswPrivate.overrideNodelinkValidation || false;
            
            cswPrivate.hideNodeLink = cswPrivate.hideNodeLink || false;

            cswPublic = cswParent.div({ cssclass: 'cswInline' });
            cswPrivate.table = cswPublic.table();

            // Default to selected node as relationship value for new nodes being added
            cswPrivate.forceSelectedAsOption = true;
            if (false === cswPrivate.denyRelatedAsSelected) { // case 30646
                if (false === Csw.isNullOrEmpty(cswPrivate.relatedTo.relatednodeid) &&
                    Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                    false === cswPrivate.isMulti) {

                    cswPrivate.selectedNodeId = cswPrivate.relatedTo.relatednodeid;
                    cswPrivate.selectedName = cswPrivate.relatedTo.relatednodename;
                    cswPrivate.forceSelectedAsOption = false;
                }
            }

            cswPrivate.ajax = null;

            cswPrivate.selectCellCol = cswPrivate.cellCol + 0;
            cswPrivate.textCellCol = cswPrivate.cellCol + 1;
            cswPrivate.editCellCol = cswPrivate.cellCol + 3;
            cswPrivate.nodeTypeCellCol = cswPrivate.cellCol + 4;
            cswPrivate.searchCellCol = cswPrivate.cellCol + 0;
            cswPrivate.searchButtonCellCol = cswPrivate.cellCol + 2;
            cswPrivate.removeSelCellCol = cswPrivate.cellCol + 1;
            cswPrivate.addCellCol = cswPrivate.cellCol + 5;
            cswPrivate.tipCellCol = cswPrivate.cellCol + 6;
            cswPrivate.previewCellCol = cswPrivate.cellCol + 7;
            cswPrivate.validateCellCol = cswPrivate.cellCol + 8;

        }());

        //#endregion _preCtor

        //#region AJAX

        /**
        
        */
        cswPrivate.getNodes = function () {
            cswPrivate.ajax = Csw.ajaxWcf.post({
                urlMethod: cswPrivate.nodesUrlMethod,
                data: cswPrivate.ajaxData || {
                    NodeTypeId: Csw.number(cswPrivate.nodeTypeId, 0),
                    ObjectClassId: Csw.number(cswPrivate.objectClassId, 0),
                    PropertySetId: Csw.number(cswPrivate.propertySetId, 0),
                    ObjectClass: Csw.string(cswPrivate.objectClassName),
                    RelatedToObjectClass: Csw.string(cswPrivate.relatedTo.objectClassName),
                    RelatedToNodeId: Csw.string(cswPrivate.relatedTo.nodeId),
                    ViewId: Csw.string(cswPrivate.viewid)
                },
                success: function (data) {
                    cswPrivate.options = [];
                    if (false === cswPrivate.isRequired) {
                        cswPrivate.options.push({ id: '', value: '' });
                    }

                    cswPrivate.extraOptions.forEach(function (obj) {
                        cswPrivate.options.push({ id: obj.id, value: obj.value });
                    });

                    data.Nodes.forEach(function (obj) {
                        if (-1 === cswPrivate.excludeNodeIds.indexOf(obj.NodeId)) {
                            cswPrivate.options.push({ id: obj.NodeId, value: obj.NodeName, nodelink: obj.NodeLink });
                        }
                    });
                    cswPrivate.canAdd = Csw.bool(cswPrivate.canAdd) && Csw.bool(data.CanAdd);
                    cswPrivate.useSearch = Csw.bool(data.UseSearch);
                    cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || data.NodeTypeId;
                    cswPrivate.objectClassId = cswPrivate.objectClassId || data.ObjectClassId;
                    cswPrivate.relatedTo.objectClassId = cswPrivate.relatedTo.objectClassId || data.RelatedToObjectClassId;

                    cswPrivate.makeControl();
                }
            });
        };

        cswPrivate.getNodeTypeOptions = function () {
            cswPrivate.blankText = '[Select One]';
            if (null === cswPrivate.selectedNodeType || undefined === cswPrivate.selectedNodeType) {
                cswPrivate.selectedNodeType = cswPrivate.table.cell(1, cswPrivate.nodeTypeCellCol)
                    .nodeTypeSelect({
                        objectClassName: cswPrivate.objectClassName,
                        objectClassId: cswPrivate.objectClassId,
                        relationshipNodeTypePropId: cswPrivate.relationshipNodeTypePropId,
                        onSelect: function() {
                            if (cswPrivate.blankText !== cswPrivate.selectedNodeType.val()) {
                                cswPrivate.nodeTypeId = cswPrivate.selectedNodeType.val();
                                cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId, cswPrivate.nodeTypeAddAction);
                            }
                        },
                        onSuccess: function(data, nodeTypeCount, lastNodeTypeId) {
                            cswPrivate.nodeTypeAddAction = data.action;
                            if (Csw.number(nodeTypeCount) > 1) {
                                cswPrivate.selectedNodeType.show();
                                cswPrivate.addImage.hide();
                            } else {
                                cswPrivate.nodeTypeId = lastNodeTypeId;
                                cswPrivate.selectedNodeType.hide();
                                cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId, cswPrivate.nodeTypeAddAction);
                            }
                        },
                        blankOptionText: cswPrivate.blankText,
                        filterToPermission: 'Create'
                    }).hide();
            } else {
                cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId, cswPrivate.nodeTypeAddAction);
            }
        };

        //#endregion AJAX

        //#region Control Construction

        cswPrivate.makeControl = function () {
            cswPrivate.makeAddImage();
            if (cswPrivate.useSearch) {
                cswPrivate.makeSearch();
            } else {
                cswPrivate.makeSelect();
            }
            Csw.tryExec(cswPrivate.onSuccess, cswPrivate.options, cswPrivate.useSearch);
        };

        cswPrivate.bindSelectMethods = function () {
            cswPublic.val = cswPrivate.select.val;
            cswPublic.selectedText = cswPrivate.select.selectedText;
            cswPublic.selectedVal = cswPrivate.select.selectedVal;
            cswPublic.makeOption = cswPrivate.select.makeOption;
            cswPublic.makeOptions = cswPrivate.select.makeOptions;
            cswPublic.addOption = cswPrivate.select.addOption;
            cswPublic.setOptions = cswPrivate.select.setOptions;
            cswPublic.removeOption = cswPrivate.select.removeOption;
            cswPublic.option = cswPrivate.select.option;
        };

        cswPrivate.setNodeLinkText = function (link) {
            if (link &&
                cswPrivate.nodeLinkCell &&
                false === cswPrivate.isMulti) {

                cswPrivate.nodeLinkCell.empty();
                if (cswPrivate.hideNodeLink) {
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkCell.span({
                        text: cswPrivate.selectedName
                    });
                } else if(false === Csw.isNullOrEmpty(link) && link !== "undefined") {
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkCell.nodeLink({
                        text: link
                    });
                };

                cswPrivate.wasNodeLinkModified = true;
            }
        };

        cswPrivate.makeSelect = function () {

            var handleChange = function () {
                var val = cswPrivate.select.val();
                cswPrivate.selectedNodeId = val;
                cswPrivate.selectedName = cswPrivate.select.selectedText();

                Csw.tryExec(cswPrivate.onChange, cswPrivate.select);

                if (cswPrivate.isRequired) {
                    cswPrivate.select.removeOption('');
                }

                cswPrivate.table.cell(1, cswPrivate.tipCellCol).empty();
                Csw.tryExec(cswPrivate.onSelectNode, {
                    nodeid: val,
                    name: cswPrivate.select.selectedText(),
                    selectedNodeId: val,
                    relatednodelink: cswPrivate.select.selectedData('link')
                });
            }; // handleChange()


            //Normally, we would assign the select to cswPublic, as it's the primary control; 
            //however, to assign now would be decoupled from the return. The caller would not receive this reference.
            //Instead, we'll assign the select as a member and we'll reassign key methods to cswPublic.
            cswPrivate.select = cswPrivate.table.cell(1, cswPrivate.selectCellCol).select({
                name: cswPrivate.name,
                cssclass: 'selectinput',
                onChange: handleChange,
                width: cswPrivate.width,
                isRequired: cswPrivate.isRequired
            });
            // Select value in a selectbox
            cswPrivate.foundSelected = false;

            Csw.iterate(cswPrivate.options, function (relatedObj) {
                if (false === cswPrivate.foundSelected && relatedObj.id === cswPrivate.selectedNodeId) {
                    //Case 29523: Even in Multi-Edit, we still want the data to be correct false === Csw.bool(cswPrivate.isMulti)
                    cswPrivate.foundSelected = true;
                    cswPrivate.select.option({ value: relatedObj.id, display: relatedObj.value, isSelected: true }).data({ link: relatedObj.link });
                } else {
                    cswPrivate.select.option({ value: relatedObj.id, display: relatedObj.value }).data({ link: relatedObj.link });
                }
            });

            if (cswPrivate.forceSelectedAsOption &&
                false === cswPrivate.isMulti &&
                false === cswPrivate.foundSelected) {

                if (false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                    cswPrivate.select.option({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName, isSelected: true }).data({ link: cswPrivate.selectedNodeLink });
                } else if (cswPublic.optionsCount(false) > 0) {
                    // case 28918 - select the first option, and trigger the change event
                    handleChange();
                }
            }

            cswPrivate.bindSelectMethods();

            //cswPrivate.select.bind('change', handleChange);

            cswPrivate.nodeLinkCell = cswPrivate.table.cell(1, cswPrivate.textCellCol);
            
            cswPrivate.setNodeLinkText(cswPrivate.selectedNodeLink);

            cswPrivate.toggleButton = cswPrivate.table.cell(1, cswPrivate.editCellCol).buttonExt({
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                size: 'small',
                enabledText: 'Edit',
                onClick: function () {
                    cswPrivate.toggleOptions(true);
                }
            });

            cswPrivate.toggleOptions(cswPrivate.showSelectOnLoad);

            cswPrivate.select.required(cswPrivate.isRequired);

            // case 28427 - default private values to currently selected
            cswPrivate.selectedNodeId = cswPrivate.select.val();
            cswPrivate.selectedName = cswPrivate.select.selectedText();
        };

        cswPrivate.makeSearch = function () {
            // Find value by using search in a dialog
            cswPrivate.validateCell = cswPrivate.table.cell(1, cswPrivate.validateCellCol).empty();

            cswPrivate.nameSpan = cswPrivate.table.cell(1, cswPrivate.searchCellCol).nodeLink({
                text: Csw.string(cswPrivate.selectedNodeLink) + '&nbsp;'
            });

            if (cswPrivate.isRequired && false === cswPrivate.overrideNodelinkValidation) {
                var returnObj = Csw.validator(
                    cswPrivate.validateCell.div(),
                    cswPrivate.nameSpan,
                    {
                        wasModified: cswPrivate.wasModified,
                        onValidation: null,
                        className: 'validateNodeLink',
                        isExtJsControl: false
                    }
                );
                cswPrivate.checkBox = returnObj.input;

                // Validate
                cswPrivate.validateNodeLink(cswPrivate.nameSpan.text());
            }

            cswPrivate.hiddenValue = cswPrivate.table.cell(1, cswPrivate.searchCellCol).input({
                name: 'hiddenvalue',
                type: Csw.enums.inputTypes.hidden,
                value: cswPrivate.selectedNodeId
            });


            cswPrivate.toggleButton = cswPrivate.table.cell(1, cswPrivate.editCellCol).buttonExt({
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                size: 'small',
                enabledText: 'Edit',
                onClick: function () {
                    cswPrivate.toggleOptions(true);
                }
            });

            cswPrivate.searchButton = cswPrivate.table.cell(1, cswPrivate.searchButtonCellCol).buttonExt({
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.magglass),
                size: 'small',
                enabledText: "Search",
                tooltip: { title: "Search " + cswPrivate.name, },
                onClick: function () {
                    $.CswDialog('SearchDialog', {
                        propname: cswPrivate.name,
                        nodetypeid: cswPrivate.nodeTypeId,
                        objectclassid: cswPrivate.objectClassId,
                        propertysetid: cswPrivate.propertySetId,
                        excludeNodeIds: cswPrivate.excludeNodeIds,
                        onSelectNode: function (nodeObj) {
                            cswPrivate.nameSpan.empty();
                            cswPrivate.nameSpan.nodeLink({
                                text: nodeObj.nodelink + '&nbsp;'
                            });

                            cswPrivate.wasNodeLinkModified = true;

                            // We use the remove icon during C3 imports - Case 30408
                            if (cswPrivate.showRemoveIcon) {
                                cswPrivate.removeIcon = cswPrivate.table.cell(1, cswPrivate.removeSelCellCol).icon({
                                    hovertext: 'Remove selected',
                                    isButton: true,
                                    iconType: Csw.enums.iconType.x,
                                    onClick: function () {
                                        cswPrivate.nameSpan.empty();
                                        cswPrivate.hiddenValue.val("");
                                        cswPrivate.selectedNodeId = "";
                                        cswPrivate.selectedNodeLink = "";
                                        cswPrivate.removeIcon.hide();
                                        Csw.tryExec(cswPrivate.onRemoveSelectedNode);
                                    }
                                });
                            }

                            // Validation
                            if (cswPrivate.isRequired && false === cswPrivate.overrideNodelinkValidation) {
                                cswPrivate.validateNodeLink(cswPrivate.nameSpan.text());
                            }

                            cswPrivate.hiddenValue.val(nodeObj.nodeid);
                            cswPrivate.selectedName = nodeObj.nodename;
                            cswPrivate.selectedNodeId = nodeObj.nodeid;
                            cswPrivate.selectedNodeLink = nodeObj.nodelink;
                            Csw.tryExec(cswPrivate.onSelectNode, nodeObj);
                        },
                        onClose: function () {
                            cswPrivate.searchButton.enable();
                        }
                    });
                }
            });


            cswPrivate.toggleOptions(cswPrivate.showSelectOnLoad);
        };

        cswPrivate.toggleOptions = function (on) {
            if (Csw.bool(on)) {
                if (cswPrivate.useSearch) {
                    cswPrivate.searchButton.show();
                } else {
                    cswPrivate.select.show();
                    cswPrivate.nodeLinkCell.hide();
                }
                if (cswPrivate.addImage) {
                    cswPrivate.addImage.show();
                }
                cswPrivate.toggleButton.hide();
            } else {
                if (cswPrivate.useSearch) {
                    cswPrivate.searchButton.hide();
                } else {
                    cswPrivate.select.hide();
                    cswPrivate.nodeLinkCell.show();
                }
                if (cswPrivate.addImage) {
                    cswPrivate.addImage.hide();
                }
                cswPrivate.toggleButton.show();
            }
        };

        cswPrivate.validateNodeLink = function (value) {
            if (Csw.isNullOrEmpty(value)) {
                cswPrivate.checkBox.val(false);
            } else {
                cswPrivate.checkBox.val(true);
            }
            if (cswPrivate.wasNodeLinkModified) {
                cswPrivate.checkBox.$.valid();
            }
        };

        //#endregion Control Construction

        //#region Add

        cswPrivate.onAddNodeFunc = function (nodeid, nodekey, nodename, nodelink) {
            if (cswPrivate.nameSpan) {
                cswPrivate.nameSpan.empty();
                cswPrivate.selectedNodeLink = nodelink;
                cswPrivate.nameSpan.nodeLink({
                    text: nodelink
                });
                cswPrivate.wasNodeLinkModified = true;
            }
            if (cswPrivate.hiddenValue) {
                cswPrivate.hiddenValue.val(nodeid);
                Csw.tryExec(cswPrivate.onSelectNode, {
                    nodeid: nodeid,
                    nodename: nodename,
                    nodelink: nodelink
                });
            }

            if (cswPrivate.nameSpan && cswPrivate.isRequired && false === cswPrivate.overrideNodelinkValidation) {
                cswPrivate.validateNodeLink(cswPrivate.nameSpan.text());
            }

            if (cswPrivate.select) {
                Csw.ajaxWcf.post({
                    urlMethod: cswPrivate.nodesUrlMethod,
                    data: cswPrivate.ajaxData || {
                        ViewId: Csw.string(cswPrivate.viewid),
                        NodeTypeId: Csw.number(cswPrivate.nodeTypeId, 0),
                        ObjectClassId: Csw.number(cswPrivate.objectClassId, 0),
                        PropertySetId: Csw.number(cswPrivate.propertySetId, 0),
                        ObjectClass: Csw.string(cswPrivate.objectClassName),
                        RelatedToObjectClass: Csw.string(cswPrivate.relatedTo.objectClassName),
                        RelatedToNodeId: Csw.string(cswPrivate.relatedTo.relatednodeid)
                    },
                    success: function (data) {
                        //Case 28798 - we only want the else condition if we expected results, but didn't get any.
                        //In the case where the current select control has no results, we expect no results.
                        var changed = (data.Nodes.length !== cswPublic.optionsCount(true) && (cswPublic.optionsCount() > 0 || data.Nodes.length > 0));
                        if (data.Nodes.length > 0 || cswPublic.optionsCount(true) === 0) {
                            var found = false;
                            //Don't rebuild the select, just add the new Node if it matches the collection of nodes scoped to the view.
                            data.Nodes.forEach(function (obj) {
                                if (obj.NodeId === nodeid) {
                                    found = true;
                                    cswPrivate.options.push({ id: obj.NodeId, value: obj.NodeName, isSelected: obj.NodeId === nodeid });
                                    cswPrivate.select.option({ value: obj.NodeId, display: obj.NodeName, selected: true }).data({ link: obj.NodeLink });
                                    cswPrivate.select.val(obj.NodeId);
                                    cswPrivate.selectedNodeId = obj.NodeId;
                                }
                            });
                            changed = changed && found;
                            if (false === found) {
                                if (cswPrivate.selectedNodeId) {
                                    cswPrivate.select.val(cswPrivate.selectedNodeId);
                                }
                                if (nodelink) {
                                    cswPrivate.table.cell(1, cswPrivate.tipCellCol).nodeLink({
                                        cssclasstext: 'CswErrorMessage_ValidatorError',
                                        text: '&nbsp;' + nodelink + ' has been added. However,<br/>&nbsp;it is not an available option for ' + cswPrivate.name + '.'
                                    });
                                }
                            }
                        } else {
                            cswPrivate.select.option({ value: nodeid, display: nodename, selected: true });
                            cswPrivate.select.val(nodeid);
                            cswPrivate.selectedNodeId = nodeid;
                        }

                        if (changed) {
                            cswPrivate.select.$.valid();

                            Csw.tryExec(cswPrivate.onSelectNode, {
                                nodeid: cswPrivate.select.selectedVal(),
                                name: cswPrivate.select.selectedText(),
                                selectedNodeId: cswPrivate.selectedNodeId,
                                relatednodelink: cswPrivate.select.selectedData('link')
                            });

                            Csw.tryExec(cswPrivate.onAfterAdd, nodeid);
                        }


                    }
                });
                cswPrivate.toggleOptions(true);
            }//if(cswPrivate.select)
        };

        cswPrivate.openAddNodeDialog = function (nodetypeToAdd, action) {
            Csw.dialogs.addnode({
                action: action,
                nodetypeid: nodetypeToAdd,
                objectClassId: cswPrivate.objectClassId,
                onAddNode: cswPrivate.onAddNodeFunc,
                title: 'Add New ' + cswPrivate.name,
                relatednodeid: cswPrivate.relatedTo.relatednodeid
            });
        };

        cswPrivate.makeAddImage = function () {
            if (cswPrivate.allowAdd) {
                cswPrivate.addImage = cswPrivate.table.cell(1, cswPrivate.addCellCol).div()
                    .buttonExt({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                        size: 'small',
                        enabledText: 'New',
                        tooltip: { title: 'Add New ' + cswPrivate.name },
                        onClick: function () {
                            cswPrivate.addImage.enable();
                            cswPrivate.table.cell(1, cswPrivate.tipCellCol).empty();
                            cswPrivate.getNodeTypeOptions();
                        }
                    });
            }
        };

        //#endregion Add

        //#region Public

        cswPublic.setSelectedNode = function (nodeid, nodename, link) {
            cswPrivate.selectedNodeId = nodeid;
            cswPrivate.selectedName = nodename;
            if (cswPrivate.useSearch && cswPrivate.nameSpan && cswPrivate.hiddenValue) {
                cswPrivate.nameSpan.text(nodename);
                cswPrivate.hiddenValue.val(nodeid);
            }
            if (cswPrivate.select) {
                cswPrivate.select.val(nodeid);
            }
            if (link) {
                cswPrivate.setNodeLinkText(link);
            }
        }; // setSelectedNode

        cswPublic.selectedNodeId = function () {
            return cswPrivate.selectedNodeId;
        }; // selectedNodeId
        cswPublic.selectedName = function () {
            return cswPrivate.selectedName;
        }; // selectedName
        cswPublic.selectedNodeLink = function () {
            return cswPrivate.selectedNodeLink;
        }; // selectedNodeLink

        cswPublic.optionsCount = function (excludeEmpty) {
            var ret = cswPrivate.options.length;
            if (excludeEmpty) {
                Csw.iterate(cswPrivate.options, function (val) {
                    if (!val.id || !val.value) {
                        ret -= 1;
                    }
                });
            }
            return ret;
        };

        cswPublic.getAjax = function () {
            return cswPrivate.ajax;
        };

        //#endregion Public

        //#region _postCtor

        (function _relationship() {
            if (false === cswPrivate.isClickable) {
                cswPrivate.nodeTextCell = cswPrivate.table.cell(1, cswPrivate.textCellCol);
                cswPrivate.nodeText = cswPrivate.nodeTextCell.span({
                    text: cswPrivate.selectedName
                });
            } else if (cswPrivate.isReadOnly) {
                cswPrivate.nodeLinkCell = cswPrivate.table.cell(1, cswPrivate.textCellCol);
                cswPrivate.setNodeLinkText(cswPrivate.selectedNodeLink);

            } else {
                if (cswPublic.optionsCount(false) > 0 || false === cswPrivate.doGetNodes) {
                    cswPrivate.makeControl();
                } else {
                    cswPrivate.getNodes();
                }
            } // if-else (o.ReadOnly) {

            if (false !== cswPrivate.usePreview && cswPrivate.isClickable) {
                cswPrivate.table.cell(1, cswPrivate.previewCellCol).css({ width: '24px' });
                cswPublic.$.hover(
                    function (event) {
                        Csw.nodeHoverIn(event, {
                            nodeid: cswPrivate.selectedNodeId,
                            nodename: cswPrivate.selectedName,
                            parentDiv: cswPrivate.table.cell(1, cswPrivate.previewCellCol),
                            useAbsolutePosition: false,
                            rightpad: 0
                        });
                    },
                    function (event) {
                        Csw.nodeHoverOut();
                    }
                );
            }
        }());

        return cswPublic;

        //#endregion _postCtor
    });
}());

