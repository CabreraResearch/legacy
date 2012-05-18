/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.makeViewVisibilitySelect = Csw.controls.makeViewVisibilitySelect ||
        Csw.controls.register('makeViewVisibilitySelect', function(table, rownum, label) {
            ///<summary>Make a View Visibility Select. Used by View Editor and Dialog.</summary>
            ///<param name="table" type="Object">A Csw.literals.table object.</param>
            ///<param name="rownum" type="Number">A row number.</param>
            ///<param name="label" type="String">A label.</param>
            ///<returns type="Object">
            ///An object representing the new jQuery DOM elements.
            /// <para>$visibilityselect: Visibility picklist</para>
            /// <para>$visroleselect: Role picklist</para>
            /// <para>$visuserselect: User picklist</para>
            ///</returns>
            'use strict';
            var cswPublicRet = {
                $visibilityselect: '',
                $visroleselect: '',
                $visuserselect: ''
            };

            Csw.clientSession.isAdministrator({
                'Yes': function() {

                    table.cell(rownum, 1).text(label);
                    var parent = table.cell(rownum, 2);
                    var id = table.id;
                    /* NO! Refactor to use Csw.literals and more wholesome methods. */
                    cswPublicRet.$visibilityselect = $('<select id="' + id + '_vissel" />')
                        .appendTo(parent.$);
                    cswPublicRet.$visibilityselect.append('<option value="User">User:</option>');
                    cswPublicRet.$visibilityselect.append('<option value="Role">Role:</option>');
                    cswPublicRet.$visibilityselect.append('<option value="Global">Global</option>');

                    cswPublicRet.$visroleselect = parent.$.CswNodeSelect('init', {
                        ID: Csw.makeId(id, 'visrolesel'),
                        objectclass: 'RoleClass'
                    }).hide();
                    cswPublicRet.$visuserselect = parent.$.CswNodeSelect('init', {
                        ID: Csw.makeId(id, 'visusersel'),
                        objectclass: 'UserClass'
                    });

                    cswPublicRet.$visibilityselect.change(function() {
                        var val = cswPublicRet.$visibilityselect.val();
                        if (val === 'Role') {
                            cswPublicRet.$visroleselect.show();
                            cswPublicRet.$visuserselect.hide();
                        } else if (val === 'User') {
                            cswPublicRet.$visroleselect.hide();
                            cswPublicRet.$visuserselect.show();
                        } else {
                            cswPublicRet.$visroleselect.hide();
                            cswPublicRet.$visuserselect.hide();
                        }
                    }); // change
                } // yes
            }); // IsAdministrator

            return cswPublicRet;

        });

} ());