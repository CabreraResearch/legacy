/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties['static'] = Csw.properties.register('static',
        function(nodeProperty) {
            
            //The render function to be executed as a callback
            var render = function() {
                var cswPrivate = Csw.object();

                cswPrivate.text = nodeProperty.propData.values.text;
                cswPrivate.columns = nodeProperty.propData.values.columns;
                cswPrivate.rows = nodeProperty.propData.values.rows;
                cswPrivate.overflow = 'auto';
                cswPrivate.width = '';
                cswPrivate.height = '';

                if (cswPrivate.columns > 0 && cswPrivate.rows > 0) {
                    cswPrivate.overflow = 'scroll';
                    cswPrivate.width = Math.round(cswPrivate.columns + 2 - (cswPrivate.columns / 2.25)) + 'em';
                    cswPrivate.height = Math.round(cswPrivate.rows + 2.5 + (cswPrivate.rows / 5)) + 'em';
                } else if (cswPrivate.columns > 0) {
                    cswPrivate.width = Math.round(cswPrivate.columns - (cswPrivate.columns / 2.25)) + 'em';
                } else if (cswPrivate.rows > 0) {
                    cswPrivate.height = Math.round(cswPrivate.rows + 0.5 + (cswPrivate.rows / 5)) + 'em';
                }

                nodeProperty.propDiv.div({
                    cssclass: 'staticvalue',
                    text: cswPrivate.text || '&nbsp;&nbsp;'
                }).css({
                    overflow: cswPrivate.overflow,
                    width: cswPrivate.width,
                    height: cswPrivate.height
                });

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());

