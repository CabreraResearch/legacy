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

                cswPrivate.value = nodeProperty.propData.values.value;
                cswPrivate.text = nodeProperty.propData.values.text;
                cswPrivate.options = nodeProperty.propData.values.options;
                cswPrivate.propid = nodeProperty.propData.id;
                cswPrivate.fieldtype = nodeProperty.propData.fieldtype;

                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.value !== val) {
                        cswPrivate.value = val;
                        if (select) {
                            select.val(val);
                        }
                        if (span) {
                            span.remove();
                            span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                        }
                    }
                });

                if (nodeProperty.isReadOnly()) {
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.text });
                } else {
                    //cswPrivate.values = cswPrivate.options.split(',');

//                    //case 28020 - if a list has a value selected that's not in the list, add it to the options
//                    if (false == Csw.contains(cswPrivate.options,{ value: cswPrivate.value })) {
//                        cswPrivate.options.push({ value: cswPrivate.value });
//                    }
                    
                    var listOptions = [];

                    //// The data store holding the states
                    //var listOptionsStore = Ext.create('Ext.data.Store', {
                    //    data: listOptions,
                    //    fields: ['display', 'value'],
                    //    autoLoad: false
                    //});

                    //// Simple ComboBox using the data store
                    //Ext.create('Ext.form.field.ComboBox', {
                    //    renderTo: nodeProperty.propDiv.div().getId(),
                    //    displayField: 'display',
                    //    labelWidth: 130,
                    //    store: listOptionsStore,
                    //    queryMode: 'local',
                    //    typeAhead: true
                    //});

                    //// The data for all states
                    //var states = [
                    //    { "display": "Matt", "value": "1" },
                    //    { "display": "Colleen", "value": "2" },
                    //    { "display": "Lucas", "value": "3" }
                    //];

                    //if (cswPrivate.options.length > 0) {
                    //    // convert to a set of data that the store will accept
                    //    var optionsArray = cswPrivate.options.split(',');
                    //    optionsArray.forEach(function (option) {
                    //        listOptions.push({ display: option, value: option });
                    //    });
                    //    // load the data
                    //    listOptionsStore.loadData(listOptions);
                    //} else {
                    //    // make the ajax call to get the data after the user has typed in a search query
                    //}

                    cswPrivate.listOptionsStore = new Ext.data.Store({
                        fields: ['display', 'value'],
                        autoLoad: false
                    });

                    cswPrivate.select = Ext.create('Ext.form.field.ComboBox', {
                        name: nodeProperty.name,
                        renderTo: nodeProperty.propDiv.div().getId(),
                        displayField: 'display',
                        store: cswPrivate.listOptionsStore,
                        queryMode: 'local',
                        listeners: {
                            select: function (combo, records, eOpts) {
                                var val = records[0].get('value');
                                cswPrivate.value = val;
                                nodeProperty.propData.values.value = val;
                                nodeProperty.broadcastPropChange(val);
                            }
                        }
                    });
                    
                    if (cswPrivate.options.length > 0) {
                        // convert to a set of data that the store will accept
                        var optionsArray = cswPrivate.options.split(',');
                        optionsArray.forEach(function (option) {
                            listOptions.push({ display: option, value: option });
                        });
                        // load the data
                        cswPrivate.listOptionsStore.loadData(listOptions);
                    } else {
                        // make the ajax call to get the data after the user has typed in a search query
                        
                        // Create a proxy
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
                                root: 'Data.FilteredListOptions',
                                getResponseData: function (response) {
                                    var json = Ext.decode(response.responseText);

                                    var listOptions = [];

                                    var optionsArray = json.Data.FilteredListOptions.split(',');
                                    optionsArray.forEach(function (option) {
                                        listOptions.push({ display: option, value: option });
                                    });

                                    json.Data.FilteredListOptions = listOptions;

                                    return this.readRecords(json);
                                }
                            }
                        });
                        // Set the store's proxy to the one created above
                        cswPrivate.listOptionsStore.setProxy(cswPrivate.proxy);

                        // Add the appropriate listeners for the remotely populated combobox
                        cswPrivate.listOptionsStore.on({
                            beforeload: function(store, operation) {

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

                    //var select = nodeProperty.propDiv.select({
                    //    name: nodeProperty.name,
                    //    cssclass: 'selectinput',
                    //    onChange: function(val) {
                    //        cswPrivate.value = val;
                    //        nodeProperty.propData.values.value = val;
                    //        nodeProperty.broadcastPropChange(val);
                    //    },
                    //    values: cswPrivate.values,
                    //    selected: cswPrivate.value
                    //});
                    //select.required(nodeProperty.isRequired());
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());
