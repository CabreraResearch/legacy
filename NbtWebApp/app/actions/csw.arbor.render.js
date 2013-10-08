(function _cswArborRenderer() {
    'use strict';


    Csw.register('ArborRenderer', function (canvas_id, nodes, edges) {
        'use strict';

        var canvas = $('#' + canvas_id)[0];
        var ctx = canvas.getContext("2d");
        var particleSystem;

        var that = {
            init: function (system) {
                particleSystem = system;

                particleSystem.screenSize(canvas.width, canvas.height);
                particleSystem.screenPadding(80);

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
                    //if (node.data.selected) {
                    //    ctx.beginPath();
                    //    ctx.arc(Math.round(pt.x), Math.round(pt.y), 40, 0, 2 * Math.PI);
                    //    ctx.lineWidth = 3;
                    //    ctx.strokeStyle = 'rgba(0,0,0,0.2)';
                    //    ctx.stroke();
                    //}

                    ctx.drawImage($('#' + node.data.iconId)[0], Math.round(pt.x) - 10, Math.round(pt.y) - 10, 16, 16);

                    ctx.font = "10px Arial";
                    ctx.fillStyle = 'blue';
                    ctx.fillText(node.data.Label, Math.round(pt.x) - 40, Math.round(pt.y) + 20);

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
                            //remove nodes that are 3 or more degrees away

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