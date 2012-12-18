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
                roleid: '',
                rolename: '',
                userid: '',
                username: '',
                
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
                        //var parentId = table.getId();

                        cswPrivate.visibilitySelect = parentTbl.cell(1, 1).select({
                            name: 'View Visibility',
                            selected: cswPrivate.visibility,
                            values: ['User', 'Role', 'Global'],
                            onChange: cswPrivate.toggle
                        });

                        cswPrivate.roleSelect = parentTbl.cell(1, 3).nodeSelect({
                            //name: parentId + '_visrolesel',
                            name: 'View Visibility Role',
                            allowAdd: false,
                            async: false,
                            selectedNodeId: cswPrivate.roleid,
                            selectedName: cswPrivate.rolename,
                            ajaxData: {
                                ObjectClass: 'RoleClass'
                            },
                            showSelectOnLoad: true
                        });

                        cswPrivate.userSelect = parentTbl.cell(1, 4).nodeSelect({
                            //name: parentId + '_visusersel',
                            name: 'View Visibility User',
                            allowAdd: false,
                            async: false,
                            selectedNodeId: cswPrivate.userid,
                            selectedName: cswPrivate.username,
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
                    roleid: '',
                    userid: ''
                };
                if (ret.visibility === 'Role') {
                    ret.roleid = cswPrivate.roleSelect.selectedNodeId();
                    ret.rolename = cswPrivate.roleSelect.selectedName();
                } else if (ret.visibility === 'User') {
                    ret.userid = cswPrivate.userSelect.selectedNodeId();
                    ret.username = cswPrivate.userSelect.selectedName();
                }
                return ret;
            }; // getSelected()


            cswPublic.setSelected = function(newval) {
                cswPrivate.visibilitySelect.val(newval.visibility);
                cswPrivate.roleSelect.setSelectedNode(newval.roleid, newval.rolename);
                cswPrivate.userSelect.setSelectedNode(newval.userid, newval.username);
                cswPrivate.toggle();
            }; // setSelected()

            return cswPublic;
        });

} ());