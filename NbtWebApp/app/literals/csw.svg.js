/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.literals.svg = Csw.literals.svg ||
        Csw.literals.register('svg', function (options) {
            'use strict';
            /// <summary>
            /// Create or extend an HTML SVG image and return a Csw.svg object
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the image.</para>
            /// </param>
            /// <returns type="table">An SVG object</returns>
            var cswPrivate = {
                $parent: '',
                ID: '',
                name: 'svg',
                width: 200,
                height: 200,
                cssClass: '',
                styles: {},
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                var $svg = $(document.createElementNS("http://www.w3.org/2000/svg", "svg"))
                .attr("id", cswPrivate.ID)
                .attr("name", cswPrivate.name)
                .attr("width", cswPrivate.width)
                .attr("height", cswPrivate.height)
                .attr("viewbox", "0 0 " + cswPrivate.width + " " + cswPrivate.height)
                .attr("class", cswPrivate.class);

                
                Csw.literals.factory($svg, cswPublic);

                cswPublic.addClass(cswPrivate.cssClass);
                cswPublic.css(cswPrivate.styles);
                cswPrivate.$parent.append(cswPublic);

            }());


            cswPublic.text = function (options) {
                /// <summary>
                /// Text to be rendered in the SVG
                /// </summary>
                /// <param name="text">the string</param>
                var cswPrivateInner = {
                    text: '',
                    x: 0,
                    y: 20,
                    fill: 'black',
                    opacity: 1.0,
                    fontsize: 20,
                    style: '',
                    transform: '',
                };

                Csw.extend(cswPrivateInner, options);

                if (cswPublic.length() > 0) {
                    var newText = document.createElementNS("http://www.w3.org/2000/svg", "text");
                    newText.appendChild(document.createTextNode(cswPrivateInner.text));

                    $(newText)
                    .attr('x', cswPrivateInner.x)
                    .attr('y', cswPrivateInner.y)
                    .attr('fill', cswPrivateInner.fill)
                    .attr('opacity', cswPrivateInner.opacity)
                    .attr('font-size', cswPrivateInner.fontsize)
                    .attr('style', cswPrivateInner.style)
                    .attr('transform', cswPrivateInner.transform)
                    .appendTo(cswPublic[0]);


                }

           };

            //TODO: implement functions for shapes and paths (the actual image stuff)


            return cswPublic;
        });

}());

