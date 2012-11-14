/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.literals.applet = Csw.literals.applet ||
        Csw.literals.register('applet', function (options) {
            /// <summary> Create or extend an HTML <a /> and return a Csw.link object
            ///     &#10;1 - link(options)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the input.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.text: Text to display</para>
            /// </param>
            /// <returns type="link">A link object</returns>
            var cswPrivate = {
                $parent: '',
                ID: '',
                name: '',
                code: '',
                archive: '#',
                width: 0,
                height: 0,
                visibility: 'hidden'
            };
            var cswPublic = {};

            Csw.tryExec(function() {


                (function() {
                    var html = '',
                        attr = Csw.makeAttr(),
                        style = Csw.makeStyle();
                    
                    var $applet;

                    Csw.extend(cswPrivate, options);

                    html += '<applet ';
                    attr.add('id', cswPrivate.ID);
                    attr.add('name', cswPrivate.name);
                    attr.add('code', cswPrivate.code);
                    attr.add('archive', cswPrivate.archive);
                    attr.add('width', cswPrivate.width);
                    attr.add('height', cswPrivate.height);
                    style.add('visibility', cswPrivate.visibility);
                    
                    html += attr.get();
                    html += style.get();
                    
                    html += '>';
                    html += '</applet>';
                    $applet = $(html);

                    Csw.literals.factory($applet, cswPublic);
                    
                    cswPrivate.$parent.append(cswPublic.$);
                }());

                cswPublic.param = function(paramOpts) {
                    var paramInternal = {
                        value: '',
                        display: ''
                    };
                    var paramExternal = {
                        
                    };

                    (function() {
                        $.extend(paramInternal, paramOpts);

                        var html = '<param ',
                            $param,
                            attr = Csw.makeAttr(),
                            display;

                        display = Csw.string(paramInternal.display, paramInternal.value);
                        attr.add('value', paramInternal.value);
                        attr.add('text', display);

                        html += attr.get();
                        html += '>';
                        html += display;
                        html += '</param>';
                        $param = $(html);

                        Csw.literals.factory($param, paramExternal);
                        cswPublic.append($param);
                    }());

                    return paramExternal;
                };
            });
            return cswPublic;
        });

} ());

