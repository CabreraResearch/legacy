/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.nbt.wizard.nodeGrid = Csw.nbt.wizard.nodeGrid ||
        Csw.nbt.wizard.register('nodeGrid', function (cswParent, options) {
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
                relatedobjectclassid: ''
            };
            if (options) $.extend(cswPrivate, options);

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

                cswPrivate.menuDiv = cswPublic.rootDiv.div({ ID: Csw.makeId(cswPrivate.ID, 'menu') }).css({ height: '25px' });
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
                    reinit: false,
                    onEditNode: function () {
                        cswPrivate.reinitGrid(viewid);
                    },
                    onDeleteNode: function () {
                        cswPrivate.reinitGrid(viewid);
                    },
                    onSuccess: function () {
                        cswPrivate.makeGridMenu(viewid);
                    }
                };
                cswPublic.grid = cswPrivate.gridDiv.$.CswNodeGrid('init', cswPrivate.gridOpts);
            };

            cswPrivate.makeGridMenu = function (viewid) {
                cswPrivate.menuDiv.$.CswMenuMain({
                    viewid: viewid,
                    nodeid: cswPrivate.nodeid,
                    cswnbtnodekey: cswPrivate.cswnbtnodekey,
                    relatednodeid: Csw.string(cswPrivate.relatednodeid),
                    relatednodetypeid: Csw.string(cswPrivate.relatednodetypeid),
                    relatedobjectclassid: Csw.string(cswPrivate.relatedobjectclassid),
                    propid: cswPrivate.ID,
                    limitMenuTo: 'Add',
                    onAddNode: function () {
                        cswPrivate.reinitGrid(viewid);
                    }
                }); // CswMenuMain
            };

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

