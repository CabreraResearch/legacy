/// <reference path="~/app/CswApp-vsdoc.js" />


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
            var cswPublic = {
                $visibilityselect: '',
                $visroleselect: '',
                $visuserselect: ''
            };

            Csw.clientSession.isAdministrator({
                'Yes': function() {

                    table.cell(rownum, 1).text(label);
                    var parent = table.cell(rownum, 2).table();
                    
                    var id = table.id;
                    /* NO! Refactor to use Csw.literals and more wholesome methods. */
                    cswPublic.$visibilityselect = $('<select id="' + id + '_vissel" />')
                        .appendTo(parent.cell(1,1).$);
                    cswPublic.$visibilityselect.append('<option value="User">User:</option>');
                    cswPublic.$visibilityselect.append('<option value="Role">Role:</option>');
                    cswPublic.$visibilityselect.append('<option value="Global">Global</option>');

                    cswPublic.$visroleselect = parent.cell(1, 3).nodeSelect({
                        ID: Csw.makeId(id, 'visrolesel'),
                        objectClassName: 'RoleClass',
                        canAdd: false
                    }).$.hide();
                    cswPublic.$visuserselect = parent.cell(1, 4).nodeSelect({
                        ID: Csw.makeId(id, 'visusersel'),
                        objectClassName: 'UserClass',
                        canAdd: false
                    }).$;

                    cswPublic.$visibilityselect.change(function() {
                        var val = cswPublic.$visibilityselect.val();
                        if (val === 'Role') {
                            cswPublic.$visroleselect.show();
                            cswPublic.$visuserselect.hide();
                        } else if (val === 'User') {
                            cswPublic.$visroleselect.hide();
                            cswPublic.$visuserselect.show();
                        } else {
                            cswPublic.$visroleselect.hide();
                            cswPublic.$visuserselect.hide();
                        }
                    }); // change
                } // yes
            }); // IsAdministrator

            return cswPublic;

        });

} ());