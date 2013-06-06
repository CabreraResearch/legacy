/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.composites.makeViewVisibilitySelect = Csw.composites.makeViewVisibilitySelect ||
        Csw.composites.register('makeViewVisibilitySelect', function (table, rownum, label, options) {
            ///<summary>Make a View Visibility Select. Used by View Editor and Dialog.</summary>
            ///<param name="table" type="Object">A Csw.literals.table object.</param>
            ///<param name="rownum" type="Number">A row number.</param>
            ///<param name="label" type="String">A label.</param>
            ///<returns type="Object">
            ///An object representing the visibility selector
            ///</returns>
            'use strict';
            var cswPrivate = {
                visibility: 'User',
                roleid: '',
                rolename: '',
                userid: '',
                username: '',

                visibilitySelect: null,
                roleSelect: null,
                userSelect: null,
                required: false
            };
            Csw.extend(cswPrivate, options);

            var cswPublic = {};

            cswPrivate.toggle = function (val) {
                cswPrivate.visibility = val;
                if (cswPrivate.visibility === 'Role') {
                    cswPrivate.roleSelect.show();
                    cswPrivate.userSelect.hide();
                } else if (cswPrivate.visibility === 'User') {
                    cswPrivate.roleSelect.hide();
                    cswPrivate.userSelect.show();
                } else {
                    cswPrivate.roleSelect.hide();
                    cswPrivate.userSelect.hide();
                }
            }; // toggle()


            // Constructor
            (function () {
                Csw.clientSession.isAdministrator({
                    'Yes': function () {

                        table.cell(rownum, 1).setLabelText(label, cswPrivate.required, false);
                        var parentTbl = table.cell(rownum, 2).table();
                        //var parentId = table.getId();

                        cswPrivate.visibilitySelect = parentTbl.cell(1, 1).select({
                            name: 'View Visibility',
                            selected: cswPrivate.visibility || 'User',
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
                            showSelectOnLoad: true,
                            onSelectNode: function () {
                                cswPrivate.roleid = cswPrivate.roleSelect.selectedNodeId();
                                cswPrivate.rolename = cswPrivate.roleSelect.selectedName();
                            }
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
                            showSelectOnLoad: true,
                            onSelectNode: function () {
                                cswPrivate.userid = cswPrivate.userSelect.selectedNodeId();
                                cswPrivate.username = cswPrivate.userSelect.selectedName();
                            }
                        });

                        cswPrivate.toggle();
                    } // yes
                }); // IsAdministrator
            })();


            cswPublic.getSelected = function () {
                var ret = {
                    visibility: cswPrivate.visibility,
                    roleid: '',
                    userid: ''
                };
                if (ret.visibility === 'Role') {
                    ret.roleid = cswPrivate.roleid;
                    ret.rolename = cswPrivate.rolename;
                } else if (ret.visibility === 'User') {
                    ret.userid = cswPrivate.userid;
                    ret.username = cswPrivate.username;
                }
                return ret;
            }; // getSelected()


            cswPublic.setSelected = function (newval) {
                if (newval) {
                    cswPrivate.visibility = newval.visibility || 'User';
                    switch (cswPrivate.visibility) {
                        case 'User':
                            cswPrivate.userid = newval.userid;
                            cswPrivate.username = newval.username;
                            if (cswPrivate.userSelect && cswPrivate.userSelect.setSelectedNode) {
                                cswPrivate.userSelect.setSelectedNode(cswPrivate.userid, cswPrivate.username);
                            }
                            break;
                        case 'Role':
                            cswPrivate.roleid = newval.roleid;
                            cswPrivate.rolename = newval.rolename;
                            if (cswPrivate.roleSelect && cswPrivate.roleSelect.setSelectedNode) {
                                cswPrivate.roleSelect.setSelectedNode(cswPrivate.roleid, cswPrivate.rolename);
                            }
                            break;

                    }
                    if (cswPrivate.visibilitySelect) {
                        cswPrivate.visibilitySelect.val(newval.visibility);
                        cswPrivate.toggle();
                    }
                }
            }; // setSelected()

            return cswPublic;
        });

}());