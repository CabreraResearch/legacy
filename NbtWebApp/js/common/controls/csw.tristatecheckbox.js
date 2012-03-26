/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {

    Csw.components.triStateCheckBox = Csw.components.triStateCheckBox ||
        Csw.components.register('triStateCheckBox', function (cswParent, options) {

            var internal = {
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
                } else if (internal.value === 'false') {
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
                    internal.checkBox = cswParent.div(internal);
                } else {
                    internal.ID = Csw.makeId(internal.ID, 'tst');
                    internal.ButtonType = external.getButtonType();
                    internal.checkBox = cswParent.div();
                    internal.checkBox.imageButton(internal);
                }
                external = Csw.dom({}, internal.checkBox);
                //$.extend(external, Csw.controls.div(internal));
                external.bind('click', function () {
                    if (!Csw.bool(internal.ReadOnly)) {
                        Csw.tryExec(internal.changeState);
                    }
                });

            } ());

            return external;
        });

} ());
