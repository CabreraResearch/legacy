/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('editviewproperty', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.propertyNode = cswPrivate.propertyNode || {};
            cswPrivate.view = cswPrivate.view || {};
            cswPrivate.viewJson = cswPrivate.viewJson || '';
            cswPrivate.stepName = cswPrivate.stepName || 'FineTuning';
            cswPrivate.onBeforeFilterAdd = cswPrivate.onBeforeFilterAdd || function () { };
            cswPrivate.onFilterEdit = cswPrivate.onFilterEdit || function () { };
        }());

        cswPrivate.editViewPropertyDialog = (function () {
            'use strict';

            var editViewPropDialog = Csw.layouts.dialog({
                title: 'Edit ' + cswPrivate.propertyNode.TextLabel + ' View Property',
                width: 800,
                height: 460,
                onClose: function() {
                    
                },
                onOpen: function () {
                    var div = editViewPropDialog.div;
                    var attributesTbl = div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });
                    attributesTbl.cell(1, 1).div().input({
                        type: Csw.enums.inputTypes.checkbox,
                        checked: cswPrivate.propertyNode.SortBy,
                        canCheck: true,
                        onChange: function (newVal) {
                            cswPrivate.propertyNode.SortBy = newVal;
                        }
                    });
                    attributesTbl.cell(1, 2).div().text('Sort By');
                    
                    div.br({ number: 2 });
                    
                    var filtersDiv = div.div();

                    var renderFilters = function() {
                        filtersDiv.empty();
                        var filtersTbl = filtersDiv.table({
                            cellspacing: 5,
                            cellpadding: 5
                        });

                        var row = 1;
                        Csw.iterate(cswPrivate.propertyNode.Filters, function (filter) {
                            var deleteBtn = filtersTbl.cell(row, 1).icon({
                                iconType: Csw.enums.iconType.x,
                                isButton: true,
                                onClick: function () {
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'ViewEditor/HandleAction',
                                        data: {
                                            ArbitraryId: filter.ArbitraryId,
                                            CurrentView: cswPrivate.view,
                                            StepName: cswPrivate.stepName,
                                            Action: "RemoveNode"
                                        },
                                        success: function (removeFilterResponse) {
                                            cswPrivate.propertyNode = removeFilterResponse.Step6.PropertyNode;
                                            cswPrivate.view = removeFilterResponse.CurrentView;
                                            Csw.tryExec(cswPrivate.onFilterEdit, cswPrivate.view);
                                            renderFilters();
                                        }
                                    });
                                }
                            });
                            Csw.nbt.viewPropFilter({
                                name: 'vieweditor_filter_' + filter.ArbitraryId,
                                parent: filtersTbl,
                                viewId: cswPrivate.view.ViewId,
                                viewJson: cswPrivate.viewJson,
                                proparbitraryid: filter.ParentArbitraryId,
                                propname: filter.PropName,
                                selectedConjunction: filter.Conjunction,
                                selectedSubFieldName: filter.SubfieldName,
                                selectedFilterMode: filter.FilterMode,
                                selectedValue: filter.Value,
                                doStringify: false,
                                readOnly: true,
                                propRow: row,
                                firstColumn: 2
                            });
                            row++;
                        });

                        var currentFilter = Csw.nbt.viewPropFilter({
                            name: 'vieweditor_filter_' + cswPrivate.propertyNode.ArbitraryId,
                            parent: filtersTbl,
                            viewJson: cswPrivate.viewJson,
                            proparbitraryid: cswPrivate.propertyNode.ArbitraryId,
                            propname: cswPrivate.propertyNode.PropName,
                            doStringify: false,
                            propRow: row,
                            firstColumn: 2
                        });

                        filtersTbl.cell(row, 8).button({
                            enabledText: 'Add New Filter',
                            onClick: function () {
                                var filterData = currentFilter.getFilterJson();
                                var ajaxData = {
                                    CurrentView: cswPrivate.view,
                                    StepName: cswPrivate.stepName,
                                    Action: 'AddFilter',
                                    Property: cswPrivate.propertyNode,
                                    FilterConjunction: filterData.conjunction,
                                    FilterMode: filterData.filter,
                                    FilterValue: filterData.filtervalue,
                                    FilterSubfield: filterData.subfieldname,
                                    PropArbId: filterData.proparbitraryid
                                };

                                Csw.tryExec(cswPrivate.onBeforeFilterAdd);
                                Csw.ajaxWcf.post({
                                    urlMethod: 'ViewEditor/HandleAction',
                                    data: ajaxData,
                                    success: function (response) {
                                        cswPrivate.propertyNode = response.Step6.PropertyNode;
                                        cswPrivate.view = response.CurrentView;
                                        Csw.tryExec(cswPrivate.onFilterEdit, response.CurrentView);
                                        renderFilters();
                                    }
                                });
                            }
                        });
                    };
                    renderFilters();

                    div.br({ number: 2 });
                    div.button({
                        enabledText: 'Close',
                        onClick: function () {
                            editViewPropDialog.close();
                        }
                    });
                }
            });

            return editViewPropDialog;
        }());

        (function _postCtor() {
            cswPrivate.editViewPropertyDialog.open();
        }());

        return cswPublic;
    });
}());