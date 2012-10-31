/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.password = Csw.properties.password ||
        Csw.properties.register('password',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.isExpired = Csw.bool(cswPrivate.propVals.isexpired);
                    cswPrivate.isAdmin = Csw.bool(cswPrivate.propVals.isadmin);
                    cswPrivate.passwordcomplexity = Csw.number(cswPrivate.propVals.passwordcomplexity, 0);
                    cswPrivate.passwordlength = Csw.number(cswPrivate.propVals.passwordlength, 0);

                    cswPublic.control = cswPrivate.parent.table({
                        OddCellRightAlign: true
                    });

                    if (cswPublic.data.isReadOnly()) {
                        //show nothing
                    } else {
                        cswPublic.control.cell(1, 1).text('Set New');
                        cswPrivate.cell12 = cswPublic.control.cell(1, 2);
                        cswPublic.control.cell(2, 1).text('Confirm');
                        cswPrivate.cell22 = cswPublic.control.cell(2, 2);
                        cswPrivate.cell31 = cswPublic.control.cell(3, 1);
                        cswPublic.control.cell(3, 2).text('Expired');

                        cswPrivate.textBox1 = cswPrivate.cell12.input({
                            name: cswPublic.data.name + '_pwd1',
                            type: Csw.enums.inputTypes.password,
                            cssclass: 'textinput',
                            value: '',
                            onChange: function () {
                                var val = cswPrivate.textBox1.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ newpassword: val });
                            }
                        });

                        /* Text Box 2 */
                        cswPrivate.cell22.input({
                            name: cswPublic.data.name + '_pwd2',
                            type: Csw.enums.inputTypes.password,
                            value: '',
                            cssclass: 'textinput password2',
                            onChange: cswPublic.data.onChange
                        });
                        if (cswPrivate.isAdmin) {
                            cswPrivate.expiredChk = cswPrivate.cell31.input({
                                name: cswPublic.data.name + '_exp',
                                type: Csw.enums.inputTypes.checkbox,
                                checked: cswPrivate.isExpired,
                                onChange: function () {
                                    var val = cswPrivate.expiredChk.$.is(':checked');
                                    Csw.tryExec(cswPublic.data.onChange, val);
                                    cswPublic.data.onPropChange({ isexpired: val });
                                }
                            });
                        }

                        if (cswPublic.data.isRequired() && Csw.isNullOrEmpty(cswPrivate.propVals.password)) {
                            cswPrivate.textBox1.addClass('required');
                        }

                        $.validator.addMethod("password2", function () {
                            cswPrivate.pwd1 = $('#' + cswPublic.data.name + '_pwd1').val();
                            cswPrivate.pwd2 = $('#' + cswPublic.data.name + '_pwd2').val();
                            return ((cswPrivate.pwd1 === '' && cswPrivate.pwd2 === '') || cswPrivate.pwd1 === cswPrivate.pwd2);
                        }, 'Passwords do not match!');
                        //Case 26096
                        if (cswPrivate.passwordlength > 0) {
                            $.validator.addMethod("password_length", function (value) {
                                return (Csw.string(value).length >= cswPrivate.passwordlength || Csw.string(value).length === 0);
                            }, 'Password must be at least ' + cswPrivate.passwordlength + ' characters long.');
                            cswPrivate.textBox1.addClass('password_length');
                        }
                        if (cswPrivate.passwordcomplexity > 0) {
                            $.validator.addMethod("password_number", function (value) {
                                return ((/.*[\d]/.test(value) && /.*[a-zA-Z]/.test(value)) || Csw.string(value).length === 0);
                            }, 'Password must contain at least one letter and one number.');
                            cswPrivate.textBox1.addClass('password_number');
                            if (cswPrivate.passwordcomplexity > 1) {
                                $.validator.addMethod("password_symbol", function (value) {
                                    return (/.*[`~!@#$%\^*()_\-+=\[{\]};:|\.?,]/.test(value) || Csw.string(value).length === 0);
                                }, 'Password must contain a symbol.');
                                cswPrivate.textBox1.addClass('password_symbol');
                            }
                        }
                    }

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());
