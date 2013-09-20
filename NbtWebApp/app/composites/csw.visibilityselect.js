/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


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
            username: Csw.clientSession.currentUserName(),

            onChange: function () { },

            visibilitySelect: null,
            roleSelect: null,
            userSelect: null,
            required: false,
            onRenderFinish: function () { }
        };
        Csw.extend(cswPrivate, options);

        var cswPublic = {};

        cswPrivate.toggle = function (visibility) {
            cswPrivate.visibility = visibility;
            if (cswPrivate.roleSelect && cswPrivate.userSelect) {
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
            }
        }; // toggle()


        // Constructor
        (function () {

            Csw.ajaxWcf.post({
                urlMethod: 'ViewEditor/InitializeVisibilitySelect',
                success: function (response) {

                    //Case 21646 - default role/user selectors to the current user if no values provided
                    if (Csw.isNullOrEmpty(cswPrivate.roleid)) {
                        cswPrivate.roleid = response.RoleId;
                        cswPrivate.rolename = response.RoleName;
                    }
                    if (Csw.isNullOrEmpty(cswPrivate.userid)) {
                        cswPrivate.userid = response.UserId;
                        cswPrivate.username = response.Username;
                    }

                    Csw.clientSession.isAdministrator({
                        'Yes': function () {

                            var reqs = [];
                            var ready = Q.all(reqs);

                            table.cell(rownum, 1).setLabelText(label, cswPrivate.required, false);
                            var parentTbl = table.cell(rownum, 2).table();
                            //var parentId = table.getId();

                            cswPrivate.visibilitySelect = parentTbl.cell(1, 1).select({
                                name: 'View Visibility',
                                selected: cswPrivate.visibility || 'User',
                                values: ['User', 'Role', 'Global'],
                                onChange: function () {
                                    cswPrivate.toggle(cswPrivate.visibilitySelect.val());
                                    Csw.tryExec(cswPrivate.onChange);
                                }
                            });

                            cswPrivate.roleSelect = parentTbl.cell(1, 3).nodeSelect({
                                //name: parentId + '_visrolesel',
                                name: 'View Visibility Role',
                                allowAdd: false,
                                selectedNodeId: cswPrivate.roleid,
                                selectedName: cswPrivate.rolename,
                                isMulti: false,
                                ajaxData: {
                                    ObjectClass: 'RoleClass'
                                },
                                showSelectOnLoad: true,
                                onSelectNode: function () {
                                    cswPrivate.roleid = cswPrivate.roleSelect.selectedNodeId();
                                    cswPrivate.rolename = cswPrivate.roleSelect.selectedName();
                                    Csw.tryExec(cswPrivate.onChange);
                                }
                            });
                            if (cswPrivate.roleSelect.getAjax()) {
                                reqs.push(cswPrivate.roleSelect.getAjax());
                            }

                            cswPrivate.userSelect = parentTbl.cell(1, 4).nodeSelect({
                                //name: parentId + '_visusersel',
                                name: 'View Visibility User',
                                allowAdd: false,
                                selectedNodeId: cswPrivate.userid,
                                selectedName: cswPrivate.username,
                                isMulti: false,
                                ajaxData: {
                                    ObjectClass: 'UserClass'
                                },
                                showSelectOnLoad: true,
                                onSelectNode: function () {
                                    cswPrivate.userid = cswPrivate.userSelect.selectedNodeId();
                                    cswPrivate.username = cswPrivate.userSelect.selectedName();
                                    Csw.tryExec(cswPrivate.onChange);
                                }
                            });
                            if (cswPrivate.userSelect.getAjax()) {
                                reqs.push(cswPrivate.userSelect.getAjax());
                            }

                            ready.then(function () {
                                cswPrivate.onRenderFinish();
                            });

                            cswPrivate.toggle(cswPrivate.visibility);
                        }, // yes
                        'No': function () {
                            cswPrivate.onRenderFinish();
                        }
                    }); // IsAdministrator     
                }
            });
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
                    cswPrivate.visibilitySelect.val(cswPrivate.visibility);
                    cswPrivate.toggle(cswPrivate.visibility);
                }
            }
        }; // setSelected()

        return cswPublic;
    });

}());