/*global Csw:true,Ext:true*/
(function () {
    'use strict';


    Csw.composites.register('comboBoxExt', function (cswParent, options) {

        //#region Variables
        var cswPrivate = {
            name: 'No Name',
            propid: '', //required to search options on the server
            width: 150, //default width,
            defaultValue: 'Type here to search',
            displayField: 'Text',
            valueField: 'Value',
            selectedField: 'Selected',
            disabledField: 'Disabled',
            queryMode: 'local',
            queryDelay: 2000,
            options: [],
            selectedValue: '',
            listeners: {
                select: null, //function(combo, records){ }
                change: null, //function(combo, records){ }
                storebeforeload: null //function(store, operation) {}
            },
            tpl: '',
            isRequired: false,
            search: false, // whether to use the proxy to search,
            searchurl: '',
            searchProxyMethod: 'POST',
            searchProxyReaderRoot: 'Data'
        };
        var cswPublic = {};

        //#endregion Variables

        //#region Pre-ctor
        (function _pre() {
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            // set the combobox display
            if (Csw.isNullOrEmpty(cswPrivate.tpl)) {
                cswPrivate.tpl = new Ext.XTemplate(
                    '<tpl for=".">' +
                        '<li style="height:22px;" class="x-boundlist-item {disabledItemCls}" role="option">' +
                            '{' + cswPrivate.displayField + '}' +
                        '</li>' +
                    '</tpl>'
                );
            }

            // To search or not to search?
            // If the server returns search === true, then the number of options exceeded
            // the relationshipoptionlimit configuration variable. When the number of
            // options exceeds this variable, the user is forced to search the options.
            if (cswPrivate.search) {
                // Set the properties on the combobox that are 
                // necessary for querying remotely
                cswPrivate.queryMode = 'remote';
                cswPrivate.queryParam = false;
                cswPrivate.minChars = 1;
                cswPrivate.triggerAction = 'query';
            } else {
                cswPrivate.triggerAction = 'all';
            }

            cswPrivate.containerTbl = cswParent.table();
            cswPrivate.comboBoxCell = cswPrivate.containerTbl.cell(1, 1);
            cswPrivate.validateCell = cswPrivate.containerTbl.cell(1, 2);

        }());

        //#endregion Pre-ctor

        //#region Define Class Members

        cswPrivate.makeStore = function () {
            /// <summary>
            /// Makes the data store for the combobox. Sorted by cswPrivate.Text field.
            /// </summary>
            /// <returns type="Ext.data.TreeStore">Ext data store</returns>
            cswPrivate.store = new Ext.data.Store({
                fields: [
                    { name: cswPrivate.valueField },
                    { name: cswPrivate.displayField },
                    { name: cswPrivate.selectedField, hidden: true },
                    { name: cswPrivate.disabledField, hidden: true },
                    { name: "disabledItemCls", hidden: true }
                ],
                autoLoad: false,
                sorters: [{ property: cswPrivate.displayField, direction: 'ASC' }]
            });

            if (false === cswPrivate.search && (cswPrivate.options && cswPrivate.options.length > 1)) {
                cswPrivate.options.forEach(function (option) {
                    if (option["Disabled"] === true) {
                        option["disabledItemCls"] = "x-combo-grayed-out-item";
                    }
                });
                cswPrivate.store.loadData(cswPrivate.options);
            }

            return cswPrivate.store;
        };

        cswPrivate.setDisabledOptsClass = function (optionsArray) {
            optionsArray.forEach(function (option) {
                if (option["Disabled"] === true) {
                    option["disabledItemCls"] = "x-combo-grayed-out-item";
                }
            });

            return optionsArray;
        };

        cswPrivate.makeComboBox = function () {
            /// <summary>
            /// Constructs the ComboBox components and attaches it to the DOM
            /// </summary>
            /// <returns type="Ext.form.field.ComboBox">A ComboBox.</returns>

            var comboBoxOpts = {
                name: cswPrivate.name,
                store: cswPrivate.makeStore(),
                renderTo: cswPrivate.comboBoxCell.div().getId(),
                displayField: cswPrivate.displayField,
                valueField: cswPrivate.valueField,
                queryMode: cswPrivate.queryMode,
                queryDelay: cswPrivate.queryDelay,
                value: cswPrivate.getInitialValue(),
                listConfig: {
                    width: 'auto',
                    loadingText: 'Searching...', // TODO: This doesn't seem to work (we might need to update our ExtJS)
                    emptyText: 'No matches found.',
                },
                listeners: {
                    beforeselect: function (combo, record, index, eOpts) {
                        if (cswPrivate.listeners.beforeselect) {
                            return Csw.tryExec(cswPrivate.listeners.beforeselect, combo, record);
                        } else {
                            return true;
                        }
                    },
                    select: function (combo, records) {
                        if (cswPrivate.isRequired) {
                            var val = records[0].get(cswPrivate.valueField);
                            checkValidity(val);
                        }
                        Csw.tryExec(cswPrivate.listeners.select, combo, records);
                    },
                    change: function (combo, newvalue) {
                        if (null != newvalue) {
                            if (cswPrivate.isRequired) {
                                // Is the newvalue an option in the list?
                                var inlist = false;
                                if (cswPrivate.options) {
                                    cswPrivate.options.forEach(function (option) {
                                        if (option[cswPrivate.valueField] === newvalue) {
                                            inlist = true;
                                        }
                                    });
                                }
                                cswPrivate.checkBox.val(false);
                                if (false === Csw.isNullOrEmpty(newvalue)) {
                                    if (inlist) {
                                        cswPrivate.checkBox.val(true);
                                    }
                                }
                                var valid = cswPrivate.checkBox.$.valid();
                                if (valid) {
                                    Csw.tryExec(cswPrivate.listeners.change, combo, newvalue);
                                }
                                onValidation(valid);
                            } //if(cswPrivate.isRequired)
                        } //if(null != newvalue)

                        //Csw.tryExec(cswPrivate.listeners.change);
                    }
                },
                tpl: cswPrivate.tpl,
                triggerAction: cswPrivate.triggerAction,
                queryParam: cswPrivate.queryParam,
                minChars: cswPrivate.minChars
            };

            // Create the Ext JS ComboBox
            cswPublic.combobox = window.Ext.create('Ext.form.field.ComboBox', comboBoxOpts);

            cswPrivate.setComboBoxSize(cswPrivate.options);

            return cswPublic.combobox;
        };

        cswPrivate.createProxy = function () {
            cswPrivate.proxy = cswPrivate.proxy || new Ext.data.proxy.Ajax({
                noCache: false,
                pageParam: undefined,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json; charset=utf-8'
                },
                startParam: undefined,
                limitParam: undefined,
                actionMethods: {
                    read: cswPrivate.searchProxyMethod
                },
                url: 'Services/' + cswPrivate.searchUrl,
                reader: {
                    type: 'json',
                    root: cswPrivate.searchProxyReaderRoot,
                    getResponseData: function (response) {
                        // This function allows us to intercept the data before the reader
                        // reads it so that we can convert it into an array of objects the 
                        // store will accept.
                        var json = Ext.decode(response.responseText);

                        if (json["Status"]["Success"] === true) {

                            // Set the css class for any disabled options
                            var sanitizedOptions = cswPrivate.setDisabledOptsClass(json[cswPrivate.searchProxyReaderRoot]);
                            json[cswPrivate.searchProxyReaderRoot] = sanitizedOptions;

                            if (json[cswPrivate.searchProxyReaderRoot].length > 0) {
                                cswPrivate.setComboBoxSize(json[cswPrivate.searchProxyReaderRoot]);
                            }

                        } else {
                            if (json["Status"]["Errors"].length > 0) {
                                var errors = json["Status"]["Errors"];
                                errors.forEach(function (error) {
                                    var errorobj = {
                                        'type': error.Type,
                                        'message': error.Message,
                                        'detail': error.Detail,
                                        'display': true
                                    };
                                    Csw.error.showError(errorobj);
                                });
                            }
                        }

                        return this.readRecords(json);
                    }
                }
            });
        };

        cswPrivate.setComboBoxSize = function (optionsArray) {
            /// <summary>
            /// Set the width of the combobox
            /// </summary>
            /// <param name="optionsArray"></param>
            var newWidth = cswPrivate.width;
            if (optionsArray.length > 0) {
                var longestOption = optionsArray.sort(function (a, b) {
                    return b[cswPrivate.displayField].length - a[cswPrivate.displayField].length;
                })[0];
                newWidth = (longestOption[cswPrivate.displayField].length * 7) + 20;
            } else {
                if (false === Csw.isNullOrEmpty(cswPrivate.selectedValue)) {
                    newWidth = (cswPrivate.selectedValue.length * 7) + 20;
                }
            }

            if (newWidth > cswPrivate.width) {
                cswPublic.combobox.setWidth(newWidth);
                cswPrivate.width = newWidth;
            }
        };//setComboBoxSize()

        cswPrivate.getInitialValue = function () {
            if (Csw.isNullOrEmpty(cswPrivate.selectedValue) && cswPrivate.search) {
                return cswPrivate.defaultValue;
            } else {
                return cswPrivate.selectedValue;
            }
        };

        //#region Validation

        var addValidation = function () {
            /// <summary>
            /// TODO: Remove when we implement CIS-52598
            /// Combobox validation 
            /// Note: This is a hack: There isn't a way to connect
            /// ExtJS validation with JQuery so we add a checkbox to the ComboBox, 
            /// set it's state based on validity of the ComboBox value and validate 
            /// the checkbox instead.
            /// </summary>
            /// <param name="isValid"></param>
            var returnObj = Csw.validator(
                cswPrivate.validateCell.div(),
                cswPublic.combobox,
                {
                    wasModified: false,
                    onValidation: onValidation,
                    className: 'validateComboBox_' + cswPrivate.propid,
                    isExtJsControl: true,
                    emptyText: 'Type here to search',
                    hiddenInputName: 'validationCtrl_' + cswPrivate.propid
                }
            );
            cswPrivate.checkBox = returnObj.input;
        };

        var onValidation = function (isValid) {
            if (isValid) {
                cswPublic.combobox.setFieldStyle("background:none #66ff66;");
            } else {
                cswPublic.combobox.setFieldStyle("background:none #ff6666;");
            }
        };//setComboBoxValidityColor()

        var checkValidity = function (value) {
            if (cswPrivate.isRequired) {
                if (Csw.isNullOrEmpty(value) || value === cswPrivate.defaultValue) {
                    cswPrivate.checkBox.val(false);
                } else {
                    cswPrivate.checkBox.val(true);
                }
                var valid = cswPrivate.checkBox.$.valid();
                onValidation(valid);
            }
        };
        //#endregion Validation

        //#endregion Define Class Members

        //#region Getters

        cswPublic.getWidth = function () {
            return cswPrivate.width;
        }

        //#endregion Getters

        //#region Post-ctor

        (function _post() {
            cswPrivate.makeComboBox();

            if (cswPrivate.search) {
                cswPrivate.createProxy();
                cswPrivate.store.setProxy(cswPrivate.proxy);

                cswPrivate.store.on({
                    beforeload: function (store, operation) {
                        // Clear the store before filling it with new data
                        cswPrivate.store.loadData([], false);

                        //Set the parameter object to be sent
                        var obj = {};
                        if (cswPrivate.listeners.storebeforeload) {
                            obj = cswPrivate.listeners.storebeforeload();
                        }

                        operation.params = obj;
                    }
                });
            }

            if (cswPrivate.isRequired) {
                addValidation();
            }//if (cswPrivate.isRequired)

        }());

        //#endregion Post-ctor

        return cswPublic;
    });

}());
