/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.nodeSelect = Csw.controls.nodeSelect ||
        Csw.controls.register('nodeSelect', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                $parent: '',
                ID: '',
                async: true,
                nodesUrlMethod: 'getNodes',
                nodeTypeId: '',
                objectClassId: '',
                objectClassName: '',
                relatedTo: {
                    objectClassName: '',
                    nodeId: ''
                },
                onSelect: null,
                onSuccess: null,
                width: '200px',
                addNewOption: false,
                labelText: null,
                excludeNodeTypeIds: '',
                canAdd: false,
                selectedNodeId: ''
            };
            var cswPublic = {};

            (function () {

                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.ID += '_nodesel';

                cswPrivate.table = cswParent.table();
                cswPrivate.select = cswPrivate.table.cell(1, 1).select(cswPrivate);

                cswPublic = Csw.dom({}, cswPrivate.select);

                cswPublic.bind('change', function () {
                    cswPrivate.selectedNodeId = cswPublic.val();
                    Csw.tryExec(cswPrivate.onChange, cswPublic);
                    Csw.tryExec(cswPrivate.onSelect, cswPublic.val());
                });

                cswPublic.selectedNodeId = function () {
                    return cswPrivate.selectedNodeId || cswPublic.val();
                };

                Csw.ajax.post({
                    urlMethod: cswPrivate.nodesUrlMethod,
                    async: Csw.bool(cswPrivate.async),
                    data: {
                        NodeTypeId: Csw.string(cswPrivate.nodeTypeId),
                        ObjectClassId: Csw.string(cswPrivate.objectClassId),
                        ObjectClass: Csw.string(cswPrivate.objectClassName),
                        RelatedToObjectClass: Csw.string(cswPrivate.relatedTo.objectClassName),
                        RelatedToNodeId: Csw.string(cswPrivate.relatedTo.nodeId)
                    },
                    success: function (data) {
                        var ret = data;
                        var canAdd = Csw.bool(cswPrivate.canAdd) && Csw.bool(ret.canadd);
                        var useSearch = Csw.bool(ret.usesearch);
                        var cellCol = 2;
                        cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || ret.nodetypeid;
                        cswPrivate.objectClassId = cswPrivate.objectClassId || ret.objectclassid;
                        cswPrivate.relatedTo.objectClassId = cswPrivate.relatedTo.objectClassId || ret.relatedobjectclassid;
                        delete ret.canadd;
                        delete ret.usesearch;
                        delete ret.nodetypeid;
                        delete ret.objectclassid;
                        delete ret.relatedobjectclassid;

                        var nodecount = 0;
                        //Case 24155
                        Csw.each(ret, function (nodeName, nodeId) {
                            nodecount += 1;
                            if (false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                                nodeId === cswPrivate.selectedNodeId) {
                                cswPublic.option({ value: nodeId, display: nodeName, isSelected: true });
                            } else {
                                cswPublic.option({ value: nodeId, display: nodeName });
                            }
                        });

                        Csw.tryExec(cswPrivate.onSuccess, ret);
                        cswPublic.css('width', Csw.string(cswPrivate.width));

                        if (useSearch) {
                            cswPrivate.select.hide();
                            var nameSpan = cswPrivate.table.cell(1, cellCol).span({
                                ID: Csw.makeId(cswPrivate.ID, '', 'selectedname'),
                                text: cswPublic.selectedText()
                            });

                            var hiddenValue = cswPrivate.table.cell(1, cellCol).input({
                                ID: Csw.makeId(cswPrivate.ID, '', 'hiddenvalue'),
                                type: Csw.enums.inputTypes.hidden,
                                value: cswPublic.selectedNodeId()
                            });
                            cellCol += 1;

                            cswPrivate.table.cell(1, cellCol)
                                .div()
                                .buttonExt({
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.search),
                                    size: 'small',
                                    tooltip: { title: 'Search' },
                                    disableOnClick: false,
                                    onClick: function () {
                                        $.CswDialog('SearchDialog', {
                                            nodetypeid: cswPrivate.nodeTypeId,
                                            objectclassid: cswPrivate.objectClassId,
                                            onSelectNode: function (nodeObj) {
                                                nameSpan.text(nodeObj.nodename);
                                                hiddenValue.val(nodeObj.nodeid);
                                            }
                                        });
                                    }
                                });

                            cellCol += 1;

                            cswPrivate.table.$.hover(function (event) { Csw.nodeHoverIn(event, hiddenValue.val()); }, Csw.nodeHoverOut);

                        }
                        if (canAdd) {
                            var openAddNodeDialog = function (nodetypeToAdd) {
                                $.CswDialog('AddNodeDialog', {
                                    nodetypeid: nodetypeToAdd,
                                    relatednodeid: cswPrivate.relatedTo.nodeId,
                                    relatedobjectclassid: cswPrivate.relatedTo.objectClassId,
                                    onAddNode: function (nodeid, nodekey, nodename) {
                                        cswPublic.option({ value: nodeid, display: nodename });
                                        cswPublic.val(nodeid);
                                        cswPrivate.select.trigger('change');
                                    }
                                });
                            };

                            var getNodeTypeOptions = function () {
                                var blankText = '[Select One]';
                                cswPrivate.selectedNodeType = cswPrivate.table.cell(1, cellCol)
                                    .nodeTypeSelect({
                                        objectClassId: cswPrivate.objectClassId,
                                        onSelect: function (data, nodeTypeCount) {
                                            if (blankText !== cswPrivate.selectedNodeType.val()) {
                                                cswPrivate.nodeTypeId = cswPrivate.selectedNodeType.val();
                                                openAddNodeDialog(cswPrivate.nodeTypeId);
                                            }
                                        },
                                        onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                            if (Csw.number(nodeTypeCount) > 1) {
                                                cswPrivate.selectedNodeType.show();
                                                cswPrivate.addBtn.hide();
                                            } else {
                                                cswPrivate.addBtn.show();
                                            }
                                            if (nodeTypeCount === 1 && false === Csw.isNullOrEmpty(lastNodeTypeId)) {
                                                openAddNodeDialog(lastNodeTypeId);
                                            }
                                        },
                                        blankOptionText: blankText,
                                        filterToPermission: 'Create'
                                    })
                                    .hide();
                                cellCol += 1;
                            };

                            cswPrivate.addBtn = cswPrivate.table.cell(1, cellCol)
                                .div()
                                .buttonExt({
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                                    size: 'small',
                                    tooltip: { title: 'Add' },
                                    disableOnClick: false,
                                    onClick: function () {
                                        if (false === Csw.isNullOrEmpty(cswPrivate.nodeTypeId)) {
                                            openAddNodeDialog(cswPrivate.nodeTypeId);
                                        }
                                        else {
                                            getNodeTypeOptions();
                                        }
                                    }
                                });
                            cellCol += 1;
                        }
                        cswPrivate.selectedNodeId = cswPublic.val();
                    }
                });

            } ());

            return cswPublic;
        });
} ());

