/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.barcode = Csw.properties.barcode ||
        Csw.properties.register('barcode',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPublic = {
                    data: propertyOption
                };
                var render = function (o) {
                    'use strict';
                    o = o || Csw.nbt.propertyOption(propertyOption);
                    
                    var propVals = o.propData.values;
                    var value = (false === o.Multi) ? Csw.string(propVals.barcode).trim() : Csw.enums.multiEditDefaultValue;

                    var table = o.propDiv.table({
                        ID: Csw.makeId(o.ID, 'tbl')
                    });

                    var cell1 = table.cell(1, 1);

                    if (o.ReadOnly) {
                        cell1.text(value);
                    } else {

                        cswPublic.control = cell1.input({
                            ID: o.ID,
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: function () {
                                var barcode = cswPublic.control.val();
                                Csw.tryExec(o.onChange, barcode);
                                o.onPropChange({ barcode: barcode });
                            },
                            value: value
                        });

                        if (o.Required) {
                            cswPublic.control.addClass('required');
                        }

                        cswPublic.control.clickOnEnter(o.saveBtn);
                    }
                    if (false === o.Multi) {
                        table.cell(1, 2).div({ ID: Csw.makeId(o.ID, 'parent', window.Ext.id()) })
                            .buttonExt({
                                ID: Csw.makeId(o.ID, 'print', window.Ext.id()),
                                enabledText: 'Print',
                                size: 'small',
                                tooltip: { title: 'Print Barcode Label' },
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.barcode),
                                onClick: function () {
                                    $.CswDialog('PrintLabelDialog', { 'nodeid': o.nodeid, 'propid': o.propid });
                                },
                                editMode: o.EditMode
                            });
                    }
                };
                propertyOption.render(render);

                return cswPublic;

            }));
}());
