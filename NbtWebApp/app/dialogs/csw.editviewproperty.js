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
            cswPrivate.onFilterAdd = cswPrivate.onFilterAdd || function () { };
        }());

        cswPrivate.editViewPropertyDialog = (function () {
            'use strict';

            var editViewPropDialog = Csw.layouts.dialog({
                title: cswPrivate.propertyNode.TextLabel,
                width: 700,
                height: 160,
                onOpen: function () {
                    var div = editViewPropDialog.div;
                    var tbl = div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });

                    var currentFilter = Csw.nbt.viewPropFilter({
                        name: 'vieweditor_filter_' + cswPrivate.propertyNode.ArbitraryId,
                        parent: tbl,
                        viewJson: cswPrivate.viewJson,
                        proparbitraryid: cswPrivate.propertyNode.ArbitraryId,
                        propname: cswPrivate.propertyNode.PropName,
                        doStringify: false
                    });

                    var btnsTbl = div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });

                    btnsTbl.cell(1, 1).button({
                        enabledText: 'Add Filter',
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
                                    Csw.tryExec(cswPrivate.onFilterAdd, response.CurrentView);
                                    editViewPropDialog.close();
                                }
                            });
                        }
                    });

                    btnsTbl.cell(1, 2).button({
                        enabledText: 'Cancel',
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