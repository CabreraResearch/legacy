
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.wizard.nodeGrid = Csw.wizard.nodeGrid ||
        Csw.wizard.register('nodeGrid', function (cswParent, options) {
            'use strict';
            ///<summary>Creates a basic grid with an Add menu.</summary>

            var cswPrivate = {
                ID: 'wizardNodeGrid',
                reinitGrid: null,
                viewid: '',
                canSelectRow: true,
                onSelect: null,
                forceFit: true,
                relatednodeid: '',
                relatednodetypeid: '',
                relatedobjectclassid: '',
                hasMenu: true,
                ReadOnly: false,
                height: 200
            };
            if (options) Csw.extend(cswPrivate, options);

            var cswPublic = { };

            (function _pre() {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard grid without a parent.', '', 'csw.wizard.grid.js', 22));
                }
                if (Csw.isNullOrEmpty(cswPrivate.viewid)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard grid without a View ID.', '', 'csw.wizard.grid.js', 25));
                }
            } ());

            cswPrivate.makeGrid = function (viewid) {
                'use strict';
                cswPublic.rootDiv = cswPublic.rootDiv || cswParent.div();
                cswPublic.rootDiv.empty();

                viewid = viewid || cswPrivate.viewid;

                cswPrivate.menuDiv = cswPublic.rootDiv.div({ ID: Csw.makeId(cswPrivate.ID, 'menu') });
                if (cswPrivate.hasMenu) {
                    cswPrivate.menuDiv.css({ height: '25px' });
                }
                cswPrivate.filterDiv = cswPublic.rootDiv.div({ ID: Csw.makeId(cswPrivate.ID, 'filter') });
                cswPrivate.gridDiv = cswPublic.rootDiv.div({ ID: Csw.makeId(cswPrivate.ID, 'property') });
                cswPrivate.reinitGrid = (function () {
                    return function (newviewid) {
                        cswPrivate.makeGrid(newviewid);
                    };
                } ());
                Csw.nbt.viewFilters({
                    ID: cswPrivate.ID + '_viewfilters',
                    parent: cswPrivate.filterDiv,
                    viewid: cswPrivate.viewid,
                    onEditFilters: function (newviewid) {
                        cswPrivate.makeGrid(newviewid);
                    } // onEditFilters
                }); // viewFilters

                cswPrivate.gridOpts = {
                    ID: cswPrivate.ID + '_grid',
                    viewid: viewid,
                    nodeid: cswPrivate.nodeid,
                    cswnbtnodekey: cswPrivate.cswnbtnodekey,
                    readonly: cswPrivate.ReadOnly,
                    canSelectRow: cswPrivate.canSelectRow,
                    forceFit: cswPrivate.forceFit,
                    onSelect: cswPrivate.onSelect,
                    showActionColumn: cswPrivate.showActionColumn,
                    showEdit: cswPrivate.showEdit,
                    showDelete: cswPrivate.showDelete,
                    height: cswPrivate.height,
                    reinit: false,
                    onEditNode: function () {
                        cswPrivate.reinitGrid(viewid);
                    },
                    onDeleteNode: function () {
                        cswPrivate.reinitGrid(viewid);
                    },
                    onSuccess: function () {
                        cswPrivate.makeGridMenu(viewid);
                        Csw.tryExec(cswPrivate.onSuccess);
                    },
                    includeInQuickLaunch: false
                };
                cswPublic.grid = cswPrivate.gridDiv.$.CswNodeGrid('init', cswPrivate.gridOpts);
            };

            cswPrivate.makeGridMenu = function (viewid) {
                if (cswPrivate.hasMenu) {
                    var menuOpts = { 
                        width: 150,
                        ajax: { 
                            urlMethod: 'getMainMenu', 
                            data: {
                                ViewId: viewid,
                                SafeNodeKey: cswPrivate.cswnbtnodekey,
                                NodeTypeId: '',
                                PropIdAttr: cswPrivate.ID,
                                LimitMenuTo: 'Add',
                                ReadOnly: false
                            }
                        },
                        onAlterNode: function() { cswPrivate.reinitGrid(viewid); },
                        Multi: false,
                        nodeTreeCheck: ''
                    };
                    cswPrivate.menuDiv.menu( menuOpts );
                }
            }; // makeGridMenu()

            cswPublic.getSelectedNodeId = function () {
                if (cswPublic.grid) {
                    var rowid = cswPublic.grid.getSelectedRowId();
                    return cswPublic.grid.getValueForColumn('nodeid', rowid);
                }
            };

            (function _post() {
                cswPrivate.makeGrid(cswPrivate.viewid);
            } ());

            return cswPublic;

        });
} ());

