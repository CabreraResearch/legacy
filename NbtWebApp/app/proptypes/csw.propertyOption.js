/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    //var cswStaticInternalClosure = {
        
    //    preparePropJsonForSave: function (isMulti, propData, attributes) {
    //        ///<summary>Takes property JSON from the form and modifies it in order to send back to the server.</summary>
    //        ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
    //        ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
    //        ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
    //        ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
    //        'use strict';
    //        var wasModified = false;
            
    //        var updateValues = function(originalData) {

    //            Csw.iterate(attributes, function(propVal, propName) {
    //                if (originalData && originalData.hasOwnProperty && originalData.hasOwnProperty(propName)) {
    //                    originalData[propName] = propVal;
    //                    wasModified = true;
    //                } else {
    //                    Csw.iterate(originalData, function(originalVal) {
    //                        if (originalVal) {
    //                            updateValues(originalVal);
    //                        }
    //                    });
    //                }
    //            }, true);
    //        };
    //        updateValues(propData.values);

    //        propData.wasmodified = propData.wasmodified || wasModified;
        
    //    }
    //};

    Csw.nbt.propertyOption = Csw.nbt.register('propertyOption',
        function(cswPrivate, cswParent) {
            /// <summary>Extends an Object with properties specific to NBT FieldTypes (for the purpose of Intellisense)</summary>
            /// <returns type="Csw.nbt.propertyOption">An Object represent a CswNbtNodeProp</returns> 
            'use strict';

            var cswPublic = {
                name: '',
                isMulti: function() {
                },
                tabState: {
                    nodeid: '',
                    nodename: '',
                    EditMode: Csw.enums.editMode.Edit,
                    ReadOnly: false,
                    Config: false,
                    showSaveButton: true,
                    relatednodeid: '',
                    relatednodename: '',
                    relatednodetypeid: '',
                    relatedobjectclassid: '',
                    tabid: '',
                    nodetypeid: ''
                },
                fieldtype: '',
                propDiv: cswParent,
                //saveBtn: {},
                propData: {
                    id: '',
                    name: '',
                    readonly: false,
                    required: false,
                    values: {}
                },
                onChange: function() {
                },
                onReload: function() {
                },    // if a control needs to reload the tab
                onEditView: function() {
                },
                onAfterButtonClick: function() {
                }
            };

            (function _preCtr() {
                if (Csw.isNullOrEmpty(cswPrivate)) {
                    Csw.error.throwException('Cannot create a Csw propertyOption without an object to define the property control.', 'propertyOption', 'csw.propertyOption.js', 86);
                }
                Csw.extend(cswPublic, cswPrivate);
                cswPublic.name = cswPublic.propData.id;
                cswPublic.eventName = 'onChange' + cswPublic.fieldtype + '_' + cswPublic.propid;

                cswPublic.onSyncProps = function(callBack) {
                    Csw.properties.subscribe(cswPublic.eventName, function(eventObj, val) {
                        callBack(val);
                    });
                };

                cswPublic.doSyncProps = function(val) {
                    Csw.properties.publish(cswPublic.eventName, val);
                };

            }());

            cswPublic.isReport = function() {
                return Csw.enums.editMode.PrintReport === cswPublic.tabState.EditMode;
            };

            cswPublic.isDisabled = function() {
                return (cswPublic.isReport() ||
                    Csw.enums.editMode.Preview === cswPublic.tabState.EditMode ||
                    Csw.enums.editMode.AuditHistoryInPopup === cswPublic.tabState.EditMode ||
                    cswPublic.tabState.Config === true
                );
            };

            cswPublic.isReadOnly = function() {
                return Csw.bool(cswPublic.tabState.ReadOnly) ||
                    Csw.bool(cswPublic.tabState.Config) ||
                    cswPublic.isDisabled() ||
                    Csw.bool(cswPublic.propData.readonly);
            };

            cswPublic.canOverride = function() {
                return Csw.bool(cswPublic.propData.canoverride);
            };

            cswPublic.isRequired = function() {
                return Csw.bool(cswPublic.propData.required);
            };

            cswPublic.isMulti = function() {
                return Csw.tryExec(cswPrivate.isMulti);
            };

            //cswPublic.doPropChangeDataBind = function() {
            //    //NOTE - if we don't verify that we're in config mode we'll get an INFINITE LOOP
            //    return (false === cswPublic.isReadOnly() && false === cswPublic.tabState.Config && false === cswPublic.isDisabled());
            //};

            //cswPublic.onPropChange = function(attributes) {
            //    /// <summary>
            //    /// Update cswPublic.data as the DOM changes. Each propType is responsible for implementing a call to this method for each relevant subfield.
            //    /// </summary>
            //    'use strict';
            //    attributes = attributes || {};
            //    cswStaticInternalClosure.preparePropJsonForSave(cswPublic.isMulti(), cswPublic.propData, attributes);
            //    if (cswPublic.doPropChangeDataBind()) {
            //        Csw.publish('onPropChange_' + cswPublic.propid, { tabid: cswPublic.tabState.tabid, propData: cswPublic.propData });
            //    }
            //};

            //cswPrivate.dataBindPropChange = function(eventObj, data) {
            //    if (data.tabid !== cswPublic.tabState.tabid) {
            //        Csw.extend(cswPublic.propData, data.propData, true);
            //        cswPrivate.renderThisProp();
            //    }
            //};

            //if (cswPublic.doPropChangeDataBind()) {
            //    Csw.subscribe('onPropChange_' + cswPublic.propid, cswPrivate.dataBindPropChange);
            //}

            cswPublic.bindRender = function(callBack) {
                /// <summary>
                /// Subscribe to the render and teardown events
                /// </summary>

                'use strict';
                cswPrivate.tearDown = function() {
                    /// <summary>
                    /// Unbind all properties on this node's layout from the 
                    /// </summary>
                    'use strict';
                    Csw.unsubscribe('render_' + cswPublic.tabState.nodeid + '_' + cswPublic.tabState.tabid, null, cswPrivate.renderer);
                    Csw.unsubscribe('initPropertyTearDown', null, cswPrivate.tearDown);
                    Csw.unsubscribe('initPropertyTearDown_' + cswPublic.tabState.nodeid, null, cswPrivate.tearDown);
                    //Csw.unsubscribe('onPropChange_' + cswPublic.propid, null, cswPrivate.dataBindPropChange);

                    Csw.properties.unsubscribe(cswPublic.eventName);

                    Csw.tryExec(cswPrivate.tearDownCallback);
                };

                cswPrivate.renderThisProp = (function() {
                    'use strict';
                    var renderMe = function() {
                        cswPublic.propDiv.empty();
                        Csw.tryExec(callBack, cswPublic);

                        // case 29095
                        if (cswPublic.isReadOnly() && cswPublic.canOverride()) {
                            cswPublic.propDiv.buttonExt({
                                name: 'override',
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.lock),
                                tooltip: {
                                    title: 'Administrative Override'
                                },
                                onClick: function() {
                                    cswPublic.propData.readonly = false;
                                    renderMe();
                                }
                            }); // buttonExt()
                        } // if (cswPublic.isReadOnly() && cswPublic.canOverride()) {
                    };
                    return renderMe;
                }());

                cswPrivate.renderer = function() {
                    /// <summary>
                    /// Execute the render callback method on publish
                    /// </summary>
                    'use strict';
                    cswPrivate.renderThisProp();
                };

                //We only want to subscribe once--not on every possible publish to render
                Csw.subscribe('render_' + cswPublic.tabState.nodeid + '_' + cswPublic.tabState.tabid, cswPrivate.renderer);
                Csw.subscribe('initPropertyTearDown', cswPrivate.tearDown);
                Csw.subscribe('initPropertyTearDown_' + cswPublic.tabState.nodeid, cswPrivate.tearDown);
            };

            if (false === Csw.isNullOrEmpty(cswPublic.propDiv)) {
                cswPublic.propDiv.data({
                    nodeid: cswPublic.tabState.nodeid,
                    propid: cswPublic.propid,
                    nodekey: cswPublic.tabState.nodekey
                });
            }

            cswPublic.unBindRender = function(callback) {
                /// <summary>
                /// This is where you would define a callback to assign to the tearDown events
                /// </summary>
                cswPrivate.tearDownCallback = callback;
            };

            return cswPublic;
        });

} ());


