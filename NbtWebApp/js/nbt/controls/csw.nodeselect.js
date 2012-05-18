/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.nodeSelect = Csw.controls.nodeSelect ||
        Csw.controls.register('nodeSelect', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
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
            var cswPublicRet = {};

            (function () {

                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                cswPrivateVar.ID += '_nodesel';

                cswPrivateVar.table = cswParent.table();
                cswPrivateVar.select = cswPrivateVar.table.cell(1, 1).select(cswPrivateVar);

                cswPublicRet = Csw.dom({}, cswPrivateVar.select);

                cswPublicRet.bind('change', function () {
                    Csw.tryExec(cswPrivateVar.onChange, cswPublicRet);
                    Csw.tryExec(cswPrivateVar.onSelect, cswPublicRet.val());
                });

                Csw.ajax.post({
                    urlMethod: cswPrivateVar.nodesUrlMethod,
                    data: {
                        NodeTypeId: Csw.string(cswPrivateVar.nodeTypeId),
                        ObjectClassId: Csw.string(cswPrivateVar.objectClassId),
                        ObjectClass: Csw.string(cswPrivateVar.objectClassName)
                    },
                    success: function (data) {
                        var ret = data;
                        var canAdd = Csw.bool(ret.canadd);
                        delete ret.canadd;

                        var nodecount = 0;
                        //Case 24155
                        Csw.each(ret, function (nodeName, nodeId) {
                            nodecount += 1;
                            cswPublicRet.option({ value: nodeId, display: nodeName });
                        });

                        Csw.tryExec(cswPrivateVar.onSuccess, ret);
                        cswPublicRet.css('width', Csw.string(cswPrivateVar.width));
                        
                        if (canAdd) {
                            cswPrivateVar.table.cell(1, 2)
                                .imageButton({
                                    ButtonType: Csw.enums.imageButton_ButtonType.Add,
                                    AlternateText: 'Add New',
                                    onClick: function() {
                                        $.CswDialog('AddNodeDialog', {
                                            nodetypeid: cswPrivateVar.nodeTypeId,
                                            onAddNode: function(nodeid, nodekey, nodename) {
                                                cswPublicRet.option({ value: nodeid, display: nodename, selected: true });
                                            }
                                        });
                                    }
                                });
                        }

                    }
                });
            } ());

            return cswPublicRet;
        });
} ());

