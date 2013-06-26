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
                cswPrivate.options = nodeProperty.propData.values.options;
                cswPrivate.propid = nodeProperty.propData.id;
                //cswPrivate.fieldtype = nodeProperty.propData.fieldtype;

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
                });//nodeProperty.onPropChangeBroadcast()

                if (nodeProperty.isReadOnly()) {
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                } else {
                    // Save the options as an array
                    cswPrivate.values = cswPrivate.options.split(',');

                    //Case 28020 - If a list has a value selected that's not in the list, add it to the options
                    if (false == Csw.contains(cswPrivate.values, cswPrivate.value)) {
                        cswPrivate.values.push(cswPrivate.value);
                    }

                    // Create the Store
                    cswPrivate.listOptionsStore = new Ext.data.Store({
                        fields: ['display', 'value'],
                        autoLoad: false
                    });

                    // Create the Ext JS ComboBox
                    cswPrivate.select = Ext.create('Ext.form.field.ComboBox', {
                        name: nodeProperty.name,
                        renderTo: nodeProperty.propDiv.div().getId(),
                        displayField: 'display',
                        store: cswPrivate.listOptionsStore,
                        queryMode: 'local',
                        value: cswPrivate.value,
                        listeners: {
                            select: function (combo, records, eOpts) {
                                var val = records[0].get('value');
                                cswPrivate.value = val;
                                nodeProperty.propData.values.value = val;
                                nodeProperty.broadcastPropChange(val);
                            }
                        },
                        tpl: new Ext.XTemplate('<tpl for=".">' + '<li style="height:22px;" class="x-boundlist-item" role="option">' + '{display}' + '</li></tpl>'),
                        queryDelay: 2000,
                        width: 200

                    });

                    /*
                     * If the server returns no options, then the number of options exceeded
                     * the relationshipoptionlimit configuration variable. When the number of
                     * options exceeds this variable, the user is forced to search the options.
                     *
                     */
                    if (cswPrivate.options.length > 0) {
                        var listOptions = [];
                        // Convert into an array of objects that the store will accept
                        cswPrivate.values.forEach(function (option) {
                            listOptions.push({ display: option, value: option });
                        });
                        cswPrivate.listOptionsStore.loadData(listOptions);
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
                                root: 'Data.FilteredListOptions',
                                getResponseData: function (response) {
                                    // This function allows us to intercept the data before the reader
                                    // reads it so that we can convert it from a comma delimited string
                                    // into an array of objects the store will accept.
                                    var json = Ext.decode(response.responseText);
                                    var listOptions = [];

                                    var optionsArray = json.Data.FilteredListOptions.split(',');
                                    optionsArray.forEach(function (option) {
                                        listOptions.push({ display: option, value: option });
                                    });

                                    json.Data.FilteredListOptions = listOptions;

                                    //Set the width of the combobox to match the longest string returned
                                    var longestOption = optionsArray.sort(function (a, b) { return b.length - a.length; })[0];
                                    var newWidth = (longestOption.length * 8);
                                    if (newWidth > 200) {
                                        cswPrivate.select.setWidth(newWidth);
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

                    }//if (cswPrivate.options.length > 0)

                }//if (nodeProperty.isReadOnly())

            };//render()

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });
}());
