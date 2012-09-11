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
            'use strict';
            var wasModified = false;
            if (false === Csw.isNullOrEmpty(propVals)) {
                Csw.crawlObject(propVals, function (prop, key) {
                    if (Csw.contains(attributes, key)) {
                        var attr = attributes[key];
                        //don't bother sending this to server unless it's changed
                        if (Csw.isPlainObject(attr)) {
                            wasModified = cswInternal.preparePropJsonForSaveRecursive(isMulti, propVals[key], attr) || wasModified;
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
            'use strict';
            var wasModified = false;
            if (false === Csw.isNullOrEmpty(propData)) {
                if (Csw.contains(propData, 'values')) {
                    var propVals = propData.values;
                    wasModified = cswInternal.preparePropJsonForSaveRecursive(isMulti, propVals, attributes);
                }
                propData.wasmodified = propData.wasmodified || wasModified;
            }
        }
    };

    Csw.nbt.propertyOption = Csw.nbt.propertyOption ||
        Csw.nbt.register('propertyOption',
            Csw.method(function (cswPrivate, cswParent) {
                /// <summary>Extends an Object with properties specific to NBT FieldTypes (for the purpose of Intellisense)</summary>
                /// <returns type="Csw.nbt.propertyOption">An Object represent a CswNbtNodeProp</returns> 
                'use strict';
                
                var cswPublic = {
                    id: '', //propId
                    name: '', //propName
                    nodeid: '',
                    fieldtype: '',
                    propDiv: cswParent,
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
                    Csw.error.throwException('Cannot create a Csw propertyOption without an object to define the property control.', 'propertyOption', 'csw.propertyOption.js', 86);
                }

                Csw.extend(cswPublic, cswPrivate);
                cswPublic.onPropChange = function(attributes) {
                	/// <summary>
                	/// Update cswPublic.data as the DOM changes. Each propType is responsible for implementing a call to this method for each relevant subfield.
                	/// </summary>
                    'use strict';
                    attributes = attributes || {};
                    cswInternal.preparePropJsonForSave(cswPublic.Multi, cswPublic.propData, attributes);
                };

                cswPublic.bindRender = function (callBack) {
                	/// <summary>
                	/// Subscribe to the render and teardown events
                	/// </summary>

                    'use strict';
                    cswPrivate.tearDown = function () {
                        /// <summary>
                        /// Unbind all properties on this node's layout from the 
                        /// </summary>
                        'use strict';
                        Csw.unsubscribe('render_' + cswPublic.nodeid, cswPrivate.renderer);
                        Csw.unsubscribe('initPropertyTearDown', cswPrivate.tearDown);
                        Csw.unsubscribe('initPropertyTearDown_' + cswPublic.nodeid, cswPrivate.tearDown);
                        Csw.tryExec(cswPrivate.tearDownCallback);
                    };
                    
                    cswPrivate.renderer = function () {
                    	/// <summary>
                    	/// Execute the render callback method on publish
                    	/// </summary>
                        'use strict';
                        cswPublic.propDiv.empty();
                        Csw.tryExec(callBack, cswPublic);
                        Csw.subscribe('initPropertyTearDown', cswPrivate.tearDown);
                        Csw.subscribe('initPropertyTearDown_' + cswPublic.nodeid, cswPrivate.tearDown);
                    };
                    Csw.subscribe('render_' + cswPublic.nodeid, cswPrivate.renderer);
                };

                if(false === Csw.isNullOrEmpty(cswPublic.propDiv)) {
                    cswPublic.propDiv.propNonDom({
                        nodeid: cswPublic.nodeid,
                        propid: cswPublic.propid,
                        cswnbtnodekey: cswPublic.cswnbtnodekey
                    });
                }

                cswPublic.unBindRender = function(callback) {
                	/// <summary>
                	/// This is where you would define a callback to assign to the tearDown events
                	/// </summary>
                    cswPrivate.tearDownCallback = callback;
                };

                return cswPublic;
            }));


} ());


