/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.triStateCheckBox = Csw.controls.triStateCheckBox ||
        Csw.controls.register('triStateCheckBox', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
                ID: '',
                prefix: '',
                Checked: '',
                ReadOnly: false,
                Required: false,
                Multi: false,
                cssclass: 'CswTristateCheckBox',
                onChange: function () {
                },
                value: 'null',
                btnValue: Csw.enums.imageButton_ButtonType.CheckboxNull
            };

            var cswPublicRet = {};

            cswPublicRet.val = function (value) {
                var ret;
                if (Csw.isNullOrEmpty(value)) {
                    ret = cswPrivateVar.value;
                } else {
                    ret = cswPublicRet;
                    cswPublicRet.propNonDom('value', value);
                    cswPublicRet.propDom('title', value);
                    cswPrivateVar.value = value;
                }
                return ret;
            };

            cswPublicRet.getButtonType = function () {
                var ret;
                switch (cswPrivateVar.value) {
                    case 'true':
                        ret = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                        break;
                    case 'false':
                        ret = Csw.enums.imageButton_ButtonType.CheckboxFalse;
                        break;
                    default:
                        ret = Csw.enums.imageButton_ButtonType.CheckboxNull;
                        break;
                }
                return ret;
            };

            cswPrivateVar.changeState = function () {
                if (cswPrivateVar.value === 'null') {
                    cswPrivateVar.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                    cswPrivateVar.value = 'true';
                } else if (cswPrivateVar.value === 'false') {
                    if (Csw.bool(cswPrivateVar.Required)) {
                        cswPrivateVar.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                        cswPrivateVar.value = 'true';
                    } else {
                        cswPrivateVar.btnValue = Csw.enums.imageButton_ButtonType.CheckboxNull;
                        cswPrivateVar.value = 'null';
                    }
                } else if (cswPrivateVar.value === 'true') {
                    cswPrivateVar.btnValue = Csw.enums.imageButton_ButtonType.CheckboxFalse;
                    cswPrivateVar.value = 'false';
                }
                cswPublicRet.val(cswPrivateVar.value);
                cswPublicRet.propNonDom('value', cswPrivateVar.value);
                cswPublicRet.propDom('title', cswPrivateVar.value);
                cswPrivateVar.onChange();
                return cswPrivateVar.checkBox.click(cswPrivateVar.btnValue);
            }; // onClick()


            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                cswPrivateVar.value = Csw.string(cswPrivateVar.Checked, 'null').toLowerCase(); //Case 21769
                cswPrivateVar.AlternateText = cswPrivateVar.value;

                if (cswPrivateVar.ReadOnly) {
                    if (cswPrivateVar.Multi) {
                        cswPrivateVar.text = Csw.enums.multiEditDefaultValue;
                    } else {
                        switch (cswPrivateVar.value) {
                            case 'true':
                                cswPrivateVar.text = 'Yes';
                                break;
                            case 'false':
                                cswPrivateVar.text = 'No';
                                break;
                        }
                    }
                    cswPrivateVar.checkBox = cswParent.div(cswPrivateVar);
                } else {
                    cswPrivateVar.ID = Csw.makeId(cswPrivateVar.ID, 'tst');
                    cswPrivateVar.ButtonType = cswPublicRet.getButtonType();
                    cswPrivateVar.checkBox = cswParent.imageButton(cswPrivateVar);
                    //cswPrivateVar.checkBox.imageButton(cswPrivateVar);
                }
                cswPublicRet = Csw.dom({}, cswPrivateVar.checkBox);
                //$.extend(cswPublicRet, Csw.literals.div(cswPrivateVar));
                cswPrivateVar.checkBox.bind('click', function () {
                    if (!Csw.bool(cswPrivateVar.ReadOnly)) {
                        Csw.tryExec(cswPrivateVar.changeState);
                    }
                });
                cswPublicRet.val(cswPrivateVar.value);

            } ());

            return cswPublicRet;
        });

} ());
