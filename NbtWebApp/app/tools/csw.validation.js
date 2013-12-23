/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    Csw.register('validator', function (cswParent, validatedControl, options) {
        /// <summary>
        /// Adds an input to a control which simulates what would happen if JQuery validation could
        /// be added to the control itself.
        /// </summary>
        /// <param name="validatedControl">The control that needs to have validation attached to it.</param>
        /// <param name="cswParent"></param>
        /// <param name="options"></param>

        var cswPrivate = {
            cssOptions: {
                visibility: 'hidden',
                width: '20px'
            },
            wasModified: false,
            onValidation: null,
            className: '',
            errorMsg: 'This field is required.',
            isExtJsControl: false, // defaults to false
            emptyText: '',
            hiddenInputName: ''
        };

        Csw.extend(cswPrivate, options);

        var cswPublic = {
            input: null,
            isValid: false
        };

        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException(
                Csw.error.exception('Cannot create an input to add validation to without a parent div.', '', 'csw.validation.js', 22)
            );
        }
        if (Csw.isNullOrEmpty(validatedControl)) {
            Csw.error.throwException(
                Csw.error.exception('Cannot add validation to an empty control.', '', 'csw.validation.js', 25)
            );
        }

        // Create the hidden input
        cswPublic.input = cswParent.input({
            name: cswPrivate.hiddenInputName
        }).css(cswPrivate.cssOptions);
        cswPublic.input.required(true);
        cswPublic.input.addClass(cswPrivate.className);

        // Get the current value of the control
        var currValue;
        if (cswPrivate.isExtJsControl) {
            currValue = validatedControl.getValue();
        } else {
            currValue = validatedControl.val();
        }

        // Check whether the control is valid
        if (false === Csw.isNullOrEmpty(currValue)) {
            cswPublic.input.val(true);
            if (false === Csw.isNullOrEmpty(cswPrivate.emptyText) && currValue === cswPrivate.emptyText) {
                cswPublic.input.val(false);
            }
        } else {
            cswPublic.input.val(false);
        }

        // Perform the validation if the control was modified
        if (cswPrivate.wasModified) {
            var valid = cswPublic.input.$.valid();
            cswPrivate.onValidation(valid);
        }

        $.validator.addMethod(cswPrivate.className, function () {
            var valid = Csw.bool(cswPublic.input.val());
            if (false == Csw.isNullOrEmpty(cswPrivate.onValidation)) {
                cswPrivate.onValidation(valid);
            }
            return valid;
        }, cswPrivate.errorMsg);

        return cswPublic;

    });
}());