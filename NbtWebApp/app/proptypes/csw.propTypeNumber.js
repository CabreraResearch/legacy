/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.number = Csw.properties.number ||
        Csw.properties.register('number',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.isMultiEditValid = function(value) {
                        return cswPublic.data.isMulti() && value === Csw.enums.multiEditDefaultValue;
                    };

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.precision = Csw.number(cswPrivate.propVals.precision, 6);
                    cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);

                    cswPublic.control = cswPrivate.parent.numberTextBox({
                        ID: cswPublic.data.ID + '_num',
                        value: (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue,
                        MinValue: Csw.number(cswPrivate.propVals.minvalue),
                        MaxValue: Csw.number(cswPrivate.propVals.maxvalue),
                        isOpenSet: Csw.bool(cswPrivate.propVals.isOpenSet),
                        ceilingVal: cswPrivate.ceilingVal,
                        Precision: cswPrivate.precision,
                        ReadOnly: Csw.bool(cswPublic.data.isReadOnly()),
                        Required: Csw.bool(cswPublic.data.isRequired()),
                        onChange: function () {
                            var val = cswPublic.control.val();
                            Csw.tryExec(cswPublic.data.onChange, val);
                            cswPublic.data.onPropChange({ value: val });
                        },
                        isValid: cswPrivate.isMultiEditValid
                    });
                    cswPublic.control.required(cswPublic.data.propData.required);
                    if (false === Csw.isNullOrEmpty(cswPublic.control) && cswPublic.control.length > 0) {
                        cswPublic.control.clickOnEnter(cswPublic.data.saveBtn);
                    }
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
