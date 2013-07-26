/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.list = Csw.properties.register('list',
        function (nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function () {
                'use strict';
                var cswPrivate = Csw.object();
                var comboBoxDefaultWidth = 150;

                cswPrivate.text = nodeProperty.propData.values.text;
                cswPrivate.value = nodeProperty.propData.values.value;
                cswPrivate.options = nodeProperty.propData.values.options;
                cswPrivate.propid = nodeProperty.propData.id;
                cswPrivate.isRequired = nodeProperty.propData.required;
                cswPrivate.wasModified = nodeProperty.propData.wasmodified;
                //cswPrivate.fieldtype = nodeProperty.propData.fieldtype;

                cswPrivate.table = nodeProperty.propDiv.table({ TableCssClass: 'cswInline' });
                cswPrivate.selectCell = cswPrivate.table.cell(1, 1);
                cswPrivate.validateCell = cswPrivate.table.cell(1, 2);

                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.value !== val) {
                        cswPrivate.value = val;
                        if (cswPrivate.select) {
                            cswPrivate.select.setValue(val);
                        }
                        if (span) {
                            span.remove();
                            span = cswPrivate.selectCell.span({ text: cswPrivate.text });
                        }
                    }
                });//nodeProperty.onPropChangeBroadcast()

                var setComboBoxSize = function (optionsArray) {
                    //Set the width of the combobox to match the longest string returned
                    var longestOption = optionsArray.sort(function (a, b) { return b.Text.length - a.Text.length; })[0];
                    var newWidth = (longestOption.Text.length * 7) + 8;
                    if (newWidth > comboBoxDefaultWidth) {
                        cswPrivate.select.setWidth(newWidth);
                    }
                };//setComboBoxSize()
                
                var setComboBoxValidityColor = function (isValid) {
                    if (isValid) {
                        cswPrivate.select.setFieldStyle("background:none #66ff66;");
                    } else {
                        cswPrivate.select.setFieldStyle("background:none #ff6666;");
                    }
                };//setComboBoxValidityColor()

                if (nodeProperty.isReadOnly()) {
                    var span = cswPrivate.selectCell.span({ text: cswPrivate.text });
                } else {
                    // Create the Store
                    cswPrivate.listOptionsStore = new Ext.data.Store({
                        fields: ['Text', 'Value'],
                        autoLoad: false
                    });

                    // Create the Ext JS ComboBox
                    cswPrivate.select = Ext.create('Ext.form.field.ComboBox', {
                        name: nodeProperty.name,
                        renderTo: cswPrivate.selectCell.div().getId(),
                        displayField: 'Text',
                        store: cswPrivate.listOptionsStore,
                        queryMode: 'local',
                        queryDelay: 2000,
                        value: cswPrivate.text || cswPrivate.value,
                        listConfig: {
                            width: 'auto'
                        },
                        listeners: {
                            select: function (combo, records, eOpts) {
                                var text = records[0].get('Text');
                                cswPrivate.text = text;
                                nodeProperty.propData.values.text = text;

                                var val = records[0].get('Value');
                                cswPrivate.value = val;
                                nodeProperty.propData.values.value = val;

                                if (cswPrivate.isRequired) {
                                    if (Csw.isNullOrEmpty(val)) {
                                        cswPrivate.checkBox.val(false);
                                    } else {
                                        cswPrivate.checkBox.val(true);
                                    }
                                    var valid = cswPrivate.checkBox.$.valid();
                                    setComboBoxValidityColor(valid);
                                }

                                nodeProperty.broadcastPropChange(text);
                            },
                            change: function(combo, newvalue, oldvalue) {
                                if (cswPrivate.isRequired) {
                                    if (Csw.isNullOrEmpty(newvalue)) {
                                        cswPrivate.checkBox.val(false);
                                    } else {
                                        cswPrivate.checkBox.val(true);
                                    }
                                    var valid = cswPrivate.checkBox.$.valid();
                                    setComboBoxValidityColor(valid);
                                }
                            }
                        },//listeners
                        tpl: new Ext.XTemplate('<tpl for=".">' + '<li style="height:22px;" class="x-boundlist-item" role="option">' + '{Text}' + '</li></tpl>'),

                    });

                    // Combobox validation
                    // Note: This is a bit of a hack (ok a full on hack); because there isn't a way to connect
                    // Ext validation with JQuery, we will add a checkbox to the ComboBox, set it's state based on
                    // validity of the ComboBox value and validate the checkbox instead.
                    if (cswPrivate.isRequired) {
                        cswPrivate.checkBox = cswPrivate.validateCell.div().input().css({ 'visibility': 'hidden', 'width': '20px' });
                        cswPrivate.checkBox.required(true);
                        cswPrivate.checkBox.addClass('validateExtComboBox');

                        if (false === Csw.isNullOrEmpty(cswPrivate.select.getValue())) {
                            cswPrivate.checkBox.val(true);
                        } else {
                            cswPrivate.checkBox.val(false);
                        }

                        if (cswPrivate.wasModified) {
                            var valid = cswPrivate.checkBox.$.valid();
                            setComboBoxValidityColor(valid);
                        }

                        $.validator.addMethod('validateExtComboBox', function () {
                            var valid = Csw.bool(cswPrivate.checkBox.val());
                            setComboBoxValidityColor(valid);
                            return valid;
                        }, 'This field is required.');
                    }
                    
                    /*
                     * To search or not to search?
                     *
                     * If the server returns search === true, then the number of options exceeded
                     * the relationshipoptionlimit configuration variable. When the number of
                     * options exceeds this variable, the user is forced to search the options.
                     *
                     */
                    if (nodeProperty.propData.values.search === false) {
                        cswPrivate.listOptionsStore.loadData(cswPrivate.options);
                        setComboBoxSize(cswPrivate.options);
                    } else {
                        // Create a proxy to call the searchListOptions web service method
                        cswPrivate.proxy = new Ext.data.proxy.Ajax({
                            noCache: false,
                            pageParam: undefined,
                            headers: {
                                'Accept': 'application/json',
                                'Content-Type': 'application/json; charset=utf-8'
                            },
                            startParam: undefined,
                            limitParam: undefined,
                            actionMethods: {
                                read: 'POST'
                            },
                            url: 'Services/Search/searchListOptions', //MUST INCLUDE "Services/" or it doesn't work
                            reader: {
                                type: 'json',
                                root: 'Data.Options',
                                getResponseData: function (response) {
                                    // This function allows us to intercept the data before the reader
                                    // reads it so that we can convert it into an array of objects the 
                                    // store will accept.
                                    var json = Ext.decode(response.responseText);

                                    //Set the width of the combobox to match the longest string returned
                                    if (json.Data.Options.length > 0) {
                                        setComboBoxSize(json.Data.Options);
                                    }

                                    return this.readRecords(json);
                                }
                            }
                        });
                        // Set the store's proxy to the one created above
                        cswPrivate.listOptionsStore.setProxy(cswPrivate.proxy);

                        // Add the appropriate listeners for the remotely populated combobox
                        cswPrivate.listOptionsStore.on({
                            beforeload: function (store, operation) {

                                // Clear the store before filling it with new data
                                cswPrivate.listOptionsStore.loadData([], false);

                                //Set the parameter object to be sent
                                var CswNbtSearchRequest = {};
                                CswNbtSearchRequest.NodeTypePropId = cswPrivate.propid;
                                CswNbtSearchRequest.SearchTerm = cswPrivate.select.getValue();

                                operation.params = Csw.serialize(CswNbtSearchRequest);
                            }
                        });

                        // Set the properties on the combobox that are needed for querying remotely
                        cswPrivate.select.queryMode = 'remote';
                        cswPrivate.select.queryParam = false;
                        cswPrivate.select.minChars = 1;
                        cswPrivate.select.triggerAction = 'query';

                    }
                }//if (nodeProperty.isReadOnly())

            };//render()

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });
}());