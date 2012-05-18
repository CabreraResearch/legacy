/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.nodeSelect = Csw.controls.nodeSelect ||
        Csw.controls.register('nodeSelect', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                $parent: '',
                ID: '',
                nodesUrlMethod: 'getNodes',
                nodeTypeId: '',
                objectClassId: '',
                objectClassName: '',
                onSelect: null,
                onSuccess: null,
                width: '200px',
                addNewOption: false,
                labelText: null,
                excludeNodeTypeIds: '',
                canAdd: false
            };
            var cswPublic = {};

            (function () {

                if (options) {
                    $.extend(cswPrivate, options);
                }
                cswPrivate.ID += '_nodesel';

                cswPrivate.table = cswParent.table();
                cswPrivate.select = cswPrivate.table.cell(1, 1).select(cswPrivate);

                cswPublic = Csw.dom({}, cswPrivate.select);

                cswPublic.bind('change', function () {
                    Csw.tryExec(cswPrivate.onChange, cswPublic);
                    Csw.tryExec(cswPrivate.onSelect, cswPublic.val());
                });

                Csw.ajax.post({
                    urlMethod: cswPrivate.nodesUrlMethod,
                    data: {
                        NodeTypeId: Csw.string(cswPrivate.nodeTypeId),
                        ObjectClassId: Csw.string(cswPrivate.objectClassId),
                        ObjectClass: Csw.string(cswPrivate.objectClassName)
                    },
                    success: function (data) {
                        var ret = data;
                        var canAdd = Csw.bool(ret.canadd);
                        delete ret.canadd;

                        var nodecount = 0;
                        //Case 24155
                        Csw.each(ret, function (nodeName, nodeId) {
                            nodecount += 1;
                            cswPublic.option({ value: nodeId, display: nodeName });
                        });

                        Csw.tryExec(cswPrivate.onSuccess, ret);
                        cswPublic.css('width', Csw.string(cswPrivate.width));
                        
                        if (canAdd) {
                            cswPrivate.table.cell(1, 2)
                                .imageButton({
                                    ButtonType: Csw.enums.imageButton_ButtonType.Add,
                                    AlternateText: 'Add New',
                                    onClick: function() {
                                        $.CswDialog('AddNodeDialog', {
                                            nodetypeid: cswPrivate.nodeTypeId,
                                            onAddNode: function(nodeid, nodekey, nodename) {
                                                cswPublic.option({ value: nodeid, display: nodename, selected: true });
                                            }
                                        });
                                    }
                                });
                        }

                    }
                });
            } ());

            return cswPublic;
        });
} ());

