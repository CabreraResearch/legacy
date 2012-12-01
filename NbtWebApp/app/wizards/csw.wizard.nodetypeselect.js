
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.wizard.nodeTypeSelect = Csw.wizard.nodeTypeSelect ||
        Csw.wizard.register('nodeTypeSelect', function (cswParent, options) {
            'use strict';
            ///<summary>Creates a basic grid with an Add menu.</summary>

            var cswPrivate = {
                name: 'wizardNodeTypeSelect',
                labelText: '',
                objectClassName: '',
                onSelect: null,
                onSuccess: null,
                data: {}
            };
            if (options) Csw.extend(cswPrivate, options);

            var cswPublic = { };

            cswPrivate.onSetVal = function () {
                cswPrivate.data.selectednodetypeid = cswPublic.select.val();
                cswPublic.selectedNodeTypeId = cswPrivate.data.selectednodetypeid;
            };

            (function _pre() {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard Node Type Select without a parent.', '', 'csw.wizard.nodetypeselect.js', 23));
                }

                cswPrivate.outerTbl = cswParent.div().hide().table();
                cswPrivate.outerTbl.cell(1, 1);
                cswPrivate.label = cswPrivate.outerTbl.cell(1, 2).span({ text: cswPrivate.labelText });
                cswPublic.select = cswPrivate.outerTbl.cell(1, 3).nodeTypeSelect({
                    name: cswPrivate.name,
                    useWide: true,
                    objectClassName: cswPrivate.objectClassName,
                    onSelect: function (selectedid, nodetypecount) {
                        cswPrivate.onSetVal();
                        Csw.tryExec(cswPrivate.onSelect, selectedid, nodetypecount);
                    },
                    onSuccess: function (types, nodetypecount, lastnodetypeid) {
                        cswPrivate.data.types = types;
                        cswPrivate.data.nodetypecount = nodetypecount;
                        if (nodetypecount > 1) {
                            cswPrivate.outerTbl.show();
                        }
                        cswPrivate.onSetVal();
                        Csw.tryExec(cswPrivate.onSuccess, cswPrivate.data.types, cswPrivate.data.nodetypecount, lastnodetypeid);
                    }
                });
            } ());


            (function _post() {

            } ());

            return cswPublic;

        });
} ());

