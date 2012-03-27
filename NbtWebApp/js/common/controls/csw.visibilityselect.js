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
            var external = {
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
                    external.$visibilityselect = $('<select id="' + id + '_vissel" />')
                        .appendTo(parent.$);
                    external.$visibilityselect.append('<option value="User">User:</option>');
                    external.$visibilityselect.append('<option value="Role">Role:</option>');
                    external.$visibilityselect.append('<option value="Global">Global</option>');

                    external.$visroleselect = parent.$.CswNodeSelect('init', {
                        ID: Csw.makeId(id, 'visrolesel'),
                        objectclass: 'RoleClass'
                    }).hide();
                    external.$visuserselect = parent.$.CswNodeSelect('init', {
                        ID: Csw.makeId(id, 'visusersel'),
                        objectclass: 'UserClass'
                    });

                    external.$visibilityselect.change(function() {
                        var val = external.$visibilityselect.val();
                        if (val === 'Role') {
                            external.$visroleselect.show();
                            external.$visuserselect.hide();
                        } else if (val === 'User') {
                            external.$visroleselect.hide();
                            external.$visuserselect.show();
                        } else {
                            external.$visroleselect.hide();
                            external.$visuserselect.hide();
                        }
                    }); // change
                } // yes
            }); // IsAdministrator

            return external;

        });

} ());