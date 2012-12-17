/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.makeViewVisibilitySelect = Csw.controls.makeViewVisibilitySelect ||
        Csw.controls.register('makeViewVisibilitySelect', function(table, rownum, label, options) {
            ///<summary>Make a View Visibility Select. Used by View Editor and Dialog.</summary>
            ///<param name="table" type="Object">A Csw.literals.table object.</param>
            ///<param name="rownum" type="Number">A row number.</param>
            ///<param name="label" type="String">A label.</param>
            ///<returns type="Object">
            ///An object representing the visibility selector
            ///</returns>
            'use strict';
            var cswPrivate = {
                visibility: '',
                roleId: '',
                userId: '',
                
                visibilitySelect: null,
                roleSelect: null,
                userSelect: null
            };
            Csw.extend(cswPrivate, options);

            var cswPublic = {};

            cswPrivate.toggle = function() {
                var visval = cswPrivate.visibilitySelect.val();
                if (visval === 'Role') {
                    cswPrivate.roleSelect.show();
                    cswPrivate.userSelect.hide();
                } else if (visval === 'User') {
                    cswPrivate.roleSelect.hide();
                    cswPrivate.userSelect.show();
                } else {
                    cswPrivate.roleSelect.hide();
                    cswPrivate.userSelect.hide();
                }
            }; // toggle()


            // Constructor
            (function() {
                Csw.clientSession.isAdministrator({
                    'Yes': function() {

                        table.cell(rownum, 1).text(label);
                        var parentTbl = table.cell(rownum, 2).table();

                        cswPrivate.visibilitySelect = parentTbl.cell(1, 1).select({
                            name: table.id + '_vissel',
                            selected: cswPrivate.visibility,
                            values: ['User', 'Role', 'Global'],
                            onChange: cswPrivate.toggle
                        });

                        cswPrivate.roleSelect = parentTbl.cell(1, 3).nodeSelect({
                            name: table.id + '_visrolesel',
                            allowAdd: false,
                            async: false,
                            selectedNodeId: cswPrivate.roleId,
                            ajaxData: {
                                ObjectClass: 'RoleClass'
                            },
                            showSelectOnLoad: true
                        });

                        cswPrivate.userSelect = parentTbl.cell(1, 4).nodeSelect({
                            name: table.id + '_visusersel',
                            allowAdd: false,
                            async: false,
                            selectedNodeId: cswPrivate.userId,
                            ajaxData: {
                                ObjectClass: 'UserClass'
                            },
                            showSelectOnLoad: true
                        });

                        cswPrivate.toggle();
                    } // yes
                }); // IsAdministrator
            })();
            

            cswPublic.getSelected = function() {
                var ret = {
                    visibility: cswPrivate.visibilitySelect.val(),
                    roleId: '',
                    userId: ''
                };
                if (visibility === 'Role') {
                    ret.roleId = cswPrivate.roleSelect.selectedNodeId();
                } else if (visibility === 'User') {
                    ret.userId = cswPrivate.userSelect.selectedNodeId();
                }
                return ret;
            }; // getSelected()


            cswPublic.setSelected = function(newval) {
                cswPrivate.visibilitySelect.val(newval.visibility);
                cswPrivate.roleSelect.selectedNodeId(newval.roleId);
                cswPrivate.userSelect.selectedNodeId(newval.userId);
                cswPrivate.toggle();
            }; // setSelected()

            return cswPublic;
        });

} ());