
/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function () {
    'use strict';


    Csw.composites.register('location', function (cswParent, options) {
        //#region variables
        var cswPrivate = {
            name: 'csw_location_tree',
            locationobjectclassid: '',
            locationnodetypeids: [],
            relatedmatch: false,
            nodeid: '',
            viewid: '',
            nodename: '',
            path: '',
            nodeKey: '',
            nodelink: '',
            ReadOnly: false,
            isRequired: false,
            onChange: null,
            overrideSelectedLocation: true,
            useDefaultLocation: true,
            value: '',
            options: [],
            search: false
        };

        var cswPublic = {};
        //#endregion variables

        //#region safety net
        Csw.tryExec(function () {

            //#region init ctor
            (function _pre() {
                var render = function () {
                    cswParent.empty();

                    if (cswPrivate.relatednodeid) {
                        cswPrivate.nodeid = Csw.string(cswPrivate.relatednodeid, cswPrivate.nodeid).trim();
                        cswPrivate.name = Csw.string(cswPrivate.relatednodename, cswPrivate.name).trim();
                        cswPrivate.path = Csw.string(cswPrivate.relatednodename, cswPrivate.path).trim();
                    } else {
                        cswPrivate.nodeid = Csw.string(cswPrivate.nodeid).trim();
                        cswPrivate.name = Csw.string(cswPrivate.name).trim();
                        cswPrivate.path = Csw.string(cswPrivate.path).trim();
                    }
                    cswPrivate.viewid = Csw.string(cswPrivate.viewid).trim();
                    cswPrivate.value = cswPrivate.nodeid;
                    cswPublic.table = cswParent.table({ TableCssClass: 'cswInline' });

                    cswPrivate.pathCell = cswPublic.table.cell(1, 1);
                    cswPrivate.selectCell = cswPublic.table.cell(1, 2);
                    cswPrivate.editCell = cswPublic.table.cell(1, 3);
                    cswPrivate.previewCell = cswPublic.table.cell(1, 4);
                    cswPrivate.validateCell = cswPublic.table.cell(1, 5);

                    if (cswPrivate.ReadOnly) {
                        //TODO: I think we are using readonly for 2 different things now so the link will never show
                        // Case 31737: Don't show link if readonly
                        cswPrivate.pathCell.text(cswPrivate.path);
                    } else {
                        cswPrivate.pathCell.nodeLink({
                            text: cswPrivate.nodelink,
                            linkText: cswPrivate.path
                        });
                    }

                    cswPrivate.selectDiv = cswPrivate.selectCell.div({
                        cssclass: 'locationselect',
                        value: cswPrivate.nodeid,
                        onChange: function () {
                            cswPrivate.selectDiv.val();
                        }
                    });
                    cswPrivate.selectCell.hide();
                }; //render()

                Csw.extend(cswPrivate, options, true);
                cswPrivate.ready = Csw.promises.all();
                if (false === cswPrivate.ReadOnly && cswPrivate.options.length === 0 && false === cswPrivate.search) {
                    cswPrivate.ready.push(Csw.ajaxWcf.get({
                        urlMethod: 'Locations/getLocationsList',
                        data: cswPrivate.viewid,
                        success: function (data) {
                            cswPrivate.options = data;
                            if (cswPrivate.options.length > 0) {
                                if (cswPrivate.useDefaultLocation) {
                                    cswPrivate.options.forEach(function(option) {
                                        if (option["Selected"] === true) {
                                            cswPrivate.nodeid = option.LocationId;
                                            cswPrivate.path = option.Path;
                                        }
                                        if (option["Disabled"] === true) {
                                            option["disabledItemCls"] = "x-combo-grayed-out-item";
                                        }
                                    });
                                } else {
                                    // What should we set this to?
                                }

                            } else {
                                cswPrivate.search = true;
                            }
                            render();
                        }
                    }));
                } else {
                    render();
                }

            }());
            //#endregion init ctor

            //#region cswPrivate/cswPublic methods and props

            cswPublic.val = function () {
                return cswPrivate.value;
            };

            cswPublic.selectedName = function () {
                return cswPrivate.selectedName;
            };

            cswPrivate.makeLocationCombo = function () {
                cswPrivate.pathCell.hide();
                cswPrivate.editCell.hide();
                cswPrivate.selectCell.show();

                cswPublic.comboBox = cswPrivate.selectDiv.comboBoxExt({
                    name: cswPrivate.name + '_comboExt',
                    displayField: 'Path',
                    valueField: 'LocationId',
                    queryMode: 'local',
                    queryDelay: 2000,
                    options: cswPrivate.options,
                    selectedValue: cswPrivate.path,
                    search: cswPrivate.search,
                    searchUrl: 'Locations/searchLocations',
                    listeners: {
                        beforeselect: function (combo, record) {
                            if (record.data.Disabled) {
                                return false === record.data.Disabled;
                            } else {
                                return true;
                            }
                        },
                        select: function (combo, records) {
                            var locpath = records[0].get('Path');
                            var nodeid = records[0].get('LocationId');

                            Csw.tryExec(cswPrivate.onChange, nodeid);
                            cswPrivate.value = nodeid;
                            cswPrivate.nodeid = nodeid;
                            cswPrivate.path = locpath;
                        },
                        change: function (combo, newvalue) {
                            Csw.tryExec(cswPrivate.onChange, newvalue);
                        },
                        storebeforeload: function () {
                            var obj = {};
                            obj.Query = cswPublic.comboBox.combobox.getValue();
                            obj.ViewId = cswPrivate.viewid;
                            return Csw.serialize(obj);
                        }
                    },
                    isRequired: cswPrivate.isRequired,
                    searchProxyMethod: 'POST',
                    searchProxyReaderRoot: 'Data'
                });

                cswPrivate.selectDiv.css({ width: cswPrivate.selectDiv.$.width() + 15 });

            }; // makeLocationCombo()

            //#endregion cswPrivate/cswPublic methods and props

            //#region final ctor
            (function _post() {
                cswPrivate.ready.then(function () {
                    if (false === cswPrivate.ReadOnly) {
                        cswPrivate.makeLocationCombo();

                        cswPrivate.previewCell.css({ width: '24px' });
                        cswParent.$.hover(function (event) {
                            Csw.nodeHoverIn(event, {
                                nodeid: cswPrivate.value,
                                nodename: cswPrivate.selectedName,
                                parentDiv: cswPrivate.previewCell,
                                useAbsolutePosition: false,
                                rightpad: 0
                            });
                        },
                        function (event) { Csw.nodeHoverOut(event, cswPrivate.value); });
                    }
                });
            }());
            //#region final ctor

        });
        //#endregion safety net

        return cswPublic;
    });

}());


