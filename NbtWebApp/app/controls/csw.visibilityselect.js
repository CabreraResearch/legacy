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
            var cswPrivate = {
                
            };
            
            var cswPublic = {
                $visibilityselect: '',
                get $visroleselect() {
                    return cswPrivate.roleSelect.select.$; 
                },
                get $visuserselect() {
                    return cswPrivate.userSelect.select.$;
                }
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

                    var showRole = false, showUser = false;

                    cswPrivate.roleSelect = parent.cell(1, 3).nodeSelect({
                        name: id + '_visrolesel',
                        allowAdd: false,
                        async: false,
                        ajaxData: {
                            ObjectClass: 'RoleClass'
                        },
                        showSelectOnLoad: true,
                        onSuccess: function() {
                             if (showRole) {
                                 cswPrivate.roleSelect.show();
                             }
                        }
                    });
                    cswPrivate.roleSelect.hide();
                    
                    cswPrivate.userSelect = parent.cell(1, 4).nodeSelect({
                        name: id + '_visusersel',
                        allowAdd: false,
                        async: false,
                        ajaxData: {
                            ObjectClass: 'UserClass'
                        },
                        showSelectOnLoad: true,
                        onSuccess: function() {
                            if (showUser) {
                                cswPrivate.userSelect.show();
                            }
                        }
                    });


                    cswPublic.$visibilityselect.change(function() {
                        var val = cswPublic.$visibilityselect.val();
                        showRole = (val === 'Role');
                        showUser = (val === 'User');
                        if (showRole) {
                            cswPrivate.roleSelect.show();
                            cswPrivate.userSelect.hide();
                        } else if (showUser) {
                            cswPrivate.roleSelect.hide();
                            cswPrivate.userSelect.show();
                        } else {
                            cswPrivate.roleSelect.hide();
                            cswPrivate.userSelect.hide();
                        }
                    }); // change
                } // yes
            }); // IsAdministrator

            return cswPublic;

        });

} ());