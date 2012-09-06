/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    var cswInternal = {
        preparePropJsonForSaveRecursive: function (isMulti, propVals, attributes) {
            ///<summary>Recurses over the subfields and sub-subfields of a property to update its JSON.</summary>
            ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
            ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
            ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
            ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
            var wasModified = false;
            if (false === Csw.isNullOrEmpty(propVals)) {
                Csw.crawlObject(propVals, function (prop, key) {
                    if (Csw.contains(attributes, key)) {
                        var attr = attributes[key];
                        //don't bother sending this to server unless it's changed
                        if (Csw.isPlainObject(attr)) {
                            wasModified = preparePropJsonForSaveRecursive(isMulti, propVals[key], attr) || wasModified;
                        } else if ((false === isMulti && propVals[key] !== attr) ||
                            (isMulti && false === Csw.isNullOrUndefined(attr) && attr !== Csw.enums.multiEditDefaultValue)) {
                            wasModified = true;
                            propVals[key] = attr;
                        }
                    }
                }, false);
            }
            return wasModified;
        },
        preparePropJsonForSave: function (isMulti, propData, attributes) {
            ///<summary>Takes property JSON from the form and modifies it in order to send back to the server.</summary>
            ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
            ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
            ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
            ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
            var wasModified = false;
            if (false === Csw.isNullOrEmpty(propData)) {
                if (Csw.contains(propData, 'values')) {
                    var propVals = propData.values;
                    wasModified = preparePropJsonForSaveRecursive(isMulti, propVals, attributes);
                }
                propData.wasmodified = propData.wasmodified || wasModified;
            }
        }
    };

    Csw.nbt.propertyOption = Csw.nbt.propertyOption ||
        Csw.nbt.register('propertyOption',
            Csw.method(function (cswPrivate) {
                /// <summary>Extends an Object with properties specific to NBT FieldTypes (for the purpose of Intellisense)</summary>
                /// <returns type="Csw.nbt.propertyOption">An Object represent a CswNbtNodeProp</returns> 
                'use strict';
                
                var cswPublic = {
                    id: '', //propId
                    name: '', //propName
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
                cswPublic.onPropChange = function(attributes) {
                    attributes = attributes || { };
                    cswInternal.preparePropJsonForSave(cswPublic.Multi, cswPublic.propData, attributes);
                };

                cswPublic.render = function (callBack) {
                    var renderer = function() {
                        Csw.tryExec(callBack, cswPublic);
                        Csw.unsubscribe('render_' + cswPublic.nodeid, renderer);
                    };
                    Csw.subscribe('render_' + cswPublic.nodeid, renderer);
                };

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


