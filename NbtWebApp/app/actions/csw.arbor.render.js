(function _cswArborRenderer() {
    'use strict';

    Csw.register('ArborRenderer', function (canvas_id, opts) {
        'use strict';

        var canvas = $('#' + canvas_id)[0];
        var ctx = canvas.getContext("2d");
        var particleSystem;
        
        var selected = opts.startNodeId;
        var nodes = opts.nodes;
        var edges = opts.edges;
        var onNodeClick = opts.onNodeClick;

        var that = {
            init: function (system) {
                particleSystem = system;

                particleSystem.screenSize(canvas.width, canvas.height);
                particleSystem.screenPadding(80);
                //dists = computeDists(graph, startNodeId);

                onNodeClick = opts.onNodeClick;
                selected = opts.startNodeId;
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
                            alpha = .6;
                            break;
                        case 3:
                            size = 20;
                            textDist = 25;
                            alpha = .45;
                            break;
                    }


                    var imgX = Math.round(pt.x) - (size / 2);
                    var imgY = Math.round(pt.y) - (size / 2);
                    ctx.globalAlpha = alpha;
                    ctx.drawImage($('#' + node.data.iconId)[0], imgX, imgY, size, size);

                    ctx.font = "10px Arial";
                    ctx.fillStyle = 'blue';
                    ctx.textAlign = 'center';
                    ctx.fillText(node.data.Label, Math.round(pt.x), Math.round(pt.y) + textDist);
                    if (node.data.Type === 'Instance') {
                        textUnderline(ctx, node.data.Label, Math.round(pt.x), Math.round(pt.y) + textDist, 'blue', "10px", 'center');
                    }
                    ctx.globalAlpha = 1;
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
                            
                            //TODO: fire on double click?
                            if (dragged.node.data.Type === 'Instance') {
                                //TODO: update properties panel
                                onNodeClick(dragged.node.data.NodeId);
                            } else {
                                //TODO: open dialog and fetch list of Nodes relating to starting node
                            }

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

    //Canvas doesn't support text-decorations (line underline)
    //courtesy of http://scriptstock.wordpress.com/2012/06/12/html5-canvas-text-underline-workaround/
    var textUnderline = function(context, text, x, y, color, textSize, align) {

        var textWidth = context.measureText(text).width;
        var startX;
        var startY = y + (parseInt(textSize) / 15);
        var endX;
        var endY = startY;

        var underlineHeight = parseInt(textSize) / 15;

        if (underlineHeight < 1) {
            underlineHeight = 1;
        }

        context.beginPath();
        if (align == "center") {
            startX = x - (textWidth / 2);
            endX = x + (textWidth / 2);
        } else if (align == "right") {
            startX = x - textWidth;
            endX = x;
        } else {
            startX = x;
            endX = x + textWidth;
        }

        context.strokeStyle = color;
        context.lineWidth = underlineHeight;
        context.moveTo(startX, startY);
        context.lineTo(endX, endY);
        context.stroke();
    };

}());