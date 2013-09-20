/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('password', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            if (nodeProperty.isReadOnly()) {
                //show nothing
            } else {
                cswPrivate.isExpired = nodeProperty.propData.values.isexpired;
                cswPrivate.isAdmin = nodeProperty.propData.values.isadmin;
                cswPrivate.passwordcomplexity = nodeProperty.propData.values.passwordcomplexity;
                cswPrivate.passwordlength = nodeProperty.propData.values.passwordlength;

                var table = nodeProperty.propDiv.table({
                    OddCellRightAlign: true
                });

                table.cell(1, 1).text('Set New');
                cswPrivate.cell12 = table.cell(1, 2);
                table.cell(2, 1).text('Confirm');
                cswPrivate.cell22 = table.cell(2, 2);
                cswPrivate.cell31 = table.cell(3, 1);
                cswPrivate.cell32 = table.cell(3, 2);

                var p1, p2;
                var tryUpdate = function (val) {
                    if (p1 === p2) {
                        nodeProperty.propData.values.newpassword = val;
                        nodeProperty.broadcastPropChange();
                    }
                };

                cswPrivate.textBox1 = cswPrivate.cell12.input({
                    name: nodeProperty.name + '_pwd1',
                    type: Csw.enums.inputTypes.password,
                    cssclass: 'textinput',
                    value: '',
                    onChange: function (val) {
                        p1 = val;
                        tryUpdate(val);
                    }
                });

                /* Text Box 2 */
                cswPrivate.textBox2 = cswPrivate.cell22.input({
                    name: nodeProperty.name + '_pwd2',
                    type: Csw.enums.inputTypes.password,
                    value: '',
                    cssclass: 'textinput password2',
                    onChange: function (val) {
                        p2 = val;
                        tryUpdate(val);
                    }
                });
                if (cswPrivate.isAdmin &&
                    true !== nodeProperty.tabState.isChangePasswordDialog) { // kludgetastic!  case 29841
                    cswPrivate.cell32.text('Expired');
                    cswPrivate.expiredChk = cswPrivate.cell31.input({
                        name: nodeProperty.name + '_exp',
                        type: Csw.enums.inputTypes.checkbox,
                        checked: cswPrivate.isExpired,
                        onChange: function() {
                            var val = cswPrivate.expiredChk.$.is(':checked');
                            nodeProperty.propData.values.expire = val;
                            nodeProperty.broadcastPropChange();
                        }
                    });
                } else {
                    nodeProperty.propData.values.expire = false;
                }

                if (nodeProperty.isRequired() && Csw.isNullOrEmpty(nodeProperty.propData.values.password)) {
                    cswPrivate.textBox1.addClass('required');
                }

                $.validator.addMethod("password2", function () {
                    cswPrivate.pwd1 = cswPrivate.textBox1.val();
                    cswPrivate.pwd2 = cswPrivate.textBox2.val();
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
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());
