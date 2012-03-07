/// <reference path="~/js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswTriStateCheckBox() {

    function triStateCheckBox(options) {

        var internal = {
            ID: '',
            prefix: '',
            Checked: '',
            ReadOnly: false,
            Required: false,
            Multi: false,
            cssclass: 'CswTristateCheckBox',
            onChange: function () { },
            value: 'null',
            btnValue: Csw.enums.imageButton_ButtonType.CheckboxNull
        };

        var external = {};

        internal.val = function () {
            return internal.buttonVal;
        };

        external.getButtonType = function () {
            var ret;
            switch (internal.value) {
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

        internal.changeState = function () {
            if (internal.value === 'null') {
                internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                internal.value = 'true';
            }
            else if (internal.value === 'false') {
                if (Csw.bool(internal.Required)) {
                    internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                    internal.value = 'true';
                } else {
                    internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxNull;
                    internal.value = 'null';
                }
            } else if (internal.value === 'true') {
                internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxFalse;
                internal.value = 'false';
            }
            external.val(internal.value);
            external.propDom('alt', internal.value);
            internal.onChange();
            return external.click(internal.btnValue);
        }; // onClick()


        (function () {
            if (options) {
                $.extend(internal, options);
            }

            internal.value = Csw.string(internal.Checked, 'null').toLowerCase(); //Case 21769
            internal.AlternateText = internal.value;

            if (internal.ReadOnly) {
                if (internal.Multi) {
                    internal.value = Csw.enums.multiEditDefaultValue;
                } else {
                    switch (internal.value) {
                        case 'true':
                            internal.value = 'Yes';
                            break;
                        case 'false':
                            internal.value = 'No';
                            break;
                    }
                }
                $.extend(external, Csw.controls.div(internal));
            } else {
                internal.ID = Csw.controls.dom.makeId(internal.ID, 'tst');
                internal.ButtonType = external.getButtonType();
                $.extend(external, Csw.controls.imageButton(internal));
                external.bind('click', internal.changeState);
            }

        } ());

        return external;
    }
    Csw.controls.register('triStateCheckBox', triStateCheckBox)
    Csw.controls.triStateCheckBox = Csw.controls.triStateCheckBox || triStateCheckBox;

} ());
