
(function () {


    Csw.actions.register('explorer', function (cswParent, cswPrivate) {
        'use strict';
        var cswPublic = {};

        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.template', '_template.js', 10);
        }


        var imgs = {};
        var i = 3;

        (function _preCtor() {
            //set default values on cswPrivate if none are supplied
            cswPrivate.name = cswPrivate.name || 'Explorer';
            cswPrivate.onSubmit = cswPrivate.onSubmit || function _onSubmit() {
            };
            cswPrivate.onCancel = cswPrivate.onCancel || function _onCancel() {
            };

            cswPrivate.startingNodeId = cswPrivate.startingNodeId;
            cswPrivate.sys = null;
            cswPrivate.extPanel = null;
            cswPrivate.maxDepth = 2;
            cswPrivate.filterVals = '';
            cswPrivate.filterOpts = [];

            cswParent.empty();

            Csw.ajaxWcf.post({
                urlMethod: "Explorer/GetFilterOpts",
                success: function (response) {
                    cswPrivate.filterOpts = response.Opts;
                    cswPrivate.filterVals = response.filterVals;
                }
            });

        }());

        cswPrivate.initPanel = function () {

            cswPrivate.extPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: cswPrivate.mainDiv.getId(),
                height: 650,
                header: false,
                layout: 'border',
                items: [{
                    title: 'Filters',
                    region: 'south',
                    xtype: 'panel',
                    height: 100,
                    items: [
                        {
                            xtype: 'slider',
                            id: 'ExtExplorerSlider',
                            hideLabel: false,
                            fieldLabel: 'Depth',
                            width: 400,
                            minValue: 1,
                            maxValue: 4,
                            value: cswPrivate.maxDepth,
                            increment: 1,
                            listeners: {
                                'change': function (slider, newValue, thumb, eOpts) {
                                    cswPrivate.maxDepth = newValue;
                                    cswPrivate.sys.stop();
                                    cswPrivate.initSystem();
                                }
                            }
                        },
                        {
                            xtype: 'button',
                            text: 'Edit Filters',
                            id: 'ExtEditFiltersBtn',
                            handler: function () {
                                Csw.dialogs.multiselectedit({
                                    opts: cswPrivate.filterOpts,
                                    title: 'Edit Explorer Filters',
                                    inDialog: true,
                                    onSave: function (updatedValues) {
                                        cswPrivate.sys.stop();

                                        cswPrivate.filterVal = updatedValues.join(',');

                                        Csw.iterate(cswPrivate.filterOpts, function (opt) {
                                            opt.selected = updatedValues.indexOf(opt.value) !== -1;
                                        });

                                        cswPrivate.initSystem();
                                    },
                                });
                            }
                        }]
                }, {
                    title: 'Properties',
                    region: 'east',
                    xtype: 'panel',
                    width: 300,
                    id: 'west-region-container',
                    autoScroll: true,
                    layout: {
                        // layout-specific configs go here
                        type: 'accordion',
                        titleCollapse: true,
                        animate: true
                    }
                }, {
                    title: 'Relationships',
                    region: 'center',
                    xtype: 'panel',
                    items: {
                        xtype: 'box',
                        autoEl: {
                            tag: 'canvas',
                            height: 510,
                            width: 1200
                        }
                    }
                }]
            });
        };

        cswPrivate.initSystem = function () {
            cswPrivate.sys = arbor.ParticleSystem(1000, 1000, 0.7);
            cswPrivate.sys.parameters({ gravity: true });

            //if (Csw.isNullOrEmpty(cswPrivate.startingNodeId)) {
            //    cswPrivate.startingNodeId = 'nodes_2';
            //}

            Csw.ajaxWcf.post({
                urlMethod: "Explorer/Initialize",
                data: {
                    Depth: cswPrivate.maxDepth,
                    NodeId: cswPrivate.startingNodeId,
                    FilterVal: cswPrivate.filterVal
                },
                success: function (response) {
                    var graph = {};

                    Csw.iterate(response.Nodes, function (arborNode) {
                        if (arborNode.NodeId === cswPrivate.startingNodeId) {
                            cswPrivate.onNodeClick(arborNode);
                        }

                        if (imgs[arborNode.data.Icon]) {
                            arborNode.data.iconId = imgs[arborNode.data.Icon];
                        } else {
                            var secretCell = cswPrivate.actionTbl.cell(1, i);
                            var img = secretCell.img({
                                src: arborNode.data.Icon,
                                ID: arborNode.NodeId
                            }).hide();
                            i++;
                            imgs[arborNode.data.Icon] = img.getId();

                            arborNode.data.iconId = img.getId();
                        }

                        cswPrivate.sys.addNode(arborNode.NodeId, arborNode.data);
                        graph[arborNode.NodeId] = [];
                    });

                    Csw.iterate(response.Edges, function (arborEdge) {
                        //graph[arborEdge.OwnerNodeId].push(arborEdge.TargetNodeId);
                        //graph[arborEdge.TargetNodeId].push(arborEdge.OwnerNodeId);
                        cswPrivate.sys.addEdge(arborEdge.OwnerNodeId, arborEdge.TargetNodeId, arborEdge.data);
                    });

                    cswPrivate.sys.renderer = Csw.ArborRenderer(cswPrivate.extPanel.items.items[2].items.items[0].el.dom.id, {
                        nodes: response.Nodes,
                        edges: response.Edges,
                        startNodeId: cswPrivate.startingNodeId,
                        onNodeClick: cswPrivate.onNodeClick
                    });
                }
            });
        };

        cswPrivate.onNodeClick = function(node) {
            cswPrivate.startingNodeId = node.data.selectedId;
            $.ajax({
                method: 'GET',
                url: node.data.URI,
                success: function(getResponse) {
                    var propData = Csw.deserialize(getResponse.propdata);
                    var propsPanel = window.Ext.getCmp('west-region-container');
                    propsPanel.removeAll();
                    propsPanel.setTitle(getResponse.nodename);
                    Csw.iterate(propData, function(prop) {
                        propsPanel.add({
                            title: prop.name,
                            html: prop.gestalt,
                            bodyPadding: 20,
                            autoScroll: true
                        });
                    });
                }
            });
        };

        (function _postCtor() {

            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'Explorer',
                finishText: 'Exit',
                onCancel: cswPrivate.onCancel
            });

            cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_tbl',
                align: 'center'
            }).css('width', '95%');

            cswPrivate.mainDiv = cswPrivate.actionTbl.cell(1, 1).div();

            cswPrivate.initPanel();
            cswPrivate.initSystem();
        }());

        return cswPublic;
    });
}());