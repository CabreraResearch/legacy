/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.checkBox = Csw.controls.checkBox ||
        Csw.controls.register('checkBox', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                prefix: '',
                Checked: '',
                ReadOnly: false,
                Multi: false,
                cssclass: 'CswTristateCheckBox',
                onChange: null, // function (newval) {},
                value: 'false',
                btnValue: Csw.enums.imageButton_ButtonType.CheckboxFalse
            };

            var cswPublic = {};

            Csw.tryExec(function() {

                (function preInit() {
                    if (options) {
                        Csw.extend(cswPrivate, options);
                    }
                    cswPrivate.value = Csw.string(cswPrivate.Checked, 'false').toLowerCase(); //Case 21769
                    cswPrivate.AlternateText = cswPrivate.value;

                    cswPublic = Csw.dom({}, cswPrivate.checkBox);
                } ());

                cswPublic.val = function(value) {
                    var ret;
                    if (Csw.isNullOrEmpty(value)) {
                        ret = cswPrivate.value;
                    } else {
                        ret = cswPublic;
                        cswPublic.propNonDom('value', value);
                        cswPublic.propDom('title', value);
                        cswPrivate.value = value;
                    }
                    return ret;
                };

                cswPublic.checked = function() {
                    return cswPrivate.value === 'true';
                };

                cswPublic.getButtonType = function() {
                    var ret;
                    switch (cswPrivate.value) {
                    case 'true':
                        ret = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                        break;
                    default:
                        ret = Csw.enums.imageButton_ButtonType.CheckboxFalse;
                        break;
                    }
                    return ret;
                };

                cswPrivate.changeState = function() {
                    if (cswPrivate.value === 'false') {
                        cswPrivate.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                        cswPrivate.value = 'true';
                    } else {
                        cswPrivate.btnValue = Csw.enums.imageButton_ButtonType.CheckboxFalse;
                        cswPrivate.value = 'false';
                    }
                    cswPublic.val(cswPrivate.value);
                    cswPublic.propNonDom('value', cswPrivate.value);
                    cswPublic.propDom('title', cswPrivate.value);
                    cswPrivate.onChange(cswPrivate.value);
                    return cswPrivate.checkBox.click(cswPrivate.btnValue);
                }; // onClick()


                (function postInit() {
                    if (cswPrivate.ReadOnly) {
                        switch (cswPrivate.value) {
                            case 'true':
                                cswPrivate.text = 'Yes';
                                break;
                            case 'false':
                                cswPrivate.text = 'No';
                                break;
                        }
                        cswPrivate.checkBox = cswParent.div(cswPrivate);
                    } else {
                        cswPrivate.ButtonType = cswPublic.getButtonType();
                        cswPrivate.checkBox = cswParent.imageButton(cswPrivate);
                    }
                    
                    cswPrivate.checkBox.bind('click', function () {
                        if (!Csw.bool(cswPrivate.ReadOnly)) {
                            Csw.tryExec(cswPrivate.changeState);
                        }
                    });
                    
                    cswPublic.val(cswPrivate.value);
                }());
            
            });
            
            return cswPublic;
        });

} ());
