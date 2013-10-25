(function _cswArborRenderer() {
    'use strict';

    Csw.register('ArborRenderer', function (canvas_id, nodes, edges, graph, startNodeId) {
        'use strict';

        var canvas = $('#' + canvas_id)[0];
        var ctx = canvas.getContext("2d");
        var particleSystem;
        var dists;
        var selected = startNodeId;

        var that = {
            init: function (system) {
                particleSystem = system;

                particleSystem.screenSize(canvas.width, canvas.height);
                particleSystem.screenPadding(80);
                //dists = computeDists(graph, startNodeId);

                selected = startNodeId;
                that.initMouseHandling();
            },

            redraw: function () {
                ctx.fillStyle = "white";
                ctx.fillRect(0, 0, canvas.width, canvas.height);

                particleSystem.eachEdge(function (edge, pt1, pt2) {
                    ctx.strokeStyle = "rgba(0,0,0, .333)";
                    ctx.lineWidth = 1;
                    ctx.beginPath();
                    ctx.moveTo(pt1.x, pt1.y);
                    ctx.lineTo(pt2.x, pt2.y);
                    ctx.stroke();
                });

                particleSystem.eachNode(function (node, pt) {

                    var size = 16;
                    var textDist = 25;
                    var alpha = 1;

                    switch(node.data.Level) {
                        case 0:
                            size = 50;
                            textDist = 35;
                            break;
                        case 1:
                            size = 25;
                            textDist = 25;
                            break;
                        case 2:
                            size = 20;
                            textDist = 25;
                            alpha = .7;
                            break;
                        case 3:
                            size = 20;
                            textDist = 25;
                            alpha = .55;
                            break;
                    }

                    if (node.data.Level === 0) {
                        size = 50;
                        textDist = 45;
                    }

                    var imgX = Math.round(pt.x) - (size / 2);
                    var imgY = Math.round(pt.y) - (size / 2);
                    ctx.globalAlpha = alpha;
                    ctx.drawImage($('#' + node.data.iconId)[0], imgX, imgY, size, size);
                    ctx.globalAlpha = 1;

                    ctx.font = "10px Arial";
                    ctx.fillStyle = 'blue';
                    ctx.textAlign = 'center';
                    ctx.fillText(node.data.Label, Math.round(pt.x), Math.round(pt.y) + textDist);

                });
            },

            initMouseHandling: function () {
                var dragged = null;
                var _mouseP;

                var handler = {
                    clicked: function (e) {
                        var pos = $(canvas).offset();
                        _mouseP = arbor.Point(e.pageX - pos.left, e.pageY - pos.top);
                        dragged = particleSystem.nearest(_mouseP);

                        if (dragged && dragged.node !== null) {
                            selected = dragged.node.data.NodeId;
                            //dists = computeDists(graph, dragged.node.data.NodeId);
                            //Csw.each(nodes, function (node) {
                            //    if (dists[node.Data.NodeId] > 2) {
                            //        particleSystem.pruneNode(dragged.node);
                            //    } else {
                            //        particleSystem.addNode(node.NodeId, node.Data);
                            //        Csw.each(edges, function(edge) {
                            //            if (edge.OwnerNodeId === node.NodeId && dists[edge.TargetNodeId] <= 2) {
                            //                particleSystem.addEdge(edge.OwnerNodeId, edge.TargetNodeId, edge.Data);
                            //            }
                            //            if (edge.TargetNodeId === node.NodeId && dists[edge.OwnerNodeId] <= 2) {
                            //                particleSystem.addEdge(edge.TargetNodeId, edge.OwnerNodeId, edge.Data);
                            //            }
                            //        });
                            //    }
                            //});

                            dragged.node.fixed = true;
                        }

                        $(canvas).bind('mousemove', handler.dragged);
                        $(window).bind('mouseup', handler.dropped);

                        return false;
                    },
                    dragged: function (e) {
                        var pos = $(canvas).offset();
                        var s = arbor.Point(e.pageX - pos.left, e.pageY - pos.top);

                        if (dragged && dragged.node !== null) {
                            var p = particleSystem.fromScreen(s);
                            dragged.node.p = p;
                        }

                        return false;
                    },
                    dropped: function (e) {
                        if (dragged === null || dragged.node === undefined) return '';
                        if (dragged.node !== null) dragged.node.fixed = false;
                        dragged.node.tempMass = 1000;
                        dragged = null;
                        $(canvas).unbind('mousemove', handler.dragged);
                        $(window).unbind('mouseup', handler.dropped);
                        _mouseP = null;
                        return false;
                    }
                };

                $(canvas).mousedown(handler.clicked);
            },
        };
        return that;

    });

}());