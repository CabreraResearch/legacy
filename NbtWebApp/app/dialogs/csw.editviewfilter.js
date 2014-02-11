/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('editviewfilter', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.filterNode = cswPrivate.filterNode || {};
            cswPrivate.view = cswPrivate.view || {};
            cswPrivate.onBeforeFilterEdit = cswPrivate.onBeforeFilterEdit || function () { };
            cswPrivate.onFilterEdit = cswPrivate.onFilterEdit || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
        }());

        cswPrivate.editViewFilterDialog = (function () {
            'use strict';

            var editViewFilterDialog = Csw.layouts.dialog({
                title: cswPrivate.filterNode.TextLabel,
                width: 600,
                height: 270,
                onOpen: function () {
                    var div = editViewFilterDialog.div;
                    var tbl = div.table({
                        cellpadding: 3,
                        cellspacing: 3
                    });
                    var caseSensitiveInput = tbl.cell(1, 1).input({
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: cswPrivate.filterNode.CaseSensitive,
                        onChange: function () { }
                    });
                    tbl.cell(1, 2).text('Case Sensitive');

                    var showAtRuntimeInput = tbl.cell(2, 1).input({
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: cswPrivate.filterNode.ShowAtRuntime,
                        onChange: function () { }
                    });
                    tbl.cell(2, 2).text('Show at Runtime');

                    var noMatchTbl = div.table({
                        cellspacing: 3,
                        cellpadding: 3
                    });

                    noMatchTbl.cell(1, 1).text('For Non-Matches');
                    var noMatchesSelect = noMatchTbl.cell(1, 2).select({
                        values: ['Hide', 'Disabled'],
                        selected: cswPrivate.filterNode.ResultMode,
                        onChange: function () { }
                    });

                    var btnsTbl = div.table({
                        cellspacing: 5,
                        cellpadding: 5
                    });

                    btnsTbl.cell(1, 1).button({
                        enabledText: 'Apply',
                        onClick: function () {
                            Csw.tryExec(cswPrivate.onBeforeFilterEdit);
                            var findFilter = function (child) {
                                var updated = false;
                                Csw.each(child.Properties, function (prop) {
                                    Csw.each(prop.Filters, function (filter) {
                                        if (filter.ArbitraryId === cswPrivate.filterNode.ArbitraryId) {
                                            filter.ResultMode = noMatchesSelect.selectedText();
                                            filter.ShowAtRuntime = showAtRuntimeInput.checked();
                                            filter.CaseSensitive = caseSensitiveInput.checked();
                                            updated = true;
                                        }
                                    });
                                });
                                if (false === updated) {
                                    Csw.each(child.ChildRelationships, function (childRel) {
                                        findFilter(childRel);
                                    });
                                }
                            };
                            findFilter(cswPrivate.view.Root);
                            Csw.tryExec(cswPrivate.onFilterEdit, cswPrivate.view);

                            div.$.dialog('close');
                        }
                    });

                    btnsTbl.cell(1, 2).button({
                        enabledText: 'Cancel',
                        onClick: function () {
                            editViewFilterDialog.close();
                        }
                    });
                }
            });

            return editViewFilterDialog;
        }());

        (function _postCtor() {
            cswPrivate.editViewFilterDialog.open();
        }());

        return cswPublic;
    });
}());