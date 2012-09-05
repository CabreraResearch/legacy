/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.nbt.propertyOption = Csw.nbt.propertyOption ||
        Csw.nbt.register('propertyOption',
            Csw.method(function (cswPrivate) {
                /// <summary>Extends an Object with properties specific to NBT FieldTypes (for the purpose of Intellisense)</summary>
                /// <returns type="Csw.nbt.propertyOption">An Object represent a CswNbtNodeProp</returns> 
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

                if(false === Csw.isNullOrEmpty(cswPublic.propDiv)) {
                    cswPublic.propDiv.propNonDom({
                        nodeid: cswPublic.nodeid,
                        propid: cswPublic.propid,
                        cswnbtnodekey: cswPublic.cswnbtnodekey
                    });
                }

                return cswPublic;
            }));


} ());


