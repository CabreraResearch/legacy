/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.jZebra = Csw.composites.jZebra ||
        Csw.composites.register('jZebra', function (cswParent, options) {
            /// <summary>
            /// Create a jZebra applet and return the an object for use with printing
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.ID: An ID for the linkGrid.</para>
            /// <para>options.linkText: linkGrid link prefix</para>
            /// </param>
            /// <returns type="linkGrid">A linkGrid object</returns>
            'use strict';
            var cswPrivate = {
                ID: 'jzebra',
                cssclass: ''
            };
            var cswPublic = {};

            Csw.tryExec(function() {
                //This is the 3rd party JS lib
                if (Csw.isNullOrEmpty(window.jZebra)) {
                    Csw.error.throwException('Cannot create a jZebra component without the jZebra library.', 'csw.jzebra.js', 'csw.jzebra.js', 27);
                }
                cswPublic.zebraJs = window.jZebra;
                
                (function _pre() {
                    if (options) {
                        $.extend(cswPrivate, options);
                    }
                    cswPrivate.div = cswParent.div();
                    
                    if (Csw.isNullOrEmpty(document.jZebra)) {
                        //The 3rd party Java applet
                        cswPrivate.applet = cswPrivate.div.applet({
                            name: 'jZebra',
                            code: 'jzebra.PrintApplet.class',
                            archive: 'js/thirdparty/jZebra/jzebra.jar'
                        });
                    }
                    
                    cswPublic.defaultPrinter = Csw.string(Csw.cookie.get('defaultPrinter'));

                    cswPrivate.div.span({ text: 'Select a Printer: ' }).br();

                    //To minimize hacking jZebra, use their ID
                    Csw.tryExec(function() {
                        var doomedElement = $('#printersList');
                        if (false === Csw.isNullOrEmpty(doomedElement, true)) {
                            doomedElement.remove();
                            Csw.debug.warn('initializing jZebra encountered a duplicate printersList.');
                        }
                        
                        cswPrivate.printerSel = cswPrivate.div.select({
                            ID: 'printersList'
                            /*
                            ,
                            selected: cswPublic.defaultPrinter,
                            onChange: function() {
                                //cswPublic.defaultPrinter = cswPrivate.printerSel.val();
                                Csw.cookie.set('defaultPrinter', cswPublic.defaultPrinter);
                            }
                            */
                        });
                        
                    });
                    
                }());

                cswPrivate.initAttempts = 0;

                cswPrivate.initJava = function() {
                    cswPrivate.initAttempts += 1;
                    if (false === Csw.isNullOrEmpty(document.jZebra) &&
                        Csw.isFunction(document.jZebra.findPrinter)) {
                        cswPublic.zebraJava = document.jZebra;

                        cswPublic.print = function(eplText) {
                            if (false === Csw.isNullOrEmpty(eplText)) {
                                cswPublic.zebraJava.findPrinter(cswPublic.defaultPrinter);
                                cswPublic.zebraJava.append(eplText);
                                cswPublic.zebraJava.print();
                            } else {
                                $.CswDialog('AlertDialog', { text: 'No EPL text submitted for print. ' });
                            }
                        };

                        cswPublic.findPrinters = function() {
                            //This will populate cswPrivate.printerSel with available printers
                            cswPublic.zebraJs.findPrinters(cswPrivate.defaultPrinter);
                            debugger;

                        };
                        Csw.publish('jZebra_Ready');
                    } else if (cswPrivate.initAttempts < 5) {
                        window.setTimeout(cswPrivate.initJava, 3000);
                    }
                };

                cswPublic.setDefaultPrinter = function(defaultPrinter) {
                    cswPublic.defaultPrinter = defaultPrinter;
                    Csw.cookie.set('defaultPrinter', defaultPrinter);
                };

                (function _post() {
                    cswPrivate.initJava();
                }());

            });
            return cswPublic;
        });

} ());
