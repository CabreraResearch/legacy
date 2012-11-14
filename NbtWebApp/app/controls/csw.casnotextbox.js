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
                    //cswPrivate.cssclass += ' textinput ';

                    cswPrivate.input = cswParent.input(cswPrivate);
                    cswPublic = Csw.dom({}, cswPrivate.input);

                    cswPublic.bind('change', function () {
                        cswPrivate.value = cswPublic.val();
                    });

                    cswPublic.required(cswPrivate.isRequired);
                } /* else */
            } ());
            return cswPublic;
        });

})(jQuery);
