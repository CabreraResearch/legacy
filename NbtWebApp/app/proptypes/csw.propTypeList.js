/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('list', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.text = nodeProperty.propData.values.text;
            cswPrivate.value = nodeProperty.propData.values.value;
            cswPrivate.options = nodeProperty.propData.values.options;
            cswPrivate.propid = nodeProperty.propData.id;
            cswPrivate.isRequired = nodeProperty.propData.required;
            cswPrivate.wasModified = nodeProperty.propData.wasmodified;
            cswPrivate.table = nodeProperty.propDiv.table({ TableCssClass: 'cswInline' });
            cswPrivate.selectCell = cswPrivate.table.cell(1, 1);
            cswPrivate.validateCell = cswPrivate.table.cell(1, 2);

            nodeProperty.onPropChangeBroadcast(function (object) {

                if (cswPrivate.value !== object.val) {
                    cswPrivate.value = object.val;
                    cswPrivate.text = object.text;

                    nodeProperty.propData.values.text = object.text;
                    nodeProperty.propData.values.value = object.val;

                    if (cswPrivate.select) {
                        cswPrivate.select.combobox.setValue(cswPrivate.value);
                    }
                    if (span) {
                        span.remove();
                        span = cswPrivate.selectCell.span({ text: object.text });
                    }
                }
            });//nodeProperty.onPropChangeBroadcast()
            
            if (nodeProperty.isReadOnly()) {
                var span = cswPrivate.selectCell.span({ text: cswPrivate.text });
            } else {
                cswPrivate.select = cswPrivate.selectCell.div().comboBoxExt({
                    name: nodeProperty.name,
                    displayField: 'Text',
                    valueField: 'Value',
                    queryMode: 'local',
                    queryDelay: 2000,
                    options: cswPrivate.options,
                    selectedValue: cswPrivate.text,
                    search: nodeProperty.propData.values.search,
                    searchUrl: 'Search/searchListOptions',
                    listeners: {
                        select: function(combo, records) {
                            var text = records[0].get('Text');
                            var val = records[0].get('Value');
                            nodeProperty.broadcastPropChange({ text: text, val: val });
                        },
                        change: function(combo, newvalue) {
                            nodeProperty.broadcastPropChange({ text: newvalue, val: newvalue });
                        },
                        storebeforeload: function() {
                            var obj = {};
                            obj.NodeTypePropId = cswPrivate.propid;
                            obj.SearchTerm = cswPrivate.select.combobox.getValue();
                            return obj;
                        }
                    },
                    isRequired: cswPrivate.isRequired
                        
                });
                
            }//if (nodeProperty.isReadOnly())

        };//render()

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });
}());