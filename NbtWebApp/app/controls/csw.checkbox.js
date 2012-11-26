/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.checkBox = Csw.controls.checkBox ||
        Csw.controls.register('checkBox', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                prefix: '',
                checked: '',
                ReadOnly: false,
                Multi: false,
                cssclass: 'CswTristateCheckBox',
                onChange: null, // function (newval) {},
                value: 'false'
                
            };

            var cswPublic = {};

            Csw.tryExec(function() {

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
                        cswPrivate.checkBox = cswParent.div(cswPrivate);
                    } else {
                        var onChange = cswPrivate.onChange;
                        cswPrivate.onChange = function () {
                            cswPrivate.value = !cswPrivate.value;
                            cswPrivate.checked = !cswPrivate.checked;
                            Csw.tryExec(onChange, cswPrivate.value);
                            return cswPrivate.value;
                        };
                        cswPrivate.checkBox = cswParent.input(cswPrivate);
                    }

                    cswPublic = Csw.dom({}, cswPrivate.checkBox.$);
                } ());

                cswPublic.val = function() {
                    return cswPrivate.value;
                };

                cswPublic.checked = function() {
                    return cswPrivate.value;
                };
                
                (function _postCtor() {
                    
                    cswPublic.val(cswPrivate.value);
                }());
            
            });
            
            return cswPublic;
        });

} ());
