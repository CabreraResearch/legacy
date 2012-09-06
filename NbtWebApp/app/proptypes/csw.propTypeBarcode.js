/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.barcode = Csw.properties.barcode ||
        Csw.properties.register('barcode',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                var render = function (o) {
                    'use strict';
                    o = o || Csw.nbt.propertyOption(propertyOption);
                    
                    cswPrivate.propVals = o.propData.values;
                    cswPrivate.value = (false === o.Multi) ? Csw.string(cswPrivate.propVals.barcode).trim() : Csw.enums.multiEditDefaultValue;

                    cswPublic.control = o.propDiv.table({
                        ID: Csw.makeId(o.ID, 'tbl')
                    });

                    cswPrivate.cell1 = cswPublic.control.cell(1, 1);

                    if (o.ReadOnly) {
                        cswPrivate.cell1.text(cswPrivate.value);
                    } else {

                        cswPrivate.input = cswPrivate.cell1.input({
                            ID: o.ID,
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: function () {
                                var barcode = cswPrivate.input.val();
                                Csw.tryExec(o.onChange, barcode);
                                o.onPropChange({ barcode: barcode });
                            },
                            value: cswPrivate.value
                        });

                        if (o.Required) {
                            cswPublic.control.addClass('required');
                        }

                        cswPublic.control.clickOnEnter(o.saveBtn);
                    }
                    if (false === o.Multi) {
                        cswPublic.control.cell(1, 2).div({ ID: Csw.makeId(o.ID, 'parent', window.Ext.id()) })
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
