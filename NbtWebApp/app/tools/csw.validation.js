/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    Csw.validateComboBox = Csw.validateComboBox ||
        Csw.register('validateComboBox', function (div, control, cssOptions, wasModified, setColorFunction) {
            /// <summary>
            /// Adds an input to a control which simulates what would happen if JQuery validation could
            /// be added to the control itself.
            /// </summary>
            /// <param name="control">The control that needs to have validation attached to it.</param>
            /// <param name="setColorFunction">(Nullable) A function that changes the color of the control. Can be null if you don't want to change the color of the control.</param>
            /// <param name="div"></param>
            /// <param name="cssOptions"></param>

            //Returns:
            // 1. an input (that is validated instead of the actual control)
            // 2. bool (T/F) as to whether the input is valid  

            var cswPublic = {
                input: null,
                isValid: false
            };

            if (Csw.isNullOrEmpty(div)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an input to add validation to without a parent div.', '', 'csw.validation.js', 22));
            }
            if (Csw.isNullOrEmpty(control)) {
                Csw.error.throwException(Csw.error.exception('Cannot add validation to an empty control.', '', 'csw.validation.js', 25));
            }

            // Create the hidden input
            cswPublic.input = div.input().css(cssOptions);
            cswPublic.input.required(true);
            cswPublic.input.addClass('validateComboBox');

            if (false === Csw.isNullOrEmpty(control.getValue())) {
                cswPublic.input.val(true);
            } else {
                cswPublic.input.val(false);
            }


            if (wasModified) {
                var valid = cswPublic.input.$.valid();
                setColorFunction(valid);
            }
            
            $.validator.addMethod('validateComboBox', function () {
                var valid = Csw.bool(cswPublic.input.val());
                setColorFunction(valid);
                return valid;
            }, 'This field is required.');

            return cswPublic;

        });
}());