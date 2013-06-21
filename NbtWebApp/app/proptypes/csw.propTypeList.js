/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.list = Csw.properties.register('list',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();
                
                cswPrivate.value = nodeProperty.propData.values.value;
                cswPrivate.options = nodeProperty.propData.values.options;
                cswPrivate.propid = nodeProperty.propData.id;
                cswPrivate.fieldtype = nodeProperty.propData.fieldtype;
                
                //// if (cswPrivate.options.length === 0) {
                //Csw.ajaxWcf.post({
                //    urlMethod: 'Search/searchListOptions',
                //    data: {
                //        NodeTypePropId: cswPrivate.propid,
                //        SearchTerm: "dd MMM yyyy"
                //    },
                //    success: function (data) {
                //        //do stuff
               // }
                //});
                //// }

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
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                } else {
                    cswPrivate.values = cswPrivate.options.split(',');

                    //case 28020 - if a list has a value selected that's not in the list, add it to the options
                    if (false == Csw.contains(cswPrivate.values, cswPrivate.value)) {
                        cswPrivate.values.push(cswPrivate.value);
                    }

                    var listOptions = [];
                    
                    // The data store holding the states
                    var listOptionsStore = Ext.create('Ext.data.Store', {
                        data: listOptions,
                        fields: ['display', 'value'],
                        autoLoad: false
                    });

                    // Simple ComboBox using the data store
                    Ext.create('Ext.form.field.ComboBox', {
                        renderTo: nodeProperty.propDiv.div().getId(),
                        displayField: 'display',
                        labelWidth: 130,
                        store: listOptionsStore,
                        queryMode: 'local',
                        typeAhead: true
                    });
                    
                    // The data for all states
                    var states = [
                        { "display": "Matt", "value": "1" },
                        { "display": "Colleen", "value": "2" },
                        { "display": "Lucas", "value": "3" }
                    ];

                    if (cswPrivate.options.length > 0) {
                        // convert to a set of data that the store will accept
                        var optionsArray = cswPrivate.options.split(',');
                        optionsArray.forEach(function (option) {
                            listOptions.push({ display: option, value: option });
                        });
                        // load the data
                        listOptionsStore.loadData(listOptions);
                    } else {
                        // make the ajax call to get the data after the user has typed in a search query
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

} ());
