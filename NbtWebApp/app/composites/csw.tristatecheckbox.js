/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.composites.register('triStateCheckBox', function (cswParent, options) {
        'use strict';
        var cswPrivate = {
            name: '',
            prefix: '',
            checked: '',
            ReadOnly: false,
            isRequired: false,
            Multi: false,
            cssclass: 'CswTristateCheckBox cswInline',
            onChange: function () {
            },
            value: 'null',
            btnValue: Csw.enums.imageButton_ButtonType.CheckboxNull
        };

        var cswPublic = {};

        cswPrivate.getButtonType = function () {
            var ret;
            if (cswPrivate.isRequired && 'true' !== cswPrivate.value && 'false' != cswPrivate.value) {
                cswPrivate.value = 'false';
            }
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

        (function _preCtor() {
            Csw.extend(cswPrivate, options);

            cswPrivate.value = Csw.string(cswPrivate.checked, 'null').toLowerCase(); //Case 21769
            cswPrivate.AlternateText = cswPrivate.value;

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
                cswPrivate.name = cswPrivate.name + '_tst';
                cswPrivate.ButtonType = cswPrivate.getButtonType();
                cswPrivate.checkBox = cswParent.imageButton(cswPrivate);
                //cswPrivate.checkBox.imageButton(cswPrivate);
            }
            cswPublic = Csw.dom({}, cswPrivate.checkBox);
        }());

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
            if (true !== cswPrivate.ReadOnly) {
                cswPrivate.checkBox.click(cswPrivate.getButtonType());
            }
            return ret;
        };

        cswPrivate.changeState = function () {
            if (cswPrivate.value === 'null') {
                cswPrivate.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                cswPrivate.value = 'true';
            } else if (cswPrivate.value === 'false') {
                if (Csw.bool(cswPrivate.isRequired)) {
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
            cswPrivate.onChange(cswPrivate.value);
            return cswPrivate.checkBox.click(cswPrivate.btnValue);
        }; // onClick()

        cswPublic.getButtonType = function () {
            return cswPrivate.getButtonType();
        };

        (function _postCtor() {
            //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));
            cswPrivate.checkBox.bind('click', function () {
                if (!Csw.bool(cswPrivate.ReadOnly)) {
                    Csw.tryExec(cswPrivate.changeState);
                }
            });
            cswPublic.val(cswPrivate.value);
        }());



        return cswPublic;
    });

}());
