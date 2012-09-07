/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.triStateCheckBox = Csw.controls.triStateCheckBox ||
        Csw.controls.register('triStateCheckBox', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
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

            var cswPublic = {};

            cswPublic.val = function (value) {
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

            cswPublic.getButtonType = function () {
                var ret;
                switch (cswPrivate.value) {
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

            cswPrivate.changeState = function () {
                if (cswPrivate.value === 'null') {
                    cswPrivate.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                    cswPrivate.value = 'true';
                } else if (cswPrivate.value === 'false') {
                    if (Csw.bool(cswPrivate.Required)) {
                        cswPrivate.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                        cswPrivate.value = 'true';
                    } else {
                        cswPrivate.btnValue = Csw.enums.imageButton_ButtonType.CheckboxNull;
                        cswPrivate.value = 'null';
                    }
                } else if (cswPrivate.value === 'true') {
                    cswPrivate.btnValue = Csw.enums.imageButton_ButtonType.CheckboxFalse;
                    cswPrivate.value = 'false';
                }
                cswPublic.val(cswPrivate.value);
                cswPublic.propNonDom('value', cswPrivate.value);
                cswPublic.propDom('title', cswPrivate.value);
                cswPrivate.onChange();
                return cswPrivate.checkBox.click(cswPrivate.btnValue);
            }; // onClick()


            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.value = Csw.string(cswPrivate.Checked, 'null').toLowerCase(); //Case 21769
                cswPrivate.AlternateText = cswPrivate.value;

                if (cswPrivate.ReadOnly) {
                    if (cswPrivate.Multi) {
                        cswPrivate.text = Csw.enums.multiEditDefaultValue;
                    } else {
                        switch (cswPrivate.value) {
                            case 'true':
                                cswPrivate.text = 'Yes';
                                break;
                            case 'false':
                                cswPrivate.text = 'No';
                                break;
                        }
                    }
                    cswPrivate.checkBox = cswParent.div(cswPrivate);
                } else {
                    cswPrivate.ID = Csw.makeId(cswPrivate.ID, 'tst');
                    cswPrivate.ButtonType = cswPublic.getButtonType();
                    cswPrivate.checkBox = cswParent.imageButton(cswPrivate);
                    //cswPrivate.checkBox.imageButton(cswPrivate);
                }
                cswPublic = Csw.dom({}, cswPrivate.checkBox);
                //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));
                cswPrivate.checkBox.bind('click', function () {
                    if (!Csw.bool(cswPrivate.ReadOnly)) {
                        Csw.tryExec(cswPrivate.changeState);
                    }
                });
                cswPublic.val(cswPrivate.value);

            } ());

            return cswPublic;
        });

} ());
