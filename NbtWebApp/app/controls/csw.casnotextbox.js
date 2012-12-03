/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {


    Csw.controls.CASNoTextBox = Csw.controls.CASNoTextBox ||
        Csw.controls.register('CASNoTextBox', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                value: '',
                cssclass: '',
                type: Csw.enums.inputTypes.text,
                Precision: '',
                ReadOnly: false,
                isRequired: false,
                onChange: function () {
                },
                width: '',
                isValid: null
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                if (cswPrivate.ReadOnly) {
                    cswPrivate.div = cswParent.div(cswPrivate);
                    cswPublic = Csw.dom({}, cswPrivate.div);
                    cswPublic.append(cswPrivate.value);
                } else {
                    cswPrivate.cssclass += ' textinput ';

                    cswPrivate.input = cswParent.input(cswPrivate);
                    cswPrivate.badCheckSumTxt = cswParent.span({ text: 'Checksum is invalid' }).css('color', 'Red').hide();
                    cswPrivate.invalidTxt = cswParent.span({ text: 'Input is not a valid CASno' }).css('color', 'Red').hide();

                    var highLightInvalid = function (value) {
                        if ((false == Csw.validateCASNo(value)) && false == (Csw.isNullOrEmpty(value) && false == cswPrivate.isRequired)) {
                            cswPrivate.input.css('background-color', '#FF4000');
                            cswPrivate.badCheckSumTxt.hide();
                            cswPrivate.invalidTxt.show();
                        } else if (false == Csw.checkSumCASNo(value) && false == (Csw.isNullOrEmpty(value) && false == cswPrivate.isRequired)) {
                            cswPrivate.input.css('background-color', '#FF4000');
                            cswPrivate.invalidTxt.hide();
                            cswPrivate.badCheckSumTxt.show();
                        } else {
                            cswPrivate.input.css('background-color', 'White');
                            cswPrivate.invalidTxt.hide();
                            cswPrivate.badCheckSumTxt.hide();
                        }
                    }

                    highLightInvalid(cswPrivate.input.val());

                    cswPublic = Csw.dom({}, cswPrivate.input);

                    cswPublic.bind('change', function () {
                        cswPrivate.value = cswPublic.val();
                        highLightInvalid(cswPrivate.value);
                    });

                    cswPublic.required(cswPrivate.isRequired);
                } /* else */
            } ());
            return cswPublic;
        });

})(jQuery);
