/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {

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

        internal.onClick = function () {
            if (internal.value === 'null') {
                internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                internal.value = 'true';
            } else if (internal.buttonVal === 'false') {
                if (Csw.bool(internal.Required)) {
                    internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxTrue;
                    internal.value = 'true';
                } else {
                    internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxNull;
                    internal.value = 'null';
                }
            } else if (internal.buttonVal === 'true') {
                internal.btnValue = Csw.enums.imageButton_ButtonType.CheckboxFalse;
                internal.value = 'false';
            }
            return internal.btnValue;
        }; // onClick()


        (function () {
            if (options) {
                $.extend(internal, options);
            }

            var elementId = Csw.controls.dom.makeId(internal.ID, 'tst'),
                tristateVal = Csw.string(internal.Checked, 'null').toLowerCase(); //Case 21769

            if (internal.ReadOnly) {
                if (internal.Multi) {
                    internal.value = Csw.enums.multiEditDefaultValue;
                } else {
                    switch (tristateVal) {
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
                internal.ID = elementId;
                internal.ButtonType = getButtonType(tristateVal);
                internal.AlternateText = tristateVal;
                internal.onClick = function () {
                    internal.onChange();
                    return internal.onClick();
                };
                $.extend(external, Csw.controls.imageButton(internal));
            }

        } ());

        return external;
    }
} ());
