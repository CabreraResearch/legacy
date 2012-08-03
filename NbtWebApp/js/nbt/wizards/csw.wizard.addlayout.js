/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.nbt.wizard.addLayout = Csw.nbt.wizard.addLayout ||
        Csw.nbt.wizard.register('addLayout', function (cswParent, options) {
            'use strict';
            ///<summary>Creates a tabsandprops Add layout for a nodetype.</summary>

            var cswPrivate = {
                ID: 'wizardTabsAndPropsAddLayout',
                nodetypeid: '',
                showSaveButton: false,
                EditMode: Csw.enums.editMode.Add,
                propertyData: {},
                ReloadTabOnSave: false,
                ShowAsReport: false,
                excludeOcProps: []
            };
            if (options) Csw.extend(cswPrivate, options);

            var cswPublic = {};

            (function _pre() {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard addLayout without a parent.', '', 'csw.wizard.addlayout.js', 25));
                }
                if (Csw.isNullOrEmpty(cswPrivate.nodetypeid)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard addLayout without a NodeType ID.', '', 'csw.wizard.addlayout.js', 28));
                }
                var div = cswParent.div({width: '80%'});
                cswPublic = Csw.layouts.tabsAndProps(div, cswPrivate);

            } ());

            (function _post() {

            } ());

            return cswPublic;

        });
} ());

