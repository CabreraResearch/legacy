/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('editviewrelationship', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.relationshipNode = cswPrivate.relationshipNode || {};
            cswPrivate.view = cswPrivate.view || {};
            cswPrivate.onBeforeRelationshipEdit = cswPrivate.onBeforeRelationshipEdit || function () { };
            cswPrivate.onRelationshiEdit = cswPrivate.onRelationshiEdit || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
            cswPrivate.properties = cswPrivate.properties || [];
            cswPrivate.relationships = cswPrivate.relationships || [];
            cswPrivate.stepName = cswPrivate.stepName || 'FineTuning';
            cswPrivate.findViewNodeByArbId = cswPrivate.findViewNodeByArbId || function () { };
        }());

        cswPrivate.editViewRelationshipDialog = (function () {
            'use strict';

            var editViewRelationshipDialog = Csw.layouts.dialog({
                title: cswPrivate.relationshipNode.TextLabel,
                width: 800,
                height: 350,
                onOpen: function () {
                    var div = editViewRelationshipDialog.div;
                    var tbl = div.table({
                        cellpadding: 3,
                        cellspacing: 3
                    });

                    var allowAddInput = tbl.cell(1, 1).input({
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: cswPrivate.relationshipNode.AllowAdd,
                        onChange: function () { }
                    });
                    tbl.cell(1, 2).text('Allow Add');

                    var allowViewInput = tbl.cell(2, 1).input({
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: cswPrivate.relationshipNode.AllowView,
                        onChange: function () { }
                    });
                    tbl.cell(2, 2).text('Allow View');

                    var allowEditInput = tbl.cell(3, 1).input({
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: cswPrivate.relationshipNode.AllowEdit,
                        onChange: function () { }
                    });
                    tbl.cell(3, 2).text('Allow Edit');

                    var allowDeleteInput = tbl.cell(4, 1).input({
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: cswPrivate.relationshipNode.AllowDelete,
                        onChange: function () { }
                    });
                    tbl.cell(4, 2).text('Allow Delete');

                    if ('Tree' === cswPrivate.view.ViewMode) {
                        var showInTreeInput = tbl.cell(5, 1).input({
                            type: Csw.enums.inputTypes.checkbox,
                            canCheck: true,
                            checked: cswPrivate.relationshipNode.ShowInTree,
                            onChange: function () {
                            }
                        });
                        tbl.cell(5, 2).text('Show In Tree');
                    }

                    var propOps = [];
                    var groupByOpts = [];
                    propOps.push({ value: 'Select...', display: 'Select...', selected: true });

                    groupByOpts.push({
                        value: 'None',
                        display: 'None',
                        isSelected: Csw.isNullOrEmpty(cswPrivate.relationshipNode.GroupByPropName) && Csw.isNullOrEmpty(cswPrivate.view.GridGroupByCol)
                    });
                    Csw.iterate(cswPrivate.properties, function (prop) {
                        var groupByOpt = {
                            value: prop.ArbitraryId,
                            display: prop.TextLabel,
                            isSelected: (prop.TextLabel === cswPrivate.relationshipNode.GroupByPropName) || (prop.TextLabel.toLowerCase() === cswPrivate.view.GridGroupByCol.toLowerCase())
                        };
                        groupByOpts.push(groupByOpt);

                        var propOpt = {
                            value: prop.ArbitraryId,
                            display: prop.TextLabel
                        };
                        var foundNode = cswPrivate.findViewNodeByArbId(prop.ArbitraryId);
                        if (null === foundNode) {
                            propOps.push(propOpt);
                        }
                    });

                    var selectsTbl = div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });

                    var groupBySelect;
                    if ('Tree' === cswPrivate.view.ViewMode || 'Grid' === cswPrivate.view.ViewMode) {
                        selectsTbl.cell(1, 1).text('Group By');
                        groupBySelect = selectsTbl.cell(1, 2).select({
                            name: 'vieweditor_advancededitrelationship_groupbyselect',
                            values: groupByOpts,
                            onChange: function () { }
                        });
                    }

                    selectsTbl.cell(2, 1).text('Add Property');
                    var propertySelect = selectsTbl.cell(2, 2).select({
                        name: 'vieweditor_advancededitrelationship_propselect',
                        values: propOps,
                        onChange: function () {
                            Csw.tryExec(cswPrivate.onBeforeRelationshipEdit);
                            var selectedProp = null;
                            Csw.iterate(cswPrivate.properties, function (prop) {
                                if (prop.ArbitraryId === propertySelect.selectedVal()) {
                                    selectedProp = prop;
                                }
                            });
                            Csw.ajaxWcf.post({
                                urlMethod: 'ViewEditor/HandleAction',
                                data: {
                                    Action: 'AddProp',
                                    StepName: cswPrivate.stepName,
                                    Relationship: cswPrivate.relationshipNode,
                                    Property: selectedProp,
                                    CurrentView: cswPrivate.view
                                },
                                success: function (response) {
                                    cswPrivate.view = response.CurrentView;
                                    Csw.tryExec(cswPrivate.onRelationshipEdit, cswPrivate.view);
                                    editViewRelationshipDialog.close();
                                }
                            });
                        }
                    });

                    var relOpts = [];
                    relOpts.push({ value: 'Select...', display: 'Select...', selected: true });
                    Csw.iterate(cswPrivate.relationships, function (relationship) {
                        var foundNode = cswPrivate.findViewNodeByArbId(relationship.ArbitraryId);
                        if (null === foundNode) {
                            relOpts.push({
                                value: relationship.UniqueId,
                                display: relationship.TextLabel
                            });
                        }
                    });
                    selectsTbl.cell(3, 1).text('Add Relationship');
                    var relationshipSelect = selectsTbl.cell(3, 2).select({
                        name: 'vieweditor_advancededitrelationship_propselect',
                        values: relOpts,
                        onChange: function () {
                            Csw.tryExec(cswPrivate.onBeforeRelationshipEdit);
                            var selectedRelationship = null;
                            Csw.iterate(cswPrivate.relationships, function (relationship) {
                                if (relationship.UniqueId === relationshipSelect.selectedVal()) {
                                    selectedRelationship = relationship;
                                }
                            });
                            Csw.ajaxWcf.post({
                                urlMethod: 'ViewEditor/HandleAction',
                                data: {
                                    CurrentView: cswPrivate.view,
                                    Relationship: selectedRelationship,
                                    ArbitraryId: cswPrivate.relationshipNode.ArbitraryId,
                                    StepName: cswPrivate.stepName,
                                    Action: 'AddRelationship'
                                },
                                success: function (response) {
                                    cswPrivate.view = response.CurrentView;
                                    Csw.tryExec(cswPrivate.onRelationshipEdit, cswPrivate.view);
                                    editViewRelationshipDialog.close();
                                }
                            });
                        }
                    });

                    var btnsTbl = div.table({
                        cellspacing: 5,
                        cellpadding: 5
                    });

                    btnsTbl.cell(1, 1).button({
                        enabledText: 'Apply',
                        onClick: function () {
                            Csw.tryExec(cswPrivate.onBeforeRelationshipEdit);
                            if (groupBySelect) {
                                var selectedRelArbId = groupBySelect.selectedVal();
                                var selectedProp = null;
                                Csw.each(cswPrivate.properties, function (prop) {
                                    if (prop.ArbitraryId === selectedRelArbId) {
                                        selectedProp = prop;
                                    }
                                });
                            }
                            cswPrivate.findRel(cswPrivate.relationshipNode.ArbitraryId, function (relToUpdate) {
                                relToUpdate.AllowAdd = allowAddInput.checked();
                                relToUpdate.AllowView = allowViewInput.checked();
                                relToUpdate.AllowEdit = allowEditInput.checked();
                                relToUpdate.AllowDelete = allowDeleteInput.checked();
                                if ('Tree' == cswPrivate.view.ViewMode) {
                                    relToUpdate.ShowInTree = showInTreeInput.checked();
                                    if ('None' === selectedRelArbId) {
                                        relToUpdate.GroupByPropName = '';
                                        relToUpdate.GroupByPropId = Csw.int32MinVal;
                                        relToUpdate.GroupByPropType = '';
                                    } else {
                                        relToUpdate.GroupByPropName = selectedProp.TextLabel;
                                        relToUpdate.GroupByPropId = (selectedProp.Type === 'NodeTypePropId' ? selectedProp.NodeTypePropId : selectedProp.ObjectClassPropId);
                                        relToUpdate.GroupByPropType = selectedProp.Type;
                                    }
                                    Csw.tryExec(cswPrivate.onRelationshipEdit, cswPrivate.view);
                                } else if ('Grid' === cswPrivate.view.ViewMode) {
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'ViewEditor/HandleAction',
                                        data: {
                                            Action: 'UpdateView',
                                            StepName: cswPrivate.stepName,
                                            CurrentView: cswPrivate.view,
                                            Property: selectedProp
                                        },
                                        success: function (response) {
                                            cswPrivate.view = response.CurrentView;
                                            Csw.tryExec(cswPrivate.onRelationshipEdit, cswPrivate.view);
                                        }
                                    });
                                }
                            });
                            editViewRelationshipDialog.close();
                        }
                    });

                    cswPrivate.findRel = function (arbId, onFind) {
                        var recurse = function (relationship) {
                            Csw.each(relationship.ChildRelationships, function (childRel) {
                                if (arbId === childRel.ArbitraryId) {
                                    Csw.tryExec(onFind, childRel);
                                } else {
                                    recurse(childRel);
                                }
                            });
                        };
                        recurse(cswPrivate.view.Root);
                    };

                    btnsTbl.cell(1, 2).button({
                        enabledText: 'Cancel',
                        onClick: function () {
                            editViewRelationshipDialog.close();
                        }
                    });
                }
            });

            return editViewRelationshipDialog;
        }());

        (function _postCtor() {
            cswPrivate.editViewRelationshipDialog.open();
        }());

        return cswPublic;
    });
}());