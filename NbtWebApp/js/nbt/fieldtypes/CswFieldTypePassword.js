/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypePassword';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var isExpired = (false === o.Multi) ? Csw.bool(propVals.isexpired) : null;
            var isAdmin = (false === o.Multi) ? Csw.bool(propVals.isadmin) : null;

            if (o.ReadOnly) {
                // show nothing
            } else {
                var table = propDiv.table({
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl'),
                    OddCellRightAlign: true
                });

                table.cell(1, 1).text('Set New');
                var cell12 = table.cell(1, 2);
                table.cell(2, 1).text('Confirm');
                var cell22 = table.cell(2, 2);
                var cell31 = table.cell(3, 1);
                table.cell(3, 2).text('Expired');

                var textBox1 = cell12.input({
                    ID: o.ID + '_pwd1',
                    type: Csw.enums.inputTypes.password,
                    cssclass: 'textinput',
                    value: (false === o.Multi) ? '' : Csw.enums.multiEditDefaultValue,
                    onChange: o.onChange
                });
                
                /* Text Box 2 */
                cell22.input({ ID: o.ID + '_pwd2',
                    type: Csw.enums.inputTypes.password,
                    value: (false === o.Multi) ? '' : Csw.enums.multiEditDefaultValue,
                    cssclass: 'textinput password2',
                    onChange: o.onChange
                });
                if (isAdmin) {
                    cell31.input({
                        ID: o.ID + '_exp',
                        name: o.ID + '_exp',
                        type: Csw.enums.inputTypes.checkbox,
                        checked: isExpired
                    });
                }

                if (o.Required && Csw.isNullOrEmpty(propVals.password)) {
                    textBox1.addClass('required');
                }

                $.validator.addMethod("password2", function () {
                    var pwd1 = $('#' + o.ID + '_pwd1').val();
                    var pwd2 = $('#' + o.ID + '_pwd2').val();
                    return ((pwd1 === '' && pwd2 === '') || pwd1 === pwd2);
                }, 'Passwords do not match!');
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = {
                isexpired: null,
                newpassword: null
            };
            var newPw = o.propDiv.find('input#' + o.ID + '_pwd1');
            if (false === Csw.isNullOrEmpty(newPw)) {
                attributes.newpassword = newPw.val();
            }

            var isExpiredCheck = o.propDiv.find('input#' + o.ID + '_exp');
            if (false === Csw.isNullOrEmpty(isExpiredCheck) && isExpiredCheck.length() > 0) {
                attributes.isexpired = isExpiredCheck.$.is(':checked');
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypePassword = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
