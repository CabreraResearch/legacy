/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _makeViewVisibilitySelect() {
    'use strict';


    function makeViewVisibilitySelect(table, rownum, label) {
        ///<summary>Make a View Visibility Select. Used by View Editor and Dialog.</summary>
        ///<param name="table" type="Object">A Csw.controls.table object.</param>
        ///<param name="rownum" type="Number">A row number.</param>
        ///<param name="label" type="String">A label.</param>
        ///<returns type="Object">
        ///An object representing the new jQuery DOM elements.
        /// <para>$visibilityselect: Visibility picklist</para>
        /// <para>$visroleselect: Role picklist</para>
        /// <para>$visuserselect: User picklist</para>
        ///</returns>
        var external = {
            $visibilityselect: '',
            $visroleselect: '',
            $visuserselect: ''
        };

        Csw.clientSession.isAdministrator({
            'Yes': function () {

                table.add(rownum, 1, label);
                var parent = table.cell(rownum, 2);
                var id = table.id;

                external.$visibilityselect = $('<select id="' + id + '_vissel" />')
                    .appendTo(parent.$);
                external.$visibilityselect.append('<option value="User">User:</option>');
                external.$visibilityselect.append('<option value="Role">Role:</option>');
                external.$visibilityselect.append('<option value="Global">Global</option>');

                external.$visroleselect = parent.$.CswNodeSelect('init', {
                    ID: parent.makeId(id, 'visrolesel'),
                    objectclass: 'RoleClass'
                }).hide();
                external.$visuserselect = parent.$.CswNodeSelect('init', {
                    ID: parent.makeId(id, 'visusersel'),
                    objectclass: 'UserClass'
                });

                external.$visibilityselect.change(function () {
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

    } // makeViewVisibilitySelect()
    Csw.controls.register('makeViewVisibilitySelect', makeViewVisibilitySelect);
    Csw.controls.makeViewVisibilitySelect = Csw.controls.makeViewVisibilitySelect || makeViewVisibilitySelect;

} ());