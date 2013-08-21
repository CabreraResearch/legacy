/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    Csw.nbt.propertyOption = Csw.nbt.register('propertyOption',
        function(cswPrivate, cswParent) {
            /// <summary>Extends an Object with properties specific to NBT FieldTypes (for the purpose of Intellisense)</summary>
            /// <returns type="Csw.nbt.propertyOption">An Object represent a CswNbtNodeProp</returns> 
            'use strict';

            var cswPublic = {
                name: '',
                isMulti: function() {
                },
                tabid: '',
                identityTabId: '',
                tabState: {
                    nodeid: '',
                    nodename: '',
                    EditMode: Csw.enums.editMode.Edit,
                    ReadOnly: false,
                    Config: false,
                    showSaveButton: true,
                    relatednodeid: '',
                    relatednodename: '',
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

                cswPublic.propData.wasmodified = false;

                cswPublic.eventName = 'onChange' + cswPublic.fieldtype + '_' + cswPublic.propid;
                cswPublic.onPropChangeBroadcast = function(callBack) {
                    Csw.properties.subscribe(cswPublic.eventName, function(eventObj, val) {
                        callBack(val);
                    });
                };

                cswPublic.broadcastPropChange = function(val) {
                    //Csw.clientChanges.setChanged();
                    cswPublic.propData.wasmodified = true;
                    cswPublic.onChange();
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
                    Csw.unsubscribe('render_' + cswPublic.tabState.nodeid + '_' + cswPublic.tabid, null, cswPrivate.renderer);
                    Csw.unsubscribe('initPropertyTearDown', null, cswPrivate.tearDown);
                    Csw.unsubscribe('initPropertyTearDown_' + cswPublic.tabState.nodeid, null, cswPrivate.tearDown);

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
                Csw.subscribe('render_' + cswPublic.tabState.nodeid + '_' + cswPublic.tabid, cswPrivate.renderer);
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


