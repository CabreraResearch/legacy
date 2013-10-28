﻿/// <reference path="~/app/CswApp-vsdoc.js" />

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
            className: 'validateComboBox',
            errorMsg: 'This field is required.',
            isExtJsControl: true
        };

        Csw.extend(cswPrivate, options);

        var cswPublic = {
            input: null,
            isValid: false
        };

        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException(Csw.error.exception('Cannot create an input to add validation to without a parent div.', '', 'csw.validation.js', 22));
        }
        if (Csw.isNullOrEmpty(validatedControl)) {
            Csw.error.throwException(Csw.error.exception('Cannot add validation to an empty control.', '', 'csw.validation.js', 25));
        }

        // Create the hidden input
        cswPublic.input = cswParent.input().css(cswPrivate.cssOptions);
        cswPublic.input.required(true);
        cswPublic.input.addClass(cswPrivate.className);

        var currValue = "";
        if (cswPrivate.isExtJsControl) {
            currValue = validatedControl.getValue();
        } else {
            currValue = validatedControl.val();
        }

        if (false === Csw.isNullOrEmpty(currValue)) {
            cswPublic.input.val(true);
        } else {
            cswPublic.input.val(false);
        }

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