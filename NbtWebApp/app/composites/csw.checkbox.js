/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    Csw.composites.register('checkBox', function (cswParent, options) {
        'use strict';
        var cswPrivate = {
            name: '',
            checked: '',
            ReadOnly: false,
            Multi: false,
            cssclass: 'CswTristateCheckBox',
            onChange: null, // function (newval) {},
            value: 'false'
        };

        var cswPublic = {};

        Csw.tryExec(function () {

            (function _preCtor() {
                Csw.extend(cswPrivate, options);
                cswPrivate.type = Csw.enums.inputTypes.checkbox;
                cswPrivate.value = Csw.bool(cswPrivate.checked) || Csw.bool(cswPrivate.value); //Case 21769
                if (cswPrivate.ReadOnly) {
                    switch (cswPrivate.value) {
                        case true:
                            cswPrivate.text = 'Yes';
                            break;
                        case false:
                            cswPrivate.text = 'No';
                            break;
                    }
                    cswPublic = cswParent.div(cswPrivate);
                } else {
                    var onChange = cswPrivate.onChange;
                    cswPrivate.onChange = function () {
                        cswPrivate.value = !cswPrivate.value;
                        cswPrivate.checked = !cswPrivate.checked;
                        Csw.tryExec(onChange, cswPrivate.value);
                        return cswPrivate.value;
                    };
                    cswPublic = cswParent.input(cswPrivate);
                }
            }());

            cswPublic.val = function (val) {
                if (arguments.length > 0) {
                    cswPrivate.value = Csw.bool(val);
                }
                return cswPrivate.value;
            };

            cswPublic.checked = function () {
                return cswPrivate.value;
            };

            (function _postCtor() {

                cswPublic.val(cswPrivate.value);
            }());

        });

        return cswPublic;
    });

}());
