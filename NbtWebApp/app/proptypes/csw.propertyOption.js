/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.nbt.propertyOption = Csw.nbt.propertyOption ||
        Csw.nbt.register('propertyOption',
            function (cswPrivate) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.controls">The options object with DOM methods attached.</returns> 
                'use strict';
                var cswPublic = {
                    nodeid: '',
                    fieldtype: '',
                    propDiv: {},
                    saveBtn: {},
                    propData: {},
                    onChange: function () {
                    },
                    onReload: function () {
                    },    // if a control needs to reload the tab
                    cswnbtnodekey: '',
                    relatednodeid: '',
                    relatednodename: '',
                    relatednodetypeid: '',
                    relatedobjectclassid: '',
                    ID: '',
                    Required: false,
                    ReadOnly: false,
                    EditMode: Csw.enums.editMode.Edit,
                    Multi: false,
                    onEditView: function () {
                    },
                    onAfterButtonClick: function () {
                    }
                };
                if (Csw.isNullOrEmpty(cswPrivate)) {
                    //Csw.error.throwException('Cannot create a Csw component without a Csw control', '_controls.factory', '_controls.factory.js', 14);
                }

                Csw.extend(cswPublic, cswPrivate);
                return cswPublic;
            });


} ());


