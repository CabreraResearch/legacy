
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.wizard.addLayout = Csw.wizard.addLayout ||
        Csw.wizard.register('addLayout', function (cswParent, options) {
            'use strict';
            ///<summary>Creates a tabsandprops Add layout for a nodetype.</summary>

            var cswPrivate = {
                ID: 'wizardTabsAndPropsAddLayout',
                globalState: {
                    propertyData: {},
                    ShowAsReport: false,
                    excludeOcProps: []
                },
                tabState: {
                    nodetypeid: '',
                    showSaveButton: false,
                    EditMode: Csw.enums.editMode.Add
                },
                ReloadTabOnSave: false
            };
            Csw.extend(cswPrivate, options);

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

