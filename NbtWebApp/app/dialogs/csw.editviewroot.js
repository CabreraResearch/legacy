/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('editviewroot', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.view = cswPrivate.view || {};
            cswPrivate.relationships = cswPrivate.relationships || [];
            cswPrivate.onBeforeRelationshipAdd = cswPrivate.onBeforeRelationshipAdd || function () { };
            cswPrivate.onAddRelationship = cswPrivate.onAddRelationship || function () { };
            cswPrivate.findViewNodeByArbId = cswPrivate.findViewNodeByArbId || function () { };
        }());

        cswPrivate.editViewRootDialog = (function () {
            'use strict';

            var editViewRootDialog = Csw.layouts.dialog({
                title: 'Add to Root',
                width: 700,
                height: 160,
                onOpen: function () {
                    var div = editViewRootDialog.div;
                    var tbl = div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });

                    tbl.cell(1, 1).text('Add Relationship');

                    var relationshipOpts = [{ value: 'Select...', display: 'Select...', selected: true }];
                    Csw.iterate(cswPrivate.relationships, function (rel) {
                        var foundNode = cswPrivate.findViewNodeByArbId(rel.ArbitraryId);
                        if (null === foundNode) {
                            relationshipOpts.push({
                                value: rel.UniqueId,
                                display: rel.TextLabel
                            });
                        }
                    });
                    var relSelect = tbl.cell(1, 2).select({
                        name: 'vieweditor_root_addrelselect',
                        values: relationshipOpts,
                        onChange: function () {
                            Csw.tryExec(cswPrivate.onBeforeRelationshipAdd);
                            var selectedRel = null;
                            Csw.iterate(cswPrivate.relationships, function (rel) {
                                if (rel.UniqueId == relSelect.selectedVal()) {
                                    selectedRel = rel;
                                }
                            });

                            cswPrivate.view.Root.ChildRelationships.push(selectedRel);
                            Csw.tryExec(cswPrivate.onAddRelationship, cswPrivate.view);
                            editViewRootDialog.close();
                        }
                    });
                }
            });

            return editViewRootDialog;
        }());

        (function _postCtor() {
            cswPrivate.editViewRootDialog.open();
        }());

        return cswPublic;
    });
}());